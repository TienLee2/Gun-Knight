using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class Filter : ScriptableObject
    {
        public bool Enabled = true;
        public bool FoldoutGUI = true;
        
        public virtual void Eval( FilterContext filterContext, int index) {}
#if UNITY_EDITOR
        public virtual void DoGUI( Rect rect, int index ) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
        public virtual string GetDisplayName() => "EMPTY_FILTER_NAME";
        public virtual string GetToolTip() => "EMPTY_TOOLTIP";
#endif
    }
}