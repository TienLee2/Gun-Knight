using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using System.Collections.Generic;

namespace MegaWorld
{
    public static class PaintUtility 
    {
        public static void PaintTexture(PrototypeTerrainTexture selectedProto, TerrainPainterRenderHelper terrainPainterRenderHelper)
        {
            if(selectedProto.active == false)
            {
                return;
            }

            PaintContext paintContext = terrainPainterRenderHelper.AcquireTexture(selectedProto.TerrainLayer);

			if (paintContext != null)
			{
                FilterMaskOperation.UpdateFilterContext(ref selectedProto.FilterContext, selectedProto.MaskFilterStack, terrainPainterRenderHelper.PainterVariables);

                RenderTexture filterMaskRT = selectedProto.FilterContext.GetFilterMaskRT();

				Material mat = FilterUtility.GetPaintMaterial();

                // apply brush
                float targetAlpha = MegaWorldPath.GeneralDataPackage.TextureTargetStrength;
                float s = 1;
				Vector4 brushParams = new Vector4(s, targetAlpha, 0.0f, 0.0f);
				mat.SetTexture("_BrushTex", terrainPainterRenderHelper.PainterVariables.Mask);
				mat.SetVector("_BrushParams", brushParams);
				mat.SetTexture("_FilterTex", filterMaskRT);
                mat.SetTexture("_SourceAlphamapRenderTextureTex", paintContext.sourceRenderTexture);

                terrainPainterRenderHelper.SetupTerrainToolMaterialProperties(paintContext, mat);

                terrainPainterRenderHelper.RenderBrush(paintContext, mat, 0);

                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");

                if(selectedProto.FilterContext != null)
                {
                    selectedProto.FilterContext.DisposeUnmanagedMemory();
                }
			}

            TerrainPaintUtility.ReleaseContextResources(paintContext);
        }

        public static void PaintQuadroItem(PrototypeQuadroItem proto, Vector3 point, Vector3 normal, float fitness)
        {
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;

            InstanceData instanceData = new InstanceData(point, proto.prefab.transform.lossyScale, proto.prefab.transform.rotation, fitness);

            proto.TransformComponentsStack.SetInstanceData(ref instanceData, normal);

            if(overlapCheckSettings.RunOverlapCheckForQuadroItem(proto, instanceData))
            {
                QuadroRendererController.AddItemToStorageTerrainCells(proto, instanceData);
            }
        }

        public static void PaintGameObject(Type type, PrototypeGameObject proto, Vector3 point, Vector3 normal, float fitness)
        {
            OverlapCheckSettings overlapCheckSettings = proto.OverlapCheckSettings;

            InstanceData instanceData = new InstanceData(point, proto.prefab.transform.lossyScale, proto.prefab.transform.rotation, fitness);

            proto.TransformComponentsStack.SetInstanceData(ref instanceData, normal);

            if(overlapCheckSettings.RunOverlapCheckForGameObject(proto, instanceData))
            {
                PlacedObjectInfo objectInfo = GameObjectUtility.PlaceObject(proto, instanceData.position, instanceData.scale, instanceData.rotation);
                MegaWorldPath.GeneralDataPackage.StorageCells.AddItemInstance(proto.ID, objectInfo.gameObject);
                GameObjectUtility.ParentGameObject(type, proto, objectInfo);
            }
        }

