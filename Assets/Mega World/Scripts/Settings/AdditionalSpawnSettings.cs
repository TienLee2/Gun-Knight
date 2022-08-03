using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class AdditionalSpawnSettings 
    {
        [Range (0, 100)]
        public float Success = 100f;
        
        [Range (0, 100)]
        public float SuccessForErase = 100f;


        #if UNITY_EDITOR
        public AdditionalSpawnSettingsEditor AdditionalSpawnSettingsEditor = new AdditionalSpawnSettingsEditor();

        public void OnGUI(MegaWorldTools tool)
        {
            AdditionalSpawnSettingsEditor.OnGUI(this, tool);
        }
        #endif

        public AdditionalSpawnSettings()
        {

        }

        public AdditionalSpawnSettings(AdditionalSpawnSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(AdditionalSpawnSettings other)
        {            
            Success = other.Success;
        }
    }
}
