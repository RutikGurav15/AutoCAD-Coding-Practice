using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DocumentFormat.OpenXml.Bibliography;


namespace AutoCAD_Project.Core
{
    public class EntityData
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    class OptimizeDrawing
    {
        public List<EntityData> GetUnusedEntities()
        {
            List<EntityData> unusedEntities = new List<EntityData>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                    // Unused Blocks
                    int unusedBlocks = 0;
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!btr.IsLayout && !btr.IsAnonymous && !btr.IsDependent && btr.GetBlockReferenceIds(true, true).Count == 0)
                            unusedBlocks++;
                    }
                    unusedEntities.Add(new EntityData { Name = "Block (Unused)", Count = unusedBlocks });

                    // Frozen/Off Layers
                    int unusedFrozenLayers = 0;
                    foreach (ObjectId layerId in lt)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);
                        if (ltr.IsFrozen || ltr.IsOff)
                            unusedFrozenLayers++;
                    }
                    unusedEntities.Add(new EntityData { Name = "Layer (Frozen/Off)", Count = unusedFrozenLayers });

                    // Zero-length lines, empty texts, invisible entities
                    int zeroLengthLines = 0, emptyTexts = 0, invisibleEntities = 0;

                    foreach (ObjectId entId in ms)
                    {
                        Entity ent = (Entity)tr.GetObject(entId, OpenMode.ForRead);
                        if (ent is Line line && line.Length == 0)
                            zeroLengthLines++;
                        if (ent is DBText text && string.IsNullOrWhiteSpace(text.TextString))
                            emptyTexts++;
                        if (ent is MText mtext && string.IsNullOrWhiteSpace(mtext.Contents))
                            emptyTexts++;
                        if (!ent.Visible)
                            invisibleEntities++;
                    }

                    unusedEntities.Add(new EntityData { Name = "Zero-Length Lines", Count = zeroLengthLines });
                    unusedEntities.Add(new EntityData { Name = "Empty Texts", Count = emptyTexts });
                    unusedEntities.Add(new EntityData { Name = "Invisible Entities", Count = invisibleEntities });

                    // Unused External References (XRefs)
                    int unusedXRefs = 0;
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (btr.IsFromExternalReference && btr.GetBlockReferenceIds(true, true).Count == 0)
                            unusedXRefs++;
                    }
                    unusedEntities.Add(new EntityData { Name = "External References (Unused)", Count = unusedXRefs });

                    // Used Linetypes
                    // Collect all used linetypes across ALL block table records (ModelSpace, PaperSpace, Blocks)
                    HashSet<ObjectId> usedLinetypes = new HashSet<ObjectId>();

                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);

                        foreach (ObjectId entId in btr)
                        {
                            if (!entId.IsErased)
                            {
                                Entity ent = (Entity) tr.GetObject(entId, OpenMode.ForRead);
                                if (ent != null)
                                    usedLinetypes.Add(ent.LinetypeId);
                            }
                        }
                    }

                    // Count and collect unused linetypes
                    LinetypeTable ltt = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                    int unusedLinetypes = 0;

                    foreach (ObjectId ltId in ltt)
                    {
                        if (!usedLinetypes.Contains(ltId))
                        {
                            LinetypeTableRecord ltr = (LinetypeTableRecord)tr.GetObject(ltId, OpenMode.ForRead);
                            if (!ltr.IsDependent &&
                                !ltr.Name.Equals("ByLayer", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("ByBlock", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("Continuous", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CENTER", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CENTER2", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CONTINU", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("DASHED", StringComparison.OrdinalIgnoreCase))
                            {
                                unusedLinetypes++;
                            }
                        }
                    }

                    unusedEntities.Add(new EntityData
                    {
                        Name = "Linetypes (Unused)",
                        Count = unusedLinetypes
                    });


                    // Used Text Styles
                    HashSet<ObjectId> usedTextStyles = new HashSet<ObjectId>();
                    foreach (ObjectId entId in ms)
                    {
                        Entity ent = (Entity)tr.GetObject(entId, OpenMode.ForRead);
                        if (ent is DBText dbText)
                            usedTextStyles.Add(dbText.TextStyleId);
                        else if (ent is MText mtext)
                            usedTextStyles.Add(mtext.TextStyleId);
                    }

                    TextStyleTable tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    int unusedTextStyles = 0;
                    foreach (ObjectId tsId in tst)
                    {
                        if (!usedTextStyles.Contains(tsId))
                            unusedTextStyles++;
                    }
                    unusedEntities.Add(new EntityData { Name = "Text Styles (Unused)", Count = unusedTextStyles });

                    // Used Dimension Styles
                    HashSet<ObjectId> usedDimStyles = new HashSet<ObjectId>();
                    foreach (ObjectId entId in ms)
                    {
                        if (tr.GetObject(entId, OpenMode.ForRead) is Dimension dim)
                            usedDimStyles.Add(dim.DimensionStyle);
                    }

                    DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    int unusedDimStyles = 0;
                    foreach (ObjectId dimId in dst)
                    {
                        if (!usedDimStyles.Contains(dimId))
                            unusedDimStyles++;
                    }
                    unusedEntities.Add(new EntityData { Name = "Dimension Styles (Unused)", Count = unusedDimStyles });

                    // Used MLeader Styles
                    HashSet<ObjectId> usedMLeaderStyleIds = new HashSet<ObjectId>();
                    foreach (ObjectId entId in ms)
                    {
                        if (entId.ObjectClass.DxfName == "MULTILEADER")
                        {
                            MLeader mleader = (MLeader)tr.GetObject(entId, OpenMode.ForRead);
                            if (mleader != null)
                                usedMLeaderStyleIds.Add(mleader.MLeaderStyle);
                        }
                    }

                    DBDictionary mleaderDict = (DBDictionary)tr.GetObject(db.MLeaderStyleDictionaryId, OpenMode.ForRead);
                    int unusedMLeaderStyles = 0;
                    foreach (DBDictionaryEntry entry in mleaderDict)
                    {
                        if (!usedMLeaderStyleIds.Contains(entry.Value))
                            unusedMLeaderStyles++;
                    }
                    unusedEntities.Add(new EntityData { Name = "MLeader Styles (Unused)", Count = unusedMLeaderStyles });

                    // Hatch Patterns
                    int hatchIssues = 0;
                    foreach (ObjectId entId in ms)
                    {
                        if (entId.ObjectClass.DxfName == "HATCH")
                        {
                            try
                            {
                                Hatch hatch = (Hatch)tr.GetObject(entId, OpenMode.ForRead);
                                bool isInvalid = false;
                                try { if (hatch.Area == 0) isInvalid = true; } catch { }
                                try { if (hatch.PatternType == HatchPatternType.CustomDefined && hatch.PatternName.Length > 50) isInvalid = true; } catch { }
                                if (isInvalid) hatchIssues++;
                            }
                            catch { }
                        }
                    }
                    unusedEntities.Add(new EntityData { Name = "Hatch Patterns (Invalid/Complex)", Count = hatchIssues });

                    tr.Commit();
                }
            }

            return unusedEntities;
        }



        public void CleanUnusedEntities()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    // === 1. Gather Usage Info ===

                    HashSet<ObjectId> usedTextStyles = new HashSet<ObjectId>();
                    HashSet<ObjectId> usedDimStyles = new HashSet<ObjectId>();
                    HashSet<ObjectId> usedMLeaderStyleIds = new HashSet<ObjectId>();
                    HashSet<ObjectId> usedLinetypes = new HashSet<ObjectId>();

                    foreach (ObjectId entId in ms)
                    {
                        Entity ent = (Entity)tr.GetObject(entId, OpenMode.ForRead);

                        if (ent is DBText dbText)
                            usedTextStyles.Add(dbText.TextStyleId);
                        else if (ent is MText mtext)
                            usedTextStyles.Add(mtext.TextStyleId);

                        if (ent is Dimension dim)
                            usedDimStyles.Add(dim.DimensionStyle);

                        if (ent.GetType().Name == "MLeader")
                        {
                            MLeader mleader = (MLeader)tr.GetObject(entId, OpenMode.ForRead);
                            usedMLeaderStyleIds.Add(mleader.MLeaderStyle);
                        }

                        usedLinetypes.Add(ent.LinetypeId);
                    }

                    // === 2. Clean Unused Blocks ===
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!btr.IsLayout && !btr.IsAnonymous && !btr.IsDependent && btr.GetBlockReferenceIds(true, true).Count == 0)
                        {
                            btr.UpgradeOpen();
                            btr.Erase();
                        }
                    }

                    // === 3. Clean Frozen/Off Layers ===
                    foreach (ObjectId layerId in lt)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);
                        if ((ltr.IsFrozen || ltr.IsOff) && !ltr.IsErased && !ltr.IsDependent && !ltr.Name.Equals("0"))
                        {
                            try
                            {
                                ltr.UpgradeOpen();
                                ltr.Erase();
                            }
                            catch { /* Ignore if cannot be erased */ }
                        }
                    }

                    // === 4. Clean zero-length, empty or invisible entities ===
                    foreach (ObjectId entId in ms)
                    {
                        Entity ent = (Entity)tr.GetObject(entId, OpenMode.ForRead);
                        bool toErase = false;

                        if (ent is Line line && line.Length == 0)
                            toErase = true;
                        else if (ent is DBText text && string.IsNullOrWhiteSpace(text.TextString))
                            toErase = true;
                        else if (ent is MText mtext && string.IsNullOrWhiteSpace(mtext.Contents))
                            toErase = true;
                        else if (!ent.Visible)
                            toErase = true;

                        if (toErase)
                        {
                            ent.UpgradeOpen();
                            ent.Erase();
                        }
                    }

                    // === 5. Clean unused XRefs ===
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (btr.IsFromExternalReference && btr.GetBlockReferenceIds(true, true).Count == 0)
                        {
                            btr.UpgradeOpen();
                            btr.Erase();
                        }
                    }

                    // === 6. Clean unused Linetypes ===
                    LinetypeTable ltt = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                    foreach (ObjectId ltId in ltt)
                    {
                        if (!usedLinetypes.Contains(ltId))
                        {
                            LinetypeTableRecord ltr = (LinetypeTableRecord)tr.GetObject(ltId, OpenMode.ForWrite);

                            if (!ltr.IsDependent &&
                                !ltr.Name.Equals("ByLayer", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("ByBlock", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("Continuous", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CENTER", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CENTER2", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("CONTINU", StringComparison.OrdinalIgnoreCase) &&
                                !ltr.Name.Equals("DASHED", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    ltr.Erase();
                                }
                                catch
                                {
                                    // Some built-in or system linetypes can't be erased
                                }
                            }
                        }
                    }


                    // === 7. Clean unused TextStyles ===
                    TextStyleTable tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    foreach (ObjectId tsId in tst)
                    {
                        if (!usedTextStyles.Contains(tsId))
                        {
                            TextStyleTableRecord ts = (TextStyleTableRecord)tr.GetObject(tsId, OpenMode.ForWrite);
                            if (!ts.IsDependent && !ts.Name.Equals("Standard"))
                            {
                                try { ts.Erase(); } catch { }
                            }
                        }
                    }

                    // === 8. Clean unused DimStyles ===
                    DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    foreach (ObjectId dsId in dst)
                    {
                        if (!usedDimStyles.Contains(dsId))
                        {
                            DimStyleTableRecord ds = (DimStyleTableRecord)tr.GetObject(dsId, OpenMode.ForWrite);
                            if (!ds.IsDependent && !ds.Name.Equals("Standard"))
                            {
                                try { ds.Erase(); } catch { }
                            }
                        }
                    }

                    // === 9. Clean unused MLeader Styles ===
                    DBDictionary mlDict = (DBDictionary)tr.GetObject(db.MLeaderStyleDictionaryId, OpenMode.ForRead);
                    foreach (DBDictionaryEntry entry in mlDict)
                    {
                        if (!usedMLeaderStyleIds.Contains(entry.Value))
                        {
                            DBObject obj = tr.GetObject(entry.Value, OpenMode.ForWrite, false);
                            if (obj != null && obj is MLeaderStyle mlStyle && !mlStyle.Name.Equals("Standard"))
                            {
                                try { mlStyle.Erase(); } catch { }
                            }
                        }
                    }

                    // === 10. Clean invalid/complex Hatches ===
                    foreach (ObjectId entId in ms)
                    {
                        if (entId.ObjectClass.DxfName == "HATCH")
                        {
                            try
                            {
                                Hatch hatch = (Hatch)tr.GetObject(entId, OpenMode.ForRead);
                                bool shouldErase = false;

                                try { if (hatch.Area == 0) shouldErase = true; } catch { }
                                try { if (hatch.PatternType == HatchPatternType.CustomDefined && hatch.PatternName.Length > 50) shouldErase = true; } catch { }

                                if (shouldErase)
                                {
                                    hatch.UpgradeOpen();
                                    hatch.Erase();
                                }
                            }
                            catch { }
                        }
                    }

                    tr.Commit();
                }
            }

            doc.Editor.WriteMessage("\nUnused or invalid entities have been cleaned.");
        }

    }
}
