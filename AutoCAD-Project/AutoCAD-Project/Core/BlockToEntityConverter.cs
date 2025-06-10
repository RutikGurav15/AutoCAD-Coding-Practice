using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCAD_Project.Core
{
    class BlockToEntityConverter
    {
        public void Convert()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (ObjectId objId in ms)
                {
                    Entity entity = (Entity) tr.GetObject(objId, OpenMode.ForRead);

                    if(entity is BlockReference)
                    {
                        BlockReference blkRef = (BlockReference)tr.GetObject(objId, OpenMode.ForWrite);
                        if (blkRef != null)
                        {
                            DBObjectCollection explodedObjs = new DBObjectCollection();
                            blkRef.Explode(explodedObjs);

                            foreach (Entity ent in explodedObjs)
                            {
                                ms.AppendEntity(ent);
                                tr.AddNewlyCreatedDBObject(ent, true);
                            }

                            blkRef.Erase();
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage("\nExploded all Blocks into individual entities.");
        }
    }
}
