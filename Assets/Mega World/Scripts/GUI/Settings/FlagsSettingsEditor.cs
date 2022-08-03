#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class FlagsSettingsEditor 
    {
        public bool flagSettingsFoldout = true;

        public void OnGUI(FlagsSettings flagsSettings)
        {
            FlagSettingsWindowGUI(flagsSettings);
        }

        public void FlagSettingsWindowGUI(FlagsSettings flagsSettings)
		{
			flagSettingsFoldout = CustomEditorGUI.Foldout(flagSettingsFoldout, "Flags Settings");

			if(flagSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				flagsSettings.TagsSelectedType = (TagsSelectedType)CustomEditorGUI.EnumPopup(new GUIContent("Tags Selected Type"), flagsSettings.TagsSelectedType);

				if(flagsSettings.TagsSelectedType == TagsSelectedType.Custom)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.BeginVertical();
						{
							flagsSettings.BatchingStatic = CustomEditorGUI.Toggle(new GUIContent("Batching Static"), flagsSettings.BatchingStatic);
							flagsSettings.LightmapStatic = CustomEditorGUI.Toggle(new GUIContent("Lightmap Static"), flagsSettings.LightmapStatic);
							flagsSettings.NavigationStatic = CustomEditorGUI.Toggle(new GUIContent("Navigation Static"), flagsSettings.NavigationStatic);
							flagsSettings.OccludeeStatic = CustomEditorGUI.Toggle(new GUIContent("Occludee Static"), flagsSettings.OccludeeStatic);
						}
						GUILayout.EndVertical();

						GUILayout.BeginVertical();
						{
							flagsSettings.OccluderStatic = CustomEditorGUI.Toggle(new GUIContent("Occluder Static"), flagsSettings.OccluderStatic);
							flagsSettings.OffMeshLinkGeneration = CustomEditorGUI.Toggle(new GUIContent("Off Mesh Link Generation"), flagsSettings.OffMeshLinkGeneration);
							flagsSettings.ReflectionProbeStatic = CustomEditorGUI.Toggle(new GUIContent("Reflection Probe Static"), flagsSettings.ReflectionProbeStatic);
						}
						GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();
				}		

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif
