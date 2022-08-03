#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class AdditionalSpawnSettingsEditor
    {
        public void OnGUI(AdditionalSpawnSettings additionalSpawnSettings, MegaWorldTools tool)
        {
            AdditionalSpawnSettings(additionalSpawnSettings, tool);
        }

        public void AdditionalSpawnSettings(AdditionalSpawnSettings additionalSpawnSettings, MegaWorldTools tool)
		{
			if(tool == MegaWorldTools.BrushErase)
			{
				additionalSpawnSettings.SuccessForErase = CustomEditorGUI.Slider(successForErase, additionalSpawnSettings.SuccessForErase, 0f, 100f);
			}
			else
			{
				additionalSpawnSettings.Success = CustomEditorGUI.Slider(success, additionalSpawnSettings.Success, 0f, 100f);
			}
		}

		public GUIContent success = new GUIContent("Success (%)");
		public GUIContent successForErase = new GUIContent("Success of Erase (%)");
    }
}
#endif
