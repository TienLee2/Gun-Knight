using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace MegaWorld
{
    public static class FilterMaskOperation 
    {
        public static void UpdateFilterMaskTextureForAllQuadroItem(Type type, PainterVariables painterVariables, MegaWorldTools tool, bool onlySelected)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                if(onlySelected)
                {
                    if(!proto.selected)
                    {
                        continue;
                    }
                }

                if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                {
                    switch (tool)
                    {
                        case MegaWorldTools.BrushErase:
                        {
                            if(proto.EraseMaskFilterStack.Filters.Count != 0)
                            {
                                UpdateMaskTexture(proto, proto.EraseMaskFilterStack, painterVariables);
                            }
                            
                            break;
                        }
                        case MegaWorldTools.BrushModify:
                        {
                            if(proto.ModifyMaskFilterStack.Filters.Count != 0)
                            {
                                UpdateMaskTexture(proto, proto.ModifyMaskFilterStack, painterVariables);
                            }
                            
                            break;
                        }
                    }
                }
                else
                {
                    if(proto.MaskFilterStack.Filters.Count != 0)
                    {
                        UpdateMaskTexture(proto, proto.MaskFilterStack, painterVariables);
                    }
                }
            }

            foreach (PrototypeQuadroItem proto in MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList)
            {
                if(proto.FilterContext != null)
                {
                    proto.FilterContext.DisposeUnmanagedMemory();
                    proto.FilterContext = null;
                }
            }
        }

        public static void UpdateFilterMaskTextureForAllGameObject(Type type, PainterVariables painterVariables, MegaWorldTools tool, bool onlySelected)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
            {
                if(onlySelected)
                {
                    if(!proto.selected)
                    {
                        continue;
                    }
                }

                if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                {
                    switch (tool)
                    {
                        case MegaWorldTools.BrushErase:
                        {
                            if(proto.EraseMaskFilterStack.Filters.Count != 0)
                            {
                                UpdateMaskTexture(proto, proto.EraseMaskFilterStack, painterVariables);
                            }
                            
                            break;
                        }
                        case MegaWorldTools.BrushModify:
                        {
                            if(proto.ModifyMaskFilterStack.Filters.Count != 0)
                            {
                                UpdateMaskTexture(proto, proto.ModifyMaskFilterStack, painterVariables);
                            }
                            
                            break;
                        }
                    }
                }
                else
                {
                    if(proto.MaskFilterStack.Filters.Count != 0)
                    {
                        UpdateMaskTexture(proto, proto.MaskFilterStack, painterVariables);
                    }
                }
            }

            foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
            {
                if(proto.FilterContext != null)
                {
                    proto.FilterContext.DisposeUnmanagedMemory();
                    proto.FilterContext = null;
                }
            }
        }

        public static void UpdateMaskTexture(PrototypeGameObject proto, FilterStack maskFilterStack, PainterVariables painterVariables)
        {
            UpdateFilterContext(ref proto.FilterContext, maskFilterStack, painterVariables);
            RenderTexture filterMaskRT = proto.FilterContext.GetFilterMaskRT();
            proto.FilterMaskTexture2D = Utility.ToTexture2D(filterMaskRT);
        }

        public static void UpdateMaskTexture(PrototypeQuadroItem proto, FilterStack maskFilterStack, PainterVariables painterVariables)
        {
            UpdateFilterContext(ref proto.FilterContext, maskFilterStack, painterVariables);
            RenderTexture filterMaskRT = proto.FilterContext.GetFilterMaskRT();
            proto.FilterMaskTexture2D = Utility.ToTexture2D(filterMaskRT);
        }

        public static void UpdateFilterMaskTextureForAllTerrainDetail(List<PrototypeTerrainDetail> protoTerrainDetailList, PainterVariables painterVariables, MegaWorldTools tool)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }
            
            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                if(tool == MegaWorldTools.BrushErase)
                {
                    if(proto.EraseMaskFilterStack.Filters.Count != 0)
                    {
                        UpdateFilterContext(ref proto.FilterContext, proto.EraseMaskFilterStack, painterVariables);
                    }
                }
                else
                {
                    if(proto.MaskFilterStack.Filters.Count != 0)
                    {
                        UpdateFilterContext(ref proto.FilterContext, proto.MaskFilterStack, painterVariables);
                    }
                }

                if(proto.FilterContext != null)
                {
                    RenderTexture filterMaskRT = proto.FilterContext.GetFilterMaskRT();

                    proto.FilterMaskTexture2D = Utility.ToTexture2D(filterMaskRT);
                }
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                if(proto.FilterContext != null)
                {
                    proto.FilterContext.DisposeUnmanagedMemory();
                    proto.FilterContext = null;
                }
            }
        }

        public static bool UpdateFilterContext(ref FilterContext filterContext, FilterStack maskFilterStack, PainterVariables painterVariables)
        {
            if(filterContext != null)
            {
                filterContext.DisposeUnmanagedMemory();
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();
            PaintContext normalContext = terrainPainterRenderHelper.AcquireNormalmap();

            RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, RenderTextureFormat.ARGB32);
            //RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;
            output.Create();

    		filterContext = new FilterContext(maskFilterStack, heightContext, normalContext, output, terrainPainterRenderHelper.PainterVariables);

            return true;
        }
    }
}