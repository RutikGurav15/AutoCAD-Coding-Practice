using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using ClosedXML.Excel;

namespace AutoCAD_Project.Core
{
    public class BlockData
    {
        public string Name { get; set; }
        public int Count { get; set; }

        [CommandMethod("Analyzer")]
        public void BlockCounterCommand()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
    class BlockCounter
    {
        public List<BlockData> GetBlockCounts()
        {
            List<BlockData> blockList = new List<BlockData>();
            Dictionary<string, int> counter = new Dictionary<string, int>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    foreach (ObjectId objId in ms)
                    {
                        if (objId.ObjectClass == RXClass.GetClass(typeof(BlockReference)))
                        {
                            BlockReference blockRef = (BlockReference) tr.GetObject(objId, OpenMode.ForRead);
                            string blockName = blockRef.Name;

                            if (counter.ContainsKey(blockName))
                                counter[blockName]++;
                            else
                                counter[blockName] = 1;
                        }
                    }

                    tr.Commit();
                }
            }

                foreach (var kvp in counter)
                {
                    blockList.Add(new BlockData { Name = kvp.Key, Count = kvp.Value });
                }

            return blockList;
        }

        public void GenerateExcel(List<BlockData> blockList, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Block Count");
                ws.Cell(1, 1).Value = "Sr.No";
                ws.Cell(1, 2).Value = "Block Name";
                ws.Cell(1, 3).Value = "Count";

                int row = 2;
                for (int i = 0; i < blockList.Count; i++)
                {
                    ws.Cell(row, 1).Value = i + 1;
                    ws.Cell(row, 2).Value = blockList[i].Name;
                    ws.Cell(row, 3).Value = blockList[i].Count;
                    row++;
                }

                workbook.SaveAs(filePath);
            }
        }

        public void CleanUnusedBlocks()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!btr.IsLayout && !btr.IsAnonymous && !btr.IsDependent && !btr.IsFromExternalReference)
                        {
                            if (btr.GetBlockReferenceIds(true, true).Count == 0)
                            {
                                btr.UpgradeOpen();
                                btr.Erase();
                            }
                        }
                    }

                    tr.Commit();
                }
            }

                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nUnused blocks cleaned successfully.");
        }

        
    }
}
