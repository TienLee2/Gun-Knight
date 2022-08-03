using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    [Serializable]
    public class CliffsAlign : TransformComponent
    {
        public override void SetInstanceData(ref InstanceData spawnInfo, Vector3 normal)
        {
            Vector3 direction = new Vector3(normal.x, 0, normal.z);

            float distancePositive = Vector3.Distance(Vector3.right, direction);
            float distanceNegative = Vector3.Distance(-Vector3.right, direction);

            if(distancePositive < distanceNegative)
            {
                float angle = Vector3.Angle(Vector3.forward, direction);
                spawnInfo.rotation = Quaternion.AngleAxis(angle, Vector3.up) * spawnInfo.rotation;
            }
            else
            {
                float angle = -Vector3.Angle(Vector3.forward, direction);
                spawnInfo.rotation = Quaternion.AngleAxis(angle, Vector3.up) * spawnInfo.rotation;
            }
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
            return "Cliffs Align";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}