        public static void PaintTerrainDetails(List<PrototypeTerrainDetail> protoTerrainDetailList, bool spawnOnlySelectedProto, PainterVariables painterVariables, float brushRotationSizeMultiplier, Terrain terrain)
        {
            if(terrain.terrainData.detailPrototypes.Length == 0)
            {
                Debug.LogWarning("Add Terrain Details");
                return;
            }

            Point spawnSize;
	        Point position, startPosition;
        
            spawnSize = new Point(
					Utility.WorldToDetail(painterVariables.Size * brushRotationSizeMultiplier, terrain.terrainData.size.x, terrain.terrainData),
					Utility.WorldToDetail(painterVariables.Size * brushRotationSizeMultiplier, terrain.terrainData.size.z, terrain.terrainData));
        
            Point halfBrushSize = spawnSize / 2;
        
            Point center = new Point(
                Utility.WorldToDetail(painterVariables.RaycastInfo.hitInfo.point.x - terrain.transform.position.x, terrain.terrainData),
                Utility.WorldToDetail(painterVariables.RaycastInfo.hitInfo.point.z - terrain.transform.position.z, terrain.terrainData.size.z, terrain.terrainData));
        
            position = center - halfBrushSize;
            startPosition = Point.Max(position, Point.zero);
                
            Point offset = startPosition - position;
        
            Point current;
            float fitness = 1;
            float detailmapResolution = terrain.terrainData.detailResolution;
            int x, y;

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                if(proto.active == false)
                {
                    continue;
                }

                if(spawnOnlySelectedProto)
                {
                    if(proto.selected == false)
                    {
                        continue;
                    }
                }
                
                float opacity = proto.SpawnDetailSettings.Opacity / 100.0f;

                int[,] localData = terrain.terrainData.GetDetailLayer(
                    startPosition.x, startPosition.y,
                    Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailmapResolution) - startPosition.x),
                    Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);

                float widthY = localData.GetLength(1);
                float heightX = localData.GetLength(0);
                
                if (proto.MaskFilterStack.Filters.Count > 0)
			    {
                    Texture2D filterMaskTexture2D = proto.FilterMaskTexture2D;

                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Point(y, x);

                            Vector2 normal = current + startPosition; 
                            normal /= detailmapResolution;

                            Vector2 worldPostion = Utility.GetTerrainWorldPositionFromRange(normal, terrain);

                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, new Vector3(worldPostion.x, 0, worldPostion.y), proto.FilterMaskTexture2D);
                            
                            float brushFitness = painterVariables.GetAlpha(current + offset, spawnSize) * opacity;

                            int targetStrength = 0;

                            if (proto.SpawnDetailSettings.UseRandomOpacity) 
                            {
                                float randomFitness = fitness;
                                randomFitness *= UnityEngine.Random.value;

                                targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, proto.SpawnDetailSettings.TargetStrength, randomFitness));
                            }
                            else
                            {
                                targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, proto.SpawnDetailSettings.TargetStrength, fitness));
                            }

                            targetStrength = Mathf.RoundToInt(Mathf.Lerp(localData[x, y], targetStrength, brushFitness));

                            if(proto.FailureSettings.CheckFailureRate(ref fitness) == false)
                            {
                                targetStrength = 0;
                            }

                            if(proto.FailureSettings.CheckFailureRate(ref brushFitness) == false)
                            {
                                targetStrength = localData[x, y];
                            }

                            localData[x, y] = targetStrength;
                        }
                    }

                    terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                }
                else
                {
                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Point(y, x);

                            float brushStrength = painterVariables.GetAlpha(current + offset, spawnSize) * opacity;

                            fitness = 1;

                            int targetStrength = 0;

                            if (proto.SpawnDetailSettings.UseRandomOpacity) 
                            {
                                float randomFitness = fitness;
                                randomFitness *= UnityEngine.Random.value;

                                targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, proto.SpawnDetailSettings.TargetStrength, randomFitness));
                            }
                            else
                            {
                                targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, proto.SpawnDetailSettings.TargetStrength, fitness));
                            }

                            targetStrength = Mathf.RoundToInt(Mathf.Lerp(localData[x, y], targetStrength, brushStrength));

                            if(proto.FailureSettings.CheckFailureRate(ref fitness) == false)
                            {
                                targetStrength = 0;
                            }

                            if(proto.FailureSettings.CheckFailureRate(ref brushStrength) == false)
                            {
                                targetStrength = localData[x, y];
                            }
                                    
                            localData[x, y] = targetStrength;
                        }
                    }

                    if(proto.TerrainProtoId > terrain.terrainData.detailPrototypes.Length - 1)
                    {
                        Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    }
                    else
                    {
                        terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                    }
                }
            }
        }

        public static void PaintTerrainTexture(Type type, Vector3 dragPoint, PainterVariables painterVariables)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

            foreach (PrototypeTerrainTexture selectedProto in MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainTextureList)
            {
                PaintUtility.PaintTexture(selectedProto, terrainPainterRenderHelper);
            }
        }
    }
}