using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ObjectMouseRotation
    {
        private TransformAxis rotationAxis;
        private GameObject gameObject;
        private Vector3 customRotationAxis;
        private bool rotatingAroundCustomAxis;
        private bool freeRotating;
        private bool _isActive;

        public bool IsActive { get { return _isActive; } }
        public TransformAxis RotationAxis { get { return rotationAxis; } }
        public bool RotatingAroundCustomAxis { get { return rotatingAroundCustomAxis; } }
        public bool FreeRotating { get { return freeRotating; } }
        public ObjectMouseRotationSettings objectMouseRotationSettings = new ObjectMouseRotationSettings();

        public void BeginRotationAroundAxis(GameObject gameObject, TransformAxis rotationAxis)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                this.rotationAxis = rotationAxis;
                this.gameObject = gameObject;
                rotatingAroundCustomAxis = false;
                freeRotating = false;
            }
        }

        public void BeginRotationAroundCustomAxis(GameObject gameObject, Vector3 rotationAxis)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                this.customRotationAxis = rotationAxis;
                this.gameObject = gameObject;
                rotatingAroundCustomAxis = true;
                freeRotating = false;
            }
        }

        public void BeginFreeRotation(GameObject gameObject)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                this.gameObject = gameObject;
                rotatingAroundCustomAxis = false;
                freeRotating = true;
            }
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(TransformSpace transformSpace)
        {
            if(_isActive && gameObject != null)
            {
                if (rotatingAroundCustomAxis) 
                {
                    RotateObjectAroundCustomAxis();
                }
                else if(freeRotating)
                {
                    FreeRotateObject();
                }
                else 
                {
                    RotateObjectAroundAxis(transformSpace);
                }
            }
        }

        private void RotateObjectAroundAxis(TransformSpace transformSpace)
        {
            float rotationAmountInDegrees = MouseCursor.Instance.OffsetSinceLastMouseMove.x * objectMouseRotationSettings.MouseSensitivity;
            Vector3 axis = TransformAxes.GetVector(rotationAxis, transformSpace, gameObject.transform);

            Quaternion rotation = Quaternion.AngleAxis(rotationAmountInDegrees, axis);
            gameObject.transform.rotation = rotation * gameObject.transform.rotation;
        }

        private void RotateObjectAroundCustomAxis()
        {
            float rotationAmountInDegrees = MouseCursor.Instance.OffsetSinceLastMouseMove.x * objectMouseRotationSettings.MouseSensitivity;
            customRotationAxis.Normalize();

            Quaternion rotation = Quaternion.AngleAxis(rotationAmountInDegrees, customRotationAxis);
            gameObject.transform.rotation = rotation * gameObject.transform.rotation;
        }

        private void FreeRotateObject()
        {
#if UNITY_EDITOR

            float rotationAmountInDegreesX = -MouseCursor.Instance.OffsetSinceLastMouseMove.x * objectMouseRotationSettings.MouseSensitivity;
            float rotationAmountInDegreesY = -MouseCursor.Instance.OffsetSinceLastMouseMove.y * objectMouseRotationSettings.MouseSensitivity;

            Quaternion rotationX = Quaternion.AngleAxis(rotationAmountInDegreesY, SceneView.currentDrawingSceneView.camera.transform.right);
            Quaternion rotationY = Quaternion.AngleAxis(rotationAmountInDegreesX, SceneView.currentDrawingSceneView.camera.transform.up);

            Quaternion rotation = rotationX * rotationY;

            gameObject.transform.rotation = rotation * gameObject.transform.rotation;

#endif
        }
    }
}

