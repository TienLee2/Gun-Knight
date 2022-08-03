using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace QuadroRendererSystem
{
    [SerializeField]
    public class PlayModeSimulation 
    {
        public QuadroRenderer QuadroRenderer;
        public QuadroRendererCamera SceneCamera;

        private Quaternion Rotation;
        private Vector3 Position;

        public bool SimulateAtEditor;

        public PlayModeSimulation(QuadroRenderer quadroRenderer)
        {
            this.QuadroRenderer = quadroRenderer;
        }

        public void StartSimulation()
        {
            SimulateAtEditor = true;

            EditorApplication.update -= UpdateEditorCallback;
            EditorApplication.update += UpdateEditorCallback;
        }

        public void StopSimulation()
        {
            SimulateAtEditor = false;

            EditorApplication.update -= UpdateEditorCallback;
        }

        private void UpdateEditorCallback()
        {        
            if(QuadroRenderer == null)
            {
                return;
            }

            if(SceneCamera == null)
            {
                SceneCamera = QuadroRenderer.GetSceneCamera();
            }

            if (SceneCamera != null && SceneCamera.Camera)
            {             
                if (Rotation != SceneCamera.Camera.transform.rotation ||
                    Position != SceneCamera.Camera.transform.position)
                {
                    Rotation = SceneCamera.Camera.transform.rotation;
                    Position = SceneCamera.Camera.transform.position;
                    QuadroRenderer.SetInstanceInfoDirty();
                }
            }
        }
    }
}
#endif