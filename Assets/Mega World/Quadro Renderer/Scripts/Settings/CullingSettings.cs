using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class CullingSettings 
    {
        [SerializeField]
        private float _maxDistance = 2000;

        public float MaxDistance
        {
            get
            {
                return _maxDistance;
            }
            set
            {
                if(value < 0)
                {
                    _maxDistance = 0;
                }
                else
                {
                    _maxDistance = value;
                }
            }
        }

        [SerializeField]
        private float _minCullingDistance = 50;
        public float MinCullingDistance 
        {
            get
            {
                return _minCullingDistance;
            }
            set
            {
                if(value < 0)
                {
                    _minCullingDistance = 0;
                }
                else
                {
                    _minCullingDistance = value;
                }
            }
        }

        public bool IsFrustumCulling = true; 
        public GetAdditionalShadow GetAdditionalShadow = GetAdditionalShadow.DirectionLightShadowVisible;
        public float IncreaseBoundingSphere = 7f;
        public float IncreaseShadowsBoundingSphere = 0f;

        public CullingSettings()
        {

        }

        public CullingSettings(CullingSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(CullingSettings other)
        {            
            MaxDistance = other.MaxDistance;
            IsFrustumCulling = other.IsFrustumCulling;
            IncreaseBoundingSphere = other.IncreaseBoundingSphere;
            MinCullingDistance = other.MinCullingDistance;
            IncreaseShadowsBoundingSphere = other.IncreaseShadowsBoundingSphere;
        }
    }
}