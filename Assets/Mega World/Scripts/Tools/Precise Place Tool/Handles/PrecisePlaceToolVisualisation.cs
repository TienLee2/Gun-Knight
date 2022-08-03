#if UNITY_EDITOR
using UnityEngine;

namespace MegaWorld.PrecisePlace
{
    public static class PrecisePlaceToolVisualisation 
    {
        public static void Draw(RaycastInfo raycastInfo)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            PrototypeGameObject selectedProto = PrecisePlaceTool.SelectedProto;

            if(selectedProto == null)
            {
                return;
            }

            PainterVariables painterVariables = new PainterVariables(precisionPaintSettings.FilterVisualisationSize, raycastInfo);

            if(precisionPaintSettings.OverlapCheck && precisionPaintSettings.VisualizeOverlapCheckSettings)
            { 
                Bounds bounds = new Bounds();
                bounds.size = new Vector3(40, 40, 40);
                bounds.center = raycastInfo.point;

                OverlapCheckSettings.VisualizeOverlapForGameObject(bounds);
            }

            if(precisionPaintSettings.EnableFilter && precisionPaintSettings.VisualizeFilterSettings)
            {
                Type type = AllAvailableTypes.GetType(selectedProto);
                if(type.FilterType == FilterType.MaskFilter)
                {
                    VisualisationUtility.DrawMaskFilterVisualization(selectedProto.MaskFilterStack, painterVariables);
                }
                else
                {
                    VisualisationUtility.DrawSimpleFilter(type, painterVariables.RaycastInfo.hitInfo.point, painterVariables, 
                        MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObject.SimpleFilterSettings);
                }
            }
        }
    }
}
#endif