#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld.Edit
{
    [Serializable]
    public class EditToolEditor 
    {
        private Vector2 _windowScrollPos;
		private bool _editSettingsFoldout = true;
		private bool _positionFoldout = true;
		private bool _rotationFoldout = true;
		private bool _scaleFoldout = true;
		public bool _hotkeysFoldout = false;

		public static GUIStyle TitleStyle
        {
            get
            {
                GUIStyle guiStyle = new GUIStyle
                {
                    richText = true,
                };

                return guiStyle;
            }
        }

        public void OnGUI(EditTool editTool)
		{
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.Edit);
			}

			EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

			if(editTool.SelectedRequiredResourceTypes() == false)
            {
                CustomEditorGUI.HelpBox("Edit tool only works with GameObject and QuadroItem");   

				return;
            }

			_windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

			if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0 
				|| MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
			{
				EditSettingsWindowGUI(MegaWorldPath.GeneralDataPackage.EditSettings);
			}

			EditorGUILayout.EndScrollView();
		}

        public void EditSettingsWindowGUI(EditSettings edit)
		{
			DrawPrecisionModifyModeButtons();
			HotKeys();
			DrawEditToolSettings();
		}

		public void DrawEditToolSettings()
		{
			EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

			if((int)edit.GroundLayers == 0)
			{
				CustomEditorGUI.HelpBox("Select Ground Layers. The Ray hits the object and detects objects around the hit ray.");
				edit.GroundLayers = CustomEditorGUI.LayerField(new GUIContent("Ground Layers"), edit.GroundLayers);
				return;
			}

			_editSettingsFoldout = CustomEditorGUI.Foldout(_editSettingsFoldout, "Edit Settings");

			if(_editSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				edit.SphereSize = CustomEditorGUI.Slider(new GUIContent("Sphere Size"), edit.SphereSize, 1f, 50);
				edit.MaxDistance = CustomEditorGUI.Slider(new GUIContent("Max Distance"), edit.MaxDistance, 1f, 500);
				edit.GroundLayers = CustomEditorGUI.LayerField(new GUIContent("Ground Layers"), edit.GroundLayers);

				_positionFoldout = CustomEditorGUI.Foldout(_positionFoldout, "Position Settings");

				if(_positionFoldout)
				{
					EditorGUI.indentLevel++;

					DrawToolsButton(ref edit.PositionTool);

					switch (edit.PositionTool)
					{
						case EditTools.UnityHandles:
						{
							edit.RaycastPositionOffset = CustomEditorGUI.FloatField(new GUIContent("Raycast Position Offset"), edit.RaycastPositionOffset);
							MegaWorldPath.GeneralDataPackage.TransformSpace = (TransformSpace)CustomEditorGUI.EnumPopup(new GUIContent("Transform Space"), MegaWorldPath.GeneralDataPackage.TransformSpace);
							edit.GroundLayers = CustomEditorGUI.LayerField(new GUIContent("Ground Layers"), edit.GroundLayers);

							break;
						}
						case EditTools.MouseModify:
						{
							edit.RaycastPositionOffset = CustomEditorGUI.FloatField(new GUIContent("Raycast Position Offset"), edit.RaycastPositionOffset);
							edit.ObjectMouseModify.ObjectMouseMoveAlongDirectionSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Move Along Direction Sensitivity"), edit.ObjectMouseModify.ObjectMouseMoveAlongDirectionSettings.MouseSensitivity, 
								ObjectMouseMoveAlongDirectionSettings.MinMouseSensitivity, ObjectMouseMoveAlongDirectionSettings.MaxMouseSensitivity);
							edit.GroundLayers = CustomEditorGUI.LayerField(new GUIContent("Ground Layers"), edit.GroundLayers);

							break;
						}
					}

					EditorGUI.indentLevel--;
				}

				_rotationFoldout = CustomEditorGUI.Foldout(_rotationFoldout, "Rotation Settings");

				if(_rotationFoldout)
				{
					EditorGUI.indentLevel++;

					DrawToolsButton(ref edit.RotationTool);

					switch (edit.RotationTool)
					{
						case EditTools.UnityHandles:
						{
							break;
						}
						case EditTools.MouseModify:
						{
							edit.ObjectMouseModify.ObjectMouseRotationSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Rotation Sensitivity"), edit.ObjectMouseModify.ObjectMouseRotationSettings.MouseSensitivity, 
								ObjectMouseRotationSettings.MinMouseSensitivity, ObjectMouseRotationSettings.MaxMouseSensitivity);
							MegaWorldPath.GeneralDataPackage.TransformSpace = (TransformSpace)CustomEditorGUI.EnumPopup(new GUIContent("Transform Space"), MegaWorldPath.GeneralDataPackage.TransformSpace);

							break;
						}
					}

					EditorGUI.indentLevel--;
				}

				_scaleFoldout = CustomEditorGUI.Foldout(_scaleFoldout, "Scale Settings");

				if(_scaleFoldout)
				{
					EditorGUI.indentLevel++;

					DrawToolsButton(ref edit.ScaleTool);

					switch (edit.ScaleTool)
					{
						case EditTools.UnityHandles:
						{
							break;
						}
						case EditTools.MouseModify:
						{
							edit.ObjectMouseModify.ObjectMouseUniformScaleSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Scale Sensitivity"), edit.ObjectMouseModify.ObjectMouseUniformScaleSettings.MouseSensitivity, 
								ObjectMouseUniformScaleSettings.MinMouseSensitivity, ObjectMouseUniformScaleSettings.MaxMouseSensitivity);

							break;
						}
					}

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
		}

		public void DrawToolsButton(ref EditTools editTool)
		{
			GUILayout.BeginVertical();
			{
				GUILayout.BeginHorizontal();
            	{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ToggleButton("Unity Handles", editTool == EditTools.UnityHandles, ButtonStyle.General))
					{
						editTool = EditTools.UnityHandles;
					}
					GUILayout.Space(3);
					if(CustomEditorGUI.ToggleButton("Mouse Modify", editTool == EditTools.MouseModify , ButtonStyle.General))
					{
						editTool = EditTools.MouseModify ;
					}
					GUILayout.Space(5);
            	}
				GUILayout.EndHorizontal();
				GUILayout.Space(2);
			}
			GUILayout.EndVertical();
		}

		public void DrawPrecisionModifyModeButtons()
		{
			EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

			GUILayout.BeginVertical();
			{
				GUILayout.BeginHorizontal();
            	{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ToggleButton("Position", edit.PrecisionModifyMode == PrecisionModifyMode.Position, ButtonStyle.General))
					{
						edit.PrecisionModifyMode = PrecisionModifyMode.Position;
					}
					GUILayout.Space(3);
					if(CustomEditorGUI.ToggleButton("Rotation", edit.PrecisionModifyMode == PrecisionModifyMode.Rotation, ButtonStyle.General))
					{
						edit.PrecisionModifyMode = PrecisionModifyMode.Rotation;
					}
					GUILayout.Space(3);
					if(CustomEditorGUI.ToggleButton("Scale", edit.PrecisionModifyMode == PrecisionModifyMode.Scale, ButtonStyle.General))
					{
						edit.PrecisionModifyMode = PrecisionModifyMode.Scale;
					}
                    GUILayout.Space(3);
					if(CustomEditorGUI.ToggleButton("Remove", edit.PrecisionModifyMode == PrecisionModifyMode.Remove, ButtonStyle.Remove))
					{
						edit.PrecisionModifyMode = PrecisionModifyMode.Remove;
					}
					GUILayout.Space(5);
            	}
				GUILayout.EndHorizontal();
				GUILayout.Space(2);
			}
			GUILayout.EndVertical();

			if(edit.PositionTool == EditTools.MouseModify )
			{
				EditorGUI.indentLevel++;
            	EditorGUI.indentLevel++;

				if(edit.PrecisionModifyMode == PrecisionModifyMode.Position)
				{
					DrawPositionMode();
				}

				EditorGUI.indentLevel--;
            	EditorGUI.indentLevel--;
			}
		}

		public void DrawPositionMode()
		{
			GUILayout.BeginVertical();
			{
				GUILayout.BeginHorizontal();
            	{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ToggleButton("Move Along Direction", MegaWorldPath.GeneralDataPackage.EditSettings.PositionMode == PositionMode .MoveAlongDirection, ButtonStyle.General))
					{
						MegaWorldPath.GeneralDataPackage.EditSettings.PositionMode = PositionMode.MoveAlongDirection;
					}
					GUILayout.Space(3);
					if(CustomEditorGUI.ToggleButton("Raycast", MegaWorldPath.GeneralDataPackage.EditSettings.PositionMode == PositionMode .Raycast, ButtonStyle.General))
					{
						MegaWorldPath.GeneralDataPackage.EditSettings.PositionMode = PositionMode .Raycast;
					}
					GUILayout.Space(5);
            	}
				GUILayout.EndHorizontal();
				GUILayout.Space(2);
			}
			GUILayout.EndVertical();
		}

		public void HotKeys()
		{
			EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

			_hotkeysFoldout = CustomEditorGUI.Foldout(_hotkeysFoldout, "Hotkeys");

			if(_hotkeysFoldout)
			{
				EditorGUI.indentLevel++;
				CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>W</i></b> - Position.</color></size>", TitleStyle);

				if(edit.PositionTool == EditTools.MouseModify )
				{
					EditorGUI.indentLevel++;
					CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>Q</i></b> - Move Along Direction.</color></size>", TitleStyle);
                	CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>Ctrl + LSHIFT</i></b> - Raycast.</color></size>", TitleStyle);
					EditorGUI.indentLevel--;
				}
				else
				{
					EditorGUI.indentLevel++;
					CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>Space</i></b> - Transform Space.</color></size>", TitleStyle);
					EditorGUI.indentLevel--;
				}
				
                CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>E</i></b> - Rotation.</color></size>", TitleStyle);

				if(edit.RotationTool == EditTools.MouseModify)
				{
					EditorGUI.indentLevel++;
					CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>Space</i></b> - Transform Space.</color></size>", TitleStyle);
					EditorGUI.indentLevel--;
				}

                CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>R</i></b> - Scale.</color></size>", TitleStyle);
                CustomEditorGUI.Label("<size=14><color=#" + ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) + ">" + "<b><i>T</i></b> - Remove.</color></size>", TitleStyle);
			
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif