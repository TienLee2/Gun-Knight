#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class EditorSettingsEditor 
    {
        public bool editorSettingsFoldout = true;

        public void OnGUI(EditorSettings editorSettings)
        {
            EditorSettingsWindowGUI(editorSettings);
        }

        public void EditorSettingsWindowGUI(EditorSettings editorSettings)
		{
			editorSettingsFoldout = CustomEditorGUI.Foldout(editorSettingsFoldout, "Editor Settings");

			if(editorSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				editorSettings.useLargeRanges = CustomEditorGUI.Toggle(new GUIContent("Use Large Ranges"), editorSettings.useLargeRanges);
				editorSettings.maxBrushSize = Mathf.Max(0.5f, CustomEditorGUI.FloatField(new GUIContent("Max Brush Size"), editorSettings.maxBrushSize));
				editorSettings.maxChecks = Mathf.Max(1, CustomEditorGUI.IntField(new GUIContent("Max Checks"), editorSettings.maxChecks));
				editorSettings.maxTargetStrength = Mathf.Max(0, CustomEditorGUI.IntField(new GUIContent("Max Target Strength"), editorSettings.maxTargetStrength));
				
				editorSettings.raycastSettings.OnGUI();
				
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif