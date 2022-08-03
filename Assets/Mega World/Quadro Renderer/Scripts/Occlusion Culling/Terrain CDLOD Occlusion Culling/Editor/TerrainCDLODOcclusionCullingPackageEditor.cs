using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(TerrainCDLODOcclusionCullingPackage))]
    public class TerrainCDLODOcclusionCullingPackageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomEditorGUI.WarningBox("Information from this package is not shown, because there is a lot of information, which can create a large lag in the Unity Editor.");
        } 
    }
}