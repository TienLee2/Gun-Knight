using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    [Serializable]
    public enum CameraCullingMode
    {
        FrustumCulling = 0,
        Complete360 = 1        
    }

    public enum CameraType
    {
        Normal,
        SceneView
    }

    [Serializable]
    public class QuadroRendererCamera
    {
        public Camera Camera;
        public CameraCullingMode CameraCullingMode = CameraCullingMode.FrustumCulling;
        public CameraType CameraType = CameraType.Normal;
        public Vector3 FloatingOriginOffset = new Vector3(0,0,0);
        
        public bool Enabled = true; 
        public float LodBias = 1;
        public bool IsShadowCasting = true;

        public bool FoldoutGUI = true;
        [NonSerialized]
        public CameraRenderData CameraRender;

        public Camera GetRenderingCamera(QuadroRenderer quadroRenderer)
        {
#if UNITY_EDITOR
            if(Application.isPlaying == false)
            {
                if(quadroRenderer.PlayModeSimulation.SimulateAtEditor)
                {
                    return null;
                }
            }
#endif
            if(QuadroRendererConstants.QuadroRendererSettings.RenderDirectToCamera)
            {
                if (CameraType == CameraType.SceneView || Application.isPlaying)
                {
                    return Camera;
                }
            }
            else
            {
                if (CameraType == CameraType.SceneView)
                {
                    return Camera;
                }
            }
                
            return null;
        }

        public QuadroRendererCamera(Camera selectedCamera)
        {
            Camera = selectedCamera; 
        }

        public Vector3 GetCameraPosition()
        {
            return Camera.transform.position - FloatingOriginOffset;
        }

        public void PrepareRenderLists(QuadroRenderer quadroRenderer)
        {
            if (!ValidateCameraRenderList(quadroRenderer))
            {
                DisposeCameraRenderList();
            }

            if (CameraRender == null)
            {
                CameraRender = new CameraRenderData(quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count);
            }
        }

        void DisposeCameraRenderList()
        {
            if (CameraRender != null)
            {
                CameraRender.DisposeData();
            }

            CameraRender = null;
        }

        bool ValidateCameraRenderList(QuadroRenderer quadroRenderer)
        {
            if (CameraRender == null) return false;
            if (CameraRender.ItemMergeMatrixList.Count != quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count)
            {
                return false;
            }

            return true;
        }

        public float GetMaxDistance(QuadroRenderer quadroRenderer)
        {
            return Mathf.Min(Camera.farClipPlane, quadroRenderer.QualitySettings.MaxRenderDistance);
        }

        public void DisposeData()
        {    
            DisposeCameraRenderList();
        }
    }
}