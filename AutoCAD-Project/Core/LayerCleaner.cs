using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCAD_Project.Core
{
    public class LayerData
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    class LayerCleaner
    {
        public List<LayerData> GetLayerUsageData()
        {
            Dictionary<string, int> layerCounts = new Dictionary<string, int>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                foreach (ObjectId objId in ms)
                {
                    Entity entity = (Entity) tr.GetObject(objId, OpenMode.ForRead);
                    if (entity != null)
                    {
                        string layerName = entity.Layer;

                        if (layerCounts.ContainsKey(layerName))
                            layerCounts[layerName]++;
                        else
                            layerCounts[layerName] = 1;
                    }
                }

                tr.Commit();
            }

            return layerCounts.Select(kvp => new LayerData { Name = kvp.Key, Count = kvp.Value }).ToList();
        }

        public void CleanUnusedAndFrozenLayers()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                foreach (ObjectId layerId in lt)
                {
                    LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);

                    if (!ltr.IsErased && !ltr.IsDependent && ltr.Name != "0" && ltr.Name != db.Clayer.ToString())
                    {
                        bool isFrozen = ltr.IsFrozen;
                        bool isUsed = IsLayerUsed(db, tr, ltr.Name);

                        if (!isUsed || isFrozen)
                        {
                            ltr.UpgradeOpen();
                            ltr.Erase();
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage("\nUnused and frozen layers cleaned successfully.");
        }

        private bool IsLayerUsed(Database db, Transaction tr, string layerName)
        {
            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

            foreach (ObjectId objId in ms)
            {
                Entity entity =(Entity) tr.GetObject(objId, OpenMode.ForRead);
                if (entity != null && entity.Layer == layerName)
                    return true;
            }

            return false;
        }
    }
}

