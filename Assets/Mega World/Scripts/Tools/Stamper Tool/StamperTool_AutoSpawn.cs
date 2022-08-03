using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld.Stamper
{
    public partial class StamperTool 
    {
        public void AutoSpawn(Type type)
        {
            StamperToolControllerSettings._modifiedType = type;
			StamperToolControllerSettings.TimerForStamperTool.StartTimer(StamperToolControllerSettings.DelayAutoSpawn);
        }

        public void AutoSpawn(PrototypeTerrainDetail proto)
        {
            StamperToolControllerSettings.SetModifiedTerrainDetailPrototype(proto);
			StamperToolControllerSettings.TimerForStamperTool.StartTimer(StamperToolControllerSettings.DelayAutoSpawn);
        }

        public void UpdateAutoModeForGlobalSpawn()
        {
            if(StamperToolControllerSettings.AutoSpawn == false)
			{
				StamperToolControllerSettings._modifiedType = null;
       			StamperToolControllerSettings.SetNullModifiedTerrainDetailPrototype();

                return;
			}

			StamperToolControllerSettings.TimerForStamperTool.UpdateTimer(() => 
            {
                TypeHasChanged();
            });
        }

        public void TypeHasChanged()
        {
            if(Area.UseSpawnCells == false)
			{
                if(StamperToolControllerSettings._modifiedType != null)
			    {
			    	UnspawnForStamper.UnspawnTypesForAutoMode(StamperToolControllerSettings, StamperToolControllerSettings._modifiedType, Data);
			    	AutoSpawn();
			    }
			    else if(StamperToolControllerSettings.ModifiedTerrainDetailPrototype != null)
			    {
			    	if(Data.SelectedVariables.HasOneSelectedType())
			    	{
			    		List<PrototypeTerrainDetail> protoTerrainDetailList = new List<PrototypeTerrainDetail>();
			    		protoTerrainDetailList.Add(StamperToolControllerSettings.ModifiedTerrainDetailPrototype);
			    		Unspawn.UnspawnTerrainDetail(protoTerrainDetailList, false);

			    		Type type = Data.SelectedVariables.SelectedType;

			    		RaycastInfo raycastInfo;
                    	Utility.Raycast(Utility.GetRayForGlobalSpawnVisualisation(transform.position, Data.SelectedVariables.SelectedType), 
                            out raycastInfo, Data.SelectedVariables.SelectedType.GetCurrentPaintLayers());

                		if(raycastInfo.isHit)
                		{
                		    PainterVariables painterVariables = new PainterVariables(Area.Bounds, Area.GetCurrentRaw(), raycastInfo);

                            FilterMaskOperation.UpdateFilterMaskTextureForAllTerrainDetail(protoTerrainDetailList, painterVariables, MegaWorldTools.StamperTool);

			    			foreach (Terrain terrain in Terrain.activeTerrains)
                            {
                                Bounds globalTerrainBounds = terrain.terrainData.bounds;
                                globalTerrainBounds.center = new Vector3(terrain.transform.position.x + globalTerrainBounds.extents.x, terrain.terrainData.bounds.center.y, terrain.transform.position.z + globalTerrainBounds.extents.z);

                                if(globalTerrainBounds.Intersects(painterVariables.Bounds))
                                {
                                    PaintUtility.PaintTerrainDetails(protoTerrainDetailList, false, painterVariables, 1, terrain);
                                }
                            }
                		}
			    	}
			    }
            }

			StamperToolControllerSettings._modifiedType = null;
			StamperToolControllerSettings.SetNullModifiedTerrainDetailPrototype();
        }

		public void AutoSpawn()
        {            
            for (int typeIndex = 0; typeIndex < Data.TypeList.Count; typeIndex++)
            {
                Type type = Data.TypeList[typeIndex];

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

                            return;
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

                            continue;
                        }

                        if(painterVariables.terrainUnderCursor == null)
                        {
                            return;
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
                            return;
                        }

                        if(type.FilterType == FilterType.MaskFilter)
                        {
                            FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.StamperTool, false);
                        }

                        PaintQuadroItem(type, painterVariables);
                
                        break;
                    }
                }
            }
        }
    }
}