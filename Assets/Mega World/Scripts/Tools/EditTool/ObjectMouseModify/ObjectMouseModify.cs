#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.Edit
{
    [Serializable]
    public class ObjectMouseModify 
    {
        [NonSerialized] private InstanceData _instanceData;
        [NonSerialized] private Vector3 _objectGlobalScaleAtSessionStart;
        [NonSerialized] private Vector3 _objectPositionAtSessionBegin;
        [NonSerialized] private Vector2 _cursorPosAtSessionStart;

        public ObjectMouseMoveAlongDirectionSettings ObjectMouseMoveAlongDirectionSettings = new ObjectMouseMoveAlongDirectionSettings();
        public ObjectMouseUniformScaleSettings ObjectMouseUniformScaleSettings = new ObjectMouseUniformScaleSettings();
        public ObjectMouseRotationSettings ObjectMouseRotationSettings = new ObjectMouseRotationSettings();

        public void Prepare(InstanceData instanceData)
        {
            if (instanceData != null)
            {
                this._instanceData = instanceData;
                _cursorPosAtSessionStart = Event.current.mousePosition;
                _objectGlobalScaleAtSessionStart = instanceData.scale;
                _objectPositionAtSessionBegin = instanceData.position;
            }
        }

        #region Scale
        public void ScaleObject()
        {
            Vector2 fromPosAtSessionStartToCurrentPos = MouseCursor.Instance.Position - _cursorPosAtSessionStart;

            float mouseSensitivityScale = CalculateMouseSensitivityScale();
            float newMouseSensitivity = ObjectMouseUniformScaleSettings.MouseSensitivity * mouseSensitivityScale * 0.1f;

            float addToScale = fromPosAtSessionStartToCurrentPos.x * newMouseSensitivity;
            float scaleFactor = 1.0f + addToScale;

            UniformScaleObject(scaleFactor);
        }

        private void UniformScaleObject(float scaleFactor)
        {
            _instanceData.scale = _objectGlobalScaleAtSessionStart * scaleFactor;
        }

        public float CalculateMouseSensitivityScale()
        {
            float maxAbsScaleComponent = Mathf.Abs(GetComponentWithBiggestAbsValue());
            if (maxAbsScaleComponent < 1e-5f) 
                maxAbsScaleComponent = 0.001f;
            return 1.0f / maxAbsScaleComponent;
        }

        public float GetComponentWithBiggestAbsValue()
        {
            float maxComponent = _objectGlobalScaleAtSessionStart.x;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(_objectGlobalScaleAtSessionStart.y)) 
                maxComponent = _objectGlobalScaleAtSessionStart.y;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(_objectGlobalScaleAtSessionStart.z)) 
                maxComponent = _objectGlobalScaleAtSessionStart.z;

            return maxComponent;
        }
        #endregion

        #region Rotation
        public void RotateObject(Vector3 axis)
        {
            float rotationAmountInDegrees = MouseCursor.Instance.OffsetSinceLastMouseMove.x * ObjectMouseRotationSettings.MouseSensitivity;

            Quaternion rotation = Quaternion.AngleAxis(rotationAmountInDegrees, axis);
            _instanceData.rotation = rotation * _instanceData.rotation;
        }
        #endregion

        #region Position
        public void PositionObject(Vector3 moveDirection)
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            switch (edit.PositionMode)
            {
                case PositionMode.Raycast:
                {
                    RaycastObject();
                    break;
                }
                case PositionMode.MoveAlongDirection:
                {
                    MouseMoveAlongDirection(moveDirection);
                    break;
                }
            }
        }

        public void MouseMoveAlongDirection(Vector3 moveDirection)
        {
            moveDirection.Normalize();

            Vector2 fromPosAtSessionStartToCurrentPos = MouseCursor.Instance.Position - _cursorPosAtSessionStart;

            _instanceData.position = _objectPositionAtSessionBegin + moveDirection * (ObjectMouseMoveAlongDirectionSettings.MouseSensitivity * 0.1f * fromPosAtSessionStartToCurrentPos.x);
        }

        public void RaycastObject()
        {
            EditSettings edit = MegaWorldPath.GeneralDataPackage.EditSettings;

            RaycastInfo dragToLayersRaycastInfo;
            Utility.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out dragToLayersRaycastInfo, edit.GroundLayers);

            if(dragToLayersRaycastInfo.isHit == false)
            {
                return;
            }

            Vector3 finalPosition = new Vector3(dragToLayersRaycastInfo.hitInfo.point.x, dragToLayersRaycastInfo.hitInfo.point.y + edit.RaycastPositionOffset, dragToLayersRaycastInfo.hitInfo.point.z);
            _instanceData.position = finalPosition;
        }
        #endregion
    }
}
#endif