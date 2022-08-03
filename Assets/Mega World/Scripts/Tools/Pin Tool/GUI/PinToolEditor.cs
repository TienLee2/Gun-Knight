#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class PinToolEditor 
    {
        private Vector2 windowScrollPos;

        #region UI Settings
		private bool _selectTypeSettingsFoldout = true;
		private bool _pinToolSettingsFoldout = true;

		private bool _positionFoldout = true;
		private bool _rotationFoldout = true;
		private bool _scaleFoldout = true;
		
		#endregion
        
        public void OnGUI(PinTool pinTool)
		{		
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.Pin);
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				if(!pinTool.IsToolSupportSelectedData())
				{
					CustomEditorGUI.HelpBox("Pin Tool only works with GameObject."); 
					return; 
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Pin Tool only works with one type.");  
				return;
			}

			windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

			if(MegaWorldPath.DataPackage.SelectedVariables.HasSelectedResourceGameObject())
            {
                MegaWorldPath.GeneralDataPackage.GameObjectControllerEditor.OnGUI();
            }

			_pinToolSettingsFoldout = CustomEditorGUI.Foldout(_pinToolSettingsFoldout, "Pin Tool Settings");

			if(_pinToolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				PinSettingsWindowGUI(MegaWorldPath.GeneralDataPackage.PinToolSettings);

				EditorGUI.indentLevel--;
			}

			_selectTypeSettingsFoldout = CustomEditorGUI.Foldout(_selectTypeSettingsFoldout, "Type Settings");

			if(_selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.LayerSettings.OnGUI();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndScrollView();
		}

        public void PinSettingsWindowGUI(PinToolSettings pinToolSettings)
    	{
			_positionFoldout = CustomEditorGUI.Foldout(_positionFoldout, "Position");

			if(_positionFoldout)
			{
				EditorGUI.indentLevel++;

				pinToolSettings.Offset = CustomEditorGUI.FloatField(new GUIContent("Offset"), pinToolSettings.Offset);

				EditorGUI.indentLevel--;
			}

			_rotationFoldout = CustomEditorGUI.Foldout(_rotationFoldout, "Rotation");

			if(_rotationFoldout)
			{
				EditorGUI.indentLevel++;

				pinToolSettings.RotationTransformMode = (TransformMode)CustomEditorGUI.EnumPopup(new GUIContent("Transform Mode"), pinToolSettings.RotationTransformMode);

				switch (pinToolSettings.RotationTransformMode)
				{
					case TransformMode.Fixed:
					{
						EditorGUI.indentLevel++;

						pinToolSettings.FixedRotationValue = CustomEditorGUI.Vector3Field(new GUIContent("Rotation"), pinToolSettings.FixedRotationValue); 

						EditorGUI.indentLevel--;

						break;
					}
					case TransformMode.Snap:
					{				
						EditorGUI.indentLevel++;

						pinToolSettings.SnapRotationValue = Mathf.Max(CustomEditorGUI.FloatField(new GUIContent("Rotation Angle"), pinToolSettings.SnapRotationValue), 0.001f);

						EditorGUI.indentLevel--;

						break;
					}
				}

				if(pinToolSettings.RotationTransformMode != TransformMode.Fixed)
				{
					pinToolSettings.FromDirection = (FromDirection)CustomEditorGUI.EnumPopup(new GUIContent("Up"), pinToolSettings.FromDirection);

					if(pinToolSettings.FromDirection == FromDirection.SurfaceNormal)
					{
						EditorGUI.indentLevel++;

						pinToolSettings.WeightToNormal = CustomEditorGUI.Slider(new GUIContent("Weight To Normal"), pinToolSettings.WeightToNormal, 0, 1);

						EditorGUI.indentLevel--;
					}
				}

				EditorGUI.indentLevel--;
			}

			_scaleFoldout = CustomEditorGUI.Foldout(_scaleFoldout, "Scale");

			if(_scaleFoldout)
			{
				EditorGUI.indentLevel++;

				pinToolSettings.ScaleTransformMode = (TransformMode)CustomEditorGUI.EnumPopup(new GUIContent("Transform Mode"), pinToolSettings.ScaleTransformMode);

				switch (pinToolSettings.ScaleTransformMode)
				{
					case TransformMode.Fixed:
					{
						EditorGUI.indentLevel++;

						pinToolSettings.FixedScaleValue = CustomEditorGUI.Vector3Field(new GUIContent("Scale"), pinToolSettings.FixedScaleValue); 

						EditorGUI.indentLevel--;

						break;
					}
					case TransformMode.Snap:
					{				
						EditorGUI.indentLevel++;

						pinToolSettings.SnapScaleValue = Mathf.Max(CustomEditorGUI.FloatField(new GUIContent("Snap Scale Value"), pinToolSettings.SnapScaleValue), 0.001f);

						EditorGUI.indentLevel--;

						break;
					}
				}

				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif