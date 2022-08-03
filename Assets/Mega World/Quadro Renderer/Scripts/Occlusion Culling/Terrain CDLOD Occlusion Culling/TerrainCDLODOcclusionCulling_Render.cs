using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace QuadroRendererSystem
{
    public partial class TerrainCDLODOcclusionCulling
    {
        public void UpdateCameraRendererData()
        {
            QuadroRenderer.UpdateFloatingOrigin();
                        
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
                        QuadroRenderer.RenderManager.UpdateRendererData(QuadroRenderer, cameraCDLODOcclusionCulling.QuadroRendererCamera, totalInstanceCount, protoIndex, currentCellList);
                    }
                } 
            }
        }
    }
}