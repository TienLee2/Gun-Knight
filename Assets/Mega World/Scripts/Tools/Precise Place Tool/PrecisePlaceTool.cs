using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VladislavTsurikov;

namespace MegaWorld.PrecisePlace
{
#if UNITY_EDITOR
    public class PrecisePlaceTool : ToolComponent
    {
        public static PrototypeGameObject SelectedProto = null;
        public PrecisionPlaceToolEditor PreciseToolEditor = new PrecisionPlaceToolEditor();

        private static int s_precisePlaceToolHash = "PrecisePlaceTool".GetHashCode();
        private DragBrush _dragBrush = new DragBrush();

        public override void OnGUI()
        {            
            HandleKeyboardEvents();
            PreciseToolEditor.OnGUI(this);
        }

        public override string GetDisplayName() 
        {
            return "Precise Place";
        }

        public override MegaWorldTools GetTool()
        {
            return MegaWorldTools.PrecisePlace;
        }

        public override void DoTool()
        {
            if(!IsToolSupportSelectedData())
            {
                return;
            }

            HandleKeyboardEvents();

            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(s_precisePlaceToolHash, FocusType.Passive);

            Type type = MegaWorldPath.DataPackage.BasicData.SelectedVariables.SelectedType;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_dragBrush.UpdateDragPosition())
                    {
                        _dragBrush.dragDistance = 0;
                        _dragBrush.strokeDirectionRefPoint = _dragBrush.raycast.hitInfo.point;
                        _dragBrush.prevRaycast = _dragBrush.raycast;
                    }

                    GUIUtility.hotControl = controlID;
                    e.Use();

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

                    if(precisionPaintSettings.ObjectMousePrecision.IsAnyMouseSessionActive)
                    {
                        return;
                    }

                    DeleteIfNecessary(type);

                    if(_dragBrush.UpdateDragPosition())
                    {
                        _dragBrush.DragMouseRaycast(true, MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings.Spacing, (dragPoint) =>
                        {
                            RaycastInfo originalRaycastInfo;
                            Utility.Raycast(Utility.GetCurrentRayForBrushTool(dragPoint, type), out originalRaycastInfo, type.GetCurrentPaintLayers());

                            if(originalRaycastInfo.isHit)
                            {
                                PlacedObjectInfoController.PlacedObjectInfo = PlaceObjectUtility.DragPlace(type, originalRaycastInfo, _dragBrush);
                            }

                            return true;
                        });
                    }

