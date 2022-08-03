using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class ScaleFitness : TransformComponent
    {
        public bool UniformScaleOffset = true;
        public bool Invert = false;
        public float OffsetUniformScale = -0.7f;
        public Vector3 OffsetScale = new Vector3(0.7f, 0.7f, 0.7f);

        public override void SetInstanceData(ref InstanceData spawnInfo, Vector3 normal)
        {
            if(Invert)
            {
                if(UniformScaleOffset)
                {
                    float value = Mathf.Lerp(0, OffsetUniformScale, spawnInfo.fitness);

                    spawnInfo.scale += new Vector3(value, value, value);
                }
                else
                {
                    float valueX = Mathf.Lerp(0, OffsetScale.x, spawnInfo.fitness);
                    float valueY = Mathf.Lerp(0, OffsetScale.y, spawnInfo.fitness);
                    float valueZ = Mathf.Lerp(0, OffsetScale.z, spawnInfo.fitness);

                    spawnInfo.scale += new Vector3(valueX, valueY, valueZ);
                }
            }
            else
            {
                if(UniformScaleOffset)
                {
                    float value = Mathf.Lerp(OffsetUniformScale, 0, spawnInfo.fitness);

                    spawnInfo.scale += new Vector3(value, value, value);
                }
                else
                {
                    float valueX = Mathf.Lerp(OffsetScale.x, 0, spawnInfo.fitness);
                    float valueY = Mathf.Lerp(OffsetScale.y, 0, spawnInfo.fitness);
                    float valueZ = Mathf.Lerp(OffsetScale.z, 0, spawnInfo.fitness);

                    spawnInfo.scale += new Vector3(valueX, valueY, valueZ);
                }
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            Invert = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Invert"), Invert);
            rect.y += EditorGUIUtility.singleLineHeight;

            UniformScaleOffset = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Scale Offset"), UniformScaleOffset);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(UniformScaleOffset)
            {
                OffsetUniformScale = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Offset Uniform Scale"), OffsetUniformScale);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                OffsetScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Offset Scale"), OffsetScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetElementHeight(int index) 
        {   
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            if(UniformScaleOffset)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Scale Fitness";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}