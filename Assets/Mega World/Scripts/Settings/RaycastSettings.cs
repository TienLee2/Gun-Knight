using System;

namespace MegaWorld
{
    [Serializable]
    public class RaycastSettings
    {
        public float MaxRayDistance = 6500f;
        public float SpawnCheckOffset = 500;

        #if UNITY_EDITOR
        public RaycastSettingsEditor raycastSettingsEditor = new RaycastSettingsEditor();

        public void OnGUI()
        {
            raycastSettingsEditor.OnGUI(this);
        }
        #endif
    }
}
