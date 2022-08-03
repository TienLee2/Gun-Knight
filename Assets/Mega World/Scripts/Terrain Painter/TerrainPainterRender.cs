using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
#if UNITY_EDITOR
using UnityEditor.Experimental.TerrainAPI;
#endif

namespace MegaWorld
{
	public class TerrainPainterRenderHelper
	{
		private readonly PainterVariables _painterVariables;
        private readonly BrushTransform _brushTransform;
        private readonly Rect _brushRect;

        public PainterVariables PainterVariables
        {
            get 
            {
                return _painterVariables;
            }
        }

        public BrushTransform BrushTransform
        {
            get
            {
                return _brushTransform;
            }
        }

		public TerrainPainterRenderHelper(PainterVariables painterVariables)
		{
			this._painterVariables = painterVariables;

            Vector2 currUV = Utility.WorldPointToUV(painterVariables.RaycastInfo.hitInfo.point, painterVariables.terrainUnderCursor);

            _brushTransform = TerrainPaintUtility.CalculateBrushTransform(painterVariables.terrainUnderCursor, currUV, painterVariables.Size, painterVariables.Rotation);
            _brushRect = _brushTransform.GetBrushXYBounds();
		}

#region Rendering
		public void RenderBrush(PaintContext paintContext, Material material, int pass)
		{
			Texture sourceTexture = paintContext.sourceRenderTexture;
			RenderTexture destinationTexture = paintContext.destinationRenderTexture;

            Graphics.Blit(sourceTexture, destinationTexture, material, pass);
		}

#if UNITY_EDITOR
		public void RenderAreaPreview(PaintContext paintContext, TerrainPaintUtilityEditor.BrushPreview previewTexture, Material material, int pass)
		{
			TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, _painterVariables.Mask, _brushTransform, material, pass);
		}
#endif
		
#endregion

#region Material Set-up
		public void SetupTerrainToolMaterialProperties(PaintContext paintContext, Material material)
		{
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, _brushTransform, material);
		}
#endregion

#region Texture Acquisition
		public PaintContext AcquireHeightmap(int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.BeginPaintHeightmap(_painterVariables.terrainUnderCursor, _brushRect, extraBorderPixels);
		}

		public PaintContext AcquireTexture(TerrainLayer layer, int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.BeginPaintTexture(_painterVariables.terrainUnderCursor, _brushRect, layer, extraBorderPixels);
		}

		public PaintContext AcquireNormalmap(int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.CollectNormals(_painterVariables.terrainUnderCursor, _brushRect, extraBorderPixels);
		}

		public PaintContext AcquireHolesTexture(int extraBorderPixels = 0)
		{
#if UNITY_2019_3_OR_NEWER
			return TerrainPaintUtility.BeginPaintHoles(_painterVariables.terrainUnderCursor, _brushRect, extraBorderPixels);
#else
			return null;
#endif
		}

#endregion
	}
}