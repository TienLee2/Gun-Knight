using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public enum TransformMode
    {
        Free,
        Snap,
        Fixed,
    }

    [Serializable]
    public class PinToolSettings
    {
        #region Position
        public float Offset = 0;
        #endregion

        #region Rotation
        public TransformMode RotationTransformMode = TransformMode.Free;
        public FromDirection FromDirection;
        public float WeightToNormal = 1;
        public float SnapRotationValue = 30;
        public Vector3 FixedRotationValue;
        #endregion

        #region Scale
        public TransformMode ScaleTransformMode = TransformMode.Free;
        public Vector3 FixedScaleValue;
        public float SnapScaleValue = 0.3f;
        #endregion
    }
}
    