                    break;
                }
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && e.button == 0)
                {
                    PlacedObjectInfoController.PlacedObjectInfo = PlaceObjectUtility.TryToPlace(type, _dragBrush.raycast);
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.MouseMove:

                if(PlacedObjectInfoController.PlacedObjectInfo != null && PlacedObjectInfoController.PlacedObjectInfo.gameObject != null)
                {
                    UpdatePlacementGuideSessions();
                    SelectedProto.pastTransform = new PastTransform(PlacedObjectInfoController.PlacedObjectInfo.gameObject.transform);
                }

                _dragBrush.UpdateDragPosition();

                e.Use();
                break;
            case EventType.Repaint:

                PlacedObjectInfoController.DestroyObjectIfNecessary(type);

                if (_dragBrush.raycast.isHit)
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        if (PlacedObjectInfoController.PlacedObjectInfo == null)
                        {
                            PlacedObjectInfoController.PlacedObjectInfo =  PlaceObjectUtility.TryToPlace(type, _dragBrush.raycast);
                        }

                        if (PlacedObjectInfoController.PlacedObjectInfo != null)
                        {
                            UpdateObjectTransform(type, e, controlID);
                        }

                        PrecisePaintHandles.DrawPrecisePaintHandles(PlacedObjectInfoController.PlacedObjectInfo, _dragBrush);
                    }
                    else
                    {
                        if(PlacedObjectInfoController.PlacedObjectInfo != null)
                        {
                            PrecisePaintHandles.DrawInitialHandle(_dragBrush.raycast.point, 
                                PrecisePaintHandles.GetCurrentColorFromFitness(_dragBrush.raycast.point, _dragBrush.raycast));
                        }
                        else
                        {
                            PrecisePaintHandles.DrawInitialHandle(_dragBrush.raycast.point, Color.red);
                        }
                    }
                }
                break;
            case EventType.Layout:
                HandleUtility.AddDefaultControl(controlID);
                break;
            case EventType.KeyDown:
                switch (e.keyCode)
                {
                case KeyCode.F:
                    // F key - Frame camera on brush hit point
                    if (MegaWorldGUIUtility.IsModifierDown(EventModifiers.None) && _dragBrush.raycast.isHit)
                    {
                        if (PlacedObjectInfoController.PlacedObjectInfo != null)
                        {
                            Bounds bounds;
                            GameObjectUtility.GetObjectWorldBounds(PlacedObjectInfoController.PlacedObjectInfo.gameObject, out bounds);

                            if (_dragBrush.raycast.isHit)
                                bounds.Encapsulate(_dragBrush.raycast.point);

                            if(bounds.size.magnitude > Mathf.Epsilon)
                                SceneView.lastActiveSceneView.LookAt(bounds.center, SceneView.lastActiveSceneView.rotation, bounds.size.magnitude * 2f);
                            else
                                SceneView.lastActiveSceneView.LookAt(bounds.center);
                        }
                        else
                            SceneView.lastActiveSceneView.LookAt(_dragBrush.raycast.point);

                        e.Use();
                    }
                    break;
                }
                break;
            }
        }

        public override void OnToolDisabled()
        {
            if(PlacedObjectInfoController.PlacedObjectInfo != null)
            {
                PlacedObjectInfoController.DestroyObject();
            }
        }

        private void UpdateObjectTransform(Type type, Event e, int controlID)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;
            
            if(PlacedObjectInfoController.PlacedObjectInfo != null)
            {
                precisionPaintSettings.ObjectMousePrecision.CancelMousePrecisionIfNecessary(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates);

                if(!precisionPaintSettings.ObjectMousePrecision.IsAnyMouseSessionActive)
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        PlacedObjectInfoController.PlacedObjectInfo.gameObject.transform.position = _dragBrush.raycast.hitInfo.point;
                        PlacedObjectInfoController.PlacedObjectInfo.RaycastInfo = _dragBrush.raycast;

                        PositionOffset.ChangePositionOffset(PlacedObjectInfoController.PlacedObjectInfo.gameObject, SelectedProto.PositionOffset);

                        if(precisionPaintSettings.AlignAxis)
                        {
                            AxisAlignment.AlignObjectAxis(PlacedObjectInfoController.PlacedObjectInfo.gameObject, precisionPaintSettings.AlignmentAxis, _dragBrush.raycast.hitInfo.normal, precisionPaintSettings.WeightToNormal);
                        }

                        GameObjectUtility.GetObjectWorldBounds(PlacedObjectInfoController.PlacedObjectInfo.gameObject, out PlacedObjectInfoController.PlacedObjectInfo.bounds);
                    }
                }
            }
        }

        public void UpdatePlacementGuideSessions()
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            precisionPaintSettings.ObjectMousePrecision.BeginGuideMouseRotationSession(PlacedObjectInfoController.PlacedObjectInfo.gameObject, MegaWorldPath.GeneralDataPackage.KeyboardButtonStates);

            precisionPaintSettings.ObjectMousePrecision.BeginMouseScaleSession(PlacedObjectInfoController.PlacedObjectInfo, MegaWorldPath.GeneralDataPackage.KeyboardButtonStates);

            if(PrecisePlaceToolAllShortcutCombos.Instance.OffsetFromPlacementSurface.IsActive(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates))
            {
                precisionPaintSettings.ObjectMousePrecision.BeginMouseMoveAlongDirectionSession(PlacedObjectInfoController.PlacedObjectInfo.RaycastInfo.hitInfo.normal, 
                    PlacedObjectInfoController.PlacedObjectInfo.gameObject);
            }

            precisionPaintSettings.ObjectMousePrecision.UpdateActiveMouseForMouseMovement(Event.current, MegaWorldPath.GeneralDataPackage.TransformSpace);
        }

        public void DeleteIfNecessary(Type type)
        {
            if(PlacedObjectInfoController.PlacedObjectInfo != null)
            {
                InstanceData instanceData = new InstanceData();

                if(!PlaceObjectUtility.CanPlace(type, SelectedProto, PlacedObjectInfoController.PlacedObjectInfo.RaycastInfo, ref instanceData))
                {
                    PlacedObjectInfoController.DestroyObject();
                }
            }
        }

        private void HandleKeyboardEvents()
        {
            Event e = Event.current;

			if(e.type == EventType.ScrollWheel)
			{
				if(MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings.SelectType == PreciseSelectType.Unit)
				{
					if(e.delta.y < 0)
					{
						MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SetPrecisionUnit(MegaWorldPath.DataPackage.SelectedVariables.SelectedType.GetPrecisionUnit() + 1, 
                            MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count);
					}
					else if(e.delta.y > 0)
					{
						MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SetPrecisionUnit(MegaWorldPath.DataPackage.SelectedVariables.SelectedType.GetPrecisionUnit() - 1, 
                            MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ProtoGameObjectList.Count);
					}

                    e.Use();
				}
			}

			if(MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.PrecisePlace)
			{
                UnityEditor.Tools.current = UnityEditor.Tool.None;
			}
		}

        public bool IsToolSupportSelectedData()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
			}
			else
			{
				return false;
			}
        }
    }
#endif
}