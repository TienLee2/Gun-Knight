using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.BrushModify
{
    public class Along : ModifyTransformComponent
    {
        public override void SetInstanceData(ref InstanceData instanceData, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            Quaternion rotation = Quaternion.identity;

            Vector3 upwards = new Vector3(0, 1, 0);

            float strength = Mathf.InverseLerp(0, 15, moveLenght);
            strength *= instanceData.fitness;

            Vector3 forward = Vector3.Cross(strokeDirection, upwards);
            
            rotation = Quaternion.LookRotation(forward, upwards);

            instanceData.rotation = Quaternion.Lerp(instanceData.rotation, rotation, strength);
        } 


#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Along";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}