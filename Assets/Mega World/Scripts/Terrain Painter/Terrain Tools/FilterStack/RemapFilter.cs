using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class RemapFilter : Filter
    {
        [Range (0, 1)]
        public float Min = 0.4f;

        [Range (0, 1)]
        public float Max = 0.6f;

        public Vector2 Height = new Vector2(0.0f, 1.0f);
        public float HeightFeather = 0.0f;

        private ComputeShader m_HeightCS = null;
        public ComputeShader GetComputeShader() 
        {
            if (m_HeightCS == null) 
            {
                m_HeightCS = (ComputeShader)Resources.Load("Remap");
            }
            return m_HeightCS;
        }

        public override void Eval(FilterContext fc, int index)
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("Remap");

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);
            cs.SetVector("HeightRange", new Vector4(Height.x, Height.y, HeightFeather, 0.0f));
            cs.SetFloat("RemapMin", Min);
            cs.SetFloat("RemapMax", Max);
            

            cs.Dispatch(kidx, fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;

            GUIStyle heightNumberFormat = new GUIStyle();

            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Remap"), ref Min, ref Max, 0, 1);

            rect.y += EditorGUIUtility.singleLineHeight;

            //Min Label
            Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            GUIContent minContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
            // Min Entry Field
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            //slope is stored as scalar value from 0 to 0.5, so we multiply / divide by 180
            Min = EditorGUI.FloatField(numFieldRect, Min);
            //Empty Label Field for Spacing
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(numFieldRect, " ");
            //MaxEntryField
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            //slope is stored as scalar value from 0 to 0.5, so we multiply / divide by 180
            Max = EditorGUI.FloatField(numFieldRect, Max);
            //Max Label
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
            
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }

        public override string GetDisplayName()
        {
            return "Remap";
        }

        public override string GetToolTip()
        {
            return "Clamps the pixels of a mask to the specified range. Change the X value to specify the low end of the range, and change the Y value to specify the high end of the range.";
        }
#endif
    }
}
