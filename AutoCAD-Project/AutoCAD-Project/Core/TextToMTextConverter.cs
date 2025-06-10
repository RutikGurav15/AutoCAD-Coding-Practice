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
    class TextToMTextConverter
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
                    if (objId.ObjectClass.Name == "AcDbText")
                    {
                        DBText dbText =(DBText) tr.GetObject(objId, OpenMode.ForWrite);
                        if (dbText != null)
                        {
                            MText mtext = new MText
                            {
                                Contents = dbText.TextString,
                                Location = dbText.Position,
                                TextHeight = dbText.Height
                            };

                            ms.AppendEntity(mtext);
                            tr.AddNewlyCreatedDBObject(mtext, true);

                            dbText.Erase();
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage("\nConverted all Text entities to MText.");
        }
    }
}
