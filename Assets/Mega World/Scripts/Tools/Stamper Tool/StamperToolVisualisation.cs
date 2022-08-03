#if UNITY_EDITOR
using UnityEngine;

namespace MegaWorld.Stamper
{
    public class StamperVisualisation 
    {
        public bool UpdateMask = false;
        public FilterContext FilterContext;
        public FilterStack PastMaskFilterStack;

        public void Draw(PainterVariables painterVariables, BasicData data, float multiplyAlpha)
        {
            if(painterVariables == null)
            {
                return;
            }

            if(data.SelectedVariables.HasOneSelectedType())
            {
                Type type = data.SelectedVariables.SelectedType;

                switch (type.ResourceType)
                {
                    case ResourceType.GameObject:
                    {
                        if(data.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            if(type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawSimpleFilter(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables, data.SelectedVariables.SelectedProtoGameObject.SimpleFilterSettings);
                                return;
                            }
                            else
                            {
                                DrawVisualization(GetCurrentMaskFilter(data), painterVariables, multiplyAlpha);
                            }
                        }
                        else
                        {
                            if(type.FilterType == FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawAreaPreview(painterVariables);
                            }
                        }

                        break;
                    }
                    case ResourceType.QuadroItem:
                    {
                        if(data.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            if(type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawSimpleFilter(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables, data.SelectedVariables.SelectedProtoQuadroItem.SimpleFilterSettings);
                                return;
                            }
                            else
                            {
                                DrawVisualization(GetCurrentMaskFilter(data), painterVariables, multiplyAlpha);
                            }
                        }
                        else
                        {
                            if(type.FilterType == FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawAreaPreview(painterVariables);
                            }
                        }

                        break;
                    }
                    default:
                    {
                        if(data.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            DrawVisualization(GetCurrentMaskFilter(data), painterVariables, multiplyAlpha);
                        }
                        else
                        {
                            VisualisationUtility.DrawAreaPreview(painterVariables);
                        }

                        break;
                    }
                }
            }
            else
            {
                if(data.SelectedVariables.SelectedProtoGameObjectList.Count == 0)
                {
                    VisualisationUtility.DrawAreaPreview(painterVariables);
                }
            }
        }

        public void DrawVisualization(FilterStack maskFilterStack, PainterVariables painterVariables, float multiplyAlpha = 1)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            if (maskFilterStack.Filters.Count > 0)
            {
                UpdateMaskIfNecessary(maskFilterStack, painterVariables);

                VisualisationUtility.DrawMaskFilter(FilterContext, painterVariables, multiplyAlpha);
            }
            else
            {
                VisualisationUtility.DrawAreaPreview(painterVariables);
            }
        }

        public void UpdateMaskIfNecessary(FilterStack maskFilterStack, PainterVariables painterVariables)
        {
            if(FilterContext == null)
            {
                FilterContext = new FilterContext(painterVariables);
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, painterVariables);

                UpdateMask = false;

                return;
            }

            if(PastMaskFilterStack != maskFilterStack || UpdateMask)
            {
                FilterContext.DisposeUnmanagedMemory();
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, painterVariables);

                PastMaskFilterStack = maskFilterStack;

                UpdateMask = false;
            }
        }

        public FilterStack GetCurrentMaskFilter(BasicData data)
        {
            if(data.SelectedVariables.HasOneSelectedProtoGameObject())
            {
                return data.SelectedVariables.SelectedProtoGameObject.MaskFilterStack;
            }
            if(data.SelectedVariables.HasOneSelectedProtoTerrainDetail())
            {
    		    return data.SelectedVariables.SelectedProtoTerrainDetail.MaskFilterStack;
            }
            if(data.SelectedVariables.HasOneSelectedProtoTerrainTexture())
            {
    		    return data.SelectedVariables.SelectedProtoTerrainTexture.MaskFilterStack;
            }
            if(data.SelectedVariables.HasOneSelectedProtoQuadroItem())
            {
                return data.SelectedVariables.SelectedProtoQuadroItem.MaskFilterStack;
            }

            return null;
        }
    }
}
#endif