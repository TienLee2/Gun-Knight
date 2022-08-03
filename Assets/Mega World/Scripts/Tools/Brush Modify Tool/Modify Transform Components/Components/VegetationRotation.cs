using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.BrushModify
{
    [Serializable]
    public class VegetationRotation : ModifyTransformComponent
    {
        public float StrengthY = 7;
        public float StrengthXY = 10;
        public float RotationXZ = 3;

        public override void SetInstanceData(ref InstanceData instanceData, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            float localstrengthRotationY  = StrengthY * instanceData.fitness;
            float localstrengthRotationXY = StrengthXY * instanceData.fitness;

            Vector3 randomVector = modifyInfo.RandomRotation * 0.5f;
            float angleXZ = RotationXZ * 3.6f;
            float t = localstrengthRotationXY / 100;

            float rotationY = localstrengthRotationY * 3.6f * randomVector.y + instanceData.rotation.eulerAngles.y;
            float rotationX = angleXZ;
            float rotationZ = angleXZ;

            instanceData.rotation = Quaternion.Euler(new Vector3(instanceData.rotation.eulerAngles.x, rotationY, instanceData.rotation.eulerAngles.z));  
            
            Quaternion rotation = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));    
            Quaternion finalRotation = Quaternion.Lerp(instanceData.rotation, rotation, t);          

            instanceData.rotation = finalRotation;
        }

#if UNITY_EDITOR

        public override void DoGUI(Rect rect, int index) 
        {
            StrengthY = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength Y (%)"), StrengthY, 0.0f, 20.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            StrengthXY = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength Rotation XY"), StrengthXY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            RotationXZ = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation XZ (%)"), RotationXZ, 0.0f, 20.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 3;

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Vegetation Rotation";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}