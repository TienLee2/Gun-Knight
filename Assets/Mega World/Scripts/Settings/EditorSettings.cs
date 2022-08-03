using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class EditorSettings 
    {
        public bool useLargeRanges = false;
        public float maxBrushSize = 200;
        public int maxChecks = 100;

        [Min (1f)]
        public int maxTargetStrength = 10;

        public RaycastSettings raycastSettings = new RaycastSettings();

        #if UNITY_EDITOR
        public EditorSettingsEditor editorSettingsEditor = new EditorSettingsEditor();

        public void OnGUI()
        {
            editorSettingsEditor.OnGUI(this);
        }
        #endif
    }
}
