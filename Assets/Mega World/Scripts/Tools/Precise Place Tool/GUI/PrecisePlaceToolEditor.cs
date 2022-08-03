#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld.PrecisePlace
{
    [Serializable]
    public class PrecisionPlaceToolEditor 
    {
        private Vector2 windowScrollPos;

        #region UI Settings
		private bool _rotationFoldout = true;

		private bool _prototypeToggleSettingsFoldout = true;
		private bool _precisionPlaceToolSettingsFoldout = true;
		private bool selectTypeSettingsFoldout = true;
		private bool selectPrototypeSettingsFoldout = true;
		private bool mouseSensitivitySettingsFoldout = true;
		#endregion

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

        public void OnGUI(PrecisePlaceTool precisePlaceTool)
		{		
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.PrecisePlace);
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				if(!precisePlaceTool.IsToolSupportSelectedData())
				{
					CustomEditorGUI.HelpBox("Precise Place Tool only works with GameObject.");  
					return;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Precise Place Tool only works with one type.");   
				return;
			}

			windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

			PrecisePlaceToolSettings precisePaint = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

			if(MegaWorldPath.DataPackage.SelectedVariables.HasSelectedResourceGameObject())
            {
                MegaWorldPath.GeneralDataPackage.GameObjectControllerEditor.OnGUI();
            }

			_precisionPlaceToolSettingsFoldout = CustomEditorGUI.Foldout(_precisionPlaceToolSettingsFoldout, "Precision Place Tool Settings");

			if(_precisionPlaceToolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				PrecisionSettingsWindowGUI();

				EditorGUI.indentLevel--;
			}

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				if(precisePaint.EnableFilter)
				{
					MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface = (SpawnMode)CustomEditorGUI.EnumPopup(new GUIContent("Spawn Surface"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface);
					MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType);
				}

				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.LayerSettings.OnGUI();

				EditorGUI.indentLevel--;
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoGameObject())
			{
				PrototypeGameObject proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObject;

				selectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(selectPrototypeSettingsFoldout, proto.prefab.name);

				if(selectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					proto.PositionOffset = CustomEditorGUI.FloatField(new GUIContent("Position Offset"), proto.PositionOffset);

					if(precisePaint.EnableFilter)
					{
						switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType)
						{
							case FilterType.SimpleFilter:
							{
								proto.SimpleFilterSettings.OnGUI("Simple Filter Settings");
								break;
							}
							case FilterType.MaskFilter:
							{
								DrawMaskFilterSettings(proto.MaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
								break;
							}
						}
					}

					if(precisePaint.OverlapCheck)
					{
						proto.OverlapCheckSettings.OnGUI();
					}

					if(precisePaint.RandomiseTransform)
					{
						TransformSettingWindowGUI(proto.TransformComponentView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
					}

					proto.FlagsSettings.OnGUI();

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display");  
			}

			EditorGUILayout.EndScrollView();
		}

        public void PrecisionSettingsWindowGUI()
		{
			PrecisePlaceToolSettings presitionPaint = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

			mouseSensitivitySettingsFoldout = CustomEditorGUI.Foldout(mouseSensitivitySettingsFoldout, "Mouse Sensitivity Settings");

			if(mouseSensitivitySettingsFoldout)
			{
				EditorGUI.indentLevel++;

				ObjectMouseRotationSettings objectMouseRotationSettings = presitionPaint.ObjectMousePrecision.ObjectMouseRotation.objectMouseRotationSettings;
				ObjectMouseUniformScaleSettings objectMouseUniformScaleSettings = presitionPaint.ObjectMousePrecision.ObjectMouseUniformScale.objectMouseUniformScaleSettings;
				ObjectMouseMoveAlongDirectionSettings objectMouseMoveAlongDirectionSettings = presitionPaint.ObjectMousePrecision.ObjectMouseMoveAlongDirection.objectMouseMoveAlongDirectionSettings;

				objectMouseRotationSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Rotation Sensitivity"), objectMouseRotationSettings.MouseSensitivity, ObjectMouseRotationSettings.MinMouseSensitivity, ObjectMouseRotationSettings.MaxMouseSensitivity);
				objectMouseUniformScaleSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Uniform Scale Sensitivity"), objectMouseUniformScaleSettings.MouseSensitivity, ObjectMouseUniformScaleSettings.MinMouseSensitivity, ObjectMouseUniformScaleSettings.MaxMouseSensitivity);
				objectMouseMoveAlongDirectionSettings.MouseSensitivity = CustomEditorGUI.Slider(new GUIContent("Mouse Move Direction Sensitivity"), objectMouseMoveAlongDirectionSettings.MouseSensitivity, ObjectMouseMoveAlongDirectionSettings.MinMouseSensitivity, ObjectMouseMoveAlongDirectionSettings.MaxMouseSensitivity);

				EditorGUI.indentLevel--;
			}

			_prototypeToggleSettingsFoldout = CustomEditorGUI.Foldout(_prototypeToggleSettingsFoldout, "Prototype Toggle Settings");

			if(_prototypeToggleSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				presitionPaint.EnableFilter = CustomEditorGUI.Toggle(new GUIContent("Enable Filter"), presitionPaint.EnableFilter);
				if(presitionPaint.EnableFilter)
				{
					EditorGUI.indentLevel++;

					presitionPaint.VisualizeFilterSettings = CustomEditorGUI.Toggle(new GUIContent("Visualize Filter Settings"), presitionPaint.VisualizeFilterSettings);
					presitionPaint.FilterVisualisationSize = CustomEditorGUI.Slider(new GUIContent("Filter Visualisation Size"), presitionPaint.FilterVisualisationSize, 0.1f, 40f);
					EditorGUI.indentLevel--;
				}

				presitionPaint.RandomiseTransform = CustomEditorGUI.Toggle(new GUIContent("Randomise Transform"), presitionPaint.RandomiseTransform);
				presitionPaint.OverlapCheck = CustomEditorGUI.Toggle(new GUIContent("Overlap Check"), presitionPaint.OverlapCheck);

				if(presitionPaint.OverlapCheck)
				{
					EditorGUI.indentLevel++;
					presitionPaint.VisualizeOverlapCheckSettings = CustomEditorGUI.Toggle(new GUIContent("Visualize Overlap Check Settings"), presitionPaint.VisualizeOverlapCheckSettings);
					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			presitionPaint.Spacing = Mathf.Max(CustomEditorGUI.FloatField(new GUIContent("Spacing"), presitionPaint.Spacing), 0.5f);
			presitionPaint.SelectType = (PreciseSelectType)CustomEditorGUI.EnumPopup(new GUIContent("Select Type"), presitionPaint.SelectType);

			_rotationFoldout = CustomEditorGUI.Foldout(_rotationFoldout, "Rotation");

			if(_rotationFoldout)
			{
				EditorGUI.indentLevel++;

				presitionPaint.AlignAxis = CustomEditorGUI.Toggle(new GUIContent("Align Axis"), presitionPaint.AlignAxis);

				if(presitionPaint.AlignAxis)
				{
					EditorGUI.indentLevel++;

					presitionPaint.AlignmentAxis = (CoordinateSystemAxis)CustomEditorGUI.EnumPopup(new GUIContent("Alignment Axis"), presitionPaint.AlignmentAxis);
					presitionPaint.WeightToNormal = CustomEditorGUI.Slider(new GUIContent("Weight To Normal"), presitionPaint.WeightToNormal, 0, 1);

					EditorGUI.indentLevel--;
				}

				presitionPaint.AlongStroke = CustomEditorGUI.Toggle(new GUIContent("Along Stroke"), presitionPaint.AlongStroke);
				MegaWorldPath.GeneralDataPackage.TransformSpace = (TransformSpace)CustomEditorGUI.EnumPopup(new GUIContent("Transform Space"), MegaWorldPath.GeneralDataPackage.TransformSpace);

				EditorGUI.indentLevel--;
			}
		}

		public static void TransformSettingWindowGUI(TransformComponentsView transformComponentsView, Type type)
		{
			if(transformComponentsView.headerFoldout)
			{
				Rect rect = EditorGUILayout.GetControlRect(true, transformComponentsView.m_reorderableList.GetHeight());
				rect = EditorGUI.IndentedRect(rect);

				transformComponentsView.OnGUI(rect, type);
			}
			else
			{
				transformComponentsView.headerFoldout = CustomEditorGUI.Foldout(transformComponentsView.headerFoldout, transformComponentsView.m_label.text);
			}
		}

		public static void DrawMaskFilterSettings(FilterStackView maskFilterStackView, Type type)
		{
			if(maskFilterStackView.headerFoldout)
			{
				Rect maskRect = EditorGUILayout.GetControlRect(true, maskFilterStackView.m_reorderableList.GetHeight());
				maskRect = EditorGUI.IndentedRect(maskRect);

				maskFilterStackView.OnGUI(maskRect, type);
			}
			else
			{
				maskFilterStackView.headerFoldout = CustomEditorGUI.Foldout(maskFilterStackView.headerFoldout, maskFilterStackView.m_label.text);
			}
		}
    }
}
#endif