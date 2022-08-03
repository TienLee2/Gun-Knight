using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.BrushModify
{
    public class Rotation : ModifyTransformComponent
    {
        public float ModifyStrengthRotation = 3;
        public bool ModifyRandomRotationX = true;
        public bool ModifyRandomRotationY = true;
        public bool ModifyRandomRotationZ = true;
        public Vector3 ModifyRandomRotationValues = new Vector3(1, 1, 1);
        public Vector3 ModifyRotationValues = new Vector3(1, 1, 1);

        public override void SetInstanceData(ref InstanceData spawnInfo, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            Vector3 modifyRotation = GetModifyRotation(modifyInfo);

            float localStrengthRotation = ModifyStrengthRotation * spawnInfo.fitness * moveLenght;
        
            Quaternion rotation = spawnInfo.rotation * Quaternion.Euler(modifyRotation * localStrengthRotation * 0.1f);
        
            spawnInfo.rotation = rotation;
        }

        public Vector3 GetModifyRotation(BrushModifyTool.ModifyInfo modifyInfo)
        {
            float x = 0;
            float y = 0;
            float z = 0;

            if(ModifyRandomRotationX)
            {
                x = ModifyRandomRotationValues.x * modifyInfo.RandomRotation.x;
            }
            else
            {
                x = ModifyRotationValues.x;
            }

            if(ModifyRandomRotationY)
            {
                y = ModifyRandomRotationValues.y * modifyInfo.RandomRotation.y;
            }
            else
            {
                y = ModifyRotationValues.y;
            }

            if(ModifyRandomRotationZ)
            {
                z = ModifyRandomRotationValues.z * modifyInfo.RandomRotation.z;
            }
            else
            {
                z = ModifyRotationValues.z;
            }
        
            return new Vector3(x, y, z);
        }


#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            ModifyStrengthRotation = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength"), ModifyStrengthRotation, 0f, 10f);
            rect.y += EditorGUIUtility.singleLineHeight;

            ModifyRandomRotationX = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Random Rotation X"), ModifyRandomRotationX);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if(ModifyRandomRotationX)
                {
                    ModifyRandomRotationValues.x = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize X"), ModifyRandomRotationValues.x, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    ModifyRotationValues.x = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation X"), ModifyRotationValues.x, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }
            
            ModifyRandomRotationY = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Random Rotation Y"), ModifyRandomRotationY);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if(ModifyRandomRotationY)
                {
                    ModifyRandomRotationValues.y = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize Y"), ModifyRandomRotationValues.y, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    ModifyRotationValues.y = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation Y"), ModifyRotationValues.y, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }
            
            ModifyRandomRotationZ = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Random Rotation Z"), ModifyRandomRotationZ);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if(ModifyRandomRotationZ)
                {
                    ModifyRandomRotationValues.z = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize Z"), ModifyRandomRotationValues.z, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    ModifyRotationValues.z = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation Z"), ModifyRotationValues.z, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if(ModifyRandomRotationX)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }
            
            height += EditorGUIUtility.singleLineHeight;
            {
                if(ModifyRandomRotationY)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }

            }

            height += EditorGUIUtility.singleLineHeight;
            {
                if(ModifyRandomRotationZ)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }

            }

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Rotation";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}