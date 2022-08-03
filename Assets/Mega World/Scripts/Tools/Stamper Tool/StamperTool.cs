using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MegaWorld.Stamper
{
    [ExecuteInEditMode]
    public partial class StamperTool : MonoBehaviour
    {
        public Area Area = new Area();
        public BasicData Data = new BasicData();
        public StamperToolControllerSettings StamperToolControllerSettings = new StamperToolControllerSettings();
        
        public IEnumerator UpdateCoroutine;
        public float SpawnProgress = 0f;
        public bool CancelSpawn = false;
		public bool SpawnComplete = true;

#if UNITY_EDITOR
        public StamperVisualisation StamperVisualisation = new StamperVisualisation();
#endif

        void OnEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);

            AllAvailableTypes.Refresh();
        }

        void Update()
        {
            AllAvailableTypes.Refresh();

            Data.SelectedVariables.DeleteNullValueIfNecessary(Data.TypeList);
            Data.SelectedVariables.SetAllSelectedParameters(Data.TypeList);

            Area.SetAreaBoundsIfNecessary(this);

            UpdateAutoModeForGlobalSpawn();
        }

        public void StartEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        public void StopEditorUpdates()
        {
  #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void EditorUpdate()
        {
            if (UpdateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                UpdateCoroutine.MoveNext();
            }
        }
        
        public void Spawn()
        {
            SpawnComplete = false;
            UpdateCoroutine = RunGlobalSpawn();
            StartEditorUpdates();
        }

        public void SpawnWithCells(List<Bounds> cellList)
        {
            SpawnComplete = false;
            UpdateCoroutine = RunStamperToolWithCells(cellList);
            StartEditorUpdates();
        }

        public void GenerateNewRandomSeed(Type type)
        {
            if (type.GenerateRandomSeed)
            {
                type.RandomSeed = UnityEngine.Random.Range(0, int.MaxValue);
            }
        }

        public IEnumerator RunGlobalSpawn()
        {
            CancelSpawn = false;

            int maxTypes = Data.TypeList.Count;
            int completedTypes = 0;
            
            for (int typeIndex = 0; typeIndex < Data.TypeList.Count; typeIndex++)
            {
                if (CancelSpawn)
                {
                    break;
                }
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Running", "Running " + Data.TypeList[typeIndex].TypeName, (float)completedTypes / (float)maxTypes);
#endif
                
                Type type = Data.TypeList[typeIndex];

                GenerateNewRandomSeed(type);

                UnityEngine.Random.InitState(type.RandomSeed);

                RaycastInfo raycastInfo;
                Utility.Raycast(Utility.GetRayForGlobalSpawnVisualisation(transform.position, type), out raycastInfo, type.GetCurrentPaintLayers());

                if(raycastInfo.isHit == false)
                {
                    break;
                }

                PainterVariables painterVariables = new PainterVariables(Area.Bounds, Area.GetCurrentRaw(), raycastInfo);
               
                switch (type.ResourceType)
                {
                    case ResourceType.GameObject:
                    {
                        if(type.FilterType == FilterType.MaskFilter)
                        {
                            FilterMaskOperation.UpdateFilterMaskTextureForAllGameObject(type, painterVariables, MegaWorldTools.StamperTool, false);
                        }

                        PaintGameObject(type, painterVariables);

                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                        {
                            TerrainResourcesController.LogWarningWithSyncProblem(type);

                            break;
                        }

                        FilterMaskOperation.UpdateFilterMaskTextureForAllTerrainDetail(type.ProtoTerrainDetailList, painterVariables, MegaWorldTools.StamperTool);
                    
                        foreach (Terrain terrain in Terrain.activeTerrains)
                        {
                            Bounds globalTerrainBounds = terrain.terrainData.bounds;
                            globalTerrainBounds.center = new Vector3(terrain.transform.position.x + globalTerrainBounds.extents.x, terrain.terrainData.bounds.center.y, terrain.transform.position.z + globalTerrainBounds.extents.z);
                            
                            if(globalTerrainBounds.Intersects(painterVariables.Bounds))
                            {
                                PaintUtility.PaintTerrainDetails(type.ProtoTerrainDetailList, false, painterVariables, 1, terrain);
                            }
                        }
                        
                        break;
                    }
                    case ResourceType.TerrainTexture:
                    {
                        if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                        {
                            TerrainResourcesController.LogWarningWithSyncProblem(type);

                            break;
                        }

                        if(painterVariables.terrainUnderCursor == null)
                        {
                            break;
                        }

                        TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

                        foreach (PrototypeTerrainTexture selectedProto in type.ProtoTerrainTextureList)
                        {
                            PaintUtility.PaintTexture(selectedProto, terrainPainterRenderHelper);
                        }
                        
                        break;
                    }
                    case ResourceType.QuadroItem:
                    {
                        if(QuadroRendererController.SpawnSupportAvailable(type) == false)
                        {
                            break;
                        }

                        if(type.FilterType == FilterType.MaskFilter)
                        {
                            FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.StamperTool, false);
                        }

                        PaintQuadroItem(type, painterVariables);
                
                        break;
                    }
                }

                completedTypes++;
                SpawnProgress = (float)completedTypes / (float)maxTypes;
                yield return null;
            }

            SpawnProgress = (float)completedTypes / (float)maxTypes;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            SpawnComplete = true;
            UpdateCoroutine = null;
        }

        public IEnumerator RunStamperToolWithCells(List<Bounds> cellList)
        {
            CancelSpawn = false;

            int maxTypes = Data.TypeList.Count;
            int completedTypes = 0;

            float oneStep = (float)1 / (float)cellList.Count;

            for (int cellIndex = 0; cellIndex < cellList.Count; cellIndex++)
            {
                float cellProgress = ((float)cellIndex / (float)cellList.Count) * 100;

                for (int typeIndex = 0; typeIndex < Data.TypeList.Count; typeIndex++)
                {
                    if (CancelSpawn)
                    {
                        break;
                    }

                    SpawnProgress = cellProgress / 100;

                    if(maxTypes != 1)
                    {
                        SpawnProgress = (cellProgress / 100) + Mathf.Lerp(0, oneStep, (float)completedTypes / (float)maxTypes);
                    }

                    float typesProgress = (float)completedTypes / (float)maxTypes;
#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("Cell: " + cellProgress + "%" + " (" + cellIndex + "/" + cellList.Count + ")", "Running " + Data.TypeList[typeIndex].TypeName, SpawnProgress);
#endif
                
                    Type type = Data.TypeList[typeIndex];

                    GenerateNewRandomSeed(type);

                    UnityEngine.Random.InitState(type.RandomSeed);

                    RaycastInfo raycastInfo;
                    Utility.Raycast(Utility.GetRayForGlobalSpawnVisualisation(cellList[cellIndex].center, type), out raycastInfo, type.GetCurrentPaintLayers());

                    if(raycastInfo.isHit == false)
                    {
                        break;
                    }

                    PainterVariables painterVariables = new PainterVariables(cellList[cellIndex], Area.GetCurrentRaw(), raycastInfo);

                    switch (type.ResourceType)
                    {
                        case ResourceType.GameObject:
                        {
                            if(type.FilterType == FilterType.MaskFilter)
                            {
                                FilterMaskOperation.UpdateFilterMaskTextureForAllGameObject(type, painterVariables, MegaWorldTools.StamperTool, false);
                            }

                            PaintGameObject(type, painterVariables);

                            break;
                        }
                        case ResourceType.TerrainDetail:
                        {
                            if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                            {
                                TerrainResourcesController.LogWarningWithSyncProblem(type);

                                break;
                            }

                            FilterMaskOperation.UpdateFilterMaskTextureForAllTerrainDetail(type.ProtoTerrainDetailList, painterVariables, MegaWorldTools.StamperTool);

                            foreach (Terrain terrain in Terrain.activeTerrains)
                            {
                                Bounds globalTerrainBounds = terrain.terrainData.bounds;
                                globalTerrainBounds.center = new Vector3(terrain.transform.position.x + globalTerrainBounds.extents.x, terrain.terrainData.bounds.center.y, terrain.transform.position.z + globalTerrainBounds.extents.z);

                                if(globalTerrainBounds.Intersects(painterVariables.Bounds))
                                {
                                    PaintUtility.PaintTerrainDetails(type.ProtoTerrainDetailList, false, painterVariables, 1, terrain);
                                }
                            }

                            break;
                        }
                        case ResourceType.TerrainTexture:
                        {
                            if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                            {
                                TerrainResourcesController.LogWarningWithSyncProblem(type);

                                break;
                            }

                            if(painterVariables.terrainUnderCursor == null)
                            {
                                break;
                            }

                            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

                            foreach (PrototypeTerrainTexture selectedProto in type.ProtoTerrainTextureList)
                            {
                                PaintUtility.PaintTexture(selectedProto, terrainPainterRenderHelper);
                            }

                            break;
                        }
                        case ResourceType.QuadroItem:
                        {
                            if(QuadroRendererController.SpawnSupportAvailable(type) == false)
                            {
                                break;
                            }

                            if(type.FilterType == FilterType.MaskFilter)
                            {
                                FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.StamperTool, false);
                            }

                            PaintQuadroItem(type, painterVariables);

                            break;
                        }
                    }

                    completedTypes++;
                    yield return null;
                }
            }

            SpawnProgress = 1;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            SpawnComplete = true;
            UpdateCoroutine = null;
        }

        public void PaintGameObject(Type type, PainterVariables painterVariables)
        {
            type.ScatterSettings.SetGlobalSpawnScatterPosition(type, painterVariables, (positionForSpawn) => 
            {
                RaycastInfo raycastInfo;
                Utility.Raycast(Utility.GetRayForGlobalSpawnVisualisation(positionForSpawn, type), 
                            out raycastInfo, type.GetCurrentPaintLayers());

                if(raycastInfo.isHit)
                {
                    if(Area.GetAreaBounds().Contains(raycastInfo.hitInfo.point) == false)
                    {
                        return true;
                    }

                    PrototypeGameObject proto = Success.GetMaxSuccessProtoGameObject(type.ProtoGameObjectList);

                    if(proto.active == false)
                    {
                        return true;
                    }

                    if(proto == null)
                    {
                        return true;
                    }

                    float fitness = 1;

                    if(type.FilterType != FilterType.MaskFilter)
                    {
                        fitness = proto.SimpleFilterSettings.GetFitness(raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal);
                    }
                    else
                    {
                        if(proto.MaskFilterStack.Filters.Count != 0)
                        {
                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, raycastInfo.hitInfo.point, proto.FilterMaskTexture2D);
                        }
                    }
                
                    float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, raycastInfo.hitInfo.point, painterVariables.Radius));

                    fitness *= brushFitness;

                    if(fitness != 0)
                    {
                        if(proto.FailureSettings.CheckFailureRate(ref fitness) == false)
                        {
                            return false;
                        }

                        PaintUtility.PaintGameObject(type, proto, raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal, fitness); 
                    }
                }

                return true;
            });
        }

        public void PaintQuadroItem(Type type, PainterVariables painterVariables)
        {            
            type.ScatterSettings.SetGlobalSpawnScatterPosition(type, painterVariables, (positionForSpawn) => 
            {
                RaycastInfo raycastInfo;
                Utility.Raycast(Utility.GetRayForGlobalSpawnVisualisation(positionForSpawn, type), out raycastInfo, type.GetCurrentPaintLayers());

                if(raycastInfo.isHit)
                {
                    if(Area.GetAreaBounds().Contains(raycastInfo.hitInfo.point) == false)
                    {
                        return true;
                    }

                    PrototypeQuadroItem proto = Success.GetMaxSuccessProtoQuadroItem(type.ProtoQuadroItemList);

                    if(proto == null || proto.active == false)
                    {
                        return true;
                    }

                    float fitness = 1;

                    if(type.FilterType != FilterType.MaskFilter)
                    {
                        fitness = proto.SimpleFilterSettings.GetFitness(raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal);
                    }
                    else
                    {
                        if(proto.MaskFilterStack.Filters.Count != 0)
                        {
                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, raycastInfo.hitInfo.point, proto.FilterMaskTexture2D);
                        }
                    }
                
                    float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, raycastInfo.hitInfo.point, painterVariables.Radius));

                    fitness *= brushFitness;
                    
                    if(fitness != 0)
                    {
                        if(proto.FailureSettings.CheckFailureRate(ref fitness) == false)
                        {
                            return false;
                        }

                        PaintUtility.PaintQuadroItem(proto, raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal, fitness);
                    }
                }

                return true;
            });
        }
    }
}