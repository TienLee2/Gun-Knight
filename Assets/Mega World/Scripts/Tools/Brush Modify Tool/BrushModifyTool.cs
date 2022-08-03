using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using QuadroRendererSystem;
using VladislavTsurikov;

namespace MegaWorld.BrushModify
{
    public class BrushModifyTool : ToolComponent
    {
        public struct ModifyInfo
        {
            public Vector3 RandomRotation;
            public float RandomScale;
            public float RandomPositionY;
            public int LastUpdate;
        }

        public List<GameObject> PrefabList = new List<GameObject>();
        public Dictionary<GameObject, ModifyInfo> ModifiedObjects = new Dictionary<GameObject, ModifyInfo>();
        public Dictionary<ObjectInstanceData, ModifyInfo> ModifiedObjectsInstanced = new Dictionary<ObjectInstanceData, ModifyInfo>();

        private static int s_BrushModifyToolHash = "MegaWorldEditor.BrushModify".GetHashCode();
        private DragBrush _dragBrush = new DragBrush();
        private int _updateTicks;

#if UNITY_EDITOR
        public BrushModifyToolEditor ModifyToolEditor = new BrushModifyToolEditor();

        public override void OnGUI()
        {
            HandleKeyboardEvents();
            ModifyToolEditor.OnGUI();
        }

        public override string GetDisplayName() 
        {
            return "Brush Modify";
        }
        
        public override void DoTool()
        {
            HandleKeyboardEvents();

            BrushSettings brush = MegaWorldPath.GeneralDataPackage.BrushSettingsForModify;

            int controlID = GUIUtility.GetControlID(s_BrushModifyToolHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
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

                    if (e.type == EventType.MouseDown)
                    {
                        _dragBrush.strokeDirectionRefPoint = _dragBrush.raycast.hitInfo.point;
                    }

                    foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
                    {
                        PainterVariables painterVariables = new PainterVariables(brush, _dragBrush.raycast);

                        _dragBrush.strokeDirection = (_dragBrush.raycast.hitInfo.point - _dragBrush.strokeDirectionRefPoint).normalized;

                        ModifyType(type, painterVariables, e, eventType);

                        _dragBrush.strokeDirectionRefPoint = _dragBrush.raycast.hitInfo.point;
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    if (e.button == 0)
                    {
                        PrefabList.Clear();
                        ModifiedObjects.Clear();
                        ModifiedObjectsInstanced.Clear();

                        if(QuadroRendererController.QuadroRenderer != null)
                        {
                            QuadroRendererController.QuadroRenderer.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
                        }
                        
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
                }
                case EventType.MouseMove:
                {
                    _dragBrush.UpdateDragPosition();

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
                    BrushModifyToolVisualisation.Draw(painterVariables);

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
            return MegaWorldTools.BrushModify;
        }
#endif

        public void ModifyType(Type type, PainterVariables painterVariables, Event e, EventType eventType)
        {
            switch (type.ResourceType)
            {
                case ResourceType.GameObject:
                {
                    if(type.FilterType == FilterType.MaskFilter)
                    {
                        FilterMaskOperation.UpdateFilterMaskTextureForAllGameObject(type, painterVariables, MegaWorldTools.BrushModify, true);
                    }

                    ModifyGameObject(type, painterVariables, e, eventType);

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
                        FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.BrushModify, true);
                    }

                    ModifyQuadroItem(type, painterVariables, e, eventType);

                    break;
                }
            }                                   
        }

        public void ModifyGameObject(Type type, PainterVariables painterVariables, Event e, EventType eventType)
        {
            BrushModifyToolSettings modifySettings = MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings;

            if (eventType == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                _updateTicks = 0;
            }

            if (eventType == EventType.MouseDrag && e.button == 0 && !e.alt)
            {
                ModifyInfo modifyInfo = new ModifyInfo();

                _updateTicks++;

                MegaWorldPath.GeneralDataPackage.StorageCells.IntersectBounds(painterVariables.Bounds, (gameObjectInfo, cellIndex) =>
                {
                    PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(gameObjectInfo.ID);

                    if(proto == null)
                    {
                        return true;
                    }

                    if(proto.selected == false)
                    {
                        return true;
                    }

                    if (modifySettings.ModifyByLayer)
                    {
                        if (((1 << proto.prefab.layer) & modifySettings.ModifyLayers.value) == 0)
                            return true;
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
                            continue;
                        }
                        
                        if(painterVariables.Bounds.Contains(prefabRoot.transform.position) == false)
                        {
                            continue;
                        }

                        if (!ModifiedObjects.TryGetValue(prefabRoot, out modifyInfo))
                        {
                            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;

                            modifyInfo.RandomScale = 1f - UnityEngine.Random.value;
                            modifyInfo.RandomPositionY = 1f - UnityEngine.Random.value;

                            modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);

                            if(MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject)
                            {
#if UNITY_EDITOR    
                                Undo.RegisterCompleteObjectUndo(prefabRoot.transform, "Modify");
#endif  
                            }
                        }

                        float fitness = 1;
                        Vector3 checkPoint = prefabRoot.transform.position;

                        switch (type.FilterType)
					    {
					    	case FilterType.SimpleFilter:
					    	{
                                RaycastHit hitInfo;
                                if (Physics.Raycast(Utility.GetCurrentRayForBrushTool(checkPoint, type), out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                                    type.GetCurrentPaintLayers()))
		                        {
                                    fitness = proto.SimpleModifyFilterSettings.GetFitness(hitInfo.point, hitInfo.normal);
                                }
					    		break;
					    	}
					    	case FilterType.MaskFilter:
					    	{
                                if(proto.ModifyMaskFilterStack.Filters.Count != 0)
                                {
                                    fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, prefabRoot.transform.position, proto.FilterMaskTexture2D);
                                }
					    		break;
					    	}
					    }

                        float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, checkPoint, painterVariables.Radius));

                        fitness *= brushFitness;

                        if (modifyInfo.LastUpdate != _updateTicks)
                        {
                            modifyInfo.LastUpdate = _updateTicks;

                            InstanceData instanceData = new InstanceData(prefabRoot.transform.position, prefabRoot.transform.localScale, prefabRoot.transform.rotation, fitness);

                            float moveLenght = e.delta.magnitude;

                            SetInstanceData(ref instanceData, ref modifyInfo, moveLenght, _dragBrush.strokeDirection, Vector3.up);

                            prefabRoot.transform.position = instanceData.position;
                            prefabRoot.transform.rotation = instanceData.rotation;
                            prefabRoot.transform.localScale = instanceData.scale;
                        }

                        ModifiedObjects[prefabRoot] = modifyInfo;
                    }

