using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public enum TagsSelectedType
    { 
        Nothing,
        Everything,
        Custom,
    }
    
    [Serializable]
    public class FlagsSettings
    {
        public TagsSelectedType TagsSelectedType = TagsSelectedType.Everything;
        public bool LightmapStatic = true;
        public bool BatchingStatic = true;
        public bool OccludeeStatic = true;
        public bool OccluderStatic = true;
        public bool NavigationStatic = true;
        public bool OffMeshLinkGeneration = true;
        public bool ReflectionProbeStatic = true;

        #if UNITY_EDITOR
        public FlagsSettingsEditor FlagsSettingsEditor = new FlagsSettingsEditor();

        public void OnGUI()
        {
            FlagsSettingsEditor.OnGUI(this);
        }
        #endif

        public FlagsSettings()
        {

        }

        public FlagsSettings(FlagsSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(FlagsSettings other)
        {            
            LightmapStatic = other.LightmapStatic;
            BatchingStatic = other.BatchingStatic;
            OccludeeStatic = other.OccludeeStatic;
            OccluderStatic = other.OccluderStatic;
            NavigationStatic = other.NavigationStatic;
            OffMeshLinkGeneration = other.OffMeshLinkGeneration;
            ReflectionProbeStatic = other.ReflectionProbeStatic;
        }
    }
}
