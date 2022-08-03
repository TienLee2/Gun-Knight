#if UNITY_EDITOR
using UnityEngine;

namespace MegaWorld
{
    public static class BrushModifyToolVisualisation 
    {
        public static void Draw(PainterVariables painterVariables)
        {
            if(painterVariables == null)
            {
                return;
            }

            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
            {
                Type type = MegaWorldPath.DataPackage.SelectedVariables.SelectedType;

                switch (type.ResourceType)
                {
                    case ResourceType.GameObject:
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            if(type.SpawnSurface == SpawnMode.Spherical || type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawSimpleFilter(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables, 
                                    MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObject.SimpleFilterSettings);
                                VisualisationUtility.DrawCircleHandles(painterVariables.Size, painterVariables.RaycastInfo);
                                return;
                            }
                            else
                            {
                                VisualisationUtility.DrawMaskFilterVisualization(GetCurrentMaskFilter(), painterVariables);
                            }
                        }
                        else
                        {
                            if(type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawCircleHandles(painterVariables.Size, painterVariables.RaycastInfo);
                            }
                            else
                            {
                                VisualisationUtility.DrawAreaPreview(painterVariables);
                            }
                        }

                        break;
                    }
                    case ResourceType.QuadroItem:
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            if(type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawSimpleFilter(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables, 
                                    MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItem.SimpleFilterSettings);
                                VisualisationUtility.DrawCircleHandles(painterVariables.Size, painterVariables.RaycastInfo);
                                return;
                            }
                            else
                            {
                                VisualisationUtility.DrawMaskFilterVisualization(GetCurrentMaskFilter(), painterVariables);
                            }
                        }
                        else
                        {
                            if(type.FilterType != FilterType.MaskFilter)
                            {
                                VisualisationUtility.DrawCircleHandles(painterVariables.Size, painterVariables.RaycastInfo);
                            }
                            else
                            {
                                VisualisationUtility.DrawAreaPreview(painterVariables);
                            }
                        }

                        break;
                    }
                    default:
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            VisualisationUtility.DrawMaskFilterVisualization(GetCurrentMaskFilter(), painterVariables);
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
                if(VisualisationUtility.IsActiveSimpleFilter(MegaWorldPath.DataPackage.SelectedVariables))
                {
                    VisualisationUtility.DrawCircleHandles(painterVariables.Size, painterVariables.RaycastInfo);
                }
                else
                {
                    VisualisationUtility.DrawAreaPreview(painterVariables);
                }
            }
        }

        public static FilterStack GetCurrentMaskFilter()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoGameObject())
            {
                return MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObject.ModifyMaskFilterStack;
            }
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoQuadroItem())
            {
                return MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItem.ModifyMaskFilterStack;
            }

            return null;
        }
    }
}
#endif