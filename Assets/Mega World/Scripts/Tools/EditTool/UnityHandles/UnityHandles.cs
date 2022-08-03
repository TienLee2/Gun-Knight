#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.Edit
{
    [Serializable]
    public static class UnityHandles 
    {
        public static bool DrawModifyHandles(InstanceData instanceData)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            switch (edit.PrecisionModifyMode)
            {
                case PrecisionModifyMode.Position:
                {
                    return MoveObject(instanceData, edit.RaycastPositionOffset);
                }
                case PrecisionModifyMode.Rotation:
                {
                    return RotateObject(instanceData);
                }
                case PrecisionModifyMode.Scale:
                {
                    return ScaleObject(instanceData);
                }
            }

            return false;
        }

        public static bool MoveObject(InstanceData instanceData, float raycastPositionOffset)
        {
            if(Event.current.control && Event.current.shift)
            {
                EditorGUI.BeginChangeCheck();

                EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

                float size = HandleUtility.GetHandleSize(instanceData.position) * 0.15f;

                Vector3 snap = Vector3.one * 0.5f;
                Vector3 newTargetPosition = Handles.FreeMoveHandle(instanceData.position, Quaternion.identity, size, snap, Handles.RectangleHandleCap);

                if(EditorGUI.EndChangeCheck())
                {
                    RaycastInfo dragToLayersRaycastInfo;
                    Utility.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out dragToLayersRaycastInfo, edit.GroundLayers);

                    if(dragToLayersRaycastInfo.isHit)
                    {
                        Vector3 finalPosition = new Vector3(dragToLayersRaycastInfo.hitInfo.point.x, dragToLayersRaycastInfo.hitInfo.point.y + raycastPositionOffset, dragToLayersRaycastInfo.hitInfo.point.z);
                        instanceData.position = finalPosition;
                        return true;
                    }

                    return true;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                Quaternion rotation = instanceData.rotation;

                if(MegaWorldPath.GeneralDataPackage.TransformSpace == TransformSpace.Local)
                {
                    rotation = Quaternion.identity;
                }

                Vector3 newTargetPosition = Handles.PositionHandle(instanceData.position, rotation);

                if(EditorGUI.EndChangeCheck())
                {
                    instanceData.position = newTargetPosition;
                    return true;
                }
            }

            return false;
        }

        public static bool RotateObject(InstanceData instanceData)
        {
            EditorGUI.BeginChangeCheck();

            Quaternion rot = Handles.RotationHandle(instanceData.rotation, instanceData.position);

            if(EditorGUI.EndChangeCheck())
            {
                instanceData.rotation = rot;
                return true;
            }

            return false;
        }

        public static bool ScaleObject(InstanceData instanceData)
        {
            EditorGUI.BeginChangeCheck();

            float size = HandleUtility.GetHandleSize(instanceData.position) * 1f;

            Vector3 localScale = Handles.ScaleHandle(instanceData.scale, instanceData.position, instanceData.rotation, size);

            if(EditorGUI.EndChangeCheck())
            {
                instanceData.scale = localScale;
                return true;
            }

            return false;
        }
    }
}
#endif