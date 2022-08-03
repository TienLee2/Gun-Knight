using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class LODSettings 
    {
        [SerializeField]
        private bool _isLODCrossFade = false;
        public bool IsLODCrossFade
        {
            get
            {
                if(QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                {
                    return _isLODCrossFade;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public float LodBias = 1;
        public float LodFadeTransitionDistance = 10;
        public bool LodFadeForLastLOD = true;

        public LODSettings()
        {

        }

        public LODSettings(LODSettings other)
        {
            _isLODCrossFade = other._isLODCrossFade;
            LodBias = other.LodBias;
        }

        public void CopyFrom(LODSettings other, RenderModelInfo renderModelInfo)
        {            
            _isLODCrossFade = other._isLODCrossFade;
            LodBias = other.LodBias;
        }

        public void SetLODFade(RenderModelInfo renderModelInfo, bool value)
        {
            if(value != _isLODCrossFade)
			{
				renderModelInfo.SetLODFadeKeyword(value);
                _isLODCrossFade = value;
			}
        }
    }
}