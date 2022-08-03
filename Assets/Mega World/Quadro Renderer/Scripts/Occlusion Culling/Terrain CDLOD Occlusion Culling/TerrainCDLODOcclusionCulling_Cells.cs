using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    public partial class TerrainCDLODOcclusionCulling
    {
        public List<Cell> GetVisibleCellList(СameraCDLODOcclusionCulling cameraCDLODOcclusionCulling)
        {
            cameraCDLODOcclusionCulling.SetFloatingOriginOffset(QuadroRenderer.FloatingOriginOffset);                  
            cameraCDLODOcclusionCulling.UpdatePotentialMaxDistanceRenderCell(QuadroRenderer, this);
            cameraCDLODOcclusionCulling.QuadroRendererCamera.PrepareRenderLists(QuadroRenderer);

            List<Cell> visibleCellList = new List<Cell>();   
            for (int renderCellIndex = 0; renderCellIndex < cameraCDLODOcclusionCulling.PotentialMaxDistanceRenderCellList.Count; renderCellIndex++)
            {
                List<Cell> cellList = new List<Cell>(); 

                Plane[] FrustumPlaneArray = new Plane[6];

                GeometryUtility.CalculateFrustumPlanes(cameraCDLODOcclusionCulling.QuadroRendererCamera.Camera, FrustumPlaneArray);

                cameraCDLODOcclusionCulling.PotentialMaxDistanceRenderCellList[renderCellIndex].QuadTree.LODSelect(TerrainCDLODOcclusionCullingSettings.cellLODsSetings.LodRanges, 
                    TerrainCDLODOcclusionCullingSettings.cellLODsSetings.LodLevels - 1, 
                    FrustumPlaneArray, cameraCDLODOcclusionCulling.QuadroRendererCamera.Camera.transform.position, 
                    cameraCDLODOcclusionCulling.QuadroRendererCamera.CameraCullingMode == CameraCullingMode.FrustumCulling, cellList);

                for (int i = 0; i < cellList.Count; i++)
                {
                    Cell cell = cellList[i];

                    QuadroRendererCamera quadroRendererCamera = cameraCDLODOcclusionCulling.QuadroRendererCamera;

                    Vector3 cameraPosition = quadroRendererCamera.Camera.transform.position;

                    if(Vector3.Distance(cameraPosition, cell.Bounds.center) > 
                        quadroRendererCamera.GetMaxDistance(QuadroRenderer) + cell.Bounds.size.x)
                    {
                        continue;
                    }

                    visibleCellList.Add(cell);
                }
            }

            return visibleCellList;
        }

        public void UpdateHeightCells()
        {
			TerrainCDLODOcclusionCullingSettings.cellLODsSetings.UpdateLODRanges();

            List<Cell> cellList = new List<Cell>();

            for (int i = 0; i < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; i++)
            {
                TerrainCDLODOcclusionCullingPackage.RenderCells[i].QuadTree.GetAllCells(cellList);
            }

            SetCellsHeightFromUnityTerrain(cellList);
        }

        public void SetCellsHeightFromUnityTerrain(List<Cell> cellList)
        {
            Bounds expandedBounds = new Bounds(TerrainManager.area.AreaBounds.center, TerrainManager.area.AreaBounds.size);
            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            NativeArray<Bounds> cellBounds =
            new NativeArray<Bounds>(cellList.Count, Allocator.Persistent);
            for (int i = 0; i < cellList.Count; i++)
            {
                cellBounds[i] = cellList[i].Bounds;
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
    
            for (int i = 0; i < cellList.Count; i++)
            {
                cellList[i].Bounds = cellBounds[i];
            }
    
            cellBounds.Dispose();
        }

        public void CreateRenderCells()
        {
            DisposeCells();
            TerrainCDLODOcclusionCullingPackage.RenderCells.Clear();

            Bounds expandedBounds = new Bounds(TerrainManager.area.AreaBounds.center, TerrainManager.area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(TerrainCDLODOcclusionCullingSettings.renderCellSize * 2f, 0, TerrainCDLODOcclusionCullingSettings.renderCellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            RenderCellQuadTree = new QuadTree<RenderCell>(expandedRect);

            int cellXCount = Mathf.CeilToInt(TerrainManager.area.AreaBounds.size.x / TerrainCDLODOcclusionCullingSettings.renderCellSize);
            int cellZCount = Mathf.CeilToInt(TerrainManager.area.AreaBounds.size.z / TerrainCDLODOcclusionCullingSettings.renderCellSize);

            Vector2 corner = new Vector2(TerrainManager.area.AreaBounds.center.x - TerrainManager.area.AreaBounds.size.x / 2f,
                TerrainManager.area.AreaBounds.center.z - TerrainManager.area.AreaBounds.size.z / 2f);

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(TerrainCDLODOcclusionCullingSettings.renderCellSize * x + corner.x, TerrainCDLODOcclusionCullingSettings.renderCellSize * z + corner.y),
                        new Vector2(TerrainCDLODOcclusionCullingSettings.renderCellSize, TerrainCDLODOcclusionCullingSettings.renderCellSize));

                    Bounds bounds = RectExtension.CreateBoundsFromRect(rect, TerrainManager.area.AreaBounds.center.y, TerrainManager.area.AreaBounds.size.y);

                    RenderCell cell = new RenderCell(bounds);

                    cell.QuadTree = new CDLODQuadTree(cell.Bounds, 
                        TerrainCDLODOcclusionCullingSettings.cellLODsSetings.LodLevels, TerrainCDLODOcclusionCullingSettings.debugColorSetSettings);
                    
                    TerrainCDLODOcclusionCullingPackage.RenderCells.Add(cell);
                    cell.Index = TerrainCDLODOcclusionCullingPackage.RenderCells.Count - 1;
                    RenderCellQuadTree.Insert(cell);
                }
            }

            UpdateHeightCells();

#if UNITY_EDITOR 
            EditorUtility.SetDirty(TerrainCDLODOcclusionCullingPackage); 
#endif
        }

        private void PrepareCells()
        {
            if(TerrainCDLODOcclusionCullingPackage == null)
            {
                return;
            }
            
            List<Cell> cellList = new List<Cell>();

            for (int i = 0; i < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; i++)
            {
                TerrainCDLODOcclusionCullingPackage.RenderCells[i].QuadTree.GetAllCells(cellList);
            }

            for (int i = 0; i < cellList.Count; i++)
            {
                cellList[i].PrepareCell(QuadroRenderer); 
            }
        }

        public void DisposeCells()
        {
            if(TerrainCDLODOcclusionCullingPackage == null)
            {
                return;
            }
            
            List<Cell> cellList = new List<Cell>();

            for (int i = 0; i < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; i++)
            {
                TerrainCDLODOcclusionCullingPackage.RenderCells[i].QuadTree.GetAllCells(cellList);
            }

            for (int i = 0; i < cellList.Count; i++)
            {
                cellList[i].DisposeUnmanagedData(); 
            }
        }

        public void SetupRenderCellQuadTree()
        {
            Bounds expandedBounds = new Bounds(TerrainManager.area.AreaBounds.center, TerrainManager.area.AreaBounds.size);
            expandedBounds.Expand(new Vector3(TerrainCDLODOcclusionCullingSettings.renderCellSize * 2f, 0, TerrainCDLODOcclusionCullingSettings.renderCellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            RenderCellQuadTree = new QuadTree<RenderCell>(expandedRect);

            for (int renderCellIndex = 0; renderCellIndex < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; renderCellIndex++)
            {
                RenderCell renderCell = TerrainCDLODOcclusionCullingPackage.RenderCells[renderCellIndex];

                RenderCellQuadTree.Insert(renderCell);
            }
        }

        public void SetupRenderCell()
        {
            DisposeCells();
            PrepareCells();
            SetupRenderCellQuadTree();
        }

        public void ConvertAllRenderCellsPersistentStorageToRenderData()
        {            
            for (int renderCellIndex = 0; renderCellIndex < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; renderCellIndex++)
            {
                if(TerrainCDLODOcclusionCullingPackage.RenderCells[renderCellIndex].QuadTree == null)
                {
                    continue;
                }

                List<Cell> cellList = new List<Cell>();
                
                TerrainCDLODOcclusionCullingPackage.RenderCells[renderCellIndex].QuadTree.GetAllCells(cellList);

                for (int i = 0; i <= cellList.Count - 1; i++)
                {
                    Cell cell = cellList[i];

                    cell.ConvertCellPersistentStorageToRenderData(QuadroRenderer);
                }
            }
        }
    }
}
