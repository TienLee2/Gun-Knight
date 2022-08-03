using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class StorageTerrainCells
    {
        private void OnDrawGizmos()
        {
            if (!enabled) return;

            if(ShowCells)
            {
                List<Cell> persistentCellList = PersistentStoragePackage.CellList;

                for (int i = 0; i <= persistentCellList.Count - 1; i++)
                {                  
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(persistentCellList[i].Bounds.center, persistentCellList[i].Bounds.size);
                }
            }

            if (ShowVisibleCells)
            {
                for (int cameraIndex = 0; cameraIndex < CameraCullingCellList.Count; cameraIndex++)
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
