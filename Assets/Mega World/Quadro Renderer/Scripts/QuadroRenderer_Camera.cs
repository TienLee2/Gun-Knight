using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class QuadroRenderer
    {      
        public void AutoFindMainCamera()
        {
            Camera selectedCamera = Camera.main;

            if (selectedCamera == null)
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                for (int i = 0; i <= cameras.Length - 1; i++)
                {
                    if (cameras[i].gameObject.name.Contains("Main Camera") ||
                        cameras[i].gameObject.name.Contains("MainCamera"))
                    {
                        selectedCamera = cameras[i];
                        break;
                    }
                }
            }
            
            AddCamera(selectedCamera);
        }

        public void FindAllCamera()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            for (int i = 0; i <= cameras.Length - 1; i++)
            {
                AddCamera(cameras[i]);
            }
        }

        public void AddCamera(Camera camera)
        {
            QuadroRendererCamera quadroRendererCamera = GetQuadroRendererCamera(camera);
            if (quadroRendererCamera == null)
            {
                quadroRendererCamera = new QuadroRendererCamera(camera);

                AddQuadroRendererCamera(quadroRendererCamera);
            }

            SetupItemModelsPerCameraBuffers();
        }

        public bool IsCameraActive(QuadroRendererCamera quadroRendererCamera)
        {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            if(PlayModeSimulation.SimulateAtEditor)
            {
                if(quadroRendererCamera.CameraType == CameraType.SceneView)
                {
                    return false;
                } 
            }
            else
            {
                if(quadroRendererCamera.CameraType == CameraType.Normal)
                {
                    return false;
                } 
            }

            return quadroRendererCamera.Enabled;
        }
        else
        {
            if(quadroRendererCamera.CameraType == CameraType.SceneView)
            {
                if(QuadroRendererConstants.QuadroRendererSettings.RenderSceneCameraInPlayMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            return quadroRendererCamera.Enabled;
        }
#else
        return quadroRendererCamera.Enabled;
#endif
        }

        private void AddQuadroRendererCamera(QuadroRendererCamera quadroRendererCamera)
        {
            this.QuadroRendererCamera.Add(quadroRendererCamera);

            SetupOcclusionCameras();
        }

        public void RemoveCamera(Camera aCamera)
        {
            QuadroRendererCamera quadroRendererCamera = GetQuadroRendererCamera(aCamera);
            if (quadroRendererCamera != null)
            {
                RemoveQuadroRendererCamera(quadroRendererCamera);
            }

            SetupOcclusionCameras();

            SetupItemModelsPerCameraBuffers();
        }
        
        public QuadroRendererCamera GetQuadroRendererCamera(Camera unityCamera)
        {
            for (int i = 0; i <= QuadroRendererCamera.Count - 1; i++)
            {
                if (QuadroRendererCamera[i].Camera == unityCamera)
                {
                    return QuadroRendererCamera[i];
                }
            }
            return null;
        }

        public QuadroRendererCamera GetSceneCamera()
        {
            for (int i = 0; i < QuadroRendererCamera.Count; i++)
            {
                if (QuadroRendererCamera[i].CameraType == CameraType.SceneView)
                {
                    return QuadroRendererCamera[i];
                }
            }
            return null;
        }

        private void RemoveQuadroRendererCamera(QuadroRendererCamera quadroRendererCamera)
        {
            quadroRendererCamera.DisposeData();
            this.QuadroRendererCamera.Remove(quadroRendererCamera);                
        }
        
        public void DisposeCameras()
        {
            for (int i = 0; i <= QuadroRendererCamera.Count - 1; i++)
            {
                QuadroRendererCamera[i].DisposeData();
            }
        }
    }
}