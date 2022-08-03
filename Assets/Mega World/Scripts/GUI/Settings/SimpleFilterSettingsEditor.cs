#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class SimpleFilterSettingsEditor 
    {
        public bool viabilitySettingsFoldout = true;
        public bool additionalNoiseSettingsFoldout = true;

        public void OnGUI(SimpleFilterSettings filterSettings, string text)
        {
            FilterSettingsWindowGUI(filterSettings, text);
        }

        public void FilterSettingsWindowGUI(SimpleFilterSettings filterSettings, string text)
		{
			viabilitySettingsFoldout = CustomEditorGUI.Foldout(viabilitySettingsFoldout, text);

			if(viabilitySettingsFoldout)
			{
				EditorGUI.indentLevel++;

				DrawCheckHeight(filterSettings);
				DrawCheckSlope(filterSettings);
				DrawCheckFractalNoise(filterSettings);

				EditorGUI.indentLevel--;
			}
		}

		public void DrawCheckHeight(SimpleFilterSettings filterSettings)
		{
			filterSettings.CheckHeight = CustomEditorGUI.Toggle(checkHeight, filterSettings.CheckHeight);

			EditorGUI.indentLevel++;

			if(filterSettings.CheckHeight)
			{
				filterSettings.MinHeight = CustomEditorGUI.FloatField(new GUIContent("Min Height"), filterSettings.MinHeight);
				filterSettings.MaxHeight = CustomEditorGUI.FloatField(new GUIContent("Max Height"), filterSettings.MaxHeight);

				DrawHeightFalloff(filterSettings);
				DrawCheckHeightNoise(filterSettings);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawHeightFalloff(SimpleFilterSettings filterSettings)
		{
			filterSettings.HeightFalloffType = (FalloffType)CustomEditorGUI.EnumPopup(heightFalloffType, filterSettings.HeightFalloffType);

			if(filterSettings.HeightFalloffType != FalloffType.None)
			{
				filterSettings.HeightFalloffMinMax = CustomEditorGUI.Toggle(heightFalloffMinMax, filterSettings.HeightFalloffMinMax);
			
				if(filterSettings.HeightFalloffMinMax == true)
				{
					filterSettings.MinAddHeightFalloff = CustomEditorGUI.FloatField(minAddHeightFalloff, filterSettings.MinAddHeightFalloff);
					filterSettings.MaxAddHeightFalloff = CustomEditorGUI.FloatField(maxAddHeightFalloff, filterSettings.MaxAddHeightFalloff);
				}
				else
				{
					filterSettings.AddHeightFalloff = CustomEditorGUI.FloatField(addHeightFalloff, filterSettings.AddHeightFalloff);
				}
			}
		}

		public void DrawSlopeFalloff(SimpleFilterSettings filterSettings)
		{
			filterSettings.SlopeFalloffType = (FalloffType)CustomEditorGUI.EnumPopup(slopeFalloffType, filterSettings.SlopeFalloffType);

			if(filterSettings.SlopeFalloffType != FalloffType.None)
			{
				filterSettings.SlopeFalloffMinMax = CustomEditorGUI.Toggle(slopeFalloffMinMax, filterSettings.SlopeFalloffMinMax);

				if(filterSettings.SlopeFalloffMinMax)
				{
					filterSettings.MinAddSlopeFalloff = CustomEditorGUI.FloatField(minAddSlopeFalloff, filterSettings.MinAddSlopeFalloff);
					filterSettings.MaxAddSlopeFalloff = CustomEditorGUI.FloatField(maxAddSlopeFalloff, filterSettings.MaxAddSlopeFalloff);
				}
				else
				{
					filterSettings.AddSlopeFalloff = CustomEditorGUI.FloatField(addSlopeFalloff, filterSettings.AddSlopeFalloff);
				}
			}
		}

		void DrawCheckSlope(SimpleFilterSettings filterSettings)
		{
			filterSettings.CheckSlope = CustomEditorGUI.Toggle(checkSlope, filterSettings.CheckSlope);

			EditorGUI.indentLevel++;

			if(filterSettings.CheckSlope)
			{
				CustomEditorGUI.MinMaxSlider(slope, ref filterSettings.MinSlope, ref filterSettings.MaxSlope, 0f, 90);
				
				DrawSlopeFalloff(filterSettings);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawCheckHeightNoise(SimpleFilterSettings filterSettings)
		{
			filterSettings.HeightNoise = CustomEditorGUI.Toggle(heightNoise, filterSettings.HeightNoise);
			
			if(filterSettings.HeightNoise)
			{
				filterSettings.MinRangeNoise = CustomEditorGUI.FloatField(minRangeNoise, filterSettings.MinRangeNoise);
				filterSettings.MaxRangeNoise = CustomEditorGUI.FloatField(maxRangeNoise, filterSettings.MaxRangeNoise);
				
				EditorGUI.indentLevel++;

				filterSettings.HeightNoiseFractal.NoiseType = (NoiseType)CustomEditorGUI.EnumPopup(noiseType, filterSettings.HeightNoiseFractal.NoiseType);

				filterSettings.HeightNoiseFractal.Seed = CustomEditorGUI.IntSlider(seed, filterSettings.HeightNoiseFractal.Seed, 0, 65000);
				filterSettings.HeightNoiseFractal.Octaves = CustomEditorGUI.IntSlider(octaves, filterSettings.HeightNoiseFractal.Octaves, 1, 12);
				filterSettings.HeightNoiseFractal.Frequency = CustomEditorGUI.Slider(frequency, filterSettings.HeightNoiseFractal.Frequency, 0f, 0.01f);

				filterSettings.HeightNoiseFractal.Persistence = CustomEditorGUI.Slider(persistence, filterSettings.HeightNoiseFractal.Persistence, 0f, 1f);
				filterSettings.HeightNoiseFractal.Lacunarity = CustomEditorGUI.Slider(lacunarity, filterSettings.HeightNoiseFractal.Lacunarity, 1f, 3.5f);

				EditorGUI.indentLevel--;
			}
		}

		public void DrawCheckFractalNoise(SimpleFilterSettings filterSettings)
		{
			EditorGUI.BeginChangeCheck();

			int width = 150;
			int height = 150;

			filterSettings.CheckGlobalFractalNoise = CustomEditorGUI.Toggle(new GUIContent("Check Global Fractal Noise"), filterSettings.CheckGlobalFractalNoise);
			
			if(filterSettings.CheckGlobalFractalNoise)
			{
				EditorGUI.indentLevel++;

				filterSettings.NoisePreviewTexture = CustomEditorGUI.Foldout(filterSettings.NoisePreviewTexture, "Noise Preview Texture");

				GUILayout.BeginHorizontal();
				{
					if(filterSettings.NoisePreviewTexture )
					{
						EditorGUI.indentLevel++;

						GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

						Rect textureRect = GUILayoutUtility.GetRect(250, 250, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
						GUI.DrawTexture(textureRect, filterSettings.NoiseTexture);

						EditorGUI.indentLevel--;
					}
				}
				GUILayout.EndHorizontal();

				filterSettings.Fractal.NoiseType = (NoiseType)CustomEditorGUI.EnumPopup(new GUIContent("Noise Type"), filterSettings.Fractal.NoiseType);

				filterSettings.Fractal.Seed = CustomEditorGUI.IntSlider(seed, filterSettings.Fractal.Seed, 0, 65000);
				filterSettings.Fractal.Octaves = CustomEditorGUI.IntSlider(octaves, filterSettings.Fractal.Octaves, 1, 12);
				filterSettings.Fractal.Frequency = CustomEditorGUI.Slider(frequency, filterSettings.Fractal.Frequency, 0f, 0.01f);

				filterSettings.Fractal.Persistence = CustomEditorGUI.Slider(persistence, filterSettings.Fractal.Persistence, 0f, 1f);
				filterSettings.Fractal.Lacunarity = CustomEditorGUI.Slider(lacunarity, filterSettings.Fractal.Lacunarity, 1f, 3.5f);

				additionalNoiseSettingsFoldout = CustomEditorGUI.Foldout(additionalNoiseSettingsFoldout, "Additional Settings");

				if(additionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					filterSettings.RemapNoiseMin = CustomEditorGUI.Slider(remapNoiseMin, filterSettings.RemapNoiseMin, 0f, 1f);
					filterSettings.RemapNoiseMax = CustomEditorGUI.Slider(remapNoiseMax, filterSettings.RemapNoiseMax, 0f, 1f);

					filterSettings.Invert = CustomEditorGUI.Toggle(invert, filterSettings.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {		
				if(filterSettings.NoisePreviewTexture)
				{
					FractalNoiseCPU fractal = new FractalNoiseCPU(filterSettings.Fractal.GetNoise(), filterSettings.Fractal.Octaves, filterSettings.Fractal.Frequency / 7, filterSettings.Fractal.Lacunarity, filterSettings.Fractal.Persistence);
					filterSettings.NoiseTexture = new Texture2D(width, height);

                	float[,] arr = new float[width, height];

                	for(int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    { 
							arr[x,y] = fractal.Sample2D(x, y);
                	    }
                	}

					Utility.NormalizeArray(arr, width, height, ref filterSettings.RangeMin, ref filterSettings.RangeMax);
	
                	for (int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    {
                	        float fractalValue = arr[x, y];
							
							if (filterSettings.Invert == true)
                   			{
                   			    fractalValue = 1 - fractalValue;
                   			}

							if (fractalValue < filterSettings.RemapNoiseMin) 
                			{
                			    fractalValue = 0;
                			}
                			else if(fractalValue > filterSettings.RemapNoiseMax)
                			{
                			    fractalValue = 1;
                			}
							else
							{
								fractalValue = Mathf.InverseLerp(filterSettings.RemapNoiseMin, filterSettings.RemapNoiseMax, fractalValue);
							}

                	        filterSettings.NoiseTexture.SetPixel(x, y, new Color(fractalValue, fractalValue, fractalValue, 1));
                	    }
                	}

                	filterSettings.NoiseTexture.Apply();
				}
				else
				{
					FindNoiseRangeMinMax(filterSettings, width, height);
				}	
			}
		}

		private void FindNoiseRangeMinMax(SimpleFilterSettings filterSettings, int width, int height)
		{
			FractalNoiseCPU fractal = new FractalNoiseCPU(filterSettings.Fractal.GetNoise(), filterSettings.Fractal.Octaves, filterSettings.Fractal.Frequency / 7, filterSettings.Fractal.Lacunarity, filterSettings.Fractal.Persistence);
			filterSettings.NoiseTexture = new Texture2D(150, 150);

            float[,] arr = new float[width, height];

            for(int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
					arr[x,y] = fractal.Sample2D(x, y);
                }
            }

			Utility.NormalizeArray(arr, width, height, ref filterSettings.RangeMin, ref filterSettings.RangeMax);
		}

		private GUIContent checkHeight = new GUIContent("Check Height");
		private GUIContent heightFalloffType = new GUIContent("Height Falloff Type");
		private GUIContent heightFalloffMinMax = new GUIContent("Height Falloff Min Max");
		private GUIContent minAddHeightFalloff = new GUIContent("Min Add Height Falloff");
		private GUIContent maxAddHeightFalloff = new GUIContent("Max Add Height Falloff");
		private GUIContent addHeightFalloff = new GUIContent("Add Height Falloff");

		private GUIContent heightNoise = new GUIContent("Height Noise");
		private GUIContent noiseType = new GUIContent("Noise Type");
		private GUIContent seed = new GUIContent("Seed");
		private GUIContent octaves = new GUIContent("Octaves");
		private GUIContent frequency = new GUIContent("Frequency");
		private GUIContent persistence = new GUIContent("Persistence");
		private GUIContent lacunarity = new GUIContent("Lacunarity");
		private GUIContent minRangeNoise = new GUIContent("Min Range Noise");
		private GUIContent maxRangeNoise = new GUIContent("Max Range Noise");
		private GUIContent remapNoiseMin = new GUIContent("Remap Noise Min");
		private GUIContent remapNoiseMax = new GUIContent("Remap Noise Max");
		private GUIContent invert = new GUIContent("Invert");

		private GUIContent checkSlope = new GUIContent("Check Slope");
		private GUIContent slope = new GUIContent("Slope");	

		private GUIContent slopeFalloffType = new GUIContent("Slope Falloff Type");
		private GUIContent slopeFalloffMinMax = new GUIContent("Slope Falloff Min Max");
		private GUIContent minAddSlopeFalloff = new GUIContent("Min Add Slope Falloff");
		private GUIContent maxAddSlopeFalloff = new GUIContent("Max Add Slope Falloff");
		private GUIContent addSlopeFalloff = new GUIContent("Add Slope Falloff");

		private GUIContent checkTextures = new GUIContent("Check Unity Terrain Textures");
		private GUIContent remapTexture = new GUIContent("Remap");
	}
}
#endif