                    return true;
                });

                e.Use();
            }
        }

        public void ModifyQuadroItem(Type type, PainterVariables painterVariables, Event e, EventType eventType)
        {
            BrushModifyToolSettings modifySettings = MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings;
            
            if (eventType == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                _updateTicks = 0;
            }

            if (eventType == EventType.MouseDrag && e.button == 0 && !e.alt)
            {
                ModifyInfo modifyInfo = new ModifyInfo();

                _updateTicks++;

                StorageTerrainCellsAPI.IntersectBounds(QuadroRendererController.StorageTerrainCells, painterVariables.Bounds, true, false, (persistentInfo, cellIndex) =>
                {
                    PrototypeQuadroItem proto = AllAvailableTypes.GetCurrentQuadroItem(type, persistentInfo.ID);

                    if(proto == null || proto.selected == false)
                    {
                        return true;
                    }

                    if (MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings.ModifyByLayer)
                    {
                        if (((1 << proto.prefab.layer) & MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings.ModifyLayers.value) == 0)
                            return true;
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        ObjectInstanceData objectInstanced = persistentInfo.ObjectInstanceData[itemIndex];

                        if(painterVariables.Bounds.Contains(objectInstanced.Position) == false)
                        {
                            continue;
                        }

                        if (!ModifiedObjectsInstanced.TryGetValue(objectInstanced, out modifyInfo))
                        {
                            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;

                            modifyInfo.RandomScale = 1f - UnityEngine.Random.value;
                            modifyInfo.RandomPositionY = 1f - UnityEngine.Random.value;
                            modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);
                        }

                        float fitness = 1;
                        Vector3 checkPoint = objectInstanced.Position;

                        switch (type.FilterType)
				        {
				        	case FilterType.SimpleFilter:
				        	{
                                RaycastHit hitInfo;
                                if (Physics.Raycast(Utility.GetCurrentRayForBrushTool(checkPoint, type), out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                                    type.GetCurrentPaintLayers()))
		                        {
                                    fitness = proto.SimpleModifyFilterSettings.GetFitness(hitInfo.point, hitInfo.normal);
                                }
				        		break;
				        	}
				        	case FilterType.MaskFilter:
				        	{
                                if(proto.ModifyMaskFilterStack.Filters.Count != 0)
                                {
                                    fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, checkPoint, proto.FilterMaskTexture2D);
                                }
				        		break;
				        	}
				        }

                        float brushFitness = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, checkPoint, painterVariables.Radius));
                        
                        fitness *= brushFitness;

                        if (modifyInfo.LastUpdate != _updateTicks)
                        {
                            modifyInfo.LastUpdate = _updateTicks;

                            InstanceData instanceData = new InstanceData(objectInstanced.Position, objectInstanced.Scale, objectInstanced.Rotation, fitness);
        
                            float moveLenght = e.delta.magnitude;
                    
                            SetInstanceData(ref instanceData, ref modifyInfo, moveLenght, _dragBrush.strokeDirection, Vector3.up);

                            objectInstanced.Position = instanceData.position;
                            objectInstanced.Rotation = instanceData.rotation.normalized;
                            objectInstanced.Scale = instanceData.scale;
                        }
        
                        ModifiedObjectsInstanced[objectInstanced] = modifyInfo;
                    }

                    return true;
                });

                e.Use();
            }
        }

        private void SetInstanceData(ref InstanceData spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            if(MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings.ModifyTransformComponentsStack != null)
            {      
                foreach (ModifyTransformComponent item in MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings.ModifyTransformComponentsStack.TransformComponents)
                {
                    if(item.Enabled)
                    {
                        item.SetInstanceData(ref spawnInfo, ref modifyInfo, moveLenght, strokeDirection, normal);
                    }
                }
            }
        }

        private void HandleKeyboardEvents()
        {
			MegaWorldPath.GeneralDataPackage.BrushSettingsForModify.ScrollBrushRadius();
		}
    }
}