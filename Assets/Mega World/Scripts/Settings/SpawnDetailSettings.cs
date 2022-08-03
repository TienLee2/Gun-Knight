using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    [System.Serializable]
    public class SpawnDetailSettings
    {
        [Range (0f, 100f)]
        public float Opacity = 100f;
        public bool UseRandomOpacity = true;
        [Range (0, 100)]
        public float SuccessOfErase = 100f;
        [Min (0f)]
        public int TargetStrength = 5;

        #if UNITY_EDITOR
        public SpawnDetailSettingsEditor SpawnDetailSettingsEditor = new SpawnDetailSettingsEditor();

        public void OnGUI(MegaWorldTools tools)
        {
            SpawnDetailSettingsEditor.OnGUI(this, tools);
        }
        #endif

        public SpawnDetailSettings()
        {

        }

        public SpawnDetailSettings(SpawnDetailSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(SpawnDetailSettings other)
        {            
            Opacity = other.Opacity;
            UseRandomOpacity = other.UseRandomOpacity;
            SuccessOfErase = other.SuccessOfErase;
            TargetStrength = other.TargetStrength;
        }
    }
}

