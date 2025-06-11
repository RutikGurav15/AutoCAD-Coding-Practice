using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Diagnostics;

namespace AutoCAD_CreateBlock
{
    public class EntityBlock
    {

        [CommandMethod("CreateBlockFromStored")]
        public void CreateBlockFromStored()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            PromptStringOptions pso = new PromptStringOptions("\nEnter block name: ");
            PromptResult prName = editor.GetString(pso);

            string blockName = prName.StringResult;

            PromptPointResult ppr = editor.GetPoint("\nSpecify base point: ");
            Point3d basePoint = ppr.Value;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                if (bt.Has(blockName))
                {
                    editor.WriteMessage($"\nBlock \"{blockName}\" already exists. Please choose a different name.");
                    return;
                }

                BlockTableRecord newBlockDef = new BlockTableRecord
                {
                    Name = blockName,
                    Origin = basePoint
                };

                bt.UpgradeOpen(); //upgrades the BlockTable from read mode to write mode
                ObjectId blockDefId = bt.Add(newBlockDef);
                transaction.AddNewlyCreatedDBObject(newBlockDef, true);

                ObjectIdCollection entityIds = new ObjectIdCollection();
                foreach (ObjectId objId in btr)
                {
                    Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForRead);
                    if (entity != null && !(entity is BlockReference))
                    {
                        entityIds.Add(objId);
                    }
                }

                foreach (ObjectId id in entityIds)
                {
                    Entity entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite);
                    Entity clone = (Entity)entity.Clone();
                    newBlockDef.AppendEntity(clone);
                    transaction.AddNewlyCreatedDBObject(clone, true);
                    entity.Erase(); 
                }

                BlockReference br = new BlockReference(basePoint, blockDefId);
                btr.AppendEntity(br);
                transaction.AddNewlyCreatedDBObject(br, true);

                transaction.Commit();
            }
        }





    }
}
