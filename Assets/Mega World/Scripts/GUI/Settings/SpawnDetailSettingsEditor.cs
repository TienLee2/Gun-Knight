#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class SpawnDetailSettingsEditor 
    {
        public bool SpawnDetailSettingsFoldout = true;

        public void OnGUI(SpawnDetailSettings spawnDetailSettings, MegaWorldTools tool)
        {
            SpawnDetailSettings(spawnDetailSettings, tool);
        }

        public void SpawnDetailSettings(SpawnDetailSettings spawnDetailSettings, MegaWorldTools tool)
		{
			SpawnDetailSettingsFoldout = CustomEditorGUI.Foldout(SpawnDetailSettingsFoldout, "Spawn Detail Settings");

			if(SpawnDetailSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				spawnDetailSettings.Opacity = CustomEditorGUI.Slider(opacity, spawnDetailSettings.Opacity, 0f, 100f);
				
				if(tool == MegaWorldTools.BrushErase)
				{
					spawnDetailSettings.SuccessOfErase = CustomEditorGUI.Slider(successOfErase, spawnDetailSettings.SuccessOfErase, 0f, 100f);
				}
				else
				{
					spawnDetailSettings.UseRandomOpacity = CustomEditorGUI.Toggle(useRandomOpacity, spawnDetailSettings.UseRandomOpacity);
					spawnDetailSettings.TargetStrength = CustomEditorGUI.IntSlider(targetStrength, spawnDetailSettings.TargetStrength, 0, MegaWorldPath.AdvancedSettings.EditorSettings.maxTargetStrength);
				}

				EditorGUI.indentLevel--;
			}
		}

		public GUIContent opacity = new GUIContent("Opacity (%)");
		public GUIContent successOfErase = new GUIContent("Success of Erase (%)");
		public GUIContent targetStrength = new GUIContent("Target Strength");
		public GUIContent useRandomOpacity = new GUIContent("Use Random Opacity");
    }
}
#endif
