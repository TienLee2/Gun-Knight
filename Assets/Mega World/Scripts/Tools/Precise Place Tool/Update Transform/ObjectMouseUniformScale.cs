using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ObjectMouseUniformScale 
    {
        private TransformAxis scaleAxis;
        private Vector2 cursorPosAtSessionStart;
        private Vector3 objectGlobalScaleAtSessionStart;
        private PlacedObjectInfo gameObject;
        private bool uniformScale;
        private bool _isActive;

        public bool IsActive { get { return _isActive; } }
        public TransformAxis ScaleAxis { get { return scaleAxis; } }
        public bool UniformScale { get { return uniformScale; } }
        public ObjectMouseUniformScaleSettings objectMouseUniformScaleSettings = new ObjectMouseUniformScaleSettings();

        public void BeginUniformScale(PlacedObjectInfo gameObject)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;

                cursorPosAtSessionStart = Event.current.mousePosition;
                objectGlobalScaleAtSessionStart = gameObject.gameObject.transform.lossyScale;
                this.gameObject = gameObject;
                uniformScale = true;
            }
        }

        public void BeginScale(PlacedObjectInfo gameObject, TransformAxis rotationAxis)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;

                cursorPosAtSessionStart = Event.current.mousePosition;
                objectGlobalScaleAtSessionStart = gameObject.gameObject.transform.lossyScale;
                this.gameObject = gameObject;
                this.scaleAxis = rotationAxis;
                uniformScale = false;
            }
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(Event e)
        {
            if(_isActive && gameObject != null)
            {
                Vector2 currentCursorPos = e.mousePosition;
                Vector2 fromPosAtSessionStartToCurrentPos = currentCursorPos - cursorPosAtSessionStart;
                
                float mouseSensitivityScale = CalculateMouseSensitivityScale();
                float newMouseSensitivity = objectMouseUniformScaleSettings.MouseSensitivity * mouseSensitivityScale * 0.2f;

                float addToScale = fromPosAtSessionStartToCurrentPos.x * newMouseSensitivity;
                float scaleFactor = 1.0f + addToScale;

                if(uniformScale)
                {
                    UniformScaleObject(scaleFactor);
                }
                else
                {
                    ScaleObjectAroundAxis(scaleFactor);
                }
            }
        }

        private float CalculateMouseSensitivityScale()
        {
            float maxAbsScaleComponent = Mathf.Abs(GetComponentWithBiggestAbsValue());
            if (maxAbsScaleComponent < 1e-5f) maxAbsScaleComponent = 0.001f;
            return 1.0f / maxAbsScaleComponent;
        }

        public float GetComponentWithBiggestAbsValue()
        {
            float maxComponent = objectGlobalScaleAtSessionStart.x;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(objectGlobalScaleAtSessionStart.y)) maxComponent = objectGlobalScaleAtSessionStart.y;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(objectGlobalScaleAtSessionStart.z)) maxComponent = objectGlobalScaleAtSessionStart.z;

            return maxComponent;
        }

        public void UniformScaleObject(float scaleFactor)
        {
            gameObject.gameObject.transform.parent = null;
            gameObject.gameObject.transform.localScale = objectGlobalScaleAtSessionStart * scaleFactor;
        }

        public void ScaleObjectAroundAxis(float scaleFactor)
        {
            Transform objectTransform = gameObject.gameObject.transform;
            Transform objectParent = objectTransform.parent;

            float x = objectGlobalScaleAtSessionStart.x;
            float y = objectGlobalScaleAtSessionStart.y;
            float z = objectGlobalScaleAtSessionStart.z;

            switch (scaleAxis)
            {
                case TransformAxis.X:
                {
                    x = x * scaleFactor;
                    break;
                }
                case TransformAxis.Y:
                {
                    y = y * scaleFactor;
                    break;
                }
                case TransformAxis.Z:
                {
                    z = z * scaleFactor;
                    break;
                }
            }

            objectTransform.parent = null;
            objectTransform.localScale = new Vector3(x, y, z);

            float minScale = 1e-4f;
            if (Mathf.Abs(objectTransform.localScale.x) < minScale) objectTransform.localScale = new Vector3(minScale, objectTransform.localScale.y, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.y) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, minScale, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.z) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, objectTransform.localScale.y, minScale);
        }
    }
}
