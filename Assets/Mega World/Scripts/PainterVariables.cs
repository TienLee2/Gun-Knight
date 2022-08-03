using UnityEngine;

namespace MegaWorld
{
    public class PainterVariables
    {
        public Terrain terrainUnderCursor { get; }
        public Texture2D Mask { get; }
        public Bounds Bounds;
        private float _size;
		private float _rotation = 0;
        private float _brushCosAngle;
        private float _brushSinAngle;
        private float _brushRotationSizeMultiplier;

        private RaycastInfo _raycastInfo;
        public RaycastInfo RaycastInfo 
        { 
            get
            {
                return _raycastInfo;
            } 
        }

        public float BrushRotationSizeMultiplier
        {
            get
            {
                return _brushRotationSizeMultiplier;
            }
        }

        public float Radius
        {
            get
            {
                return _size / 2;
            }
        }

        public float Size 
        { 
            get
            {
                return _size;
            }
        }

		public float Rotation 
        { 
            get
            {
                return _rotation;
            }
        }

        public PainterVariables(float size, RaycastInfo hit)
        {
            Mask = Texture2D.whiteTexture;
            _size = size;
            _raycastInfo = hit;
            
            if(_raycastInfo.isHit)
            {
                terrainUnderCursor = Utility.GetTerrain(_raycastInfo.hitInfo.point);
            }

            Bounds = new Bounds();
            Bounds.size = new Vector3(size, size, size);
            Bounds.center = hit.hitInfo.point;
        }

        public PainterVariables(Type type, BrushSettings brush, RaycastInfo hit)
        {
            Mask = brush.GetCurrentRaw();

            brush.BrushJitterSettings.SetRandomVariables(ref _raycastInfo, ref _size, ref _rotation, hit.hitInfo.point, brush, type, type.GetCurrentPaintLayers());

            if(_raycastInfo.isHit)
            {
                terrainUnderCursor = Utility.GetTerrain(_raycastInfo.hitInfo.point);
            }

            SetVariablesForRotation();

            Bounds = new Bounds();
            Bounds.size = new Vector3(Size, Size, Size);
            Bounds.center = hit.hitInfo.point;
        }

        public PainterVariables(BrushSettings brush, RaycastInfo hit)
        {
            Mask = brush.GetCurrentRaw();
            _size = brush.BrushSize;
            _rotation = brush.BrushRotation;
            terrainUnderCursor = Utility.GetTerrain(hit.hitInfo.point);
            _raycastInfo = hit;

            SetVariablesForRotation();

            Bounds = new Bounds();
            Bounds.size = new Vector3(Size, Size, Size);
            Bounds.center = hit.hitInfo.point;
        }

        public PainterVariables(Bounds bounds, Texture2D mask, RaycastInfo hit)
        {
            Mask = mask;
            _size = bounds.size.x;
            _rotation = 0;
            _raycastInfo = hit;

            terrainUnderCursor = Utility.GetTerrain(hit.hitInfo.point);

            this.Bounds = bounds;
        }

        public void SetVariablesForRotation()
        {
            _brushRotationSizeMultiplier = Mathf.Abs(_brushCosAngle = Mathf.Cos(Rotation * Mathf.Deg2Rad));
            _brushRotationSizeMultiplier += Mathf.Abs(_brushSinAngle = Mathf.Sin(Rotation * Mathf.Deg2Rad));
        }

        public float GetAlpha(Vector2 pos, Vector2 brushSize)
        {
            if (Mask == null) { return 1.0f; }
            pos += Point.one;
            if (Rotation == 0.0f) { return GetAlphaRaw(pos, brushSize, Mask); }
            Vector2 halfTarget = (Vector2)brushSize / 2.0f;
            Vector2 origin = pos - halfTarget;
            origin *= _brushRotationSizeMultiplier;
            origin = new Vector2(
                origin.x * _brushCosAngle - origin.y * _brushSinAngle + halfTarget.x,
                origin.x * _brushSinAngle + origin.y * _brushCosAngle + halfTarget.y);

            if (origin.x < 0.0f || origin.x > brushSize.x || origin.y < 0.0f || origin.y > brushSize.y) { return 0.0f; }

            return GetAlphaRaw(origin, brushSize, Mask);
        }

        public float GetAlphaRaw(Vector2 normalizedCheckPoint)
        {
            if (Mask == null) { return 1.0f; }

            return Mask.GetPixelBilinear(normalizedCheckPoint.x, normalizedCheckPoint.y).grayscale;
        }

        public float GetAlphaRaw(Vector2 pos, Vector2 target, Texture2D tex)
        {
            if (tex == null) { return 1.0f; }
	        float x = (pos.x - 1) / target.x;
	        float y = (pos.y - 1) / target.y;

            return tex.GetPixelBilinear(x, y).grayscale;
        }
    }
}

