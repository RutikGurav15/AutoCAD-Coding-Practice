using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_RemoveEntity
{
    public class RemoveEntity
    {
        [CommandMethod("RemoveEntities")]
        public void CreateBlockFromStored()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                ObjectIdCollection entityIds = new ObjectIdCollection();
                foreach (ObjectId objId in btr)
                {
                    Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForRead);
                    if (entity != null)
                    {
                        entityIds.Add(objId);
                    }
                }

                foreach (ObjectId id in entityIds)
                {
                    Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite);
                    entity.Erase();
                }

                transaction.Commit();
            }
        }
    }
}
