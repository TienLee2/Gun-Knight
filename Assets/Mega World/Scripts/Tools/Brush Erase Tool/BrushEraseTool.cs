using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using QuadroRendererSystem;
using VladislavTsurikov;

namespace MegaWorld
{
    public class BrushEraseTool : ToolComponent
    {
        private int _editorHash = "Editor".GetHashCode();

        private DragBrush _dragBrush = new DragBrush();

#if UNITY_EDITOR
        public BrushEraseToolEditor EraseToolEditor = new BrushEraseToolEditor();

        public override void OnGUI()
        {
            HandleKeyboardEvents();
            EraseToolEditor.OnGUI();
        }

        public override string GetDisplayName() 
        {
            return "Brush Erase";
        }
        
        public override void DoTool()
        {
            HandleKeyboardEvents();
            BrushSettings brush = MegaWorldPath.GeneralDataPackage.BrushSettingsForErase;
            
            int controlID = GUIUtility.GetControlID(_editorHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_dragBrush.UpdateDragPosition() == false)
                    {
                        return;
                    }

                    if(_dragBrush.raycast.isHit)
                    {
                        foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
                        {
                            PainterVariables painterVariables = new PainterVariables(type, brush, _dragBrush.raycast);

                            if(painterVariables.RaycastInfo.isHit)
                            {
                                EraseType(type, painterVariables);
                            }
                        }
                    }
                    
                    _dragBrush.dragDistance = 0;
                    _dragBrush.prevRaycast = _dragBrush.raycast;

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_dragBrush.UpdateDragPosition() == false)
                    {
                        return;
                    }

                    _dragBrush.DragMouseRaycast(brush.SpacingMode == SpacingMode.Drag, brush.GetCurrentBrushSpacing(), (dragPoint) =>
                    {
                        foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
                        {
                            RaycastInfo originalRaycastInfo;

                            if(Utility.Raycast(Utility.GetCurrentRayForBrushTool(dragPoint, type), out originalRaycastInfo, type.GetCurrentPaintLayers()))
                            {
                                if(originalRaycastInfo.isHit)
                                {
                                    PainterVariables painterVariables = new PainterVariables(brush, originalRaycastInfo);

                                    if(painterVariables.RaycastInfo.isHit)
                                    {
                                        EraseType(type, painterVariables);
                                    }
                                }
                            }
                        }
                        
                        return true;
                    });

                    e.Use();

                    break;
                }
                case EventType.MouseMove:
                {
                    if(_dragBrush.UpdateDragPosition() == false)
                    {
                        return;
                    }

                    e.Use();
                    break;
                }
                case EventType.Repaint:
                {           
                    if(MegaWorldPath.GeneralDataPackage.ShowCells)
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                        {
                            MegaWorldPath.GeneralDataPackage.StorageCells.ShowCells();
                        }
                    }

	                if(_dragBrush.raycast.isHit == false)
                    {
                        return;
                    }

                    PainterVariables painterVariables = new PainterVariables(brush, _dragBrush.raycast);
                    BrushEraseToolVisualisation.Draw(painterVariables);

                    break;
                }
                case EventType.Layout:
                {           
                    HandleUtility.AddDefaultControl(controlID);

                    break;
                }
                case EventType.KeyDown:
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.F:
                        {
                            if (MegaWorldGUIUtility.IsModifierDown(EventModifiers.None) && _dragBrush.raycast.isHit)
                            {
                                SceneView.lastActiveSceneView.LookAt(_dragBrush.raycast.hitInfo.point, SceneView.lastActiveSceneView.rotation, brush.BrushSize);
                                e.Use();
                            }
                        }

                        break;
                    }
                    break;
                }
            }
        }

        public override MegaWorldTools GetTool()
        {
            return MegaWorldTools.BrushErase;
        }
