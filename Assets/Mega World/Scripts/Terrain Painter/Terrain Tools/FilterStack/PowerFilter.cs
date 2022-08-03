using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class PowerFilter : Filter
    {
        public float Value = 2;
        
        public override void Eval(FilterContext fc, int index)
        {
            FilterUtility.builtinMaterial.SetFloat("_Pow", Value);

            Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Power );
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            Value = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Value);
        }

        public override string GetDisplayName()
        {
            return "Power";
        }

        public override string GetToolTip()
        {
            return "Applies an exponential function to each pixel on the Brush Mask. The function is pow(value, e), where e is the input value.";
        }
#endif
    }
}
