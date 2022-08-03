using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ObjectMousePrecision
    {
        public ObjectMouseUniformScale ObjectMouseUniformScale = new ObjectMouseUniformScale();
        public ObjectMouseRotation ObjectMouseRotation = new ObjectMouseRotation();
        public ObjectMouseMoveAlongDirection ObjectMouseMoveAlongDirection = new ObjectMouseMoveAlongDirection();
        public bool IsMouseMoveAlongDirectionSessionActive { get { return ObjectMouseMoveAlongDirection.IsActive; } }
        public bool IsMouseUniformScaleSessionActive { get { return ObjectMouseUniformScale.IsActive; } }
        public bool IsMouseRotationSessionActive { get { return ObjectMouseRotation.IsActive; } }
        public TransformAxis MouseRotationSessionAxis { get { return ObjectMouseRotation.RotationAxis; } }
        public TransformAxis MouseScaleSessionAxis { get { return ObjectMouseUniformScale.ScaleAxis; } }
        public bool IsAnyMouseSessionActive { get { return IsMouseRotationSessionActive || IsMouseUniformScaleSessionActive || IsMouseMoveAlongDirectionSessionActive; } }
        public bool IsMouseRotationSessionForCustomAxis { get { return ObjectMouseRotation.RotatingAroundCustomAxis; } }
        public bool IsMouseFreeRotation { get { return ObjectMouseRotation.FreeRotating; } }
        public bool IsUniformScale { get { return ObjectMouseUniformScale.UniformScale; } }

        public void UpdateActiveMouseForMouseMovement(Event e, TransformSpace transformSpace)
        {
            if(IsMouseMoveAlongDirectionSessionActive) 
            {
                ObjectMouseMoveAlongDirection.UpdateForMouseMovement(e);
            }
            else if(IsMouseUniformScaleSessionActive)
            {
                ObjectMouseUniformScale.UpdateForMouseMovement(e);
            }
            else if(IsMouseRotationSessionActive)
            {
                ObjectMouseRotation.UpdateForMouseMovement(transformSpace);
            }
        }

        public void CancelMousePrecisionIfNecessary(KeyboardButtonStates keyboardButtonStates)
        {
            if (MustEndMousePrecisionScale(keyboardButtonStates)) 
            {
                EndMouseScale();
            }
            if (MustEndMousePrecisionRotation(keyboardButtonStates)) 
            {
                EndMouseRotation();
            }
            if (MustEndOffsetFromPlacement(keyboardButtonStates)) 
            {
                EndMouseMoveAlongDirection();
            }
        }

        public bool MustEndMousePrecisionRotation(KeyboardButtonStates keyboardButtonStates)
        {
            if(IsMouseFreeRotation)
            {
                if(!PrecisePlaceToolAllShortcutCombos.Instance.MouseFreeRotate.IsActive(keyboardButtonStates))
                {
                    return true;
                }
                 
            }
            else
            {
                if (MouseRotationSessionAxis == TransformAxis.X && !PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundX.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                if (MouseRotationSessionAxis == TransformAxis.Y && !PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundY.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                if (MouseRotationSessionAxis == TransformAxis.Z && !PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundZ.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                
            }

            return false;
        }

        public bool MustEndMousePrecisionScale(KeyboardButtonStates keyboardButtonStates)
        {
            if(IsUniformScale)
            {
                if(!PrecisePlaceToolAllShortcutCombos.Instance.MouseUniformScale.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                
            }
            else
            {
                if (MouseScaleSessionAxis == TransformAxis.X && !PrecisePlaceToolAllShortcutCombos.Instance.MouseScaleX.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                if (MouseScaleSessionAxis == TransformAxis.Y && !PrecisePlaceToolAllShortcutCombos.Instance.MouseScaleY.IsActive(keyboardButtonStates)) 
                {
                    return true;
                }
                if (MouseScaleSessionAxis == TransformAxis.Z && !PrecisePlaceToolAllShortcutCombos.Instance.MouseScaleZ.IsActive(keyboardButtonStates))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MustEndOffsetFromPlacement(KeyboardButtonStates keyboardButtonStates)
        {
            return !PrecisePlaceToolAllShortcutCombos.Instance.OffsetFromPlacementSurface.IsActive(keyboardButtonStates);
        }

        public void BeginGuideMouseRotationSession(GameObject gameObject, KeyboardButtonStates keyboardButtonStates)
        {
#if UNITY_EDITOR
            if (!IsAnyMouseSessionActive)
            {
                if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundX.IsActive(keyboardButtonStates))
                {
                    ObjectMouseRotation.BeginRotationAroundAxis(gameObject, TransformAxis.X);
                }
                else if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundY.IsActive(keyboardButtonStates))
                {
                    ObjectMouseRotation.BeginRotationAroundAxis(gameObject, TransformAxis.Y);
                }
                else if (PrecisePlaceToolAllShortcutCombos.Instance.MouseRotateAroundZ.IsActive(keyboardButtonStates))
                {
                    ObjectMouseRotation.BeginRotationAroundAxis(gameObject, TransformAxis.Z);
                }
                else if(PrecisePlaceToolAllShortcutCombos.Instance.MouseFreeRotate.IsActive(keyboardButtonStates)) 
                {
                    ObjectMouseRotation.BeginFreeRotation(gameObject);
                }   
            }
#endif
        }

        public void EndMouseRotation()
        {
            ObjectMouseRotation.End();
        }

        public void BeginMouseMoveAlongDirectionSession(Vector3 moveDirection, GameObject gameObject)
        {
            if (!IsAnyMouseSessionActive)
            {
                ObjectMouseMoveAlongDirection.Begin(gameObject, moveDirection);
            }
        }

        public void EndMouseMoveAlongDirection()
        {
            ObjectMouseMoveAlongDirection.End();
        }

        public void BeginMouseScaleSession(PlacedObjectInfo gameObject, KeyboardButtonStates keyboardButtonStates)
        {
            if (!IsAnyMouseSessionActive)
            {
                if (PrecisePlaceToolAllShortcutCombos.Instance.MouseUniformScale.IsActive(keyboardButtonStates))
                {
                    ObjectMouseUniformScale.BeginUniformScale(gameObject);
                }

                /*
                if (AllShortcutCombos.Instance.MousePlacementGuideScaleX.IsActive(keyboardButtonStates))
                {
                    objectMouseUniformScale.BeginScale(gameObject, TransformAxis.X);
                }
                else if (AllShortcutCombos.Instance.MousePlacementGuideScaleY.IsActive(keyboardButtonStates))
                {
                    objectMouseUniformScale.BeginScale(gameObject, TransformAxis.Y);
                }
                else if(AllShortcutCombos.Instance.MousePlacementGuideScaleZ.IsActive(keyboardButtonStates)) 
                {
                    objectMouseUniformScale.BeginScale(gameObject, TransformAxis.Z);
                } 
                else if (AllShortcutCombos.Instance.MousePlacementGuideUniformScale.IsActive(keyboardButtonStates))
                {
                    objectMouseUniformScale.BeginUniformScale(gameObject);
                }
                */
            }
        }

        public void EndMouseScale()
        {
            ObjectMouseUniformScale.End();
        }
    }
}
