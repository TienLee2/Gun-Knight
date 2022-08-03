using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class QuadroRenderer
    {
        public void SetupItemModels()
        { 
            ClearItemModels();

            QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.ClearEmptyShaderInstances();
            
            for (int i = 0; i < QuadroPrototypesPackage.PrototypeList.Count; i++)
            {
#if UNITY_EDITOR
                GPUInstancedIndirectShaderUtility.GenerateInstancedShadersIfNecessary(QuadroPrototypesPackage.PrototypeList[i]);
#endif
                
                RenderModelInfo renderModelInfo = new RenderModelInfo(QuadroPrototypesPackage.PrototypeList[i], QuadroRendererCamera.Count);
                QuadroPrototypesPackage.RenderModelInfoList.Add(renderModelInfo);
 
                if(BillboardGenerator != null)
                { 
                    BillboardUtility.GeneratePrototypeBillboardIfNecessary(QuadroPrototypesPackage.PrototypeList[i], renderModelInfo, BillboardGenerator.QuadroRenderer.QualitySettings);
                }
            }
 
            SetupItemModelsPerCameraBuffers();

            SetInstanceInfoDirty();
        }

        public void SetupItemModelsPerCameraBuffers()
        {
            CreateCameraBuffers(QuadroRendererCamera.Count);
        }

        void ClearItemModels()
        {
            QuadroPrototypesPackage.DisposeData();
        }

        public void CreateCameraBuffers(int cameraCount)
        {
            for (int i = 0; i < QuadroPrototypesPackage.RenderModelInfoList.Count; i++)
            {
                QuadroPrototypesPackage.RenderModelInfoList[i].CreateCameraBuffers(cameraCount);
            }
        }
    }
}