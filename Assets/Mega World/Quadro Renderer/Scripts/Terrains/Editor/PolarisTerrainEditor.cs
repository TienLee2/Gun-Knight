#if GRIFFIN_2020
using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(PolarisTerrain))]
    public class PolarisTerrainEditor : Editor
    {
        private PolarisTerrain terrain;
        
        void OnEnable()
        {
            terrain = (PolarisTerrain)target;
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

        public void DrawTerrainSettings(PolarisTerrain terrain)
        {
            if(terrain.Terrain == null)
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
#endif