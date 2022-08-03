#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class ScatterSettingsEditor 
    {
        public bool ScatterSettingsFoldout = true;

        public void OnGUI(ScatterSettings scatterSettings, MegaWorldTools tools)
        {
            if(tools == MegaWorldTools.BrushPaint)
            {
                ScatterSettingsWindowGUI(scatterSettings);
            }
            else if(tools == MegaWorldTools.StamperTool)
            {
                GlobalScatterSettingsWindowGUI(scatterSettings);
            }
        }

        public void GlobalScatterSettingsWindowGUI(ScatterSettings scatterSettings)
		{
			ScatterSettingsFoldout = CustomEditorGUI.Foldout(ScatterSettingsFoldout, "Scatter Settings");

			if(ScatterSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				scatterSettings.StamperScatterAlgorithm = (GlobalSpawnType)CustomEditorGUI.EnumPopup(new GUIContent("Scatter Algorithm"), scatterSettings.StamperScatterAlgorithm);		
			
				switch (scatterSettings.StamperScatterAlgorithm)
				{
					case GlobalSpawnType.Grid:
					{
						GridScatterSettings(scatterSettings);

						break;
					}
					case GlobalSpawnType.RandomPoint:
					{
						scatterSettings.RandomPoint.NumberOfChecks = CustomEditorGUI.IntField(new GUIContent("Number Of Checks"), scatterSettings.RandomPoint.NumberOfChecks);	
						break;
					}
					case GlobalSpawnType.RandomClustered:
					{
						RandomClusteredScatterSettings(scatterSettings);
						
						break;
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		public void ScatterSettingsWindowGUI(ScatterSettings scatterSettings)
		{
			ScatterSettingsFoldout = CustomEditorGUI.Foldout(ScatterSettingsFoldout, "Scatter Settings");

			if(ScatterSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				Type type = MegaWorldPath.DataPackage.SelectedVariables.SelectedType;

				if(MegaWorldPath.DataPackage.SelectedVariables.SelectedType.SpawnSurface == SpawnMode.Spherical)
				{
					type.ScatterSettings.ScatterAlgorithm = ScatterAlgorithm.RandomPoint;
					RandomPointStatterSettings(scatterSettings); 	
				}
				else
				{
					type.ScatterSettings.ScatterAlgorithm = (ScatterAlgorithm)CustomEditorGUI.EnumPopup(new GUIContent("Scatter Algorithm"), type.ScatterSettings.ScatterAlgorithm);

					switch (type.ScatterSettings.ScatterAlgorithm)
					{
						case ScatterAlgorithm.RandomPoint:
						{
							RandomPointStatterSettings(scatterSettings); 	

							break;
						}
						case ScatterAlgorithm.Grid:
						{
							GridScatterSettings(scatterSettings);

							break;
						}
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		private void RandomPointStatterSettings(ScatterSettings scatterSettings) 
		{
			scatterSettings.RandomPoint.OnlyOneCheck = CustomEditorGUI.Toggle(new GUIContent("Only One Check"), scatterSettings.RandomPoint.OnlyOneCheck);

			if(!scatterSettings.RandomPoint.OnlyOneCheck)
			{
				scatterSettings.RandomPoint.MinMaxSlider = CustomEditorGUI.Toggle(new GUIContent("Min Max Slider"), scatterSettings.RandomPoint.MinMaxSlider);

				if(scatterSettings.RandomPoint.MinMaxSlider)
				{
					CustomEditorGUI.MinMaxIntSlider(new GUIContent("Checks"), ref scatterSettings.RandomPoint.MinChecks, ref scatterSettings.RandomPoint.MaxChecks, 1, MegaWorldPath.AdvancedSettings.EditorSettings.maxChecks);
				}
				else
				{
					scatterSettings.RandomPoint.Instance = CustomEditorGUI.IntSlider(new GUIContent("Checks"), scatterSettings.RandomPoint.Instance, 1, MegaWorldPath.AdvancedSettings.EditorSettings.maxChecks);
				}
			}
		}

		private void GridScatterSettings(ScatterSettings scatterSettings)
		{
			scatterSettings.Grid.RandomisationType = (RandomisationType)CustomEditorGUI.EnumPopup(new GUIContent("Randomisation Type"), scatterSettings.Grid.RandomisationType);

			if(scatterSettings.Grid.RandomisationType == RandomisationType.Sphere)
			{
				scatterSettings.Grid.Vastness = CustomEditorGUI.Slider(new GUIContent("Vastness"), scatterSettings.Grid.Vastness, 0f, 1f);
			}

			float distance = scatterSettings.Grid.GridStep.x;

			scatterSettings.Grid.UniformGrid = CustomEditorGUI.Toggle(new GUIContent("Uniform Grid"), scatterSettings.Grid.UniformGrid);

			if(scatterSettings.Grid.UniformGrid)
			{
				if(MegaWorldPath.AdvancedSettings.EditorSettings.useLargeRanges)
				{
					distance = CustomEditorGUI.FloatField(new GUIContent("Distance"), distance);
				}
				else
				{
					distance = CustomEditorGUI.Slider(new GUIContent("Distance"), distance, 0.1f, 50f);
				}
				
				scatterSettings.Grid.GridAngle = CustomEditorGUI.Slider(new GUIContent("Angle"), scatterSettings.Grid.GridAngle, 0, 360);

				scatterSettings.Grid.GridStep = Vector2.Max(new Vector2(0.5f, 0.5f), new Vector2(distance, distance));
			}
			else
			{
				scatterSettings.Grid.GridStep = Vector2.Max(new Vector2(0.1f, 0.1f), CustomEditorGUI.Vector2Field(new GUIContent("Step"), scatterSettings.Grid.GridStep));
				
				scatterSettings.Grid.GridAngle = CustomEditorGUI.Slider(new GUIContent("Angle"), scatterSettings.Grid.GridAngle, 0, 360);
			}
		}

		private void RandomClusteredScatterSettings(ScatterSettings scatterSettings)
		{
			scatterSettings.RandomClustered.NumberOfCluster = Mathf.Max(1, CustomEditorGUI.IntField(new GUIContent("Max Cluster Сhecks"), scatterSettings.RandomClustered.NumberOfCluster));
			scatterSettings.RandomClustered.MinMaxRange = CustomEditorGUI.Toggle(new GUIContent("Min Max Range"), scatterSettings.RandomClustered.MinMaxRange);

			if(scatterSettings.RandomClustered.MinMaxRange)
			{
				CustomEditorGUI.MinMaxIntSlider(new GUIContent("Max Cluster Сhecks"), ref scatterSettings.RandomClustered.MaxClusterСhecks, ref scatterSettings.RandomClustered.MinClusterСhecks, 1, 1000);
				CustomEditorGUI.MinMaxSlider(new GUIContent("Max Seed Distance"), ref scatterSettings.RandomClustered.MaxSeedDistance, ref scatterSettings.RandomClustered.MinSeedDistance, 0.1f, 500f);	
			}	
			else
			{
				scatterSettings.RandomClustered.MaxClusterСhecks = Mathf.Max(1, CustomEditorGUI.IntField(new GUIContent("Max Cluster Сhecks"), scatterSettings.RandomClustered.MaxClusterСhecks)); 
				scatterSettings.RandomClustered.MaxSeedDistance = CustomEditorGUI.Slider(new GUIContent("Max Seed Distance"), scatterSettings.RandomClustered.MaxSeedDistance, 0.1f, 500f);
			}
		}
    }
}
#endif