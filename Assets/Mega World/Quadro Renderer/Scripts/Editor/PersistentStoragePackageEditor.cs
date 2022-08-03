using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(PersistentStoragePackage))]
    public class PersistentStoragePackageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomEditorGUI.WarningBox("Information from this package is not shown, because there is a lot of information, which can create a large lag in the Unity Editor.");
        }
    }
}