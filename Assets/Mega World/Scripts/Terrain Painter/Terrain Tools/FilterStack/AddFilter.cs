using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class AddFilter : Filter
    {
        public float Value;

        public override void Eval(FilterContext fc, int index)
        {
            FilterUtility.builtinMaterial.SetFloat("_Add", Value);

            Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Add );
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            Value = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Value);
        }
        public override string GetDisplayName()
        {
            return "Add";
        }

        public override string GetToolTip()
        {
            return "Adds a constant to the Brush Mask filter stack";
        }
#endif
    }
}