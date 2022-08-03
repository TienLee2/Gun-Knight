using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace MegaWorld.Edit
{
#if UNITY_EDITOR
    public class EditTool : ToolComponent
    {
        public MouseModifyEditTool MouseModifyEditTool = new MouseModifyEditTool();
        public UnityHandlesEditTool UnityHandlesEditTool = new UnityHandlesEditTool();

        public static QuadroItemInfo FindQuadroItemInfo;
        public static GameObjectInfo FindGameObjectInfo;
        
        public static int EditToolHash = "editTool".GetHashCode();
        private static bool _mouseUp = true;

        public EditToolEditor EditToolEditor = new EditToolEditor();

        public override void OnGUI()
        {
            EditToolEditor.OnGUI(this);
        }

        public override string GetDisplayName() 
        {
            return "Edit";
        }

        public override void DoTool()
        {
            if(SelectedRequiredResourceTypes() == false)
            {
                return;
            }

            HandleKeyboardEvents();
 
            int controlID = GUIUtility.GetControlID(EditToolHash, FocusType.Passive);

            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            if(edit.PrecisionModifyMode == PrecisionModifyMode.Remove)
            {
                Remove.RemoveHandles();
            }
            else
            {
                RemoveFindObjectsIfNecessary();

                if(GetCurrentEditTool() == EditTools.UnityHandles)
                {
                    UnityHandlesEditTool.DoTool();
                }
                else
                {
                    MouseModifyEditTool.DoTool();
                }
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                {
                    _mouseUp = false;
                    break;
                }
                case EventType.MouseUp:
                {
                    _mouseUp = true;
                    break;
                }
                case EventType.Layout:
                {        
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                }
            }
        }

        public override MegaWorldTools GetTool()
        {
            return MegaWorldTools.Edit;
        }

        public EditTools GetCurrentEditTool()
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            switch (edit.PrecisionModifyMode)
            {
                case PrecisionModifyMode.Position:
                {
                    if(edit.PositionTool == EditTools.UnityHandles)
                    {
                        return EditTools.UnityHandles;
                    }
                    else
                    {
                        return EditTools.MouseModify;
                    }
                }
                case PrecisionModifyMode.Rotation:
                {
                    if(edit.RotationTool == EditTools.UnityHandles)
                    {
                        return EditTools.UnityHandles;
                    }
                    else
                    {
                        return EditTools.MouseModify;
                    }
                }
                case PrecisionModifyMode.Scale:
                {
                    if(edit.ScaleTool == EditTools.UnityHandles)
                    {
                        return EditTools.UnityHandles;
                    }
                    else
                    {
                        return EditTools.MouseModify;
                    }
                }
            }

            return EditTools.UnityHandles;
        }

        public void RemoveFindObjectsIfNecessary()
        {
            if(FindQuadroItemInfo != null)
            {
                if(FindQuadroItemInfo.IsValid() == false)
                {
                    FindQuadroItemInfo = null;
                }
            }

            if(FindGameObjectInfo != null)
            {
                if(FindGameObjectInfo.IsValid() == false)
                {
                    FindGameObjectInfo = null;
                }
            }
        }

        public void HandleKeyboardEvents()
        {
            UnityEditor.Tools.current = UnityEditor.Tool.None;

            Event e = Event.current;
            KeyboardButtonStates keyboardButtonStates = MegaWorldPath.GeneralDataPackage.KeyboardButtonStates;
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;  

            if(_mouseUp)
            {
                switch (e.keyCode)
			    {
			    	case KeyCode.Space:
			    	{
			    		if(EventType.KeyUp == e.type)
			    		{
			    			MegaWorldPath.GeneralDataPackage.TransformSpace = MegaWorldPath.GeneralDataPackage.TransformSpace == TransformSpace.Global ? TransformSpace.Local : TransformSpace.Global;
                        }
    
			    		break;
			    	}
			    }

                SetCurrentEditMouseRaycastMode(keyboardButtonStates, edit);
            }
		}

        private void SetCurrentEditMouseRaycastMode(KeyboardButtonStates keyboardButtonStates, EditSettings edit)
        {
            if (EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_Position.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Position;
            }
            else if(EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_Rotation.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Rotation;
            }
            else if(EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_Scale.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Scale;
            }
            else if(EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_Remove.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Remove;
            }
            if (EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_Raycast.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Position;
                edit.PositionMode = PositionMode.Raycast;
            }
            else if(EditToolAllShortcutCombos.Instance.EditToolPrecisionModifyMode_MoveAlongDirection.IsActive(keyboardButtonStates))
            {
                edit.PrecisionModifyMode = PrecisionModifyMode.Position;
                edit.PositionMode = PositionMode.MoveAlongDirection;
            }
        }

        public bool SelectedRequiredResourceTypes()
		{
			if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0 || MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
            {
				return true;
			}

			return false;
		}
    }
#endif
}