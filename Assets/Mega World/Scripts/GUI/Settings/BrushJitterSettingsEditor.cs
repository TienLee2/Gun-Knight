#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushJitterSettingsEditor 
    {
        public void OnGUI(BrushSettings brush, BrushJitterSettings jitter)
        {
            if(MegaWorldPath.AdvancedSettings.EditorSettings.useLargeRanges)
			{
				brush.BrushSize = CustomEditorGUI.FloatField(brushSize, brush.BrushSize);
			}
			else
			{
				brush.BrushSize = CustomEditorGUI.Slider(brushSize, brush.BrushSize, 0.1f, MegaWorldPath.AdvancedSettings.EditorSettings.maxBrushSize);
			}

            jitter.BrushSizeJitter = CustomEditorGUI.Slider(brushJitter, jitter.BrushSizeJitter, 0f, 1f);

			CustomEditorGUI.Separator();

			jitter.BrushScatter = CustomEditorGUI.Slider(brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUI.Slider(brushJitter, jitter.BrushScatterJitter, 0f, 1f);

			CustomEditorGUI.Separator();

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				if(MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ResourceType == ResourceType.TerrainDetail
				|| MegaWorldPath.DataPackage.SelectedVariables.SelectedType.ResourceType == ResourceType.TerrainTexture)
				{
					brush.BrushRotation = CustomEditorGUI.Slider(brushRotation, brush.BrushRotation, -180f, 180f);
            		jitter.BrushRotationJitter = CustomEditorGUI.Slider(brushJitter, jitter.BrushRotationJitter, 0f, 1f);

					CustomEditorGUI.Separator();
				}
			}
        }

		[NonSerialized]
        private GUIContent brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
		[NonSerialized]
		private GUIContent brushJitter = new GUIContent("Jitter", "Control brush stroke randomness.");
		[NonSerialized]
		private GUIContent brushScatter = new GUIContent("Brush Scatter", "Randomize brush position by an offset.");
		[NonSerialized]
		private GUIContent brushRotation = new GUIContent("Brush Rotation", "Rotation of the brush.");
    }
}
#endif