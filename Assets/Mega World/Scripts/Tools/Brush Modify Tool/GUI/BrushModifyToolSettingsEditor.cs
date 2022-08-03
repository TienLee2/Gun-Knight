#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld.BrushModify
{
    [Serializable]
    public class BrushModifyToolSettingsEditor 
    {
        public bool modifyToolFoldout = true;

        public void OnGUI(BrushModifyToolSettings modifySettings)
        {
            ModifySettings(modifySettings);
        }

        public void ModifySettings(BrushModifyToolSettings modifySettings)
		{
            modifyToolFoldout = CustomEditorGUI.Foldout(modifyToolFoldout, "Modify Settings");

			if(modifyToolFoldout)
			{
                ++EditorGUI.indentLevel;

                modifySettings.ModifyByLayer = CustomEditorGUI.Toggle(new GUIContent("Modify By Layer"), modifySettings.ModifyByLayer);
                GUI.enabled = modifySettings.ModifyByLayer;
                modifySettings.ModifyLayers = CustomEditorGUI.LayerField(new GUIContent("Modify Layers"), modifySettings.ModifyLayers);
                GUI.enabled = true;

                TransformSettingWindowGUI(modifySettings.ModifyTransformComponentsView);

                --EditorGUI.indentLevel;
            }
		}

        public void TransformSettingWindowGUI(ModifyTransformComponentsView transformComponentsView)
		{
			if(transformComponentsView.headerFoldout)
			{
				Rect rect = EditorGUILayout.GetControlRect(true, transformComponentsView.m_reorderableList.GetHeight());
				rect = EditorGUI.IndentedRect(rect);

				transformComponentsView.OnGUI(rect, MegaWorldPath.GeneralDataPackage);
			}
			else
			{
				transformComponentsView.headerFoldout = CustomEditorGUI.Foldout(transformComponentsView.headerFoldout, transformComponentsView.m_label.text);
			}
		}
    }
}
#endif