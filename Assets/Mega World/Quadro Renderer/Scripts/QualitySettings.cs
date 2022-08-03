using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class QualitySettings 
    {
        public float LodBias = 1;
        public bool IsShadowCasting = true;
        public bool ActiveRenderer = true;
        public Light DirectionalLight;

        [SerializeField]
        private float _maxRenderDistance = 1000;
        public float MaxRenderDistance
        {
            get
            {
                return _maxRenderDistance;
            }
            set
            {
                if(value < 0)
                {
                    _maxRenderDistance = 0;
                }
                else
                {
                    _maxRenderDistance = value;
                }
            }
        }

        public ShadowCastingMode GetShadowCastingMode()
        {
            return IsShadowCasting ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }
    }
}
