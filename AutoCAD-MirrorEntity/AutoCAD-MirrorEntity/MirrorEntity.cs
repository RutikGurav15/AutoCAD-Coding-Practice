using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_MirrorEntity
{
    public class MirrorEntity
    {
        [CommandMethod("MirrorSingleEntity")]
        public void MirrorSingleEntityCommand()
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


                    foreach (ObjectId objectId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objectId, OpenMode.ForWrite);

                        if(entity != null )
                        {
                            Point3d mirrorStart = new Point3d(0, 0, 0);
                            Point3d mirrorEnd = new Point3d(0, 10, 0);

                            Line3d mirrorLine = new Line3d(mirrorEnd,mirrorStart);

                            Entity mirroredEntity = (Entity) entity.GetTransformedCopy(Matrix3d.Mirroring(mirrorLine));

                            if(mirroredEntity != null)
                            {
                                btr.AppendEntity(mirroredEntity);
                                transaction.AddNewlyCreatedDBObject(mirroredEntity, true);
                            }
                            break;
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("MirrorAllEntity")]
        public void MirrorAllEntityCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            PromptPointResult pp1 = editor.GetPoint("\nEnter first point to mirror : ");
            PromptPointResult pp2 = editor.GetPoint("\nEnter second point to mirror : ");

            Point3d startPoint = pp1.Value;
            Point3d endPoint = pp2.Value;
            Line3d mirrorLine = new Line3d(startPoint, endPoint);


            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                    ObjectIdCollection entityIds = new ObjectIdCollection();
                    //List<ObjectId> entityIds = new List<ObjectId>();
                    foreach (ObjectId id in btr)
                    {
                        entityIds.Add(id);
                    }


                    foreach (ObjectId objectId in entityIds)
                    {
                        Entity entity = (Entity)transaction.GetObject(objectId, OpenMode.ForRead);


                        Entity mirroredEntity = (Entity)entity.GetTransformedCopy(Matrix3d.Mirroring(mirrorLine));

                        if (mirroredEntity != null)
                        {
                            btr.AppendEntity(mirroredEntity);
                            transaction.AddNewlyCreatedDBObject(mirroredEntity, true);
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
