using UnityEditor;
using UnityEngine;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(SnapToObject))]
    public class SnapToObjectEditor : Editor
    {
        public SnapToObject snapToObject;
        public bool selectAdvancedSettingsFoldout = true;
        public bool selectPrototypesWindowFoldout = true;

        void OnEnable()
        {
            snapToObject = (SnapToObject)target;
        }

        public override void OnInspectorGUI()
        {			
            CustomEditorGUI.isInspector = true;

            //CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.gcwjdzp8papg");

            GUILayout.Space(3);

            if(snapToObject.HasAllNecessaryData() == false)
            {
                DrawNecessaryData();
                return;
            }

            EditorGUI.BeginChangeCheck();

            Draw();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(snapToObject);
            }
        }

        public void Draw()
		{
            selectPrototypesWindowFoldout = CustomEditorGUI.Foldout(selectPrototypesWindowFoldout, "Prototypes");

			if(selectPrototypesWindowFoldout)
			{
				EditorGUI.indentLevel++;

				DrawSelectedPrototypeWindow();

				EditorGUI.indentLevel--;
			}

            snapToObject.Layers = CustomEditorGUI.LayerField(new GUIContent("Layers"), snapToObject.Layers);
            snapToObject.RaycastPositionOffset = CustomEditorGUI.FloatField(new GUIContent("Raycast Position Offset"), snapToObject.RaycastPositionOffset);

            selectAdvancedSettingsFoldout = CustomEditorGUI.Foldout(selectAdvancedSettingsFoldout, "Advanced Settings");

			if(selectAdvancedSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				snapToObject.MaxRayDistance = CustomEditorGUI.FloatField(new GUIContent("Max Ray Distance", "The max distance the ray should check for collisions."), snapToObject.MaxRayDistance);
		        snapToObject.SpawnCheckOffset = CustomEditorGUI.FloatField(new GUIContent("Spawn Check Offset", "Raises the ray from the spawn point."), snapToObject.SpawnCheckOffset);

				EditorGUI.indentLevel--;
			}

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Snap To Object"))
				{
                    snapToObject.Snap();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
		}

        public void DrawSelectedPrototypeWindow()
        {
            InternalDragAndDrop.OnBeginGUI();

            QuadroRenderEditorSelectedEditor.DrawSelectedWindowForPrototypes(snapToObject.QuadroRenderer);
    
    		InternalDragAndDrop.OnEndGUI();

            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
    		{
    			Repaint();
    		}
        }

        public void DrawNecessaryData()
        {
			GUILayout.Space(3); 
				
			snapToObject.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), snapToObject.QuadroRenderer == null, snapToObject.QuadroRenderer, typeof(QuadroRenderer));
            snapToObject.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), snapToObject.StorageTerrainCells == null, 
                snapToObject.StorageTerrainCells, typeof(StorageTerrainCells ));
        }
    }
}