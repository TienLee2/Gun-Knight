using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(GameObjectConverter))]
    public class GameObjectConverterEditor : Editor
    {
        public GameObjectConverter gameObjectConverter;

        void OnEnable()
        {
            gameObjectConverter = (GameObjectConverter)target;
        }

        public override void OnInspectorGUI()
        {			
            CustomEditorGUI.isInspector = true;

            if(gameObjectConverter.HasAllNecessaryData() == false)
            {
                DrawNecessaryData();
                return;
            }

            EditorGUI.BeginChangeCheck();

            DrawGameObjectConverter();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(gameObjectConverter);
            }
        }

        public void DrawGameObjectConverter()
		{
            GUILayout.Space(3);

            CustomEditorGUI.HelpBox("Only added prefabs in Quadro Renderer are converted when using this button.");
            
			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Convert GameObject To Quadro Renderer"))
				{
					gameObjectConverter.ConvertGameObjectToQuadroRenderer();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Convert Quadro Renderer To GameOject"))
				{
					gameObjectConverter.ConvertQuadroRendererToGameObject();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
		}

        public void DrawNecessaryData()
        {
            GUILayout.Space(3); 

			gameObjectConverter.quadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), gameObjectConverter.quadroRenderer == null, gameObjectConverter.quadroRenderer, typeof(QuadroRenderer));
            gameObjectConverter.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), gameObjectConverter.StorageTerrainCells == null, 
                gameObjectConverter.StorageTerrainCells, typeof(StorageTerrainCells));
        }
    }
}