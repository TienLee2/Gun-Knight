using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using QuadroRendererSystem;
using VladislavTsurikov;

namespace MegaWorld
{
    public enum OverlapShape 
    { 
        None, 
        Bounds,
        Sphere,
    }
    
    [Serializable]
    public class OverlapCheckSettings
    {
        public OverlapShape OverlapShape = OverlapShape.Sphere;
        public BoundsCheck BoundsCheck = new BoundsCheck();
        public SphereCheck SphereCheck = new SphereCheck();
        public CollisionCheck CollisionCheck = new CollisionCheck();

#if UNITY_EDITOR
        public OverlapCheckSettingsEditor OverlapСheckTypeEditor = new OverlapCheckSettingsEditor();

        public void OnGUI()
        {
            OverlapСheckTypeEditor.OnGUI(this);
        }
#endif

        public OverlapCheckSettings()
        {

        }

        public OverlapCheckSettings(OverlapCheckSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(OverlapCheckSettings other)
        {            
            OverlapShape = other.OverlapShape;

            BoundsCheck = new BoundsCheck(other.BoundsCheck);
            SphereCheck = new SphereCheck(other.SphereCheck);
            CollisionCheck = new CollisionCheck(other.CollisionCheck);
        }

        public bool RunOverlapCheckForQuadroItem(PrototypeQuadroItem proto, InstanceData instanceData)
        {
            if(RunCollisionCheck(proto.Extents, instanceData))
            {
                return false;
            }

            if(OverlapShape == OverlapShape.Bounds)
            {
                Bounds bounds = BoundsCheck.GetBounds(proto.OverlapCheckSettings.BoundsCheck, instanceData.position, instanceData.scale, proto.Extents);
                
                if(!RunOverlapCheckForQuadroItem(bounds))
                {
                    return true;
                }
            }
            else if(OverlapShape == OverlapShape.Sphere)
            {
                Bounds bounds = SphereCheck.GetBounds(proto.OverlapCheckSettings.SphereCheck, instanceData.position, instanceData.scale);

                if(!RunOverlapCheckForQuadroItem(bounds))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public bool RunOverlapCheckForGameObject(PrototypeGameObject proto, InstanceData instanceData)
        {
            if(RunCollisionCheck(proto.Extents, instanceData))
            {
                return false;
            }

            if(OverlapShape == OverlapShape.Bounds)
            {
                Bounds bounds = BoundsCheck.GetBounds(proto.OverlapCheckSettings.BoundsCheck, instanceData.position, instanceData.scale, proto.Extents);
                
                if(!RunOverlapCheckForGameObject(bounds))
                {
                    return true;
                }
            }
            else if(OverlapShape == OverlapShape.Sphere)
            {
                Bounds bounds = SphereCheck.GetBounds(proto.OverlapCheckSettings.SphereCheck, instanceData.position, instanceData.scale);

                if(!RunOverlapCheckForGameObject(bounds))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public bool RunOverlapCheckForQuadroItem(Bounds checkBounds)
        {
            List<Cell> overlapCellList = new List<Cell>();  
                           
            QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellQuadTree.Query(RectExtension.CreateRectFromBounds(checkBounds), overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                List<ItemInfo> persistentInfoList = QuadroRendererController.QuadroRenderer.StorageTerrainCells.PersistentStoragePackage.CellList[overlapCellList[i].Index].ItemInfoList;
                
                for (int infoIndex = 0; infoIndex < persistentInfoList.Count; infoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[infoIndex];

                    PrototypeQuadroItem prototypeQuadroItem = AllAvailableTypes.GetCurrentQuadroItem(persistentInfo.ID);

                    if(prototypeQuadroItem == null)
                    {
                        continue;
                    }

                    if(prototypeQuadroItem.OverlapCheckSettings.OverlapShape == OverlapShape.None)
                    {
                        continue;
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        QuadroRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];
                        
                        if(OverlapCheckForQuadroItem(prototypeQuadroItem, persistentItem, checkBounds))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        private bool RunOverlapCheckForGameObject(Bounds checkBounds)
        {
            bool overlaps = false;

            MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(checkBounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                if(proto == null)
                {
                    return true;
                }

                if(proto.OverlapCheckSettings.OverlapShape == OverlapShape.None)
                {
                    return true;
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = Utility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        continue;
                    }

                    if(OverlapCheckForGameObject(proto, prefabRoot, checkBounds))
                    {
                        overlaps = true;
                        return false;
                    }
                }

                return true;
            });

            return overlaps;
        }

        private bool OverlapCheckForGameObject(PrototypeGameObject proto, GameObject go, Bounds checkBounds)
        {
            OverlapCheckSettings localIntersectSettings = proto.OverlapCheckSettings;

            switch (localIntersectSettings.OverlapShape)
            {
                case OverlapShape.Bounds:
                {
                    return BoundsCheck.OverlapCheck(proto.OverlapCheckSettings.BoundsCheck, proto.Extents, go.transform.position, go.transform.localScale, checkBounds);
                }
                case OverlapShape.Sphere:
                {
                    return SphereCheck.OverlapCheck(proto.OverlapCheckSettings.SphereCheck, go.transform.position, checkBounds);
                }
            }

            return false;
        }

        public bool OverlapCheckForQuadroItem(PrototypeQuadroItem proto, QuadroRendererSystem.InstanceData persistentItem, Bounds checkBounds)
        {
            OverlapCheckSettings localIntersectSettings = proto.OverlapCheckSettings;

            switch (localIntersectSettings.OverlapShape)
            {
                case OverlapShape.Bounds:
                {
                    return BoundsCheck.OverlapCheck(proto.OverlapCheckSettings.BoundsCheck, proto.Extents, persistentItem.Position, persistentItem.Scale, checkBounds);
                }
                case OverlapShape.Sphere:
                {
                    return SphereCheck.OverlapCheck(proto.OverlapCheckSettings.SphereCheck, persistentItem.Position, checkBounds);
                }
            }

            return false;
        }

        public bool RunCollisionCheck(Vector3 prefabExtents, InstanceData instanceData)
        {
            if(CollisionCheck.collisionCheckType)
            {
                Vector3 extents = Vector3.Scale(prefabExtents * CollisionCheck.multiplyBoundsSize, instanceData.scale);

                if(CollisionCheck.IsBoundHittingWithCollisionsLayers(instanceData.position, instanceData.rotation.eulerAngles.y, extents))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        public static void VisualizeOverlapForQuadroItem(Bounds bounds, bool showSelectedProto = false)
        {
            if (QuadroRendererController.CheckForMissingOfData() == false) return;

            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                List<ItemInfo> persistentInfoList = QuadroRendererController.QuadroRenderer.StorageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    PrototypeQuadroItem prototypeQuadroItem = AllAvailableTypes.GetCurrentQuadroItem(persistentInfo.ID);

                    if(prototypeQuadroItem == null)
                    {
                        continue;
                    }

                    if(showSelectedProto)
                    {
                        if(prototypeQuadroItem.selected == false)
                        {
                            continue;
                        }
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        QuadroRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];

                        if(bounds.Contains(persistentItem.Position) == true)
                        {
                            DrawOverlapСheckType(persistentItem.Position, persistentItem.Scale, prototypeQuadroItem.Extents, prototypeQuadroItem.OverlapCheckSettings);
                        }
                    }
                }
            }
        }

        public static void VisualizeOverlapForGameObject(Bounds bounds, bool showSelectedProto = false)
        {
            MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(bounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                if(proto == null)
                {
                    return true;
                }

                if(showSelectedProto)
                {
                    if(proto.selected == false)
                    {
                        return true;
                    }
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = Utility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        continue;
                    }

                    if(bounds.Contains(prefabRoot.transform.position) == false)
                    {
                        continue;
                    }

                    DrawOverlapСheckType(prefabRoot.transform.position, prefabRoot.transform.localScale, proto.Extents, proto.OverlapCheckSettings);
                }

                return true;
            });
        }

        public static void DrawOverlapСheckType(Vector3 position, Vector3 scale, Vector3 extents, OverlapCheckSettings overlapCheckSettings)
        {
            switch (overlapCheckSettings.OverlapShape)
            {
                case OverlapShape.Sphere:
                {
                    SphereCheck.DrawOverlapСheck(position, overlapCheckSettings.SphereCheck);

                    break;
                }
                case OverlapShape.Bounds:
                {
                    BoundsCheck.DrawIntersectionСheckType(position, scale, extents, overlapCheckSettings.BoundsCheck);
                    
                    break;
                }
            }
        }
#endif
    }
}