using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    [System.Serializable]
    public class PrototypeTerrainDetail : Prototype
    {
        public string TerrainDetailName = "Default";
        public int TerrainProtoId = 0;
        
        public FailureSettings FailureSettings = new FailureSettings();
        public TerrainDetailSettings TerrainDetailSettings = new TerrainDetailSettings();
        public SpawnDetailSettings SpawnDetailSettings = new SpawnDetailSettings();
        
        public PrefabType PrefabType = PrefabType.Mesh;
        public Texture2D DetailTexture;

        [SerializeField]
        public FilterStack EraseMaskFilterStack = new FilterStack();

        [SerializeField]
        public FilterStack MaskFilterStack = new FilterStack();

        public FilterContext FilterContext;
        public Texture2D FilterMaskTexture2D;
        

#if UNITY_EDITOR
		private FilterStackView _eraseMaskFilterStackView = null;
		public FilterStackView EraseMaskFilterStackView
		{
			get
			{
				if( _eraseMaskFilterStackView == null || _eraseMaskFilterStackView.m_filterStack == null )
				{
					_eraseMaskFilterStackView = new FilterStackView(new GUIContent("Erase Mask Filters Settings"), EraseMaskFilterStack );
				}

				return _eraseMaskFilterStackView;
			}
		}

		
		private FilterStackView _maskFilterStackView = null;
		public FilterStackView MaskFilterStackView
		{
			get
			{
				if( _maskFilterStackView == null || _maskFilterStackView.m_filterStack == null )
				{
					_maskFilterStackView = new FilterStackView(new GUIContent("Mask Filters Settings"), MaskFilterStack );
				}

				return _maskFilterStackView;
			}
		}
#endif

        public PrototypeTerrainDetail()
        {
            FailureSettings.FailureRate = 0;
        }

        public PrototypeTerrainDetail(GameObject detailProtoype)
        {
            PrefabType = PrefabType.Mesh;
            
            TerrainDetailName = detailProtoype.name;
            prefab = detailProtoype;
            TerrainDetailSettings.RenderMode = DetailRenderMode.Grass;

            FailureSettings.FailureRate = 0;
        }

        public PrototypeTerrainDetail(Texture2D detailTexture, string name)
        {
            PrefabType = PrefabType.Texture;

            this.DetailTexture = detailTexture;
            TerrainDetailName = name;
            TerrainDetailSettings.RenderMode = DetailRenderMode.Grass;

            FailureSettings.FailureRate = 0;
        }
    }
}

