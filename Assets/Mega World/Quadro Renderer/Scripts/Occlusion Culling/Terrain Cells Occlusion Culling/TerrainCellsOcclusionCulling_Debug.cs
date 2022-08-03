using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class TerrainCellsOcclusionCulling
    {
        private void OnDrawGizmos()
        {
            if (!enabled) return;

            if (ShowCells)
            {
                Gizmos.color = Color.blue;
                
                for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
                {                  
                    Bounds bounds = TerrainCellsOcclusionCullingPackage.CellList[i].Bounds;
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }
            }

            if (ShowVisibleCells)
            {
                for (int cameraIndex = 0; cameraIndex <= CameraCullingCellList.Count - 1; cameraIndex++)
                {
                    СameraCellOcclusionCulling cameraCellOcclusionCulling = CameraCullingCellList[cameraIndex];

                    if(cameraCellOcclusionCulling.QuadroRendererCamera.Camera == null)
                    {
                        continue;
                    }

                    if (!QuadroRenderer.IsCameraActive(cameraCellOcclusionCulling.QuadroRendererCamera)) 
                    {
                        continue;
                    }

                    List<Cell> visibleCellList = GetVisibleCellList(cameraCellOcclusionCulling);

                    for (int i = 0; i < visibleCellList.Count; i++) 
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(visibleCellList[i].Bounds.center, visibleCellList[i].Bounds.size);
                    }
                }
            }
        }
    }
}

