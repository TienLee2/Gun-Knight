using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ObjectMouseMoveAlongDirection
    {
        private Vector3 normalizedMoveDirection;
        private Vector3 objectPositionAtSessionBegin;
        private Vector2 cursorPosAtSessionStart;
        private GameObject gameObject;
        private bool _isActive;
        public bool IsActive { get { return _isActive; } }
        public ObjectMouseMoveAlongDirectionSettings objectMouseMoveAlongDirectionSettings = new ObjectMouseMoveAlongDirectionSettings();

        public void Begin(GameObject gameObject, Vector3 moveDirection)
        {
            if (_isActive || moveDirection.magnitude == 0 || gameObject == null) 
            {
                return;
            }

            this.gameObject = gameObject;

            objectPositionAtSessionBegin = gameObject.transform.position;
    
            cursorPosAtSessionStart = Event.current.mousePosition;
            normalizedMoveDirection = moveDirection;
            normalizedMoveDirection.Normalize();

            _isActive = true;
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(Event e)
        {
            Vector2 currentCursorPos = e.mousePosition;
            Vector2 fromPosAtSessionStartToCurrentPos = currentCursorPos - cursorPosAtSessionStart;

            gameObject.transform.position = objectPositionAtSessionBegin + normalizedMoveDirection * ((objectMouseMoveAlongDirectionSettings.MouseSensitivity * 0.2f) * fromPosAtSessionStartToCurrentPos.x);
        }
    }
}

