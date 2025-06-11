using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_FindBlockReferenceAndInsert
{
    public class FindBlockReferenceAndInsert
    {
        [CommandMethod("FindBlockReferenceAndInsert")]
        public void FindBlockReferenceAndInsertCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            PromptStringOptions pso = new PromptStringOptions("\nEnter block name : ");
            PromptResult pr = editor.GetString(pso);
            string blockName = pr.StringResult;

            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    PromptPointOptions ppo = new PromptPointOptions("\nEnter insertion point : ");
                    PromptPointResult ppr = editor.GetPoint(ppo);
                    Point3d insertion = ppr.Value;

                    //here we reading the entered block name
                    BlockTableRecord blockDef = (BlockTableRecord)transaction.GetObject(bt[blockName], OpenMode.ForRead); 

                    BlockReference br = new BlockReference(insertion, blockDef.ObjectId);
                    btr.AppendEntity(br);
                    transaction.AddNewlyCreatedDBObject(br, true);

                    transaction.Commit();

                }


            }
        }
    }
}
