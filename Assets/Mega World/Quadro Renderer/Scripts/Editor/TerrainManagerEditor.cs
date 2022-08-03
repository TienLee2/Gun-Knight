using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(TerrainManager))]
    public class TerrainManagerEditor : Editor
    {
        private TerrainManager terrainManager;

        public override void OnInspectorGUI()
        {			
            EditorGUI.BeginChangeCheck();

			CustomEditorGUI.isInspector = true;

			CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.pf3u0w2f8jkt");

			GUILayout.Space(3);

			if(terrainManager.HasAllNecessaryData() == false)
			{
				DrawNecessaryData();
				return;
			}

            DrawTerrainsSettings();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(terrainManager);
            }
        }

		public void DrawNecessaryData()
        {			
			GUILayout.Space(3); 
			
			GUILayout.BeginHorizontal();
			{
				terrainManager.area = (Area)CustomEditorGUI.ObjectField(new GUIContent("Area"), terrainManager.area == null, terrainManager.area, typeof(Area), 1);

            	if (CustomEditorGUI.ClickButton("Create"))
            	{
            	    terrainManager.area = terrainManager.CreateAreaParent();
					terrainManager.CalculateArea();
					terrainManager.SetupTerrainDataManager();
            	}

            	GUILayout.Space(5); 
			}
            GUILayout.EndHorizontal();
        }

        void OnEnable()
        {
            terrainManager = (TerrainManager)target;
        }

        void SetSceneDirty()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(terrainManager.gameObject.scene);
                EditorUtility.SetDirty(terrainManager);
            }
        }

        public void DrawTerrainsSettings()
        {
			DrawUnityTerrain();
		}

		public void AddTerrainField()
		{
			EditorGUI.BeginChangeCheck();
            GameObject newTerrain = (GameObject)CustomEditorGUI.ObjectField(addTerrain, null, typeof(GameObject));
            if (EditorGUI.EndChangeCheck())
            {
                if (newTerrain != null)
                {
                    bool hasInterface = false;
                    MonoBehaviour[] list = newTerrain.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour mb in list)
                    {
                        if (mb is ITerrain)
                        {
                            terrainManager.AddTerrain(mb.gameObject);
                            hasInterface = true;
                        }
                    }

                    if (!hasInterface)
                        EditorUtility.DisplayDialog("Add terrain Data Helper",
                            "Could not find any component with ITerrainDataHelper Interface", "OK");
                    SetSceneDirty();
                    SceneView.RepaintAll();
                }
            }
		}

		public void DrawUnityTerrain()
		{
			GUILayout.BeginHorizontal();
         	{
				GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
				if(CustomEditorGUI.ClickButton("Find All \"Unity Terrain\"", ButtonStyle.Add))
				{
					terrainManager.AddAllUnityTerrains();
                	SetSceneDirty();
                	SceneView.RepaintAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
         	{
				GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
				if(CustomEditorGUI.ClickButton("Find All \"Mesh Terrain\" ", ButtonStyle.Add))
				{
					terrainManager.AddMeshTerrains();
                	SetSceneDirty();
                	SceneView.RepaintAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
         	{
				GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
				if(CustomEditorGUI.ClickButton("Find All \"Polaris Terrain\" ", ButtonStyle.Add))
				{
					terrainManager.AddAllPolarisTerrains();
                	SetSceneDirty();
                	SceneView.RepaintAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			AddTerrainField();

			if(terrainManager.TerrainObjectList.Count != 0)
			{
				Terrains();
			}
			else
			{
				CustomEditorGUI.WarningBox("Missing in the database any component with ITerrainDataHelper Interface. You need to add the UnityTerrainDataHelper component to the Unity Terrain and then drag the terrain GameObject here.");
			}
		}

		public void Terrains()
		{
			string foldoutCurrentTerrains = string.Format("Current Terrains: {0}", terrainManager.TerrainObjectList.Count);

			terrainManager.CurrentTerrainsFoldout = CustomEditorGUI.Foldout(terrainManager.CurrentTerrainsFoldout, foldoutCurrentTerrains);

			if(terrainManager.CurrentTerrainsFoldout)
			{
				EditorGUI.indentLevel++;

				for (int i = 0; i <= terrainManager.TerrainObjectList.Count - 1; i++)
            	{
            	    ITerrain terrain =
            	        Utility.GetITerrainDataHelper(terrainManager
            	            .TerrainObjectList[i]);
            	    string terrainType = "Unknown";
            	    if (terrain != null)
            	    {
            	        terrainType = terrain.TerrainType;
            	    }

            	    EditorGUI.BeginChangeCheck();
            	    GameObject terrainObject = (GameObject)CustomEditorGUI.ObjectField(new GUIContent(terrainType + ":"),
            	        terrainManager.TerrainObjectList[i], typeof(GameObject));
            	    if (EditorGUI.EndChangeCheck())
            	    {
            	        if (!terrainObject)
            	        {
            	            terrainManager.RemoveTerrain(terrainManager.TerrainObjectList[i]);
            	        }

            	        SetSceneDirty();
            	        SceneView.RepaintAll();
            	    }
            	}

				if(terrainManager.TerrainObjectList.Count != 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

						if (CustomEditorGUI.ClickButton("Remove All", ButtonStyle.Remove))
            			{
            			    terrainManager.RemoveAllTerrains();
            			    SetSceneDirty();
            			    SceneView.RepaintAll();
            			}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
				}

				EditorGUI.indentLevel--;
			}
		}

		public GUIContent addTerrain = new GUIContent("Add terrain", "You can add any terrain that implements the ITerrainDataHelper interface. You need to add the UnityTerrainDataHelper component to the Unity Terrain and then drag the terrain GameObject here.");
    }
}