#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	public enum PrefabDetailRenderMode
    {
        Grass,
        VertexLit
    }
	
    [Serializable]
    public class TerrainDetailsSettingsEditor 
    {
        public bool TerrainDetailSettingsFoldout = true;

        public void OnGUI(PrototypeTerrainDetail protoTerrainDetail, MegaWorldTools tool)
        {
            TerrainDetailSettingsWindowGUI(protoTerrainDetail, tool);
        }

        public void TerrainDetailSettingsWindowGUI(PrototypeTerrainDetail protoTerrainDetail, MegaWorldTools tool)
		{
			TerrainDetailSettingsFoldout = CustomEditorGUI.Foldout(TerrainDetailSettingsFoldout, "Terrain Detail Settings");

			if(TerrainDetailSettingsFoldout)
			{
				CustomEditorGUI.BeginChangeCheck();

				EditorGUI.indentLevel++;

				TerrainDetailSettings terrainDetailSettings = protoTerrainDetail.TerrainDetailSettings;

				terrainDetailSettings.NoiseSpread = CustomEditorGUI.Slider(noiseSpread, terrainDetailSettings.NoiseSpread, 0f, 10f);
				
				GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
                    if(CustomEditorGUI.ClickButton("Set Random For Width and Height"))
			        {
			        	terrainDetailSettings.SetRandomForWidthHeight();

						foreach (Terrain activeTerrain in Terrain.activeTerrains)
						{
							TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, protoTerrainDetail);
						}
			        }
                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();

				terrainDetailSettings.MinMax = CustomEditorGUI.Toggle(minMax, terrainDetailSettings.MinMax);

				if(terrainDetailSettings.MinMax)
				{
					float min = terrainDetailSettings.MinWidth;
					float max = terrainDetailSettings.MaxWidth;

					min = CustomEditorGUI.FloatField(new GUIContent("Min Scale"), min);
					max = CustomEditorGUI.FloatField(new GUIContent("Max Scale"), max);

					terrainDetailSettings.MinWidth = min;
					terrainDetailSettings.MaxWidth = max;
					terrainDetailSettings.MinHeight = min;
					terrainDetailSettings.MaxHeight = max;
				}
				else
				{
					terrainDetailSettings.MinWidth = CustomEditorGUI.FloatField(minWidth, terrainDetailSettings.MinWidth);
					terrainDetailSettings.MaxWidth = CustomEditorGUI.FloatField(maxWidth, terrainDetailSettings.MaxWidth);
					terrainDetailSettings.MinHeight = CustomEditorGUI.FloatField(minHeight, terrainDetailSettings.MinHeight);
					terrainDetailSettings.MaxHeight = CustomEditorGUI.FloatField(maxHeight, terrainDetailSettings.MaxHeight);
				}

				GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
                    if(CustomEditorGUI.ClickButton("Set Random For Color"))
			        {
			        	terrainDetailSettings.SetRandomForColor();

						foreach (Terrain activeTerrain in Terrain.activeTerrains)
						{
							TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, protoTerrainDetail);
						}
			        }
                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();

				terrainDetailSettings.OnlyOneColor = CustomEditorGUI.Toggle(onlyOneColor, terrainDetailSettings.OnlyOneColor);
				
				if(terrainDetailSettings.OnlyOneColor)
				{
					Color color = terrainDetailSettings.HealthyColour;
					color = CustomEditorGUI.ColorField(new GUIContent("Color"), color);

					terrainDetailSettings.HealthyColour = color;
					terrainDetailSettings.DryColour = color;
				}
				else
				{
					terrainDetailSettings.HealthyColour = CustomEditorGUI.ColorField(healthyColour, terrainDetailSettings.HealthyColour);
					terrainDetailSettings.DryColour = CustomEditorGUI.ColorField(dryColour, terrainDetailSettings.DryColour);
				}

				if(protoTerrainDetail.PrefabType == PrefabType.Texture)
				{
					terrainDetailSettings.Billboard = CustomEditorGUI.Toggle(billboard, terrainDetailSettings.Billboard);

					if(terrainDetailSettings.Billboard)
					{
						terrainDetailSettings.RenderMode = DetailRenderMode.GrassBillboard;
					}
					else
					{
						terrainDetailSettings.RenderMode = DetailRenderMode.Grass;
					}
				}
				else
				{
					PrefabDetailRenderMode prefabDetailRenderMode;

					if(terrainDetailSettings.RenderMode == DetailRenderMode.Grass)
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.Grass;
					}
					else if(terrainDetailSettings.RenderMode == DetailRenderMode.VertexLit)
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.VertexLit;
					}
					else
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.Grass;
					}

					prefabDetailRenderMode = (PrefabDetailRenderMode)CustomEditorGUI.EnumPopup(new GUIContent("Render Mode"), prefabDetailRenderMode);

					terrainDetailSettings.RenderMode = prefabDetailRenderMode == PrefabDetailRenderMode.Grass ? DetailRenderMode.Grass : DetailRenderMode.VertexLit;
				}

				GUILayout.Space(3);

				EditorGUI.indentLevel--;

				if(CustomEditorGUI.EndChangeCheck())
				{
					foreach (Terrain activeTerrain in Terrain.activeTerrains)
					{
						TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, protoTerrainDetail);
					}
				}
			}
		}

		public GUIContent noiseSpread = new GUIContent("Noise Spread");
		public GUIContent minMax = new GUIContent("Min Max");
		public GUIContent minWidth = new GUIContent("Min Width");
		public GUIContent maxWidth = new GUIContent("Max Width");
		public GUIContent minHeight = new GUIContent("Min Height");
		public GUIContent maxHeight = new GUIContent("Max Height");
		public GUIContent onlyOneColor = new GUIContent("Only One Color");
		public GUIContent healthyColour = new GUIContent("Healthy Colour");
		public GUIContent dryColour = new GUIContent("Dry Colour");
		public GUIContent billboard = new GUIContent("Billboard");
    }
}
#endif