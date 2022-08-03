using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class GeneralSettings 
    {
        public RenderMode RenderMode = RenderMode.GPUInstancedIndirect;
        public LightProbeUsage LightProbeUsage = LightProbeUsage.BlendProbes;
        public bool ActiveRenderer = true;

        public GeneralSettings()
        {

        }

        public GeneralSettings(GeneralSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(GeneralSettings other)
        {            
            RenderMode = other.RenderMode;
            LightProbeUsage = other.LightProbeUsage;
            ActiveRenderer = other.ActiveRenderer;
        }
    }
}