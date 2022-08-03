using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    [System.Serializable]
    public class TerrainTexture
    {
        public Texture2D texture;
        public int terrainProtoId = 0;
        public bool selected = false;

        public TerrainTexture()
        {
        }

        public void CopyFrom(TerrainTexture other)
        {
            texture = other.texture;
            terrainProtoId = other.terrainProtoId;
            selected = other.selected;
        }
    }
}
