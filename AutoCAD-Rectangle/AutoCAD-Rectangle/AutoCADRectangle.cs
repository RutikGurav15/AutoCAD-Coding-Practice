

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Net;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace AutoCAD_Rectangle
{
    public class AutoCADRectangle
    {
        [CommandMethod("DrawLine")]
        public void DrawLine()
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

                    Point3d startpoint = new Point3d(0, 0, 0);
                    Point3d endpoint = new Point3d(100, 0, 0);

                    Line line = new Line(startpoint, endpoint);

                    btr.AppendEntity(line);

                    transaction.AddNewlyCreatedDBObject(line, true);

                    editor.WriteMessage("\nLine has been drawn successfully..");

                    transaction.Commit();

                }
            }
        }

        [CommandMethod("DrawPolyline")]

        public void DrawPolyline()
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

                    Polyline polyline = new Polyline();

                    polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    polyline.AddVertexAt(1, new Point2d(100, 0), 0, 0, 0);
                    polyline.AddVertexAt(2, new Point2d(100, 50), 0, 0, 0);
                    polyline.AddVertexAt(3, new Point2d(0, 50), 0, 0, 0);
                    polyline.Closed = true;


                    Point3d firstPoint = polyline.GetPoint3dAt(0);
                    Point3d secondPoint = polyline.GetPoint3dAt(1);
                    Point3d thirdPoint = polyline.GetPoint3dAt(2);
                    Point3d forthPoint = polyline.GetPoint3dAt(3);


                    Line line1 = new Line(firstPoint, thirdPoint);
                    Line line2 = new Line(secondPoint, forthPoint);

                    btr.AppendEntity(polyline); transaction.AddNewlyCreatedDBObject(polyline, true);
                    btr.AppendEntity(line1); transaction.AddNewlyCreatedDBObject(line1, true);
                    btr.AppendEntity(line2); transaction.AddNewlyCreatedDBObject(line2, true);

                    editor.WriteMessage("\nPolyline has been drawn successfully...");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawRectangle")]

        public void DrawRectangle()
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

                    Polyline polyline = new Polyline();

                    polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    polyline.AddVertexAt(1, new Point2d(100, 0), 0, 0, 0);
                    polyline.AddVertexAt(2, new Point2d(100, 50), 0, 0, 0);
                    polyline.AddVertexAt(3, new Point2d(0, 50), 0, 0, 0);
                    polyline.Closed = true;


                    btr.AppendEntity(polyline); transaction.AddNewlyCreatedDBObject(polyline, true);

                    editor.WriteMessage("\nRectangle has been drawn successfully...");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawCircle")]
        public void DrawCircle()
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

                    Circle circle = new Circle();
                    circle.Radius = 10;

                    btr.AppendEntity(circle);

                    transaction.AddNewlyCreatedDBObject(circle, true);

                    editor.WriteMessage("\nCircle has been drawn successfully");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawArc")]
        public void DrawArc()
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

                    Point3d center = new Point3d(0, 0, 0);
                    double radius = 10;
                    double startAngle = 0;
                    double endAngle = Math.PI / 2;

                    Arc arc = new Arc(center, radius, startAngle, endAngle);
                    arc.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);

                    Circle circle = new Circle();
                    circle.Radius = radius;

                    btr.AppendEntity(circle); transaction.AddNewlyCreatedDBObject(circle, true);
                    btr.AppendEntity(arc); transaction.AddNewlyCreatedDBObject(arc, true);

                    editor.WriteMessage("\nArc has been drawn successfully");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawRadius")]
        public void DrawRadius()
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

                    Point3d center = new Point3d(0, 0, 0);
                    double radius = 10;

                    Line line = new Line(center, new Point3d(0, 10, 0));
                    line.Color = Color.FromColorIndex(ColorMethod.ByAci, 2);

                    Circle circle = new Circle();
                    circle.Radius = radius;

                    btr.AppendEntity(circle); transaction.AddNewlyCreatedDBObject(circle, true);
                    btr.AppendEntity(line); transaction.AddNewlyCreatedDBObject(line, true);

                    editor.WriteMessage("\nRadius has been drawn successfully");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawDiameter")]
        public void DrawDiameter()
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

                    Point3d center = new Point3d(0, 0, 0);
                    double radius = 10;

                    Circle circle = new Circle();
                    circle.Radius = radius;

                    Point3d start = new Point3d(-radius, 0, 0);
                    Point3d end = new Point3d(radius, 0, 0);

                    Line line = new Line(start, end);
                    line.ColorIndex = 6;

                    btr.AppendEntity(circle); transaction.AddNewlyCreatedDBObject(circle, true);
                    btr.AppendEntity(line); transaction.AddNewlyCreatedDBObject(line, true);

                    editor.WriteMessage("\nDiameter has been drawn successfully");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("ScanDrawing")]
        public void ScanDrawing()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            string filePath = @"D:\AutoDesk\AutoDesk Training\Test-Drawing.dwg";

            if (!File.Exists(filePath))
            {
                ed.WriteMessage($"\nFile not found: {filePath}");
                return;
            }

            Dictionary<string, int> entityTypeCounts = new Dictionary<string, int>();

            using (Database db = new Database(false, true))
            {
                db.ReadDwgFile(filePath, FileShare.ReadWrite, true, "");

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    ed.WriteMessage($"\n--- Analyzing File: {Path.GetFileName(filePath)} ---");

                    foreach (ObjectId objId in btr)
                    {
                        Entity ent = (Entity)tr.GetObject(objId, OpenMode.ForRead);

                        if (ent != null)
                        {
                            string typeName = ent.GetType().Name;

                            // Count entity types
                            if (!entityTypeCounts.ContainsKey(typeName))
                                entityTypeCounts[typeName] = 1;
                            else
                                entityTypeCounts[typeName]++;

                            ed.WriteMessage($"\nEntity: {typeName}, Layer: {ent.Layer}");

                            // Print geometry if applicable
                            if (ent is Line line)
                            {
                                ed.WriteMessage($"\n  Start: {line.StartPoint}, End: {line.EndPoint}");
                            }
                            else if (ent is Polyline pl)
                            {
                                ed.WriteMessage($"\n  Polyline Vertices:");
                                for (int i = 0; i < pl.NumberOfVertices; i++)
                                {
                                    ed.WriteMessage($"\n    Vertex {i}: {pl.GetPoint2dAt(i)}");
                                }
                            }
                            else if (ent is Circle circle)
                            {
                                ed.WriteMessage($"\n  Center: {circle.Center}, Radius: {circle.Radius}");
                            }
                            else if (ent is Arc arc)
                            {
                                ed.WriteMessage($"\n  Arc Center: {arc.Center}, Start Angle: {arc.StartAngle}, End Angle: {arc.EndAngle}");
                            }
                        }
                    }

                    tr.Commit();
                }

                // Final summary
                ed.WriteMessage("\n\n--- Entity Type Summary ---");
                foreach (var pair in entityTypeCounts)
                {
                    ed.WriteMessage($"\n{pair.Key}: {pair.Value}");
                }

                ed.WriteMessage("\n--- End of Report ---\n");
            }
        }


        [CommandMethod("ScanDrawingToFile")]
        public void ScanDrawingToFile()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            // Specify your drawing file path
            string drawingPath = @"D:\AutoDesk\AutoDesk Training\Test-Drawing.dwg";

            // Specify your report output file path
            string reportPath = @"D:\AutoDesk\AutoDesk Practice Drawing\drawing-report.txt";

            if (!File.Exists(drawingPath))
            {
                ed.WriteMessage($"\nFile not found: {drawingPath}");
                return;
            }

            Dictionary<string, int> entityTypeCounts = new Dictionary<string, int>();

            try
            {
                using (StreamWriter writer = new StreamWriter(reportPath, false)) // overwrite if exists
                {
                    using (Database db = new Database(false, true))
                    {
                        db.ReadDwgFile(drawingPath, FileShare.ReadWrite, true, "");

                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                            writer.WriteLine($"--- Analyzing File: {Path.GetFileName(drawingPath)} ---\n");

                            foreach (ObjectId objId in btr)
                            {
                                Entity ent = (Entity)tr.GetObject(objId, OpenMode.ForRead);

                                if (ent != null)
                                {
                                    string typeName = ent.GetType().Name;

                                    // Count entity types
                                    if (!entityTypeCounts.ContainsKey(typeName))
                                        entityTypeCounts[typeName] = 1;
                                    else
                                        entityTypeCounts[typeName]++;

                                    writer.WriteLine($"Entity: {typeName}, Layer: {ent.Layer}");

                                    // Geometry details
                                    if (ent is Line line)
                                    {
                                        writer.WriteLine($"  Start: {line.StartPoint}, End: {line.EndPoint}");
                                    }
                                    else if (ent is Polyline pl)
                                    {
                                        writer.WriteLine($"  Polyline Vertices:");
                                        for (int i = 0; i < pl.NumberOfVertices; i++)
                                        {
                                            writer.WriteLine($"    Vertex {i}: {pl.GetPoint2dAt(i)}");
                                        }
                                    }
                                    else if (ent is Circle circle)
                                    {
                                        writer.WriteLine($"  Center: {circle.Center}, Radius: {circle.Radius}");
                                    }
                                    else if (ent is Arc arc)
                                    {
                                        writer.WriteLine($"  Arc Center: {arc.Center}, Start Angle: {arc.StartAngle}, End Angle: {arc.EndAngle}");
                                    }
                                }
                            }

                            tr.Commit();
                        }

                        // Summary
                        writer.WriteLine();
                        writer.WriteLine("--- Entity Type Summary ---");
                        foreach (var pair in entityTypeCounts)
                        {
                            writer.WriteLine($"{pair.Key}: {pair.Value}");
                        }

                        writer.WriteLine("\n--- End of Report ---");
                    }

                    ed.WriteMessage($"\nReport successfully written to: {reportPath}");
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\nError: {ex.Message}");
            }
        }

        [CommandMethod("Test")]
        public void ExtractData()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using(DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = document.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    string reportPath = @"D:\AutoDesk\AutoDesk Practice Drawing\report.txt";

                    StreamWriter streamWriter = new StreamWriter(reportPath, true);


                    foreach (ObjectId objId in btr)
                    {
                        Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForRead);

                        //For Reading 
                        //editor.WriteMessage(entity.ObjectId.ToString());
                        //editor.WriteMessage(entity.Layer);
                        //editor.WriteMessage(entity.GeometricExtents.ToString());

                        //For Writing
                        streamWriter.WriteLine("\nObject Id: "+entity.ObjectId.ToString());
                        streamWriter.WriteLine("\nLayer: " + entity.Layer);
                        streamWriter.WriteLine("\nGeometric Extends: " + entity.GeometricExtents.ToString());
                        streamWriter.WriteLine("\nColor: " + entity.Color.ToString());
                        streamWriter.WriteLine("\nLine Type Id: " + entity.LinetypeId);
                        streamWriter.WriteLine("\nLine Type: " + entity.Linetype);
                        //string[] str = entity.Drawable.ToString().Split(".");
                        //streamWriter.WriteLine(str[str.Length-1]);

                        streamWriter.WriteLine("\nName: " + entity.GetType().Name);
                        streamWriter.WriteLine("\nCollision Type: " + entity.CollisionType.ToString());
                        streamWriter.WriteLine("\nDrawable Type: " + entity.DrawableType.ToString());
                        streamWriter.WriteLine("\nHandle: " + entity.Handle.ToString());
                        streamWriter.WriteLine("\nVisible: " + entity.Visible.ToString());
                        streamWriter.WriteLine("\nIsErased: " + entity.IsErased.ToString());
                        streamWriter.WriteLine("\nIs Write Enabled: " + entity.IsWriteEnabled.ToString());


                        if (entity is MText mText)
                        {
                            streamWriter.WriteLine("\nContents: "+mText.Contents);
                            streamWriter.WriteLine("\nText Height: " + mText.TextHeight);
                            streamWriter.WriteLine("\nLocation: " + mText.Location);
                            streamWriter.WriteLine("\nWidth: " + mText.Width);

                        }

                        if(entity is Line line)
                        {
                            streamWriter.WriteLine("\nStart Point: " + line.StartPoint);
                            streamWriter.WriteLine("\nEnd Point: " + line.EndPoint);
                            streamWriter.WriteLine("\nLength: " + line.Length);

                        }
                        streamWriter.WriteLine();
                        streamWriter.WriteLine();

                    }

                    

                    streamWriter.Close();
                }
            }
        }

        [CommandMethod("ModifyProp")]
        public void ModifyProp()
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

                    foreach (ObjectId objId in btr)
                    {
                        Entity entity = transaction.GetObject(objId, OpenMode.ForWrite) as Entity;

                        if (entity is Line line)
                        {
                            Point3d startPoint = line.StartPoint;
                            Point3d endPoint = line.EndPoint;

                            double lineLength = line.Length;
                            editor.WriteMessage("\nLine found! Current length: " + lineLength);

                            PromptDoubleOptions lengthPrompt = new PromptDoubleOptions("\nEnter new length: ");
                            PromptDoubleResult result = editor.GetDouble(lengthPrompt);

                            double newLength = result.Value;

                            Vector3d direction = (endPoint - startPoint).GetNormal();
                            Point3d newEndPoint = startPoint + (direction * newLength);

                            line.EndPoint = newEndPoint;
                            editor.WriteMessage($"\nLine updated. New length: {line.Length}");
                        }

                        if (entity is Polyline pline && pline.NumberOfVertices == 2)
                        {
                            Point2d startPoint2D = pline.GetPoint2dAt(0);
                            Point2d endPoint2D = pline.GetPoint2dAt(1);

                            Point3d startPoint = new Point3d(startPoint2D.X, startPoint2D.Y, 0);
                            Point3d endPoint = new Point3d(endPoint2D.X, endPoint2D.Y, 0);

                            double currentLength = startPoint.DistanceTo(endPoint);
                            editor.WriteMessage("\nPolyline found! Current length: " + currentLength);

                            PromptDoubleOptions lengthPrompt = new PromptDoubleOptions("\nEnter new polyline length: ");
                            PromptDoubleResult result = editor.GetDouble(lengthPrompt);

                            double newLength = result.Value;

                            Vector3d direction = (endPoint - startPoint).GetNormal();
                            Point3d newEndPoint = startPoint + (direction * newLength);

                            pline.SetPointAt(1, new Point2d(newEndPoint.X, newEndPoint.Y));

                            double updatedLength = startPoint.DistanceTo(newEndPoint);
                            editor.WriteMessage($"\nPolyline updated. New length: {updatedLength}");
                        }

                        if (entity is Circle circle)
                        {
                            double currentRadius = circle.Radius;
                            editor.WriteMessage("\nCircle found! Current radius: " + currentRadius);

                            // Prompt user for new radius
                            PromptDoubleOptions radiusPrompt = new PromptDoubleOptions("\nEnter new radius: ");
                            radiusPrompt.AllowNegative = false;
                            radiusPrompt.AllowZero = false;

                            PromptDoubleResult result = editor.GetDouble(radiusPrompt);
                            if (result.Status != PromptStatus.OK)
                                return;

                            double newRadius = result.Value;

                            // Modify circle's radius
                            circle.Radius = newRadius;
                            editor.WriteMessage($"\nCircle updated. New radius: {circle.Radius}");
                        }


                    }

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("Modify")]
        public void Modify()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (ObjectId objId in btr)
                {
                    Entity ent =(Entity) tr.GetObject(objId, OpenMode.ForWrite);
                    if (ent == null) continue;

                    string typeName = ent.GetType().Name;
                    ed.WriteMessage($"\nEntity found: {typeName}");

                    switch (typeName)
                    {
                        case "Line":
                            Line line = (Line)ent;
                            ed.WriteMessage($"\nLine length: {line.Length}");

                            PromptDoubleResult lineRes = ed.GetDouble("\nEnter new length: ");

                            Vector3d dir = (line.EndPoint - line.StartPoint).GetNormal();
                            line.EndPoint = line.StartPoint + dir * lineRes.Value;
                            break;

                        case "Polyline":
                            Polyline pline = (Polyline)ent;

                            if (pline.NumberOfVertices == 4 && pline.Closed)
                            {
                                ed.WriteMessage("\nRectangle (closed polyline with 4 vertices) found.");

                                // Get the bottom-left and bottom-right points to calculate width
                                Point2d pt0 = pline.GetPoint2dAt(0);
                                Point2d pt1 = pline.GetPoint2dAt(1);
                                Point2d pt2 = pline.GetPoint2dAt(2);

                                double width = pt0.GetDistanceTo(pt1);
                                double height = pt1.GetDistanceTo(pt2);

                                ed.WriteMessage($"\nCurrent Width: {width}, Height: {height}");

                                // Ask for new width and height
                                PromptDoubleResult widthRes = ed.GetDouble("\nEnter new width: ");

                                PromptDoubleResult heightRes = ed.GetDouble("\nEnter new height: ");

                                double newWidth = widthRes.Value;
                                double newHeight = heightRes.Value;

                                // Rebuild rectangle with same base point (pt0)
                                pline.SetPointAt(1, new Point2d(pt0.X + newWidth, pt0.Y));
                                pline.SetPointAt(2, new Point2d(pt0.X + newWidth, pt0.Y + newHeight));
                                pline.SetPointAt(3, new Point2d(pt0.X, pt0.Y + newHeight));

                                ed.WriteMessage($"\nRectangle updated to Width: {newWidth}, Height: {newHeight}");
                            }
                            else if (pline.NumberOfVertices == 2)
                            {
                                // Line-like polyline
                                Point2d pt1 = pline.GetPoint2dAt(0);
                                Point2d pt2 = pline.GetPoint2dAt(1);
                                double len = pt1.GetDistanceTo(pt2);

                                ed.WriteMessage($"\nPolyline (2-vertex) length: {len}");

                                PromptDoubleResult plRes = ed.GetDouble("\nEnter new length: ");

                                Vector2d plDir = (pt2 - pt1).GetNormal();
                                Point2d newPt2 = pt1 + plDir * plRes.Value;

                                pline.SetPointAt(1, newPt2);
                            }
                            else
                            {
                                ed.WriteMessage($"\nPolyline with {pline.NumberOfVertices} vertices. Modification not supported.");
                            }
                            break;


                        case "Circle":
                            Circle circle = (Circle)ent;
                            ed.WriteMessage($"\nCircle radius: {circle.Radius}");

                            PromptDoubleOptions radPrompt = new PromptDoubleOptions("\nEnter new radius: ");

                            PromptDoubleResult circRes = ed.GetDouble(radPrompt);

                            circle.Radius = circRes.Value;
                            break;

                        default:
                            ed.WriteMessage("\nThis entity type is not supported for modification.");
                            break;
                    }
                }

                tr.Commit();
            }
        }


    }
}

