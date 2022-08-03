using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(UnityTerrain))]
    public class UnityTerrainDataEditor : Editor
    {
        private UnityTerrain terrain;
        
        void OnEnable()
        {
            terrain = (UnityTerrain)target;
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

        public void DrawTerrainSettings(UnityTerrain unityTerrain)
        {
            if(unityTerrain.terrain == null)
            {
                CustomEditorGUI.WarningBox("No terrain was found. Add this component to your terrain.");
            }
            else
            {
                CustomEditorGUI.HelpBox("This component is used for Quadro Renderer.");
            }
        }
    }
}