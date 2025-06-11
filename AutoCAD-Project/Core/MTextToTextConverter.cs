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
    class MTextToTextConverter
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
                    if (objId.ObjectClass.Name == "AcDbMText")
                    {
                        MText mtext = (MText) tr.GetObject(objId, OpenMode.ForWrite);
                        if (mtext != null)
                        {
                            DBText dbText = new DBText
                            {
                                TextString = mtext.Contents,
                                Position = mtext.Location,
                                Height = mtext.TextHeight
                            };

                            ms.AppendEntity(dbText);
                            tr.AddNewlyCreatedDBObject(dbText, true);

                            mtext.Erase();
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage("\nConverted all MText entities to Text.");
        }
    }
}
