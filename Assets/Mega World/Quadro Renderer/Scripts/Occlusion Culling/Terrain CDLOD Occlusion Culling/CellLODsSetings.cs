using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class CellLODsSetings 
    {
        public float Lod0Distance = 300;
        public int LodLevels = 4;
        public float[] LodRanges;

        public CellLODsSetings()
        {
            LodRanges = new float[LodLevels];

            for (int i = 0; i < LodLevels; i++)
            {
                
                LodRanges[i] = Lod0Distance * Mathf.Pow(2, i);
            }
        }

        public void UpdateLODRanges()
        {
            LodRanges = new float[LodLevels];

            for (int i = 0; i < LodLevels; i++)
            {
                LodRanges[i] = Lod0Distance * Mathf.Pow(2, i);
            }
        }
    }
}
