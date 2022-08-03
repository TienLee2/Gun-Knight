using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class SlopePosition : TransformComponent
    {
        [Range(0.1f, 90f)]
        public float MaxSlope = 90;

        public float MaxPositionOffset = -1;

        public override void SetInstanceData(ref InstanceData spawnInfo, Vector3 normal)
        {
            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;
            
            float positionY = Mathf.Lerp(0, MaxPositionOffset, difference);

            spawnInfo.position += new Vector3(0, positionY, 0);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            MaxSlope = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Slope"), MaxSlope, 0, 90);
            rect.y += EditorGUIUtility.singleLineHeight;
            MaxPositionOffset = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Position Offset"), MaxPositionOffset);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
        
        public override string GetDisplayName() 
        {
            return "Slope Position";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}
