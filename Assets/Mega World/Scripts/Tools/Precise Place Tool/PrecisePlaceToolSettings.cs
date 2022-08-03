using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld.PrecisePlace
{
    public enum PreciseSelectType 
    { 
        Unit, 
        RandomRange 
    }

    public enum CoordinateSystemAxis
    {
        PositiveRight = 0,
        NegativeRight,
        PositiveUp,
        NegativeUp,
        PositiveLook,
        NegativeLook
    }

    [Serializable]
    public class PrecisePlaceToolSettings
    {
        public PreciseSelectType SelectType = PreciseSelectType.RandomRange;
        public ObjectMousePrecision ObjectMousePrecision = new ObjectMousePrecision();

        public float Spacing = 5;
        public bool RandomiseTransform = true;
        public bool OverlapCheck = true;
        public bool EnableFilter = true;

        public float FilterVisualisationSize = 10;
        public bool VisualizeOverlapCheckSettings = false;
        public bool VisualizeFilterSettings = false;

        #region Rotation
        public bool AlignAxis = true;
        public CoordinateSystemAxis AlignmentAxis = CoordinateSystemAxis.PositiveUp;
        public float WeightToNormal = 1;
        public bool AlongStroke;
        #endregion 
    }
}
    