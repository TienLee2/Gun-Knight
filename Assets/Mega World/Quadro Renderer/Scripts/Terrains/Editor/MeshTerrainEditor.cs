using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(MeshTerrain))]
    public class MeshTerrainEditor : Editor
    {
        private MeshTerrain terrain;
        
        void OnEnable()
        {
            terrain = (MeshTerrain)target;
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawTerrainSettings(terrain);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(terrain, "Made changes");
                EditorUtility.SetDirty(terrain);
            }
        }

        public void DrawTerrainSettings(MeshTerrain terrain)
        {
            CustomEditorGUI.HelpBox("This component is used for Quadro Renderer.");

            EditorGUI.BeginChangeCheck();

            Bounds oldBounds = terrain.RaycastTerrainBounds;
            terrain.RaycastTerrainBounds = EditorGUILayout.BoundsField("Area", terrain.RaycastTerrainBounds);
            if (EditorGUI.EndChangeCheck())
            {
                oldBounds.Encapsulate(terrain.RaycastTerrainBounds);

                EditorUtility.SetDirty(terrain);
            }
        }
    }
}