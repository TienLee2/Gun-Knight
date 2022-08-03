using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class PositionOffset : TransformComponent
    {
        public bool OnlyY = true;
        public Vector3 MinPositionOffset = new Vector3(0f, -0.15f, 0f);
        public Vector3 MaxPositionOffset = new Vector3(0f, 0f, 0f);

        public override void SetInstanceData(ref InstanceData spawnInfo, Vector3 normal)
        {
            if(OnlyY)
            {
                spawnInfo.position += new Vector3(0, UnityEngine.Random.Range(MinPositionOffset.y, MaxPositionOffset.y), 0);
            }
            else
            {
                spawnInfo.position += new Vector3(
                UnityEngine.Random.Range(MinPositionOffset.x, MaxPositionOffset.x),
                UnityEngine.Random.Range(MinPositionOffset.y, MaxPositionOffset.y),
                UnityEngine.Random.Range(MinPositionOffset.z, MaxPositionOffset.z));
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            OnlyY = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Only Y"), OnlyY);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(OnlyY)
            {
                MinPositionOffset.y = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Position Offset"), MinPositionOffset.y);
                rect.y += EditorGUIUtility.singleLineHeight;
                MaxPositionOffset.y = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Position Offset"), MaxPositionOffset.y);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                MinPositionOffset = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Position Offset"), MinPositionOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
                MaxPositionOffset = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Position Offset"), MaxPositionOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if(OnlyY)
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Position Offset";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}

