using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    [System.Serializable]
    public class PrototypeTerrainTexture : Prototype
    {
        public string TerrainTextureName = "Default";

        public TerrainTextureSettings TerrainTextureSettings = new TerrainTextureSettings();
        public TerrainLayer TerrainLayer;

        [SerializeField]
        public FilterStack MaskFilterStack = new FilterStack();

        public FilterContext FilterContext;

#if UNITY_EDITOR
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

        public PrototypeTerrainTexture()
        {
        }

        public PrototypeTerrainTexture(Texture2D texture, string name)
        {
            TerrainTextureName = name;
            TerrainTextureSettings = new TerrainTextureSettings(texture);
        }

        public PrototypeTerrainTexture(TerrainLayer terrainLayer, string name)
        {
            this.TerrainLayer = terrainLayer;
            TerrainTextureName = name;
            TerrainTextureSettings = new TerrainTextureSettings(terrainLayer);
        }
    }
}

