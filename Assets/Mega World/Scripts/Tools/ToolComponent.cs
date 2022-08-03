using System;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public enum MegaWorldTools
    {
        None = -1,
        BrushPaint = 0,
        BrushErase = 1,
        BrushModify = 2,
        StamperTool = 3,
    	Edit = 4,
        PrecisePlace = 5,
        Pin = 6
    }
    
    [Serializable]
    public class ToolComponent : ScriptableObject
    {
        public bool Enabled = false;
        public virtual MegaWorldTools GetTool() => MegaWorldTools.None; 

        public virtual void DoTool() {}
        public virtual void OnToolDisabled(){}

#if UNITY_EDITOR
        public virtual void OnGUI(){}
        public virtual string GetDisplayName() => "EMPTY_TOOL_NAME";
        public virtual string GetToolTip() => "EMPTY_TOOLTIP";
#endif
    }
}