using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public partial class StorageTerrainCells
    {
        public List<Cell> GetVisibleCellList(СameraCellOcclusionCulling cameraCellOcclusionCulling)
        {
            cameraCellOcclusionCulling.ScheduleCull(QuadroRenderer, CellQuadTree, CellSize, QuadroRenderer.FloatingOriginOffset, false);
            
            List<Cell> visibleCellList = new List<Cell>();   
            for (int j = 0; j < cameraCellOcclusionCulling.JobOcclusionCulling.VisibleCellIndexList.Length; j++)
            {
                int potentialVisibleCellIndex = cameraCellOcclusionCulling.JobOcclusionCulling.VisibleCellIndexList[j];
                Cell cell = cameraCellOcclusionCulling.PotentialVisibleCellList[potentialVisibleCellIndex];

                BoundingSphereInfo boundingSphereInfo = cameraCellOcclusionCulling.GetBoundingSphereInfo(potentialVisibleCellIndex);

                if(boundingSphereInfo.Visibility == false)
                {
                    continue;
                }

                visibleCellList.Add(cell);
            }

            return visibleCellList;
        }
        
        public void CreateCells()
        {
            DisposeCells();
            PersistentStoragePackage.CellList.Clear(); 

            List<Cell> cellList = PersistentStoragePackage.CellList;
            
            Bounds expandedBounds = new Bounds(Area.AreaBounds.center, Area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            CellQuadTree = new QuadTree<Cell>(expandedRect);

            int cellXCount = Mathf.CeilToInt(Area.AreaBounds.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(Area.AreaBounds.size.z / CellSize);

            Vector2 corner = new Vector2(Area.AreaBounds.center.x - Area.AreaBounds.size.x / 2f,
                Area.AreaBounds.center.z - Area.AreaBounds.size.z / 2f);

            Bounds bounds = new Bounds();

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                        new Vector2(CellSize, CellSize));

                    bounds = RectExtension.CreateBoundsFromRect(rect, Area.AreaBounds.center.y, Area.AreaBounds.size.y);

                    Cell cell = new Cell(bounds);

                    cellList.Add(cell);
                    cell.Index = cellList.Count - 1;
                    CellQuadTree.Insert(cell);
                }
            }

#if UNITY_EDITOR 
            EditorUtility.SetDirty(PersistentStoragePackage); 
#endif
        }

        public void SetupCellQuadTree()
        {
            if(PersistentStoragePackage == null)
            {
                return;
            }
            
            Bounds expandedBounds = new Bounds(Area.AreaBounds.center, Area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            CellQuadTree = new QuadTree<Cell>(expandedRect);
            
            List<Cell> cell = PersistentStoragePackage.CellList;

            for (int i = 0; i < cell.Count; i++)
            {
                CellQuadTree.Insert(cell[i]);
            }
        }

        private void PrepareCells()
        {
            if(PersistentStoragePackage == null)
            { 
                return;
            }

            for (int i = 0; i <= PersistentStoragePackage.CellList.Count - 1; i++)
            {
                PersistentStoragePackage.CellList[i].PrepareCell(QuadroRenderer);
            }
        }

        public void DisposeCells()
        {
            if(PersistentStoragePackage == null)
            {
                return;
            }

            for (int i = 0; i <= PersistentStoragePackage.CellList.Count - 1; i++)
            {
                PersistentStoragePackage.CellList[i].DisposeUnmanagedData();
            }
        }

        public void SetupCells()
        {
            DisposeCells();
            PrepareCells();
            SetupCellQuadTree();
        }

        public void ConvertModifiedСellsPersistentStorageToRenderData()
        {
            ConvertAllCellsPersistentStorageToRenderData(CellModifier.GetCells());

            if(CellModifier.RemoveAfterConvert)
            {
                CellModifier.RemoveCells();
            }
        }

        public void ConvertAllCellsPersistentStorageToRenderData(List<Cell> cellList)
        {            
            for (int cellIndex = 0; cellIndex < cellList.Count; cellIndex++)
            {
                Cell cell = cellList[cellIndex];

                cell.ConvertObjectInstancedToPersistentItem();
                cell.ConvertCellPersistentStorageToRenderData(QuadroRenderer);
            }
        }
    }
}
