#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushModifyToolEditor 
    {
        private Vector2 windowScrollPos;

        #region UI Settings
		private bool selectTypeSettingsFoldout = true;
		private bool selectPrototypeSettingsFoldout = true;
		#endregion

        public void OnGUI()
		{
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldTools.BrushModify);
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

			if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0 || MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
			{
				MegaWorldPath.GeneralDataPackage.BrushSettingsForModify.OnGUI("Modify Brush Settings");

				MegaWorldPath.GeneralDataPackage.BrushModifyToolSettings.OnGUI();
			}

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
						CustomEditorGUI.HelpBox("Terrain Detail doesn't work with this tool");    

						break;
					}
					case ResourceType.TerrainTexture:
					{
						CustomEditorGUI.HelpBox("Texturing doesn't work with this tool");    

						break;
					}
					case ResourceType.QuadroItem:
					{
						DrawBrushToolSettingsForQuadroRendererItem();
						break;
					}
				}
			}

			EditorGUILayout.EndScrollView();
        }

        private void DrawBrushToolSettingsForGameObject()
		{
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
							proto.SimpleModifyFilterSettings.OnGUI("Modify Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.ModifyFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
							break;
						}
					}

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
		}

		private void DrawBrushToolSettingsForQuadroRendererItem()
		{
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
							proto.SimpleModifyFilterSettings.OnGUI("Modify Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.ModifyFilterStackView, MegaWorldPath.DataPackage.SelectedVariables.SelectedType);
							break;
						}
					}

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}
		}

		public void DrawMaskFilterSettings(FilterStackView maskFilterStackView, Type type)
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