using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_ScaleABlock
{
    public class ScaleABlock
    {
        [CommandMethod("ScaleABlock")]

        public void ScaleABlockCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database databse = document.Database;
            Editor editor = document.Editor;

            double scaleFactor = editor.GetDouble("\nEnter scale factor : ").Value;

            PromptPointResult promptPointResult = editor.GetPoint("\nEnter base point : ");
            Point3d basePoint = promptPointResult.Value;

            using (DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = databse.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(databse.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(entity is BlockReference blockRef)
                        {
                            // move base pt to origin
                            Matrix3d toOrigin = Matrix3d.Displacement(basePoint.GetVectorTo(Point3d.Origin));

                            //create uniform scaling
                            Matrix3d scaleMatrix = Matrix3d.Scaling(scaleFactor, Point3d.Origin);

                            // move origin pt to base
                            Matrix3d backToBasePoint = Matrix3d.Displacement(Point3d.Origin.GetVectorTo(basePoint));

                            // combine transformation
                            Matrix3d finalTransform = toOrigin * scaleMatrix * backToBasePoint;

                            blockRef.TransformBy(finalTransform);

                        }
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
