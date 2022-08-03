#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MegaWorld.PrecisePlace
{
    public static class PrecisePaintHandles 
    {
        public static void DrawPrecisePaintHandles(PlacedObjectInfo placedObjectInfo, DragBrush drag)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            if(placedObjectInfo != null)
            {
                DrawPrecisePaintHandles(placedObjectInfo, drag.raycast);

                if (!precisionPaintSettings.ObjectMousePrecision.IsAnyMouseSessionActive)
                {
                    PrecisePlaceToolVisualisation.Draw(drag.raycast);
                }
            }
            else
            {
                DrawInitialHandle(drag.raycast.point, Color.green);
            }
        }

        public static void DrawUpdateMouseTransformSettings(PlacedObjectInfo placedObjectInfo)
        {
            GameObject gameObject = placedObjectInfo.gameObject;

            Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
            Vector3 worldSpaceNode = gameObject.transform.position;
            float distance = Vector3.Distance(cameraPosition, worldSpaceNode);

            float size = 0.13f * distance;
            Transform parentObjectTransform = gameObject.transform;

            if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundX.IsActive(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates))
            {
                Vector3 axis = TransformAxes.GetVector(TransformAxis.X, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);

                Handles.color = Handles.xAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axis),
                    size,
                    EventType.Repaint
                );
            }
            else if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundY.IsActive(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates))
            {
                Vector3 axis = TransformAxes.GetVector(TransformAxis.Y, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);

                Handles.color = Handles.yAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axis),
                    size,
                    EventType.Repaint
                );
            }
            else if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundZ.IsActive(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates))
            {
                Vector3 axis = TransformAxes.GetVector(TransformAxis.Z, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);

                Handles.color = Handles.zAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axis),
                    size,
                    EventType.Repaint
                );
            }
            else if(PrecisePlaceToolAllShortcutCombos.Instance.MouseFreeRotate.IsActive(MegaWorldPath.GeneralDataPackage.KeyboardButtonStates)) 
            {
                Vector3 axisX = TransformAxes.GetVector(TransformAxis.X, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);
                Vector3 axisY = TransformAxes.GetVector(TransformAxis.Y, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);
                Vector3 axisZ = TransformAxes.GetVector(TransformAxis.Z, MegaWorldPath.GeneralDataPackage.TransformSpace, parentObjectTransform);

                Handles.color = Handles.xAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axisX),
                    size,
                    EventType.Repaint
                );

                Handles.color = Handles.yAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axisY),
                    size,
                    EventType.Repaint
                );

                Handles.color = Handles.zAxisColor;
                Handles.CircleHandleCap(
                    0,
                    gameObject.transform.position,
                    Quaternion.LookRotation(axisZ),
                    size,
                    EventType.Repaint
                );
            }   
        }

        private static void DrawPrecisePaintHandles(PlacedObjectInfo placedObjectInfo, RaycastInfo raycastInfo)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            if (precisionPaintSettings.ObjectMousePrecision.IsAnyMouseSessionActive)
            {
                DrawUpdateMouseTransformSettings(placedObjectInfo);
            }
            else
            {            
                DrawInitialHandle(placedObjectInfo.gameObject.transform.position, GetCurrentColorFromFitness(placedObjectInfo.gameObject.transform.position, raycastInfo));
            }
        }

        public static void DrawInitialHandle(Vector3 position, Color color)
        {
            VladislavTsurikov.DrawHandles.HandleButton(0, position, color, color);
        }

        public static Color GetCurrentColorFromFitness(Vector3 position, RaycastInfo raycastInfo)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            Color color = Color.green;
            float fitness = 1;

            if(precisionPaintSettings.EnableFilter)
            {
                PainterVariables painterVariables = new PainterVariables(precisionPaintSettings.FilterVisualisationSize, raycastInfo);
                Type type = AllAvailableTypes.GetType(PrecisePlaceTool.SelectedProto);

                if(type.FilterType == FilterType.MaskFilter)
                {
                    FilterMaskOperation.UpdateMaskTexture(PrecisePlaceTool.SelectedProto, PrecisePlaceTool.SelectedProto.MaskFilterStack, painterVariables);
                }

                fitness = GetFitnessUtility.GetFitnessForGameObject(type, painterVariables.Bounds, PrecisePlaceTool.SelectedProto, raycastInfo);

                if(fitness < 0.5)
                {
                    float difference = fitness / 0.5f;
                    color = Color.Lerp(Color.red, Color.yellow, difference);
                }
                else
                {
                    float difference = (fitness - 0.5f) / 0.5f;
                    color = Color.Lerp(Color.yellow, Color.green, difference);
                }                    
            }

            if(precisionPaintSettings.OverlapCheck)
            {
                Vector3 scale = PlacedObjectInfoController.PlacedObjectInfo.gameObject.transform.localScale;
                Quaternion rotation = PlacedObjectInfoController.PlacedObjectInfo.gameObject.transform.rotation;

                InstanceData instanceData = new InstanceData(position, scale, rotation, fitness);

                if(!PrecisePlaceTool.SelectedProto.OverlapCheckSettings.RunOverlapCheckForGameObject(PrecisePlaceTool.SelectedProto, instanceData))
                {
                    return Color.red;
                }
            }

            return color;
        }
    }
}
#endif