using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace MegaWorld
{
    public class FilterContext 
    {
        public PainterVariables PainterVariables;
        public Vector3 BrushPos;
        public RenderTexture SourceRenderTexture;
        public RenderTexture DestinationRenderTexture;
        public Dictionary<string, float> Properties;
        public PaintContext HeightContext; 
        public PaintContext NormalContext;
        public RenderTexture Output;

        public FilterContext(PainterVariables painterVariables)
        {
            PainterVariables = painterVariables;
        }

        public FilterContext(FilterStack maskFilterStack, PaintContext heightContext, PaintContext normalContext, RenderTexture output, PainterVariables painterVariables)
        {
            PainterVariables = painterVariables;
            BrushPos = new Vector3(painterVariables.RaycastInfo.hitInfo.point.x, 0, painterVariables.RaycastInfo.hitInfo.point.z);
            Properties = new Dictionary<string, float>();
            SourceRenderTexture = null;
            DestinationRenderTexture = null;
            HeightContext = heightContext;
            NormalContext = normalContext;
            Output = output;
            Properties.Add("brushRotation", painterVariables.Rotation);
            Properties.Add("terrainScale", Mathf.Sqrt(painterVariables.terrainUnderCursor.terrainData.size.x * painterVariables.terrainUnderCursor.terrainData.size.x + painterVariables.terrainUnderCursor.terrainData.size.z * painterVariables.terrainUnderCursor.terrainData.size.z));
            DestinationRenderTexture = output;
            
            maskFilterStack.Eval(this);
        }

        public RenderTexture GetFilterMaskRT()
        {
            return Output;
        }

        public void DisposeUnmanagedMemory()
        {
            if(HeightContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(HeightContext);
                HeightContext = null;
            }
            if(NormalContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(NormalContext);
                NormalContext = null;
            }
            if(Output != null)
            {
                Output.Release();
                Output = null;
            }
        }
    }
}