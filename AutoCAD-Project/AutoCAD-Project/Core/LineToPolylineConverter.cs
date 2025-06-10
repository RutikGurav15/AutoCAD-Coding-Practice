using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;


namespace AutoCAD_Project.Core
{
    class LineToPolylineConverter
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
                    Entity ent = (Entity) tr.GetObject(objId, OpenMode.ForRead);

                    if (ent is Line)
                    {
                        Line line = (Line)tr.GetObject(objId, OpenMode.ForWrite); // safe now
                        if (line != null)
                        {
                            Polyline pline = new Polyline();
                            pline.AddVertexAt(0, new Autodesk.AutoCAD.Geometry.Point2d(line.StartPoint.X, line.StartPoint.Y), 0, 0, 0);
                            pline.AddVertexAt(1, new Autodesk.AutoCAD.Geometry.Point2d(line.EndPoint.X, line.EndPoint.Y), 0, 0, 0);

                            ms.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);

                            line.Erase();
                        }
                    }
                }


                tr.Commit();
            }

            ed.WriteMessage("\nConverted all Lines to Polylines.");
        }
    }
}
