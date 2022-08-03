#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushEraseToolEditor 
    {
        private Vector2 windowScrollPos;

        #region UI Settings
		public bool selectTypeSettingsFoldout = true;
		public bool selectPrototypeSettingsFoldout = true;
		public bool eraseToolSettingsFoldout = true;
		#endregion

        public void OnGUI()
		{
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.BrushErase);
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
				MegaWorldPath.GeneralDataPackage.BrushSettingsForErase.OnGUI("Erase Brush Settings");

				DrawEraseToolSettings();

				CustomEditorGUI.HelpBox("Select one type to display type settings");   
			}
			else 
			{
				if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
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
					    	CustomEditorGUI.HelpBox("Texturing doesn't work with this tool");    

					    	break;
					    }
						case ResourceType.QuadroItem:
						{
							DrawBrushToolSettingsForQuadroItem();
							break;
						}
					}
				}
			}

			EditorGUILayout.EndScrollView();
        }

        private void DrawBrushToolSettingsForGameObject()
		{
			MegaWorldPath.GeneralDataPackage.BrushSettingsForErase.OnGUI("Erase Brush Settings");

			DrawEraseToolSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType);
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

					switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType)
					{
						case FilterType.SimpleFilter:
						{
							proto.SimpleEraseFilterSettings.OnGUI("Erase Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.EraseMaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
							break;
						}
					}

					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.BrushErase);

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
			MegaWorldPath.GeneralDataPackage.BrushSettingsForErase.OnGUI("Erase Brush Settings");

			DrawEraseToolSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType);
				QuadroRendererController.QuadroRendererControllerEditor.QuadroRenderControllerWindowGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
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

					switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedType.FilterType)
					{
						case FilterType.SimpleFilter:
						{
							proto.SimpleEraseFilterSettings.OnGUI("Erase Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.EraseMaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
							break;
						}
					}
					
					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.BrushErase);

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
			MegaWorldPath.GeneralDataPackage.BrushSettingsForErase.OnGUI("Erase Brush Settings");

			DrawEraseToolSettings();

			selectTypeSettingsFoldout = CustomEditorGUI.Foldout(selectTypeSettingsFoldout, "Type Settings");

			if(selectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.TerrainResourcesControllerEditor.OnGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedType);

				EditorGUI.indentLevel--;
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainDetail())
			{
				PrototypeTerrainDetail proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainDetail;

				selectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(selectPrototypeSettingsFoldout, proto.TerrainDetailName);

				if(selectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					proto.SpawnDetailSettings.OnGUI(MegaWorldTools.BrushErase);
					DrawMaskFilterSettings(proto.EraseMaskFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
					proto.TerrainDetailSettings.OnGUI(proto, MegaWorldTools.BrushErase);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
        }

		private void DrawEraseToolSettings()
		{
			eraseToolSettingsFoldout = CustomEditorGUI.Foldout(eraseToolSettingsFoldout, "Erase Tool Settings");

			if(eraseToolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.EraseStrength = CustomEditorGUI.Slider(new GUIContent("Erase Strength"), MegaWorldPath.GeneralDataPackage.EraseStrength, 0, 1);

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