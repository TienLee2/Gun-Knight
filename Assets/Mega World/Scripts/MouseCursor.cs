using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    public class MouseCursor : Singleton<MouseCursor>
    {
        #region Private Static Variables
        private static bool _defaultObjectMaskEnabledState = true;
        #endregion

        #region Private Variables
        private Vector2 _previousPosition;
        private Vector2 _offsetSinceLastMouseMove;
        private Stack<bool> _objectMaskEnabledStates = new Stack<bool>();
        #endregion

        #region Public Properties
        public Vector2 PreviousPosition { get { return _previousPosition; } }
        public Vector2 OffsetSinceLastMouseMove { get { return _offsetSinceLastMouseMove; } }
        public Vector2 Position { get { return Event.current.mousePosition; } }
        public bool IsObjectMaskEnabled { get { return _objectMaskEnabledStates.Count != 0 ? _objectMaskEnabledStates.Peek() : _defaultObjectMaskEnabledState; } }
        #endregion

        #region Public Methods

        public void HandleMouseMoveEvent(Event e)
        {
            _offsetSinceLastMouseMove = e.mousePosition - _previousPosition;
            _previousPosition = e.mousePosition;
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
