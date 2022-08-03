using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class TerrainCDLODOcclusionCulling
    {
        private void OnDrawGizmos()
        {
            if (!enabled) return;

            DebugCDLODOcclusionCulling();
        }

        public void DebugCDLODOcclusionCulling()
        {
            if (TerrainCDLODOcclusionCullingSettings.showVisibleCells)
            {
                for (int cameraIndex = 0; cameraIndex < CameraCullingCellList.Count; cameraIndex++)
                {
                    СameraCDLODOcclusionCulling cameraCDLODOcclusionCulling = CameraCullingCellList[cameraIndex];

                    if(cameraCDLODOcclusionCulling.QuadroRendererCamera.Camera == null)
                    {
                        continue;
                    }

                    if (!QuadroRenderer.IsCameraActive(cameraCDLODOcclusionCulling.QuadroRendererCamera)) 
                    {
                        continue;
                    }

                    List<Cell> visibleCellList = GetVisibleCellList(cameraCDLODOcclusionCulling);

                    Color debugColor = Color.white;
                    Color occlusionOpacityColorFactor = new Color(1.0f, 1.0f, 1.0f, 0.2f);

                    for (int i = 0; i < visibleCellList.Count; i++) 
                    {
                        Cell cell = visibleCellList[i];

                        debugColor = cell.DebugColor;

                        Gizmos.color = debugColor;
                        Gizmos.DrawWireCube(cell.Bounds.center, cell.Bounds.size); 

                        Gizmos.color = debugColor * occlusionOpacityColorFactor;
                        Gizmos.DrawCube(cell.Bounds.center, cell.Bounds.size);
                    }

                    cameraCDLODOcclusionCulling.DrawPotentialMaxDistanceRenderCellGizmos();
                } 
            }
        }
    }
}
