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
    class PolylineToLineConverter
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
                    Entity ent =(Entity) tr.GetObject(objId, OpenMode.ForRead);

                    if(ent is Polyline)
                    {
                        Polyline pline = (Polyline)tr.GetObject(objId, OpenMode.ForWrite);
                        if (pline != null && pline.NumberOfVertices > 1)
                        {
                            for (int i = 0; i < pline.NumberOfVertices - 1; i++)
                            {
                                var p1 = pline.GetPoint3dAt(i);
                                var p2 = pline.GetPoint3dAt(i + 1);
                                Line line = new Line(p1, p2);

                                ms.AppendEntity(line);
                                tr.AddNewlyCreatedDBObject(line, true);
                            }

                            pline.Erase();
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage("\nConverted all Polylines to Lines.");
        }
    }
}
