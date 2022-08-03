using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class ShadowsSettings 
    {
        public bool IsShadowCasting = true;
        public bool UseCustomShadowDistance = false;

        [SerializeField]
        private float _shadowDistance = 300;
        public float ShadowDistance
        {
            get
            {
                return _shadowDistance;
            }
            set
            {
                if(value < 0)
                {
                    _shadowDistance = 0;
                }
                else
                {
                    _shadowDistance = value;
                }
            }
        }

        public int[] ShadowLODMap = new int[] 
        {
            0, 1, 2, 3, 4, 5, 6, 7,
        };

        public bool[] DrawShadowLODMap = new bool[] 
        {
            true, true, 
            true, true, 
            true, true, 
            true, true,
        };

        public bool[] ShadowWithOriginalShaderLODMap = new bool[] 
        {
            true, true, 
            true, true, 
            true, true, 
            true, true,
        };

        public ShadowsSettings()
        {

        }

        public ShadowsSettings(ShadowsSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(ShadowsSettings other)
        {            
            IsShadowCasting = other.IsShadowCasting;
            UseCustomShadowDistance = other.UseCustomShadowDistance;
            ShadowDistance = other.ShadowDistance;
            DrawShadowLODMap = other.DrawShadowLODMap;
            ShadowWithOriginalShaderLODMap = other.ShadowWithOriginalShaderLODMap;
        }

        public float GetShadowDistanceForRendering()
        {
            if(UseCustomShadowDistance)
            {
                return _shadowDistance;
            }
            else
            {
                float localShadowDistance = _shadowDistance;

                if(QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                {
                    localShadowDistance = UnityEngine.QualitySettings.shadowDistance;
                }

                return localShadowDistance;
            }
        }
    }
}