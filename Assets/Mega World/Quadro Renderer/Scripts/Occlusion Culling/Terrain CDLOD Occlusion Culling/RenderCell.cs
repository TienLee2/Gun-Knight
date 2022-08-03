using UnityEngine;
using System;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [Serializable]
    public class RenderCell : IHasRect
    {
        public Bounds Bounds;
        public int Index;

        [SerializeReference]
        public CDLODQuadTree QuadTree = null;

        public RenderCell(Bounds bounds)
        {
            this.Bounds = bounds;
        }

        public Rect Rectangle
        {
            get
            {
                return RectExtension.CreateRectFromBounds(Bounds);
            }
            set
            {
                Bounds = RectExtension.CreateBoundsFromRect(value);
            }
        }
    }
}