#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace MegaWorld.PrecisePlace
{
    public static class PlaceObjectUtility 
    {
        public static PlacedObjectInfo DragPlace(Type type, RaycastInfo raycastInfo, DragBrush drag)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            PlacedObjectInfo placedObjectInfo = TryToPlace(type, raycastInfo);

            if(placedObjectInfo == null)
            {
                return null;
            }

            if(precisionPaintSettings.AlongStroke)
            {
                RotationAlongStroke.RotateAlongStroke(placedObjectInfo.gameObject, drag);
            }

            return placedObjectInfo;
        }

        public static PlacedObjectInfo TryToPlace(Type type, RaycastInfo raycastInfo)
        {
            PrecisePlaceToolSettings precisionPaintSettings  = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            SelectedProtoUtility.SetSelectedProto(type); 

            PrototypeGameObject SelectedProto = PrecisePlaceTool.SelectedProto;

            if(SelectedProto == null)
            {
                return null;
            }

            InstanceData instanceData = new InstanceData();

            if(!CanPlace(type, SelectedProto, raycastInfo, ref instanceData))
            {
                return null;
            }

            return Place(type, SelectedProto, raycastInfo, instanceData);
        }

        public static PlacedObjectInfo Place(Type type, RaycastInfo raycastInfo)
        {
            PrecisePlaceToolSettings precisionPaintSettings  = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            SelectedProtoUtility.SetSelectedProto(type); 

            PrototypeGameObject SelectedProto = PrecisePlaceTool.SelectedProto;

            return Place(type, SelectedProto, raycastInfo, GetInstancedData(SelectedProto, raycastInfo, 1));
        }

        public static PlacedObjectInfo Place(Type type, PrototypeGameObject SelectedProto, RaycastInfo raycastInfo, InstanceData instanceData)
        {
            PrecisePlaceToolSettings precisionPaintSettings  = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            PlacedObjectInfo placedObjectInfo = GameObjectUtility.PlaceObject(SelectedProto, instanceData.position, instanceData.scale, instanceData.rotation);
            placedObjectInfo.RaycastInfo = raycastInfo;
            GameObjectUtility.ParentGameObject(type, SelectedProto, placedObjectInfo);

            if(precisionPaintSettings.AlignAxis)
            {
                AxisAlignment.AlignObjectAxis(placedObjectInfo.gameObject, precisionPaintSettings.AlignmentAxis, raycastInfo.normal, precisionPaintSettings.WeightToNormal);
            }

            PositionOffset.ChangePositionOffset(placedObjectInfo.gameObject, SelectedProto.PositionOffset);

            return placedObjectInfo;
        }

        public static InstanceData GetInstancedData(PrototypeGameObject SelectedProto, RaycastInfo raycastInfo, float fitness)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            Vector3 scaleFactor = Vector3.one;
            Quaternion rotation = Quaternion.identity;

            if(SelectedProto.pastTransform != null)
            {
                scaleFactor = SelectedProto.pastTransform.Scale;
                rotation = SelectedProto.pastTransform.Rotation;
            }

            InstanceData instanceData = new InstanceData(raycastInfo.hitInfo.point, scaleFactor, rotation, fitness);

            if(precisionPaintSettings.RandomiseTransform)
            {
                SelectedProto.TransformComponentsStack.SetInstanceData(ref instanceData, raycastInfo.normal);
            }

            return instanceData;
        }

        public static bool CanPlace(Type type, PrototypeGameObject SelectedProto, RaycastInfo raycastInfo, ref InstanceData instanceData)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            float fitness = 1;

            if(precisionPaintSettings.EnableFilter)
            {
                PainterVariables painterVariables = new PainterVariables(precisionPaintSettings.FilterVisualisationSize, raycastInfo);

                if(type.FilterType == FilterType.MaskFilter)
                {
                    FilterMaskOperation.UpdateMaskTexture(SelectedProto, SelectedProto.MaskFilterStack, painterVariables);
                }

                fitness = GetFitnessUtility.GetFitnessForGameObject(type, painterVariables.Bounds, SelectedProto, raycastInfo);

                if(fitness == 0)
                {
                    return false;
                }
            }

            instanceData = GetInstancedData(SelectedProto, raycastInfo, fitness);

            if(precisionPaintSettings.OverlapCheck)
            {
                if(!SelectedProto.OverlapCheckSettings.RunOverlapCheckForGameObject(SelectedProto, instanceData))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif