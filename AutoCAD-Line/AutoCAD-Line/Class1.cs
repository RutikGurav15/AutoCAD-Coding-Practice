using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace AutoCAD_Line
{
    public class DrawLine
    {
        //This is a command method whenever we give this 'DrawLine' Command in our AutoCAD it should execute this Method and create a line.
        [CommandMethod("DrawLine")]
        public void DrawLineCommand()
        {
            //We are making the opened document as a current document and opening the database.
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            //Here we are locking the document so that AutoCAD does not throws any transaction and database error,
            //Its always good practice to lock the docuemnt before starting a transaction and modifying objects.
            using (DocumentLock docLock = document.LockDocument())
            {
                //Here we are starting a transaction
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    //Open the block table for read
                    //We are reading the block table for its current objects.
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                    //Open the block table record[ModelSpace] for write
                    //Here we are opening the model space for write.
                    //Whenever we are going to create some entities into model space we have to open modelspace for Write otherwise it throws an error 'eNotOpenForWrite'
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    //Defining the start and end point of the line
                    Point3d startPoint = new Point3d(0, 0, 0);
                    Point3d endPoint = new Point3d(60, 100, 0);

                 

                    //Creating a  line
                    Line line = new Line(startPoint, endPoint);

                    //Here we are adding the newly line into the model space after this code the newly created line is going to show in model space
                    btr.AppendEntity(line);
                    //In this line we are saving the new line into AutoCAD database.
                    transaction.AddNewlyCreatedDBObject(line, true);

                    //This line will display 'Hello World' in AutoCAD command line.
                    editor.WriteMessage("\nThe line has been created successfullyy!");

                    //Commit the transaction
                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawPolyLine")]
        public void DrawPolyLine()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using(DocumentLock docLock = document.LockDocument())
            {
                using(Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Polyline polyline = new Polyline();



                    for (int i = 0; i < 4; i++)
                    {
                        PromptDoubleResult xRes = editor.GetDouble($"\nEnter X for point {i + 1}: ");
                        if (xRes.Status != PromptStatus.OK) return;

                        PromptDoubleResult yRes = editor.GetDouble($"\nEnter Y for point {i + 1}: ");
                        if (yRes.Status != PromptStatus.OK) return;

                        Point2d point = new Point2d(xRes.Value, yRes.Value);
                        polyline.AddVertexAt(i, point, 0, 0, 0);
                    }


                    polyline.Closed = true;

                    btr.AppendEntity(polyline);

                    transaction.AddNewlyCreatedDBObject(polyline, true);

                    editor.WriteMessage("\nPolyLine hasbeen drawn successfully!");

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

            PromptDoubleOptions pDoubleOpts = new PromptDoubleOptions("\nEnter a double value: ");

            PromptDoubleResult pDoubleRes = editor.GetDouble(pDoubleOpts);


            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Circle circle = new Circle();
                    circle.Radius = pDoubleRes.Value;

                    btr.AppendEntity(circle);

                    transaction.AddNewlyCreatedDBObject(circle, true);

                    editor.WriteMessage("\nCircle hasbeen drawn successfully!");

                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawGrid")]
        public void DrawGrid()
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

                    int rows = 5;
                    int cols = 5;
                    double cellWidth = 5;
                    double cellHeight = 5;

                   


                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            double x = j * cellWidth;
                            double y = i * cellHeight;

                         

                            Polyline polyline = new Polyline();
                            polyline.AddVertexAt(0, new Point2d(x, y), 0, 0, 0);
                            polyline.AddVertexAt(1, new Point2d(x + cellWidth, y), 0, 0, 0);
                            polyline.AddVertexAt(2, new Point2d(x + cellWidth, y + cellHeight), 0, 0, 0);
                            polyline.AddVertexAt(3, new Point2d(x, y + cellHeight), 0, 0, 0);
                            polyline.Closed = true;

                            btr.AppendEntity(polyline);
                            transaction.AddNewlyCreatedDBObject(polyline, true);


                        }
                    }

                    

                    editor.WriteMessage("\nGrid has been drawn successfully!");
                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawGridSimple")]
        public void DrawGridSimple()
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

                    int offset = 0;

                    // Horizontal lines (6 lines for 5 rows)
                    Polyline line1 = new Polyline();
                    line1.AddVertexAt(0, new Point2d(0, offset), 0, 0, 0);
                    line1.AddVertexAt(1, new Point2d(25, offset), 0, 0, 0);

                    Polyline line2 = new Polyline();
                    line2.AddVertexAt(0, new Point2d(0, offset + 5), 0, 0, 0);
                    line2.AddVertexAt(1, new Point2d(25, offset + 5), 0, 0, 0);

                    Polyline line3 = new Polyline();
                    line3.AddVertexAt(0, new Point2d(0, offset + 10), 0, 0, 0);
                    line3.AddVertexAt(1, new Point2d(25, offset + 10), 0, 0, 0);

                    Polyline line4 = new Polyline();
                    line4.AddVertexAt(0, new Point2d(0, offset + 15), 0, 0, 0);
                    line4.AddVertexAt(1, new Point2d(25, offset + 15), 0, 0, 0);

                    Polyline line5 = new Polyline();
                    line5.AddVertexAt(0, new Point2d(0, offset + 20), 0, 0, 0);
                    line5.AddVertexAt(1, new Point2d(25, offset + 20), 0, 0, 0);

                    Polyline line6 = new Polyline();
                    line6.AddVertexAt(0, new Point2d(0, offset + 25), 0, 0, 0);
                    line6.AddVertexAt(1, new Point2d(25, offset + 25), 0, 0, 0);

                    // Vertical lines (6 lines for 5 columns)
                    Polyline clone = new Polyline();
                    clone.AddVertexAt(0, new Point2d(offset, 0), 0, 0, 0);
                    clone.AddVertexAt(1, new Point2d(offset, 25), 0, 0, 0);

                    Polyline clone2 = new Polyline();
                    clone2.AddVertexAt(0, new Point2d(offset + 5, 0), 0, 0, 0);
                    clone2.AddVertexAt(1, new Point2d(offset + 5, 25), 0, 0, 0);

                    Polyline clone3 = new Polyline();
                    clone3.AddVertexAt(0, new Point2d(offset + 10, 0), 0, 0, 0);
                    clone3.AddVertexAt(1, new Point2d(offset + 10, 25), 0, 0, 0);

                    Polyline clone4 = new Polyline();
                    clone4.AddVertexAt(0, new Point2d(offset + 15, 0), 0, 0, 0);
                    clone4.AddVertexAt(1, new Point2d(offset + 15, 25), 0, 0, 0);

                    Polyline clone5 = new Polyline();
                    clone5.AddVertexAt(0, new Point2d(offset + 20, 0), 0, 0, 0);
                    clone5.AddVertexAt(1, new Point2d(offset + 20, 25), 0, 0, 0);

                    Polyline clone6 = new Polyline();
                    clone6.AddVertexAt(0, new Point2d(offset + 25, 0), 0, 0, 0);
                    clone6.AddVertexAt(1, new Point2d(offset + 25, 25), 0, 0, 0);

                    // Append horizontal lines
                    btr.AppendEntity(line1); transaction.AddNewlyCreatedDBObject(line1, true);
                    btr.AppendEntity(line2); transaction.AddNewlyCreatedDBObject(line2, true);
                    btr.AppendEntity(line3); transaction.AddNewlyCreatedDBObject(line3, true);
                    btr.AppendEntity(line4); transaction.AddNewlyCreatedDBObject(line4, true);
                    btr.AppendEntity(line5); transaction.AddNewlyCreatedDBObject(line5, true);
                    btr.AppendEntity(line6); transaction.AddNewlyCreatedDBObject(line6, true);

                    // Append vertical lines
                    btr.AppendEntity(clone); transaction.AddNewlyCreatedDBObject(clone, true);
                    btr.AppendEntity(clone2); transaction.AddNewlyCreatedDBObject(clone2, true);
                    btr.AppendEntity(clone3); transaction.AddNewlyCreatedDBObject(clone3, true);
                    btr.AppendEntity(clone4); transaction.AddNewlyCreatedDBObject(clone4, true);
                    btr.AppendEntity(clone5); transaction.AddNewlyCreatedDBObject(clone5, true);
                    btr.AppendEntity(clone6); transaction.AddNewlyCreatedDBObject(clone6, true);

                    editor.WriteMessage("\nGrid lines (6 horizontal and 6 vertical) drawn successfully.");
                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawGridSimple1")]
        public void DrawGrid1()
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

                    int offsetStep = 5;

                    // Base horizontal line at y=0
                    Polyline mainLine = new Polyline();
                    mainLine.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    mainLine.AddVertexAt(1, new Point2d(25, 0), 0, 0, 0);

                    // Clone and move each clone vertically
                    Polyline clone1 = (Polyline)mainLine.Clone();
                    clone1.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 1, 0)));

                    Polyline clone2 = (Polyline)mainLine.Clone();
                    clone2.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 2, 0)));

                    Polyline clone3 = (Polyline)mainLine.Clone();
                    clone3.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 3, 0)));

                    Polyline clone4 = (Polyline)mainLine.Clone();
                    clone4.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 4, 0)));

                    Polyline clone5 = (Polyline)mainLine.Clone();
                    clone5.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 5, 0)));

                    Polyline clone6 = (Polyline)mainLine.Clone();
                    clone6.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 0, 0)));

                    //Polyline clone7 = (Polyline)mainLine.Clone();
                    //clone7.TransformBy(Matrix3d.Displacement(new Vector3d(0, offsetStep * 7, 0)));

                    // Base vertical line at x=0
                    Polyline verticleLine = new Polyline();
                    verticleLine.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    verticleLine.AddVertexAt(1, new Point2d(0, 25), 0, 0, 0);

                    // Clone and move each clone horizontally (important: apply transform on vertical clones)
                    Polyline clone8 = (Polyline)verticleLine.Clone();
                    clone8.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 1, 0, 0)));

                    Polyline clone9 = (Polyline)verticleLine.Clone();
                    clone9.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 2, 0, 0)));

                    Polyline clone10 = (Polyline)verticleLine.Clone();
                    clone10.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 3, 0, 0)));

                    Polyline clone11 = (Polyline)verticleLine.Clone();
                    clone11.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 4, 0, 0)));

                    Polyline clone12 = (Polyline)verticleLine.Clone();
                    clone12.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 5, 0, 0)));

                    Polyline clone13 = (Polyline)verticleLine.Clone();
                    clone13.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 0, 0, 0)));

                    //Polyline clone14 = (Polyline)verticleLine.Clone();
                    //clone14.TransformBy(Matrix3d.Displacement(new Vector3d(offsetStep * 7, 0, 0)));

                    // Append horizontal lines
                    btr.AppendEntity(clone1); transaction.AddNewlyCreatedDBObject(clone1, true);
                    btr.AppendEntity(clone2); transaction.AddNewlyCreatedDBObject(clone2, true);
                    btr.AppendEntity(clone3); transaction.AddNewlyCreatedDBObject(clone3, true);
                    btr.AppendEntity(clone4); transaction.AddNewlyCreatedDBObject(clone4, true);
                    btr.AppendEntity(clone5); transaction.AddNewlyCreatedDBObject(clone5, true);
                    btr.AppendEntity(clone6); transaction.AddNewlyCreatedDBObject(clone6, true);
                    //btr.AppendEntity(clone7); transaction.AddNewlyCreatedDBObject(clone7, true);

                    // Append vertical lines
                    btr.AppendEntity(clone8); transaction.AddNewlyCreatedDBObject(clone8, true);
                    btr.AppendEntity(clone9); transaction.AddNewlyCreatedDBObject(clone9, true);
                    btr.AppendEntity(clone10); transaction.AddNewlyCreatedDBObject(clone10, true);
                    btr.AppendEntity(clone11); transaction.AddNewlyCreatedDBObject(clone11, true);
                    btr.AppendEntity(clone12); transaction.AddNewlyCreatedDBObject(clone12, true);
                    btr.AppendEntity(clone13); transaction.AddNewlyCreatedDBObject(clone13, true);
                    //btr.AppendEntity(clone14); transaction.AddNewlyCreatedDBObject(clone14, true);

                    editor.WriteMessage("\nGrid lines (7 horizontal and 7 vertical) drawn successfully.");
                    transaction.Commit();
                }
            }
        }

        [CommandMethod("DrawRectangleWithCross")]
        public void DrawRectangleWithCross()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Step 1: Get first point
            PromptPointResult ppr1 = ed.GetPoint("\nSelect first corner: ");
            if (ppr1.Status != PromptStatus.OK) return;
            Point3d pt1 = ppr1.Value;

            // Step 2: Get opposite corner
            PromptPointOptions ppo = new PromptPointOptions("\nSelect opposite corner: ")
            {
                BasePoint = pt1,
                UseBasePoint = true
            };

            PromptPointResult ppr2 = ed.GetPoint(ppo);
            if (ppr2.Status != PromptStatus.OK) return;
            Point3d pt2 = ppr2.Value;

            // Calculate all 4 corners
            double x1 = Math.Min(pt1.X, pt2.X);
            double y1 = Math.Min(pt1.Y, pt2.Y);
            double x2 = Math.Max(pt1.X, pt2.X);
            double y2 = Math.Max(pt1.Y, pt2.Y);

            Point2d bl = new Point2d(x1, y1);
            Point2d br = new Point2d(x2, y1);
            Point2d trPt = new Point2d(x2, y2);
            Point2d tl = new Point2d(x1, y2);

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Draw the rectangle
                Polyline rect = new Polyline();
                rect.AddVertexAt(0, bl, 0, 0, 0);
                rect.AddVertexAt(1, br, 0, 0, 0);
                rect.AddVertexAt(2, trPt, 0, 0, 0);
                rect.AddVertexAt(3, tl, 0, 0, 0);
                rect.Closed = true;

                btr.AppendEntity(rect);
                trans.AddNewlyCreatedDBObject(rect, true);

                // Draw first diagonal (bottom-left to top-right)
                Line diag1 = new Line(new Point3d(bl.X, bl.Y, 0), new Point3d(trPt.X, trPt.Y, 0));
                btr.AppendEntity(diag1);
                trans.AddNewlyCreatedDBObject(diag1, true);

                // Draw second diagonal (bottom-right to top-left)
                Line diag2 = new Line(new Point3d(br.X, br.Y, 0), new Point3d(tl.X, tl.Y, 0));
                btr.AppendEntity(diag2);
                trans.AddNewlyCreatedDBObject(diag2, true);

                trans.Commit();
            }

            ed.WriteMessage("\n✅ Rectangle with cross lines drawn successfully!");
        }







    }
}
