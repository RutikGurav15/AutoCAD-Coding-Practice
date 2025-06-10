using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Drawing;

namespace AutoCAD_Layers
{
    public class Layers
    {
        [CommandMethod("AssignLayersAndColor")]
        public void AssignLayersAndColorCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            PromptResult lineResult  = editor.GetString("\nEnter Line layer Name : ");
            PromptResult polylineResult = editor.GetString("\nEnter Polyline layer Name : ");
            PromptResult circleResult = editor.GetString("\nEnter Circle layer Name : ");
            PromptResult arcResult = editor.GetString("\nEnter Arc layer Name : ");

            string lineLayerName = lineResult.StringResult;
            string polylineLayerName = polylineResult.StringResult;
            string circleLayerName = circleResult.StringResult;
            string arcLayerName = arcResult.StringResult;

            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    LayerTable lt = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

                    if (!lt.Has(lineLayerName))
                    {
                        lt.UpgradeOpen();
                        LayerTableRecord lineLayer = new LayerTableRecord
                        {
                            Name = lineLayerName
                        };
                        lt.Add(lineLayer);
                        transaction.AddNewlyCreatedDBObject(lineLayer, true);
                    }
                    if (!lt.Has(polylineLayerName))
                    {
                        lt.UpgradeOpen();
                        LayerTableRecord polylineLayer = new LayerTableRecord
                        {
                            Name = polylineLayerName
                        };
                        lt.Add(polylineLayer);
                        transaction.AddNewlyCreatedDBObject(polylineLayer, true);
                    }
                    if (!lt.Has(circleLayerName))
                    {
                        LayerTableRecord circleLayer = new LayerTableRecord
                        {
                            Name = circleLayerName
                        };
                        lt.Add(circleLayer);
                        transaction.AddNewlyCreatedDBObject(circleLayer, true);
                    }
                    if (!lt.Has(arcLayerName))
                    {
                        LayerTableRecord arcLayer = new LayerTableRecord
                        {
                            Name = arcLayerName
                        };
                        lt.Add(arcLayer);
                        transaction.AddNewlyCreatedDBObject(arcLayer, true);
                    }

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(entity is Line line)
                        {
                            entity.Layer = lineLayerName;
                            entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 1);
                        }
                        else if(entity is Polyline polyline)
                        {
                            entity.Layer = polylineLayerName;
                            entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 2);
                        }else if(entity is Circle circle)
                        {
                            entity.Layer = circleLayerName;
                            entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 3);
                        }else if(entity is Arc arc)
                        {
                            entity.Layer = arcLayerName;
                            entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 4);
                        }
                    }

                    transaction.Commit();   
                }
            }
        }

        [CommandMethod("FindEntityUsingLayer")]
        public void FindEntityUsingLayer()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using(DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForRead);

                        if(entity.Layer == "Doors")
                        {
                            editor.WriteMessage("\n"+entity.GetType().Name);
                        }

                    }

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("FindEntityAndAssignLayer")]

        public void FindEntityAndAssignLayerCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction= database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    LayerTable lt = (LayerTable) transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

                    if (!lt.Has("Line"))
                    {
                        lt.UpgradeOpen();
                        LayerTableRecord ltr = new LayerTableRecord
                        {
                            Name = "Line",
                            Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 1)
                        };
                    }

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(entity is Line)
                        {
                            entity.Layer = "Line";
                            entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, 1);
                        }
                    }

                    transaction.Commit();   
                }
            }
        }

        [CommandMethod("RemoveLayer")]
        public void RemoveLayerCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(entity !=null)
                        {
                            entity.Layer = "0";
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
