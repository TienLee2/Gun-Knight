#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class LayerSettingsEditor 
    {
        public bool layerSettingsFoldout = true;

        public void OnGUI(LayerSettings layerSettings)
        {
            LayerSettings(layerSettings);
        }

        public void LayerSettings(LayerSettings layerSettings)
		{
			layerSettingsFoldout = CustomEditorGUI.Foldout(layerSettingsFoldout, "Layer Settings");

			if(layerSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				layerSettings.PaintLayers = CustomEditorGUI.LayerField(new GUIContent("Paint Layers", "Allows you to set the layers on which to spawn."), layerSettings.PaintLayers);

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif