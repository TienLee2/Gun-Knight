#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class ProceduralMaskEditor 
    {
        public bool proceduralMaskFoldout = true;
        public bool additionalNoiseSettingsFoldout = true;
        public bool proceduralBrushPreviewTextureFoldout = true;

        public void OnGUI(ProceduralMask proceduralMask)
        {
            DrawProceduralMask(proceduralMask);
        }

        public void DrawProceduralMask(ProceduralMask proceduralMask)
		{
			proceduralMaskFoldout = CustomEditorGUI.Foldout(proceduralMaskFoldout, "Procedural Mask");

			if(proceduralMaskFoldout)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginChangeCheck();

				proceduralBrushPreviewTextureFoldout = CustomEditorGUI.Foldout(proceduralBrushPreviewTextureFoldout, "Preview Texture");

				if(proceduralBrushPreviewTextureFoldout)
				{
					EditorGUI.indentLevel++;

					Rect textureRect = EditorGUILayout.GetControlRect(GUILayout.Height(200), GUILayout.Width(200), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
					Rect indentedRect = EditorGUI.IndentedRect(textureRect);
					Rect finalRect = new Rect(new Vector2(indentedRect.x, indentedRect.y), new Vector2(200, 200));

					GUI.DrawTexture(finalRect, proceduralMask.Mask);

					EditorGUI.indentLevel--;
				}

				proceduralMask.Shape = (Shape)CustomEditorGUI.EnumPopup(shape, proceduralMask.Shape);
				proceduralMask.Falloff = CustomEditorGUI.Slider(brushFalloff, proceduralMask.Falloff, 0f, 100f);
				proceduralMask.Strength = CustomEditorGUI.Slider(brushStrength, proceduralMask.Strength, 0f, 100f);

				DrawNoiseForProceduralBrush(proceduralMask);

				if (EditorGUI.EndChangeCheck())
        		{
					proceduralMask.CreateProceduralTexture();
				}

				EditorGUI.indentLevel--;
			}
		}

        public void DrawNoiseForProceduralBrush(ProceduralMask proceduralMask)
		{
			EditorGUI.BeginChangeCheck();

			proceduralMask.FractalNoise = CustomEditorGUI.Toggle(fractalNoise, proceduralMask.FractalNoise);

			if(proceduralMask.FractalNoise)
			{
				EditorGUI.indentLevel++;

				proceduralMask.NoiseType = (NoiseType)CustomEditorGUI.EnumPopup(new GUIContent("Check Fractal Noise"), proceduralMask.NoiseType);

				proceduralMask.Seed = CustomEditorGUI.IntSlider(new GUIContent("Seed"), proceduralMask.Seed, 0, 65000);
				proceduralMask.Octaves = CustomEditorGUI.IntSlider(new GUIContent("Octaves"), proceduralMask.Octaves, 1, 12);
				proceduralMask.Frequency = CustomEditorGUI.Slider(new GUIContent("Frequency"), proceduralMask.Frequency, 0f, 0.1f);

				proceduralMask.Persistence = CustomEditorGUI.Slider(new GUIContent("Persistence"), proceduralMask.Persistence, 0f, 1f);
				proceduralMask.Lacunarity = CustomEditorGUI.Slider(new GUIContent("Lacunarity"), proceduralMask.Lacunarity, 1f, 3.5f);

				additionalNoiseSettingsFoldout = CustomEditorGUI.Foldout(additionalNoiseSettingsFoldout, "Additional Settings");

				if(additionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					proceduralMask.RemapMin = CustomEditorGUI.Slider(new GUIContent("Remap Min"), proceduralMask.RemapMin, 0f, 1f);
					proceduralMask.RemapMax = CustomEditorGUI.Slider(new GUIContent("Remap Max"), proceduralMask.RemapMax, 0f, 1f);
					
					proceduralMask.Invert = CustomEditorGUI.Toggle(new GUIContent("Invert"), proceduralMask.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {
                proceduralMask.Fractal = new FractalNoiseCPU(proceduralMask.GetNoiseForProceduralBrush(), proceduralMask.Octaves, proceduralMask.Frequency / 7, proceduralMask.Lacunarity, proceduralMask.Persistence);

				proceduralMask.FindNoiseRangeMinMaxForProceduralNoise(150, 150);
			}
		}

		[NonSerialized]
		private GUIContent shape = new GUIContent("Shape", "Allows you to select the geometric shape of the mask.");
		[NonSerialized]
		private GUIContent brushFalloff = new GUIContent("Brush Falloff (%)", "Allows you to control the brush fall by creating a gradient.");
		[NonSerialized]
		private GUIContent brushStrength = new GUIContent("Brush Strength (%)", "Allows you to change the maximum strength of the brush the lower this parameter, the closer the value.");
		[NonSerialized]
		private GUIContent fractalNoise = new GUIContent("Fractal Noise", "Mathematical algorithm for generating a procedural texture by a pseudo-random method.");
    }
}
#endif
