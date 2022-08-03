using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    [PreferBinarySerialization]
    [Serializable]
    public class TerrainCDLODOcclusionCullingPackage : ScriptableObject
    {
        public List<RenderCell> RenderCells = new List<RenderCell>();
    }
}