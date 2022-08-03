using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class InvertFilter : Filter
    {
        public float StrengthInvert = 1;
        
        public override void Eval(FilterContext fc, int index)
        {
            FilterUtility.builtinMaterial.SetFloat("_Complement", StrengthInvert);

            Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Complement );
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            StrengthInvert = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength Invert"), StrengthInvert, 0f, 1f);
        }

        public override string GetDisplayName()
        {
            return "Invert";
        }

        public override string GetToolTip()
        {
            return "Subtracts each pixel value in the current Brush Mask from the specified constant. To invert the mask results, leave the complement value unchanged as 1.";
        }
#endif
    }
}
