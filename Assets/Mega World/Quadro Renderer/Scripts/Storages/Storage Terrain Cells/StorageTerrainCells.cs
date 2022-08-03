using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class StorageTerrainCells : MonoBehaviour
    {
        public List<СameraCellOcclusionCulling> CameraCullingCellList = new List<СameraCellOcclusionCulling>();
        public QuadroRenderer QuadroRenderer;
        public Area Area;
        public PersistentStoragePackage PersistentStoragePackage;
        public QuadTree<Cell> CellQuadTree;
        public СellModifier CellModifier = new СellModifier();

        public float CellSize = 100;
        public bool ActiveRenderer = true;
        public bool DontRenderInPlayMode = true;
        public bool ShowCells;
        public bool ShowVisibleCells;

        private bool _initDone = false;  

        void OnEnable()
        {
            DetectNecessaryData();

            if(HasAllNecessaryData() == false)
            {
                return;
            }
            
            SetupTerrainPersistentCells();

            if(Application.isPlaying && DontRenderInPlayMode)
            {
                ActiveRenderer = false;
            }

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

            if(Application.isPlaying && DontRenderInPlayMode)
            {
                ActiveRenderer = true;
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

            if(CellModifier.GetCells().Count != 0)
            {
                ConvertModifiedСellsPersistentStorageToRenderData();
            }

            if (PersistentStoragePackage.CellList.Count > 0) 
            {
                UpdateCameraRendererData();
                QuadroRenderer.RenderManager.Render(QuadroRenderer);
            }
        }

        public void SetupTerrainPersistentCells()
        {
            if(HasAllNecessaryData() == false)
            {
                return;
            }

            SetupCells();
            ConvertAllCellsPersistentStorageToRenderData(PersistentStoragePackage.CellList);
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

            if(Area == null)
            {
                if((Area)FindObjectOfType(typeof(Area)) != null)
                {
                    Area = (Area)FindObjectOfType(typeof(Area));
                }
                else
                {
                    Area = CreateAreaParent();
                }
            }

            if(PersistentStoragePackage == null)
            {
                CreateTerrainPersistentCellsPackage();
                CreateCells();
            }
        }

        public Area CreateAreaParent()
        {
            GameObject area = new GameObject("Quadro Renderer Area");
            area.transform.localScale = new Vector3(150, 150, 150);
    		area.AddComponent<Area>();
            area.transform.SetParent(transform);

            return area.GetComponent<Area>();
        }

        public bool HasAllNecessaryData()
        {
            if(QuadroRenderer == null)
            {
                return false;
            }

            if(Area == null)
            {
                return false;
            }

            if(PersistentStoragePackage == false)
            {
                return false;
            }

            return true;
        }
    }
}