using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
#endif

using UnityEngine;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class QuadroRenderer : MonoBehaviour
    {
        public RenderManager RenderManager = new RenderManager();

        public QualitySettings QualitySettings = new QualitySettings();
        public CurrentTab CurrentTab = CurrentTab.QualitySettings;

        public StorageTerrainCells StorageTerrainCells;
        public TerrainCellsOcclusionCulling TerrainCellsOcclusionCulling;
        public TerrainCDLODOcclusionCulling TerrainCDLODOcclusionCulling;

        public BillboardGenerator BillboardGenerator;
        
        public Transform TransformOfFloatingOrigin;
        public Vector3 FloatingOriginOffset;
        public Vector3 FloatingOriginStartPosition;

        public List<QuadroRendererCamera> QuadroRendererCamera = new List<QuadroRendererCamera>();
        public QuadroPrototypesPackage QuadroPrototypesPackage = new QuadroPrototypesPackage();

#if UNITY_EDITOR 
        public PlayModeSimulation PlayModeSimulation;
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                QuadroPrototypesPackage.CheckPrototypeChanges(this, QuadroRendererCamera.Count);
            }  
#endif
        }

        private void Reset()
        {
            AutoFindMainCamera();
            FindDirectionalLight();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                QuadroPrototypesPackage.CheckPrototypeChanges(this, QuadroRendererCamera.Count);
            }  
#endif
        }

        private void LateUpdate() 
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                QuadroPrototypesPackage.CheckPrototypeChanges(this, QuadroRendererCamera.Count);
            }
#endif
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (PlayModeSimulation == null)
                PlayModeSimulation = new PlayModeSimulation(this);

            if (!Application.isPlaying)
            {
                QuadroPrototypesPackage.CheckPrototypeChanges(this, QuadroRendererCamera.Count);
            }  
#endif

            SetupQuadroRenderer();
        }
        
        void OnDisable()
        {
            DisposeCameras();
            ClearItemModels();
            RenderManager.GPUInstancedIndirectRenderer.DisposeComputeShaders();
        }

        public void RefreshQuadroRenderer()
        {
            SetupQuadroRenderer();
        }

        void SetupQuadroRenderer()
        {
            DetectAdditionalData();

            SetupCamera();
            SetupFloatingOrigin();

            RenderManager.GPUInstancedIndirectRenderer.DisposeComputeShaders();
            MergeInstancedIndirectBuffersID.Setup();
            GPUFrustumCullingID.Setup();
            RenderManager.GPUInstancedRenderer.SetupInstancedRenderMaterialPropertiesIDs();
            
            DisposeCameras();
            SetupItemModels(); 
            SetupOcclusionCullings();
        }

        public void FindDirectionalLight()
        {
            Light selectedLight = null;
            float intensity = float.MinValue;

            Light[] lights = FindObjectsOfType<Light>();
            for (int i = 0; i <= lights.Length - 1; i++)
            {
                if (lights[i].type == LightType.Directional)
                {
                    if (lights[i].intensity > intensity)
                    {
                        intensity = lights[i].intensity;
                        selectedLight = lights[i];
                    }
                }
            }

            QualitySettings.DirectionalLight = selectedLight;
        }

        public void SetupFloatingOrigin()
        {
            Transform anchor = GetFloatingOriginAnchor();
            FloatingOriginStartPosition = anchor.position;
        }

        public void SetupOcclusionCullings()
        {
            if(StorageTerrainCells != null)
            {
                StorageTerrainCells.SetupTerrainPersistentCells();
            }

            if(TerrainCellsOcclusionCulling != null)
            {
                TerrainCellsOcclusionCulling.SetupCellOcclusionCulling();
            }

            if(TerrainCDLODOcclusionCulling != null)
            {
                TerrainCDLODOcclusionCulling.SetupCDLODOcclusionCulling();
            }
        }

        public void SetInstanceInfoDirty()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void UpdateFloatingOrigin()
        {
            if (Application.isPlaying)
            {
                Transform anchor = GetFloatingOriginAnchor();
                FloatingOriginOffset = anchor.transform.position - FloatingOriginStartPosition;
            }
            else
            {
                FloatingOriginOffset = Vector3.zero;
            }
        }

        public Transform GetFloatingOriginAnchor()
        {
            if (TransformOfFloatingOrigin) return TransformOfFloatingOrigin;
            return transform;
        }
        
        public void SetupCamera()
        {
            for (int i = QuadroRendererCamera.Count - 1; i >= 0; i--)
            {
                if (QuadroRendererCamera[i].CameraType == CameraType.SceneView || QuadroRendererCamera[i].Camera == null)
                {
                    RemoveQuadroRendererCamera(QuadroRendererCamera[i]);
                }
            }

#if UNITY_EDITOR 
            if (!Application.isPlaying || QuadroRendererConstants.QuadroRendererSettings.RenderSceneCameraInPlayMode)
            {
                StartFindSceneViewCamera();
            }
#endif

            AutoFindMainCamera();
        }

#if UNITY_EDITOR 
        public void StartFindSceneViewCamera() 
        {
            QuadroRendererCamera sceneviewCamera =
                new QuadroRendererCamera(null)
                {
                    CameraType = CameraType.SceneView,
                    CameraCullingMode = CameraCullingMode.FrustumCulling,
                };

            AddQuadroRendererCamera(sceneviewCamera);
                
            EditorApplication.update -= FindSceneViewCamera;
            EditorApplication.update += FindSceneViewCamera;
        }

        public void FindSceneViewCamera()
        {
            QuadroRendererCamera sceneViewCameraData = GetSceneCamera();
            
            if (sceneViewCameraData.Camera == null || sceneViewCameraData.Camera.name != "SceneCamera")
            {
                Camera currentCam = Camera.current;
                if (currentCam != null && currentCam.name == "SceneCamera")
                {
                    sceneViewCameraData.Camera = currentCam;
                }
                else
                {
                    return;
                }

                SetupOcclusionCameras();
            }
        }
#endif

        public void SetupOcclusionCameras()
        {
            if(StorageTerrainCells != null)
            {
                StorageTerrainCells.SetupСameraCullingCell();
            }

            if(TerrainCellsOcclusionCulling != null)
            {
                TerrainCellsOcclusionCulling.SetupСameraCullingCell();
            }

            if(TerrainCDLODOcclusionCulling != null)
            {
                TerrainCDLODOcclusionCulling.SetupСameraCullingCell();
            }
        }

        public void DetectAdditionalData()
        {
            if (StorageTerrainCells == null)
            {
                StorageTerrainCells = GetComponent<StorageTerrainCells>();
            }

            if (TerrainCellsOcclusionCulling == null)
            {
                TerrainCellsOcclusionCulling = GetComponent<TerrainCellsOcclusionCulling>();
            }

            if (TerrainCDLODOcclusionCulling == null)
            {
                TerrainCDLODOcclusionCulling = GetComponent<TerrainCDLODOcclusionCulling>();
            }

            if(BillboardGenerator == null)
            {
                BillboardGenerator = GetComponent<BillboardGenerator>();
            }
        }

        public bool IsActiveRender()
        {
            if(QualitySettings.ActiveRenderer && this.isActiveAndEnabled)
            {
                return true;
            }

            return false;
        }
    }
}