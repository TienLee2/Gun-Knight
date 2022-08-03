using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class TerrainCDLODOcclusionCulling : MonoBehaviour
    {
        public List<СameraCDLODOcclusionCulling> CameraCullingCellList = new List<СameraCDLODOcclusionCulling>();
        public QuadroRenderer QuadroRenderer;
        public TerrainManager TerrainManager;
        public StorageTerrainCells StorageTerrainCells;
        public TerrainCDLODOcclusionCullingPackage TerrainCDLODOcclusionCullingPackage;
        
        public QuadTree<RenderCell> RenderCellQuadTree;

        public TerrainCDLODOcclusionCullingSettings TerrainCDLODOcclusionCullingSettings = new TerrainCDLODOcclusionCullingSettings();

        public bool ActiveRenderer = true;  
        public bool BakeAndRenderInPlayMode = true;

        public float MinHeight;
        public bool ExcludeСellsByMinHeight;

        private bool _initDone = false;  

        void OnEnable()
        {
            DetectNecessaryData();

            if(HasAllNecessaryData() == false)
            {
                return;
            }
                
            if(Application.isPlaying && BakeAndRenderInPlayMode)
            {
                Bake();
                ActiveRenderer = true;
            }

            SetupCDLODOcclusionCulling();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                StartEditorSimulation();
            }
#endif
                        
            _initDone = true;
        }

        void OnDisable()
        {
            DisposeCells();

            if(Application.isPlaying && BakeAndRenderInPlayMode)
            {
                ActiveRenderer = false;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                StopEditorSimulation();
            }
#endif

            _initDone = false;
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if(QuadroRenderer.PlayModeSimulation.SimulateAtEditor && Application.isPlaying == false)
            {
                Render();
                return;
            }
#endif
            
            if (Application.isPlaying)
            {
                Render();
            }
        }

        public void Render()
        {
            if (!_initDone) return;

            if(HasAllNecessaryData() == false)
            {
                return;
            }

            if(ActiveRenderer == false)
            {
                return;
            }

            if(QuadroRenderer.IsActiveRender() == false)
            {
                return;
            }

            if (TerrainCDLODOcclusionCullingPackage.RenderCells.Count > 0) 
            {
                UpdateCameraRendererData();
                QuadroRenderer.RenderManager.Render(QuadroRenderer);
            }
        }

        public void SetupCDLODOcclusionCulling()
        {
            if(HasAllNecessaryData() == false)
            {
                return;
            }
            
            SetupRenderCell();
            ConvertAllRenderCellsPersistentStorageToRenderData();
            SetupСameraCullingCell();
        }

        public void SetupСameraCullingCell()
        {
            CameraCullingCellList.Clear();
            foreach (QuadroRendererCamera camera in QuadroRenderer.QuadroRendererCamera)
            {
                CameraCullingCellList.Add(new СameraCDLODOcclusionCulling(camera));
            }
        }

        public void DetectNecessaryData()
        {
            if (QuadroRenderer == null)
            {
                QuadroRenderer = GetComponent<QuadroRenderer>();
            }

            if(StorageTerrainCells == null)
            {
                StorageTerrainCells = GetComponent<StorageTerrainCells>();
            }

            if(TerrainManager == null)
            {
                if(GetComponent<TerrainManager>() != null)
                {
                    TerrainManager = GetComponent<TerrainManager>();
                }
                else
                {
                    TerrainManager = gameObject.AddComponent<TerrainManager>();
                    TerrainManager.AddAllUnityTerrains();
                    TerrainManager.AddAllPolarisTerrains();
                }
            }

            if(TerrainCDLODOcclusionCullingPackage == null)
            {
                CreateTerrainCDLODOcclusionCullingPackage();
                CreateRenderCells();
                TerrainCDLODOcclusionCullingSettings.cellLODsSetings.UpdateLODRanges();
            }
        } 

        public bool HasAllNecessaryData()
        {
            if(QuadroRenderer == null)
            {
                return false;
            }

            if(StorageTerrainCells == null)
            {
                return false;
            }

            if(TerrainCDLODOcclusionCullingPackage == null)
            {
                return false;
            }

            if(TerrainManager == null)
            {
                return false;
            }

            if(TerrainManager.HasAllNecessaryData() == false) 
            {
                return false;
            }

            return true;
        }
    }
}
