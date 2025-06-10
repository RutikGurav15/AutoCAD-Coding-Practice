using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace WPF_FORM
{
    public class Entities
    {
        public static void DrawLine(string start, string end)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var startCoords = start.Split(',');
            var endCoords = end.Split(',');

            Point3d pt1 = new Point3d(
                double.Parse(startCoords[0]),
                double.Parse(startCoords[1]),
                0);

            Point3d pt2 = new Point3d(
                double.Parse(endCoords[0]),
                double.Parse(endCoords[1]),
                0);

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Line line = new Line(pt1, pt2);
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);

                    tr.Commit();
                }
            }
        }

        public static void DrawPolyline(string start, string mid, string end)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var startCoords = start.Split(',');
            var midCoords = mid.Split(',');
            var endCoords = end.Split(',');

            Point2d pt1 = new Point2d(double.Parse(startCoords[0]), double.Parse(startCoords[1]));
            Point2d pt2 = new Point2d(double.Parse(midCoords[0]), double.Parse(midCoords[1]));
            Point2d pt3 = new Point2d(double.Parse(endCoords[0]), double.Parse(endCoords[1]));

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Polyline poly = new Polyline();
                    poly.AddVertexAt(0, pt1, 0, 0, 0);
                    poly.AddVertexAt(1, pt2, 0, 0, 0);
                    poly.AddVertexAt(2, pt3, 0, 0, 0);

                    btr.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);
                    tr.Commit();
                }
            }
        }

        public static void DrawCircle(string center, double radius)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var coords = center.Split(',');
            Point3d centerPt = new Point3d(
                double.Parse(coords[0]),
                double.Parse(coords[1]),
                0);

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Circle circle = new Circle(centerPt, Vector3d.ZAxis, radius);
                    btr.AppendEntity(circle);
                    tr.AddNewlyCreatedDBObject(circle, true);

                    tr.Commit();
                }
            }
        }

        public static void DrawRectangle(double height, double width)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    Polyline rect = new Polyline(4);
                    rect.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    rect.AddVertexAt(1, new Point2d(width, 0), 0, 0, 0);
                    rect.AddVertexAt(2, new Point2d(width, height), 0, 0, 0);
                    rect.AddVertexAt(3, new Point2d(0, height), 0, 0, 0);
                    rect.Closed = true;

                    btr.AppendEntity(rect);
                    tr.AddNewlyCreatedDBObject(rect, true);

                    tr.Commit();
                }
            }
        }

        public static void DrawArc(string center, double radius, double startAngle, double endAngle)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            {
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);

                    string[] parts = center.Split(',');
                    if (parts.Length != 2 ||
                        !double.TryParse(parts[0], out double x) ||
                        !double.TryParse(parts[1], out double y))
                    {
                        editor.WriteMessage("\nInvalid center coordinates.");
                        return;
                    }

                    Point3d centerPoint = new Point3d(x, y, 0);
                    Arc arc = new Arc(centerPoint, radius,
                                      startAngle * (Math.PI / 180.0),
                                      endAngle * (Math.PI / 180.0));

                    btr.AppendEntity(arc);
                    transaction.AddNewlyCreatedDBObject(arc, true);

                    transaction.Commit();
                }
            }
        }

        public static void FindEntityGiveDimensionCommand()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            using (DocumentLock docLock = document.LockDocument())
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (ObjectId objId in btr)
                {
                    Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForRead);
                    if (entity == null) continue;

                    if (entity is Line line)
                    {
                        Vector3d delta = line.Delta;
                        double angle = delta.GetAngleTo(Vector3d.XAxis);
                        Vector3d offsetDir = delta.GetPerpendicularVector().GetNormal() * 100;
                        Point3d dimLinePoint = line.StartPoint + offsetDir;

                        bool isHorizontal = Math.Abs(angle) < 1e-6 || Math.Abs(angle - Math.PI) < 1e-6;
                        bool isVertical = Math.Abs(angle - Math.PI / 2) < 1e-6;

                        if (isHorizontal || isVertical)
                        {
                            double rotation = isHorizontal ? 0 : Math.PI / 2;
                            RotatedDimension dim = new RotatedDimension(
                                rotation, line.StartPoint, line.EndPoint, dimLinePoint, "", database.Dimstyle);
                            dim.Dimtxt = 25;
                            btr.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                        }
                        else
                        {
                            AlignedDimension dim = new AlignedDimension(
                                line.StartPoint, line.EndPoint, dimLinePoint, "", database.Dimstyle);
                            dim.Dimtxt = 25;
                            btr.AppendEntity(dim);
                            transaction.AddNewlyCreatedDBObject(dim, true);
                        }
                    }
                    else if (entity is Polyline pline)
                    {
                        for (int i = 0; i < pline.NumberOfVertices - 1; i++)
                        {
                            Point3d pt1 = pline.GetPoint3dAt(i);
                            Point3d pt2 = pline.GetPoint3dAt(i + 1);
                            Vector3d delta = pt2 - pt1;
                            double angle = delta.GetAngleTo(Vector3d.XAxis);
                            Vector3d offsetDir = delta.GetPerpendicularVector().GetNormal() * 100;
                            Point3d dimLinePoint = pt1 + offsetDir;

                            bool isHorizontal = Math.Abs(angle) < 1e-6 || Math.Abs(angle - Math.PI) < 1e-6;
                            bool isVertical = Math.Abs(angle - Math.PI / 2) < 1e-6;

                            if (isHorizontal || isVertical)
                            {
                                double rotation = isHorizontal ? 0 : Math.PI / 2;
                                RotatedDimension dim = new RotatedDimension(
                                    rotation, pt1, pt2, dimLinePoint, "", database.Dimstyle);
                                dim.Dimtxt = 25;
                                btr.AppendEntity(dim);
                                transaction.AddNewlyCreatedDBObject(dim, true);
                            }
                            else
                            {
                                AlignedDimension dim = new AlignedDimension(
                                    pt1, pt2, dimLinePoint, "", database.Dimstyle);
                                dim.Dimtxt = 25;
                                btr.AppendEntity(dim);
                                transaction.AddNewlyCreatedDBObject(dim, true);
                            }
                        }
                    }
                    else if (entity is Circle circle)
                    {
                        Point3d center = circle.Center;
                        double radius = circle.Radius;
                        Point3d radiusPoint = center + (Vector3d.XAxis * radius);

                        RadialDimension radDim = new RadialDimension(
                            center, radiusPoint, radius, "", database.Dimstyle);
                        radDim.Dimtxt = 25;
                        btr.AppendEntity(radDim);
                        transaction.AddNewlyCreatedDBObject(radDim, true);
                    }
                    else if (entity is Arc arc)
                    {
                        Point3d center = arc.Center;
                        double radius = arc.Radius;
                        double angle = arc.EndAngle;

                        Point3d radiusPoint = center + new Vector3d(
                            Math.Cos(angle) * radius, Math.Sin(angle) * radius, 0);

                        RadialDimension radDim = new RadialDimension(
                            center, radiusPoint, radius, "", database.Dimstyle);
                        radDim.Dimtxt = 25;
                        btr.AppendEntity(radDim);
                        transaction.AddNewlyCreatedDBObject(radDim, true);
                    }

                }

                transaction.Commit();
            }
        }

        //public static void CreateLayer(string layerName, short colorIndex)
        //{
        //    Document document = Application.DocumentManager.MdiActiveDocument;
        //    Database database = document.Database;
        //    Editor editor = document.Editor;

        //    using (DocumentLock docLock = document.LockDocument())
        //    {
        //        using(Transaction transaction = database.TransactionManager.StartTransaction())
        //        {
        //            BlockTable bt = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
        //            BlockTableRecord btr = (BlockTableRecord)transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

        //            LayerTable lt = (LayerTable) transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

        //            if (!lt.Has(layerName))
        //            {
        //                lt.UpgradeOpen();
        //                LayerTableRecord ltr = new LayerTableRecord
        //                {
        //                    Name = layerName,
        //                    Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, colorIndex)
        //                };
        //                lt.Add(ltr);
        //                transaction.AddNewlyCreatedDBObject(ltr, true);
        //            }

        //            editor.WriteMessage(layerName+" is  Created!!");

        //            transaction.Commit();
        //        }
        //    }
        //}
        public static void CreateLayer(string layerName, short colorIndex, string entityType)
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

                    LayerTable lt = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);
                    ObjectId layerId;

                    if (!lt.Has(layerName))
                    {
                        lt.UpgradeOpen();
                        LayerTableRecord ltr = new LayerTableRecord
                        {
                            Name = layerName,
                            Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, colorIndex)
                        };
                        layerId = lt.Add(ltr);
                        transaction.AddNewlyCreatedDBObject(ltr, true);
                    }
                    else
                    {
                        layerId = lt[layerName];
                    }

                    foreach(ObjectId objId in btr)
                    {
                        Entity entity = (Entity) transaction.GetObject(objId, OpenMode.ForWrite);

                        if(IsEntityMatch(entity, entityType))
                        {
                            entity.LayerId = layerId;
                        }
                    }

                    editor.WriteMessage(layerName + " is  Created!!");

                    transaction.Commit();
                }
            }
        }
        private static bool IsEntityMatch(Entity entity, string entityType)
        {
            switch (entityType)
            {
                case "Line":
                    return entity is Line;
                case "Polyline":
                    return entity is Polyline;
                case "Circle":
                    return entity is Circle;
                case "Rectangle":
                    return entity is Polyline poly && poly.Closed && poly.NumberOfVertices == 4;
                case "Arc":
                    return entity is Arc;
                default:
                    return false;
            }
        }

        public static void RemoveLayerCommand()
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
                        Entity entity = (Entity)transaction.GetObject(objId, OpenMode.ForWrite);

                        if (entity != null)
                        {
                            entity.Layer = "0";
                        }
                    }

                    transaction.Commit();
                }
            }
        }


        [CommandMethod("Draw")]
        public void Draw()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
