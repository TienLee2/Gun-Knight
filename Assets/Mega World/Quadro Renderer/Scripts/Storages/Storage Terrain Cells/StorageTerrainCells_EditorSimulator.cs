#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace QuadroRendererSystem
{
    public partial class StorageTerrainCells
    {
        public void StartEditorSimulation()
        {
            if (Application.isPlaying)
            {
                return;
            }

            RenderPipelineManager.beginCameraRendering += BeginRenderingSRP;
            Camera.onPreCull += BeginRendering;
        }

        public void StopEditorSimulation()
        {
            RenderPipelineManager.beginCameraRendering -= BeginRenderingSRP;
            Camera.onPreCull -= BeginRendering;
        }

        private void BeginRendering(Camera cam)
        {
            if(QuadroRenderer.PlayModeSimulation.SimulateAtEditor)
            {
                return;
            }

            QuadroRendererCamera camera = QuadroRenderer.GetQuadroRendererCamera(cam);

            if (camera != null)
            {
                Render();
            }
        }

        private void BeginRenderingSRP(ScriptableRenderContext context, Camera cam)
        {
            if(QuadroRenderer.PlayModeSimulation.SimulateAtEditor)
            {
                return;
            }

            QuadroRendererCamera camera = QuadroRenderer.GetQuadroRendererCamera(cam);

            if (camera != null)
            {
                Render();
            }
        }
    }
}
#endif