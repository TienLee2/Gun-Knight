using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    public class LOD 
    {
        public Mesh Mesh;
        public List<Material> Materials = new List<Material>();
        public MaterialPropertyBlock Mpb;
        public MaterialPropertyBlock ShadowMPB;
        public float Distance = 0;
    }
}