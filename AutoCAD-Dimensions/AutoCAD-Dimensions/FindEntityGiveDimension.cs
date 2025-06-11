using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Diagnostics.Metrics;

namespace AutoCAD_Dimensions
{
    public class FindEntityGiveDimension
    {
        [CommandMethod("AddDimension")]
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
    }
}


//if (entity is Line line)
//{
//    Point3d dimLinePoint = Point3d.Origin;

//    bool isHorizontal = line.StartPoint.Y == line.EndPoint.Y;
//    //bool isVertical = Math.Abs(angle - Math.PI / 2) < 1e-6;

//    if (isHorizontal)
//    {
//        dimLinePoint = new Point3d((line.StartPoint.X + line.EndPoint.X) / 2, line.StartPoint.Y + 50, 0);

//    }
//    else if(line.StartPoint.X == line.EndPoint.X)
//    {
//        dimLinePoint = new Point3d(line.StartPoint.X + 50, (line.StartPoint.Y + line.EndPoint.Y) / 2, 0);
//    }
//    else
//    {
//        continue;
//    }
//        double rotation = isHorizontal ? 0 : Math.PI / 2;
//        RotatedDimension dim = new RotatedDimension(
//            rotation, line.StartPoint, line.EndPoint, dimLinePoint, "", database.Dimstyle);
//        dim.Dimtxt = 40.0;
//        btr.AppendEntity(dim);
//        transaction.AddNewlyCreatedDBObject(dim, true);

//}