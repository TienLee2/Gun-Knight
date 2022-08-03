using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace QuadroRendererSystem
{
    public partial class StorageTerrainCells
    {   
        public void UpdateCameraRendererData()
        {
            QuadroRenderer.UpdateFloatingOrigin();
            
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

                cameraCellOcclusionCulling.QuadroRendererCamera.PrepareRenderLists(QuadroRenderer);

                List<Cell> visibleCellList = GetVisibleCellList(cameraCellOcclusionCulling);

                for (int protoIndex = 0; protoIndex <= QuadroRenderer.QuadroPrototypesPackage.PrototypeList.Count - 1; protoIndex++)
                {
                    int totalInstanceCount;
                    List<Cell> currentCellList = QuadroRenderer.RenderManager.GetCellsWithItem(visibleCellList, protoIndex, out totalInstanceCount);

                    if (QuadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex].GeneralSettings.RenderMode == RenderMode.GPUInstancedIndirect) 
                    {
                        QuadroRenderer.RenderManager.GPUInstancedIndirectRenderer.UpdateGPUBuffer(QuadroRenderer, currentCellList, totalInstanceCount, protoIndex, cameraIndex);
                    }
                    else
                    {
                        QuadroRenderer.RenderManager.UpdateRendererData(QuadroRenderer, cameraCellOcclusionCulling.QuadroRendererCamera, totalInstanceCount, protoIndex, currentCellList);
                    }
                }
            }
        }
    }
}