#endif

        public void EraseType(Type type, PainterVariables painterVariables)
        {
            switch (type.ResourceType)
            {
                case ResourceType.GameObject:
                {
                    if(type.FilterType == FilterType.MaskFilter)
                    {
                        FilterMaskOperation.UpdateFilterMaskTextureForAllGameObject(type, painterVariables, MegaWorldTools.BrushErase, true);
                    }

                    BrushEraseGameObject(type, painterVariables);

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
                        FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.BrushErase, true);
                    }
                    
                    BrushEraseQuadroItem(type, painterVariables);

                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                    {
                        TerrainResourcesController.LogWarningWithSyncProblem(type);
                        
                        return;
                    }
                    
                    FilterMaskOperation.UpdateFilterMaskTextureForAllTerrainDetail(type.ProtoTerrainDetailList, painterVariables, MegaWorldTools.BrushErase);
                    
                    EraseTerrainDetails(type, painterVariables);

                    break;
                }
            }                                   
        }

        public void BrushEraseGameObject(Type type, PainterVariables painterVariables)
        {
            MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(painterVariables.Bounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                if(proto == null)
                {
                    return true;
                }

                if(proto == null || proto.active == false)
                {
                    return true;
                }
    
                if(proto.selected == false)
                {
                    return true;
                }

                if(MegaWorldPath.GeneralDataPackage.StorageCells.cellList.Contains(MegaWorldPath.GeneralDataPackage.StorageCells.cellList[cellIndex]) == false)
                {
                    MegaWorldPath.GeneralDataPackage.StorageCells.modifiedСells.Add(MegaWorldPath.GeneralDataPackage.StorageCells.cellList[cellIndex]);
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = Utility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        return true;
                    }

                    float fitness = 1;

                    if(type.FilterType == FilterType.SimpleFilter)
                    {
                        RaycastHit hitInfo;
                        if (Physics.Raycast(Utility.WorldPointToRay(prefabRoot.transform.position), out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                            type.GetCurrentPaintLayers()))
		                {
                            fitness = proto.SimpleEraseFilterSettings.GetFitness(hitInfo.point, hitInfo.normal);
                        }
                    }
                    else
                    {
                        if(proto.EraseMaskFilterStack.Filters.Count != 0)
                        {
                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, prefabRoot.transform.position, proto.FilterMaskTexture2D);
                        }
                    }

                    float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, prefabRoot.transform.position, painterVariables.Radius));

                    fitness *= brushFitness;

                    fitness *= MegaWorldPath.GeneralDataPackage.EraseStrength;

                    float successOfErase = UnityEngine.Random.Range(0.0f, 1.0f);

                    if(successOfErase < fitness)
                    {
                        float randomSuccessForErase = UnityEngine.Random.Range(0.0f, 1.0f);

                        if(randomSuccessForErase < proto.AdditionalSpawnSettings.SuccessForErase / 100)
                        {
                            if(MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject)
                            {
#if UNITY_EDITOR
                                Undo.DestroyObjectImmediate(prefabRoot);
#else 
                                UnityEngine.Object.DestroyImmediate(prefabRoot);
#endif
                            
                            }
                            else
                            {
                                UnityEngine.Object.DestroyImmediate(prefabRoot);
                            }
                        }
                    }
                }

                return true;
            });

            MegaWorldPath.GeneralDataPackage.StorageCells.RemoveNullGameObjects(true);
        }

        private void EraseTerrainDetails(Type type, PainterVariables painterVariables)
        {
            Point eraseSize;
	        Point position, startPosition;
        
            eraseSize = new Point(
					Utility.WorldToDetail(painterVariables.Size * painterVariables.BrushRotationSizeMultiplier, painterVariables.terrainUnderCursor.terrainData.size.x, painterVariables.terrainUnderCursor.terrainData),
					Utility.WorldToDetail(painterVariables.Size * painterVariables.BrushRotationSizeMultiplier, painterVariables.terrainUnderCursor.terrainData.size.z, painterVariables.terrainUnderCursor.terrainData));
        
            Point halfBrushSize = eraseSize / 2;
        
            Point center = new Point(
                Utility.WorldToDetail(painterVariables.RaycastInfo.hitInfo.point.x - painterVariables.terrainUnderCursor.transform.position.x, painterVariables.terrainUnderCursor.terrainData),
                Utility.WorldToDetail(painterVariables.RaycastInfo.hitInfo.point.z - painterVariables.terrainUnderCursor.transform.position.z, painterVariables.terrainUnderCursor.terrainData.size.z, 
                painterVariables.terrainUnderCursor.terrainData));
        
            position = center - halfBrushSize;
            startPosition = Point.Max(position, Point.zero);
        
            Point offset = startPosition - position;
        
            Point current;
            float fitness = 1;
            float detailmapResolution = painterVariables.terrainUnderCursor.terrainData.detailResolution;
            int x, y;

            foreach (PrototypeTerrainDetail proto in type.ProtoTerrainDetailList)
            {
                if(proto.active == false)
                {
                    continue;
                }

                if(proto.selected == false)
                {
                    continue;
                }

                float opacity = proto.SpawnDetailSettings.Opacity / 100.0f;

                int[,] localData = painterVariables.terrainUnderCursor.terrainData.GetDetailLayer(
                    startPosition.x, startPosition.y,
                    Mathf.Max(0, Mathf.Min(position.x + eraseSize.x, (int)detailmapResolution) - startPosition.x),
                    Mathf.Max(0, Mathf.Min(position.y + eraseSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);

                float widthY = localData.GetLength(1);
                float heightX = localData.GetLength(0);
                
                if (proto.EraseMaskFilterStack.Filters.Count > 0)
			    {
                    Texture2D filterMaskTexture2D = proto.FilterMaskTexture2D;

                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Point(y, x);

                            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(randomSuccess < proto.SpawnDetailSettings.SuccessOfErase / 100)
                            {
                                Vector2 normal = Vector2.zero;
                                normal.y = Mathf.InverseLerp(0, eraseSize.y, current.y);
                                normal.x = Mathf.InverseLerp(0, eraseSize.x, current.x);

                                fitness = Utility.GetGrayscale(normal, proto.FilterMaskTexture2D);

                                float brushFitness = painterVariables.GetAlpha(current + offset, eraseSize) * opacity;

                                fitness *= brushFitness;

                                fitness *= MegaWorldPath.GeneralDataPackage.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, MegaWorldPath.AdvancedSettings.EditorSettings.maxTargetStrength, fitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    painterVariables.terrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                }
                else
                {
                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Point(y, x);

                            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(randomSuccess < proto.SpawnDetailSettings.SuccessOfErase / 100)
                            {
                                float brushFitness = painterVariables.GetAlpha(current + offset, eraseSize) * opacity;

                                brushFitness *= MegaWorldPath.GeneralDataPackage.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, MegaWorldPath.AdvancedSettings.EditorSettings.maxTargetStrength, brushFitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    painterVariables.terrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                }
            }
        }

        public void BrushEraseQuadroItem(Type type, PainterVariables painterVariables)
        {
            Rect positionRect = RectExtension.CreateRectFromBounds(painterVariables.Bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellModifier.AddModifiedСell(overlapCellList[i], false, true);

                List<ItemInfo> persistentInfoList = QuadroRendererController.QuadroRenderer.StorageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;
                
                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    List<QuadroRendererSystem.InstanceData> persistentItemForDestroy = new List<QuadroRendererSystem.InstanceData>();

                    PrototypeQuadroItem proto = AllAvailableTypes.GetCurrentQuadroItem(type, persistentInfo.ID);

                    if(proto == null || proto.active == false)
                    {
                        continue;
                    }

                    if(proto.selected == false)
                    {
                        continue;
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        QuadroRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];

                        if(painterVariables.Bounds.Contains(persistentInfo.InstanceDataList[itemIndex].Position) == true)
                        {
                            float fitness = 1;

                            if(type.FilterType == FilterType.SimpleFilter)
                            {
                                RaycastHit hitInfo;
                                if (Physics.Raycast(Utility.WorldPointToRay(persistentItem.Position), out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                                    type.GetCurrentPaintLayers()))
		                        {
                                    fitness = proto.SimpleEraseFilterSettings.GetFitness(hitInfo.point, hitInfo.normal);
                                }
                            }
                            else
                            {
                                if(proto.EraseMaskFilterStack.Filters.Count != 0)
                                {
                                    fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, persistentItem.Position, proto.FilterMaskTexture2D);
                                }
                            }

                            float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, persistentItem.Position, painterVariables.Radius));

                            fitness *= brushFitness;

                            fitness *= MegaWorldPath.GeneralDataPackage.EraseStrength;

                            float successOfErase = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(successOfErase < fitness)
                            {
                                float randomSuccessForErase = UnityEngine.Random.Range(0.0f, 1.0f);

                                if(randomSuccessForErase < proto.AdditionalSpawnSettings.SuccessForErase / 100)
                                {
                                    persistentItemForDestroy.Add(persistentItem);
                                }
                            }
                        } 
                    }

                    foreach (QuadroRendererSystem.InstanceData item in persistentItemForDestroy)
                    {
                        persistentInfo.InstanceDataList.Remove(item);
                    }
                }
            }
        }

        private void HandleKeyboardEvents()
        {
			MegaWorldPath.GeneralDataPackage.BrushSettingsForErase.ScrollBrushRadius();
		}
    }
}