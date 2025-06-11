using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_DeleteSingleBlockReference
{
    public class DeleteSingleBlockReference
    {
        [CommandMethod("DeleteSingleBlockReference")]
        public void RemoveSpecificBlock()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable) transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord) transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    List<BlockReference> blockRefList = new List<BlockReference>();

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForRead);

                        if(entity is BlockReference br)
                        {
                            //updating existing block table record
                            BlockTableRecord blockDef = (BlockTableRecord) transaction.GetObject(br.BlockTableRecord, OpenMode.ForRead);

                            if(blockDef.Name == "REVA")
                            {
                                blockRefList.Add(br);
                            }
                        }
                    }

                    var sorted = blockRefList.OrderBy(b => b.Position.DistanceTo(Point3d.Origin)).ToList();

                    BlockReference middle = sorted[1];

                    Entity toErase = (Entity)transaction.GetObject(middle.ObjectId, OpenMode.ForWrite);
                    toErase.Erase();

                    transaction.Commit();
                }
            }
        }


        [CommandMethod("DeleteEntityAtPosition")]
        public void DeleteEntityAtPosition()
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

                    double x = editor.GetDouble("\nEnter X position: ").Value;
                    double y = editor.GetDouble("\nEnter Y position: ").Value;

                    Point3d target = new Point3d(x, y, 0);
                    double tolerance = 0.5;

                    bool deleted = false;

                    foreach (ObjectId objId in btr)
                    {
                        Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForRead);
                        if (entity == null) continue;

                        try
                        {
                            Extents3d ext = entity.GeometricExtents;
                            Point3d center = new Point3d(
                            (ext.MinPoint.X + ext.MaxPoint.X) / 2,
                            (ext.MinPoint.Y + ext.MaxPoint.Y) / 2,
                            0);

                            if (target.X >= ext.MinPoint.X - tolerance && target.X <= ext.MaxPoint.X + tolerance && target.Y >= ext.MinPoint.Y - tolerance && target.Y <= ext.MaxPoint.Y + tolerance)

                            {
                                entity.UpgradeOpen();
                                entity.Erase();
                                deleted = true;
                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }

                    }

                    if (!deleted)
                        editor.WriteMessage("\nNo entity found at that position.");

                    transaction.Commit();
                }
            }
            
        }

        [CommandMethod("FindIntersectionPoint")]
        public void FindIntersectionPoint()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable) transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    List<Line> lines = new List<Line>(2);

                    foreach(ObjectId objId in btr){
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForRead);

                        if (lines.Count == 2) break;

                        if(entity is Line line)
                        {
                            lines.Add(line);
                        }

                    }

                    Point3dCollection points = new Point3dCollection();
                    lines[0].IntersectWith(lines[1], Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);

                    if (points.Count > 0)
                        editor.WriteMessage($"\nIntersection point: {points[0]}");
                    else
                        editor.WriteMessage("\nLines do not intersect.");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("ScaleABlock")]
        public void ScaleABlock()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            PromptDoubleResult scaleX = editor.GetDouble("\nEnter Scale X : ");

            PromptDoubleResult scaleY = editor.GetDouble("\nEnter Scale Y : ");

            PromptDoubleResult scaleZ = editor.GetDouble("\nEnter Scale Z : ");

            PromptPointResult newPoint = editor.GetPoint("\nEnter new base point : ");
            Point3d basepoint = newPoint.Value;

            using (DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable) transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(entity is BlockReference blockRef)
                        {
                            // blockRef.ScaleFactors = new Scale3d(scaleX.Value, scaleY.Value, scaleZ.Value);  

                            Matrix3d toOrigin = Matrix3d.Displacement(basepoint.GetVectorTo(Point3d.Origin)); // create transformation matrix and move to the specified vector
                            Matrix3d scaleMatrix = new Matrix3d(
                    new double[]{
                            scaleX.Value, 0, 0, 0,
                            0, scaleY.Value, 0, 0,
                            0, 0, scaleZ.Value, 0,
                            0, 0, 0, 1
                    }
                );
                            Matrix3d backToBase = Matrix3d.Displacement(Point3d.Origin.GetVectorTo(basepoint));

                            Matrix3d finalTransform = toOrigin * scaleMatrix * backToBase;
                            blockRef.TransformBy(finalTransform);

                        }
                    }

                    transaction.Commit();
                }
            }
        }

    }
}
