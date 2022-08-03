#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushPaintToolEditor 
    {
        private Vector2 windowScrollPos;

        #region UI Settings
		public bool selectTypeSettingsFoldout = true;
		public bool selectPrototypeSettingsFoldout = true;
        public bool brushPaintToolSettingsFoldout = true;
		#endregion

        public void OnGUI()
		{
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.BrushPaint);
			}

            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList.Count == 0)
            {
                return;
            }

			windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

			if(MegaWorldPath.DataPackage.SelectedVariables.HasSelectedResourceGameObject())
            {
                MegaWorldPath.GeneralDataPackage.GameObjectControllerEditor.OnGUI();
            }

			if(!MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				MegaWorldPath.GeneralDataPackage.MultipleBrushSettings.OnGUI("Multiple Brush Settings");

				CustomEditorGUI.HelpBox("Select one type to display type settings");   
			}
			else 
			{
				switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ResourceType)
				{
					case ResourceType.GameObject:
					{
						DrawBrushToolSettingsForGameObject();

						break;
					}
					case ResourceType.TerrainDetail:
					{
						DrawBrushToolSettingsForTerrainDetail();

						break;
					}
					case ResourceType.TerrainTexture:
					{
						DrawBrushToolSettingsForTerrainTexture();

						break;
					}
					case ResourceType.QuadroItem:
					{
						DrawBrushToolSettingsForQuadroItem();
						break;
					}
				}
			}

			EditorGUILayout.EndScrollView();
        }

        private void DrawBrushToolSettingsForGameObject()
		{
			DrawGlobalSpawnSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface = (SpawnMode)CustomEditorGUI.EnumPopup(new GUIContent("Spawn Surface"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.BrushSettings.OnGUI("Brush Settings");
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ScatterSettings.OnGUI(MegaWorldTools.BrushPaint);
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

					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.BrushPaint);

					if(MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface == SpawnMode.Spherical)
					{
						proto.SimpleFilterSettings.OnGUI("Simple Filter Settings");
					}
					else
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
								CustomEditorGUI.HelpBox("\"Mask Filter\" works only with Unity terrain");
								DrawMaskFilterSettings(proto.MaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
								break;
							}
						}
					}

					proto.FailureSettings.OnGUI();
					proto.OverlapCheckSettings.OnGUI();
					TransformSettingWindowGUI(proto.TransformComponentView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
					proto.FlagsSettings.OnGUI();

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
		}

		private void DrawBrushToolSettingsForQuadroItem()
		{
			DrawGlobalSpawnSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType);
				QuadroRendererController.QuadroRendererControllerEditor.QuadroRenderControllerWindowGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.BrushSettings.OnGUI("Brush Settings");
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ScatterSettings.OnGUI(MegaWorldTools.BrushPaint);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.LayerSettings.OnGUI();

				EditorGUI.indentLevel--;
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoQuadroItem())
			{
				PrototypeQuadroItem proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItem;

				selectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(selectPrototypeSettingsFoldout, proto.prefab.name);

				if(selectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					TransformComponentsStack terrainTransformSettings = proto.TransformComponentsStack;

					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.BrushPaint);

					switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType)
					{
						case FilterType.SimpleFilter:
						{
							proto.SimpleFilterSettings.OnGUI("Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							CustomEditorGUI.HelpBox("\"Mask Filter\" works only with Unity terrain");
							DrawMaskFilterSettings(proto.MaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
							break;
						}
					}

					proto.FailureSettings.OnGUI();
					proto.OverlapCheckSettings.OnGUI();
					TransformSettingWindowGUI(proto.TransformComponentView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
		}

		private void DrawBrushToolSettingsForTerrainDetail()
		{
			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.TerrainResourcesControllerEditor.OnGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.BrushSettings.OnGUI("Brush Settings");

				EditorGUI.indentLevel--;
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainDetail())
			{
				PrototypeTerrainDetail proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainDetail;

				selectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(selectPrototypeSettingsFoldout, proto.TerrainDetailName);

				if(selectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					proto.SpawnDetailSettings.OnGUI(MegaWorldTools.BrushPaint);
					DrawMaskFilterSettings(proto.MaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
					proto.FailureSettings.OnGUI();
					proto.TerrainDetailSettings.OnGUI(proto, MegaWorldTools.BrushPaint);				

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
		}

		private void DrawBrushToolSettingsForTerrainTexture()
		{
			DrawTextureToolSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.TerrainResourcesControllerEditor.OnGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.BrushSettings.OnGUI("Brush Settings");

				EditorGUI.indentLevel--;
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainTexture())
			{
				PrototypeTerrainTexture proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainTexture;
				
				selectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(selectPrototypeSettingsFoldout, proto.TerrainTextureName);

				if(selectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawMaskFilterSettings(proto.MaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");    
			}
		}

        public void DrawTextureToolSettings()
		{
			brushPaintToolSettingsFoldout = CustomEditorGUI.Foldout(brushPaintToolSettingsFoldout, "Brush Paint Tool Settings");

			if(brushPaintToolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.TextureTargetStrength = CustomEditorGUI.Slider(new GUIContent("Target Strength"), MegaWorldPath.GeneralDataPackage.TextureTargetStrength, 0, 1);

				EditorGUI.indentLevel--;
			}
		}

		public void DrawGlobalSpawnSettings()
		{
			brushPaintToolSettingsFoldout = CustomEditorGUI.Foldout(brushPaintToolSettingsFoldout, "Brush Paint Tool Settings");

			if(brushPaintToolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.DragFailureRate = CustomEditorGUI.Slider(new GUIContent("Drag Failure Rate (%)"), MegaWorldPath.GeneralDataPackage.DragFailureRate, 0f, 100f);

				EditorGUI.indentLevel--;
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
    }
}
#endif