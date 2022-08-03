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
    public partial class TerrainCellsOcclusionCulling
    {
        public List<Cell> GetVisibleCellList(СameraCellOcclusionCulling cameraCellOcclusionCulling)
        {
            cameraCellOcclusionCulling.ScheduleCull(QuadroRenderer, CellQuadTree, CellSize, QuadroRenderer.FloatingOriginOffset, false);
            
            List<Cell> visibleCellList = new List<Cell>();   
            for (int j = 0; j <= cameraCellOcclusionCulling.JobOcclusionCulling.VisibleCellIndexList.Length - 1; j++)
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
            TerrainCellsOcclusionCullingPackage.CellList.Clear();
            
            Bounds expandedBounds = new Bounds(TerrainManager.area.AreaBounds.center, TerrainManager.area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            CellQuadTree = new QuadTree<Cell>(expandedRect);
            
            int cellXCount = Mathf.CeilToInt(TerrainManager.area.AreaBounds.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(TerrainManager.area.AreaBounds.size.z / CellSize);

            Vector2 corner = new Vector2(TerrainManager.area.AreaBounds.center.x - TerrainManager.area.AreaBounds.size.x / 2f,
                TerrainManager.area.AreaBounds.center.z - TerrainManager.area.AreaBounds.size.z / 2f);

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Cell cell = new Cell(new Rect(
                        new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                        new Vector2(CellSize, CellSize)));
                    TerrainCellsOcclusionCullingPackage.CellList.Add(cell);
                    cell.Index = TerrainCellsOcclusionCullingPackage.CellList.Count - 1;
                    CellQuadTree.Insert(cell);
                }
            }

            NativeArray<Bounds> cellBounds =
                new NativeArray<Bounds>(TerrainCellsOcclusionCullingPackage.CellList.Count, Allocator.Persistent);
            for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
            {
                cellBounds[i] = TerrainCellsOcclusionCullingPackage.CellList[i].Bounds;
            }

            float minBoundsHeight = TerrainManager.area.AreaBounds.center.y - TerrainManager.area.AreaBounds.extents.y;
            float maxBoundsHeight = TerrainManager.area.AreaBounds.center.y + TerrainManager.area.AreaBounds.extents.y;

            float minHeightCells = minBoundsHeight + MinHeight;
            if (!ExcludeСellsByMinHeight)
            {
                minHeightCells = minBoundsHeight;
            } 

            JobHandle jobHandle = default(JobHandle);
            
            TerrainManager.RefreshTerrains();
            for (int i = 0; i <= TerrainManager.TerrainList.Count - 1; i++)
            {
                jobHandle = TerrainManager.TerrainList[i].SetCellHeight(cellBounds, minHeightCells, expandedRect, jobHandle);
            }

            jobHandle.Complete();

            for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
            {
                TerrainCellsOcclusionCullingPackage.CellList[i].Bounds = cellBounds[i];
            }  

            cellBounds.Dispose();  

#if UNITY_EDITOR 
            EditorUtility.SetDirty(TerrainCellsOcclusionCullingPackage); 
#endif
        } 
        
        public void SetupCellQuadTree()
        {            
            if(TerrainCellsOcclusionCullingPackage == null)
            {
                return;
            }
            
            Bounds expandedBounds = new Bounds(TerrainManager.area.AreaBounds.center, TerrainManager.area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            CellQuadTree = new QuadTree<Cell>(expandedRect);

            for (int cellIndex = 0; cellIndex < TerrainCellsOcclusionCullingPackage.CellList.Count; cellIndex++)
            {
                CellQuadTree.Insert(TerrainCellsOcclusionCullingPackage.CellList[cellIndex]);
            }
        }

        public void SetupCells()
        {
            DisposeCells();
            PrepareCells();
            SetupCellQuadTree();
        }

        public void ConvertAllCellsPersistentStorageToRenderData(List<Cell> cellList)
        {            
            for (int cellIndex = 0; cellIndex < cellList.Count; cellIndex++)
            {
                Cell cell = cellList[cellIndex];

                cell.ConvertCellPersistentStorageToRenderData(QuadroRenderer);
            }
        }

        public void PrepareCells()
        {
            if(TerrainCellsOcclusionCullingPackage == null)
            {
                return;
            }

            for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
            {
                TerrainCellsOcclusionCullingPackage.CellList[i].PrepareCell(QuadroRenderer);
            }
        }

        public void DisposeCells()
        {        
            if(TerrainCellsOcclusionCullingPackage == null)
            {
                return;
            }    

            for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
            {
                TerrainCellsOcclusionCullingPackage.CellList[i].DisposeUnmanagedData();
            }
        }
    }
}
