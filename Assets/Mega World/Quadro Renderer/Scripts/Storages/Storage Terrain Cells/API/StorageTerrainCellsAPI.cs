using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov;
using System;

namespace QuadroRendererSystem
{
    public static class StorageTerrainCellsAPI 
    {
        public static void RemoveItemInstances(StorageTerrainCells storageTerrainCells, int ID)
        {
            if(storageTerrainCells == null)
            {
                return;
            }
            
            storageTerrainCells.PersistentStoragePackage.RemoveItemInstances(storageTerrainCells.QuadroRenderer, ID);            
        }

        public static void AddItemInstance(StorageTerrainCells storageTerrainCells, int ID, Vector3 worldPosition, Vector3 scale, Quaternion rotation)
        {
            if(storageTerrainCells == null)
            {
                return;
            }

            storageTerrainCells.AddItemInstance(ID, worldPosition, scale, rotation);
        }

        public static void IntersectBounds(StorageTerrainCells storageTerrainCells, Bounds bounds, Func<ItemInfo, int, bool> func)
        {
            if(storageTerrainCells == null)
            {
                return;
            }

            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            storageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                List<ItemInfo> persistentInfoList = storageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    func.Invoke(persistentInfo, cellIndex);
                }
            }
        }

        public static void IntersectBounds(StorageTerrainCells storageTerrainCells, Bounds bounds, bool convertPersistentItemToObjectInstanced, bool removeAfterConvert, Func<ItemInfo, int, bool> func)
        {
            if(storageTerrainCells == null)
            {
                return;
            }

            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            storageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                storageTerrainCells.CellModifier.AddModifiedСell(overlapCellList[i], convertPersistentItemToObjectInstanced, removeAfterConvert);

                List<ItemInfo> persistentInfoList = storageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    func.Invoke(persistentInfo, cellIndex);
                }
            }
        }

        public static void RepositionObject(StorageTerrainCells storageTerrainCells, ObjectInstanceData obj, int ID, int cellIndex)
        {
            if(storageTerrainCells == null)
            {
                return;
            }

            storageTerrainCells.CellModifier.RemoveAfterConvert = true;

            foreach (ItemInfo persistentInfo in storageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList)
            {
                if(persistentInfo.ID == ID)
                {
                    persistentInfo.ObjectInstanceData.Remove(obj);
                    break;
                }
            }

            storageTerrainCells.ConvertModifiedСellsPersistentStorageToRenderData();

            storageTerrainCells.AddItemInstance(ID, obj.Position, obj.Scale, obj.Rotation);

            storageTerrainCells.ConvertModifiedСellsPersistentStorageToRenderData();
        }
    }
}
