using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ClampFilter : Filter
    {
        public Vector2 Range = new Vector2(0, 1);

        public override void Eval(FilterContext fc, int index)
        {
            FilterUtility.builtinMaterial.SetVector("_ClampRange", Range);

            Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Clamp );
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            Range = EditorGUI.Vector2Field(rect, "", Range);
        }

        public override string GetDisplayName()
        {
            return "Clamp";
        }

        public override string GetToolTip()
        {
            return "Clamps the pixels of a mask to the specified range. Change the X value to specify the low end of the range, and change the Y value to specify the high end of the range.";
        }
#endif
    }
}
