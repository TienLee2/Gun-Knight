using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class LayerSettings 
    {
        public LayerMask PaintLayers = 1;

        #if UNITY_EDITOR
        public LayerSettingsEditor LayerSettingsEditor = new LayerSettingsEditor();

        public void OnGUI()
        {
            LayerSettingsEditor.OnGUI(this);
        }
        #endif

        public LayerSettings()
        {
        }

        public LayerMask GetCurrentPaintLayers(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
                }
                case ResourceType.TerrainTexture:
                {
                    return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
                }
                default:
                {
                    return PaintLayers;
                }
            }
        }
    }
}

