using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(ColliderSystem))]
    public class ColliderSystemEditor : Editor
    {
        public ColliderSystem collider;

        void OnEnable()
        {
            collider = (ColliderSystem)target;
        }

        public override void OnInspectorGUI()
        {			
            CustomEditorGUI.isInspector = true;
            
            CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.gcwjdzp8papg");

            GUILayout.Space(3);

            if(collider.HasAllNecessaryData() == false)
            {
                DrawNecessaryData();
                return;
            }

            EditorGUI.BeginChangeCheck();

            Draw();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(collider);
            }
        }

        public void Draw()
		{
			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Bake colliders to scene"))
				{
                    collider.UnspawnAllTerrainTree();
					collider.ConvertQuadroRendererToUnityTerrainTree();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
		}

        public void DrawNecessaryData()
        {
			GUILayout.Space(3); 
				
			collider.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), collider.QuadroRenderer == null, collider.QuadroRenderer, typeof(QuadroRenderer));
            collider.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), collider.StorageTerrainCells == null, 
                collider.StorageTerrainCells, typeof(StorageTerrainCells ));
        }
    }
}