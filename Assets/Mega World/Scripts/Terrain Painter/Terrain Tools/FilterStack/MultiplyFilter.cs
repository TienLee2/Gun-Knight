using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class MultiplyFilter : Filter
    {
        public float Value = 1;

        public override void Eval(FilterContext fc, int index)
        {
            FilterUtility.builtinMaterial.SetFloat("_Multiply", Value);

            Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Multiply );
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            Value = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Value);
        }

        public override string GetDisplayName()
        {
            return "Multiply";
        }

        public override string GetToolTip()
        {
            return "Multiply the Brush Mask filter stack by a constant";
        }
#endif
    }
}
