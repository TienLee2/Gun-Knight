using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(UnityTerrainTreeConverter))]
    public class UnityTerrainTreeConverterEditor : Editor
    {
        public UnityTerrainTreeConverter unityTerrainTreeConverter;

        void OnEnable()
        {
            unityTerrainTreeConverter = (UnityTerrainTreeConverter)target;
        }

        public override void OnInspectorGUI()
        {			
            if(unityTerrainTreeConverter.HasAllNecessaryData() == false)
            {
                DrawNecessaryData();
                return;
            }

            EditorGUI.BeginChangeCheck();

            DrawUnityTerrainTreeConverter();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(unityTerrainTreeConverter);
            }
        }

        public void DrawUnityTerrainTreeConverter()
		{
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
			{
                GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

				if(CustomEditorGUI.ClickButton("Remove All Unity Terrain Prototypes From Terrains", ButtonStyle.Remove))
				{
					if (EditorUtility.DisplayDialog("WARNING!",
						"Are you sure you want to remove all Unity Terrain Tree Resources from the scene?",
						"OK", "Cancel"))
					{
                        unityTerrainTreeConverter.RemoveAllPrototypesFromTerrains();
					}
				}

                GUILayout.Space(3);
			}
			GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
			{
                GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

				if(CustomEditorGUI.ClickButton("Unspawn Unity Terrain Tree", ButtonStyle.Remove))
				{
					if (EditorUtility.DisplayDialog("WARNING!",
						"Are you sure you want to remove all spawned Unity Terrain Tree from the scene?",
						"OK", "Cancel"))
					{
                        unityTerrainTreeConverter.UnspawnAllTerrainTree();
					}
				}

                GUILayout.Space(3);
			}
			GUILayout.EndHorizontal();

            GUILayout.Space(3);
            
			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Convert All Unity Terrain Tree To Quadro Renderer"))
				{
					unityTerrainTreeConverter.ConvertAllUnityTerrainTreeToQuadroRenderer();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 

            GUILayout.Space(3);

            CustomEditorGUI.HelpBox("Only added prefabs in Quadro Renderer are converted when using this button.");
            
			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Convert Unity Terrain Tree To Quadro Renderer"))
				{
					unityTerrainTreeConverter.ConvertUnityTerrainTreeToQuadroRenderer();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Convert Quadro Renderer To Unity Terrain Tree"))
				{
					unityTerrainTreeConverter.ConvertQuadroRendererToUnityTerrainTree();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
		}

        public void DrawNecessaryData()
        {
			GUILayout.Space(3); 
				
			unityTerrainTreeConverter.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), unityTerrainTreeConverter.QuadroRenderer == null, unityTerrainTreeConverter.QuadroRenderer, typeof(QuadroRenderer));
            unityTerrainTreeConverter.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), unityTerrainTreeConverter.StorageTerrainCells == null, 
                unityTerrainTreeConverter.StorageTerrainCells, typeof(StorageTerrainCells ));
        }
    }
}