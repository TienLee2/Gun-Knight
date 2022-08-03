using UnityEngine;
using System;

namespace MegaWorld.Edit
{
    public enum EditTools
    {
        UnityHandles,
        MouseModify 
    }

    public enum PrecisionModifyMode
    {
        Position,
        Rotation,
        Scale,
        Remove
    }

    public enum PositionMode
    {
        Raycast,
        MoveAlongDirection,
    }

    [Serializable]
    public class EditSettings
    {
#if UNITY_EDITOR
        public ObjectMouseModify ObjectMouseModify = new ObjectMouseModify();
#endif
        
        public PrecisionModifyMode PrecisionModifyMode = PrecisionModifyMode.Scale;
        public PositionMode PositionMode = PositionMode.Raycast;

        public EditTools PositionTool = EditTools.UnityHandles;
		public EditTools RotationTool = EditTools.UnityHandles;
		public EditTools ScaleTool = EditTools.UnityHandles;

        public float SphereSize = 10;
        public float MaxDistance = 30;
        public float RaycastPositionOffset = 0;
        public LayerMask GroundLayers;
    }
}
    
