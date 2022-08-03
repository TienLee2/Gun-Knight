using UnityEngine;
using UnityEditor;
using VladislavTsurikov;

namespace MegaWorld
{
    public class BrushPaintTool : ToolComponent
    {
        private int _editorHash = "Editor".GetHashCode();
        private DragBrush _dragBrush = new DragBrush();
        private bool _startDrag = false;
        
#if UNITY_EDITOR
        public BrushPaintToolEditor BrushToolEditor = new BrushPaintToolEditor();

        public override void OnGUI()
        {
            HandleKeyboardEvents();
            BrushToolEditor.OnGUI();
        }

        public override string GetDisplayName() 
        {
            return "Brush Paint";
        }

        public override void DoTool()
        {
            HandleKeyboardEvents();
            
            BrushSettings brush = GetBrushSettings();
            
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
                                PaintType(type, brush, painterVariables);
                            }
                        }
                    }
                    
                    _dragBrush.dragDistance = 0;
                    _dragBrush.prevRaycast = _dragBrush.raycast;
                    _startDrag = true;

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

                    float brushSpacing = brush.GetCurrentBrushSpacing();
                    if(_startDrag)
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0
                        || MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                        {
                            if(brushSpacing < brush.BrushSize / 2)
                            {
                                brushSpacing = brush.BrushSize / 2;
                            }
                        }
                    }

                    _dragBrush.DragMouseRaycast(brush.SpacingMode == SpacingMode.Drag, brushSpacing, (dragPoint) =>
                    {
                        _startDrag = false;

                        foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
                        {
                            RaycastInfo originalRaycastInfo;

                            if(Utility.Raycast(Utility.GetCurrentRayForBrushTool(dragPoint, type), out originalRaycastInfo, type.GetCurrentPaintLayers()))
                            {
                                if(originalRaycastInfo.isHit)
                                {
                                    PainterVariables painterVariables = new PainterVariables(type, brush, originalRaycastInfo);

                                    if(painterVariables.RaycastInfo.isHit)
                                    {
                                        PaintType(type, brush, painterVariables, true);
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
                    BrushPaintToolVisualisation.Draw(painterVariables);

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
            return MegaWorldTools.BrushPaint;
        }
#endif

        public void PaintType(Type type, BrushSettings brush, PainterVariables painterVariables, bool dragMouse = false)
        {
            switch (type.ResourceType)
            {
                case ResourceType.GameObject:
                {
                    PaintGameObject(type, painterVariables, dragMouse);
                    
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                    {
                        TerrainResourcesController.LogWarningWithSyncProblem(type);
                        
                        return;
                    }

                    FilterMaskOperation.UpdateFilterMaskTextureForAllTerrainDetail(type.ProtoTerrainDetailList, painterVariables, MegaWorldTools.BrushPaint);
                    
                    PaintUtility.PaintTerrainDetails(type.ProtoTerrainDetailList, true, painterVariables, painterVariables.BrushRotationSizeMultiplier, painterVariables.terrainUnderCursor);

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
                    {
                        TerrainResourcesController.LogWarningWithSyncProblem(type);
                        
                        return;
                    }

                    PaintUtility.PaintTerrainTexture(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables);

                    break;
                }
                case ResourceType.QuadroItem:
                {
                    if(QuadroRendererController.SpawnSupportAvailable(type) == false)
                    {
                        return;
                    }

                    PaintQuadroItem(type, painterVariables, dragMouse);

                    break;
                }
            }
        }

        public void PaintGameObject(Type type, PainterVariables painterVariables, bool dragMouse)
        {
            if(type.SpawnSurface != SpawnMode.Spherical && type.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateFilterMaskTextureForAllGameObject(type, painterVariables, MegaWorldTools.BrushPaint, true);
            }

            type.ScatterSettings.SetBrushScatterPosition(type, painterVariables, (position) =>
            {
                if(dragMouse)
                {
                    if(UnityEngine.Random.Range(0f, 100f) < MegaWorldPath.GeneralDataPackage.DragFailureRate)
                    {
                        return true;
                    }
                }
                
                RaycastInfo raycastInfo;
                Utility.Raycast(Utility.GetCurrentRayForBrushTool(position, type), out raycastInfo, type.GetCurrentPaintLayers());

                if(raycastInfo.isHit)
                {
                    PrototypeGameObject proto = Success.GetMaxSuccessProtoGameObject(type.GetAllSelectedGameObject());

                    if(proto == null || proto.active == false)
                    {
                        return true;
                    }

                    float fitness = 1;

                    if(type.SpawnSurface != SpawnMode.Spherical && type.FilterType == FilterType.MaskFilter)
                    {
                        if(proto.MaskFilterStack.Filters.Count != 0)
                        {
                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, raycastInfo.hitInfo.point, proto.FilterMaskTexture2D);
                        }
                    }
                    else
                    {
                        fitness = proto.SimpleFilterSettings.GetFitness(raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal);
                    }
                
                    float brushViability = painterVariables.GetAlphaRaw(Utility.GetNormalizedCheckPoint(painterVariables.RaycastInfo.hitInfo.point, raycastInfo.hitInfo.point, painterVariables.Radius));

                    fitness *= brushViability;

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

        public void PaintQuadroItem(Type type, PainterVariables painterVariables, bool dragMouse = false)
        {
            if(type.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateFilterMaskTextureForAllQuadroItem(type, painterVariables, MegaWorldTools.BrushPaint, true);
            }

            type.ScatterSettings.SetBrushScatterPosition(type, painterVariables, (position) =>
            {
                if(dragMouse)
                {
                    if(UnityEngine.Random.Range(0f, 100f) < MegaWorldPath.GeneralDataPackage.DragFailureRate)
                    {
                        return true;
                    }
                }

                RaycastInfo raycastInfo;
                Utility.Raycast(Utility.GetCurrentRayForBrushTool(position, type), out raycastInfo, type.GetCurrentPaintLayers());

                if(raycastInfo.isHit)
                {
                    PrototypeQuadroItem proto = Success.GetMaxSuccessProtoQuadroItem(type.GetAllSelectedQuadroItem());

                    if(proto == null || proto.active == false)
                    {
                        return true;
                    }

                    float fitness = 1;

                    if(type.FilterType == FilterType.MaskFilter)
                    {
                        if(proto.MaskFilterStack.Filters.Count != 0)
                        {
                            fitness = Utility.GetGrayscaleFromWorldPosition(painterVariables.Bounds, raycastInfo.hitInfo.point, proto.FilterMaskTexture2D);
                        }
                    }
                    else
                    {
                        fitness = proto.SimpleFilterSettings.GetFitness(raycastInfo.hitInfo.point, raycastInfo.hitInfo.normal);
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
        
        public BrushSettings GetBrushSettings()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
            {
                return MegaWorldPath.DataPackage.SelectedVariables.SelectedType.BrushSettings;
            }
            else
            {
                return MegaWorldPath.GeneralDataPackage.MultipleBrushSettings;
            }
        }

        private void HandleKeyboardEvents()
        {
			GetBrushSettings().ScrollBrushRadius();
		}
    }
}