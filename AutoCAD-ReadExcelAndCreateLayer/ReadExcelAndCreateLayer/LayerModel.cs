using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using ClosedXML.Excel;

namespace AutoCAD_ReadExcelAndCreateLayer
{
    public class LayerModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string LineType { get; set; }
        public double LineWeight { get; set; }

        public static List<LayerModel> ReadLayersFromExcel(string filePath)
        {
            var layers = new List<LayerModel>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    layers.Add(new LayerModel
                    {
                        Name = row.Cell(1).GetString(),
                        Color = row.Cell(2).GetString(),
                        LineType = row.Cell(3).GetString(),
                        LineWeight = double.TryParse(row.Cell(4).GetString(), out double lw) ? lw : 0
                    });
                }
            }

            return layers;
        }
        private short GetColorIndex(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => 1,
                "yellow" => 2,
                "green" => 3,
                "cyan" => 4,
                "blue" => 5,
                "magenta" => 6,
                "white" => 7,
                _ => 7 
            };
        }
        private void EnsureLinetypeLoaded(string linetypeName, Database database)
        {
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                LinetypeTable ltt = (LinetypeTable)trans.GetObject(database.LinetypeTableId, OpenMode.ForRead);

                if (!ltt.Has(linetypeName))
                {
                    try
                    {
                        string acadLinPath = HostApplicationServices.Current.FindFile("acad.lin", database, FindFileHint.Default);
                        database.LoadLineTypeFile(linetypeName, acadLinPath);
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        if (ex.ErrorStatus == ErrorStatus.UndefinedLineType)
                        {
                            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                                $"\nLinetype '{linetypeName}' is not defined in acad.lin.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                trans.Commit();
            }
        }



        [CommandMethod("CreateLayersFromExcel")]
        public void CreateLayersFromExcel()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            string filePath = @"D:\AutoDesk\AutoDesk Practice Drawing\LayerDefinitions.xlsx";

            List<LayerModel> layers = ReadLayersFromExcel(filePath);

            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

                    foreach (var layer in layers)
                    {
                        if (!layerTable.Has(layer.Name))
                        {
                            layerTable.UpgradeOpen();

                            LayerTableRecord ltr = new LayerTableRecord
                            {
                                Name = layer.Name,
                                Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, GetColorIndex(layer.Color)),
                                LineWeight = (LineWeight)(layer.LineWeight * 100)
                            };

                            EnsureLinetypeLoaded(layer.LineType, database);

                            LinetypeTable ltt = (LinetypeTable)transaction.GetObject(database.LinetypeTableId, OpenMode.ForRead);
                            if (ltt.Has(layer.LineType))
                            {
                                ltr.LinetypeObjectId = ltt[layer.LineType];
                            }


                            layerTable.Add(ltr);
                            transaction.AddNewlyCreatedDBObject(ltr, true);
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
