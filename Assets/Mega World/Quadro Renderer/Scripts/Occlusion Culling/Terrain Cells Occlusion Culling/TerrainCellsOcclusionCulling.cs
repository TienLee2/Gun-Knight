using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class TerrainCellsOcclusionCulling : MonoBehaviour
    {
        public List<СameraCellOcclusionCulling> CameraCullingCellList = new List<СameraCellOcclusionCulling>();
        public QuadroRenderer QuadroRenderer;
        public TerrainManager TerrainManager;
        public StorageTerrainCells StorageTerrainCells;
        public PersistentStoragePackage TerrainCellsOcclusionCullingPackage;
        
        public QuadTree<Cell> CellQuadTree;

        public float MinHeight;
        public bool ExcludeСellsByMinHeight;

        public float CellSize = 300;
        public bool ShowCells;
        public bool ShowVisibleCells;

        public bool ActiveRenderer = false;
        public bool BakeAndRenderInPlayMode = true;

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

            SetupCellOcclusionCulling();

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
            DisposeCameraCullingCell();

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

            if (TerrainCellsOcclusionCullingPackage.CellList.Count > 0) 
            {
                UpdateCameraRendererData();
                QuadroRenderer.RenderManager.Render(QuadroRenderer);
            }
        }

        public void SetupCellOcclusionCulling()
        {
            if(HasAllNecessaryData() == false)
            {
                return;
            }
            
            SetupCells();
            ConvertAllCellsPersistentStorageToRenderData(TerrainCellsOcclusionCullingPackage.CellList);

            SetupСameraCullingCell();
        }

        public void SetupСameraCullingCell()
        {
            DisposeCameraCullingCell();

            CameraCullingCellList.Clear();
            foreach (QuadroRendererCamera camera in QuadroRenderer.QuadroRendererCamera)
            {
                СameraCellOcclusionCulling cameraCellOcclusionCulling = new СameraCellOcclusionCulling();
                cameraCellOcclusionCulling.QuadroRendererCamera = camera;
                CameraCullingCellList.Add(cameraCellOcclusionCulling);
            }
        }

        public void DisposeCameraCullingCell()
        {
            for (int i = 0; i <= CameraCullingCellList.Count - 1; i++)
            {
                CameraCullingCellList[i].DisposeData();
            }

            CameraCullingCellList.Clear();
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

            if(TerrainCellsOcclusionCullingPackage == null)
            {
                CreateTerrainCellsOcclusionCullingPackage();
                CreateCells();
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

            if(TerrainManager == null)
            {
                return false;
            }

            if(TerrainManager.HasAllNecessaryData() == false) 
            {
                return false;
            }

            if(TerrainCellsOcclusionCullingPackage == false)
            {
                return false;
            }

            return true;
        }
    }
}