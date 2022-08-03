using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ObjectMouseMoveAlongDirectionSettings
    {
        #region Private Variables
        [SerializeField]
        private float _mouseSensitivity = 0.2f;
        #endregion
    
        #region Public Static Properties
        public static float MinMouseSensitivity { get { return 0.001f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }
        #endregion
    
        #region Public Properties
        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
        #endregion
    }
}

