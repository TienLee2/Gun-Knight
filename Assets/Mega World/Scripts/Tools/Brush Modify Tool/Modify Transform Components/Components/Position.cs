using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.BrushModify
{
    public class Position : ModifyTransformComponent
    {
        public float Strength = -0.1f;
        public float YStrengthRandomize = 100;   

        public override void SetInstanceData(ref InstanceData spawnInfo, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            float randomPositionY = modifyInfo.RandomPositionY * (YStrengthRandomize / 100f);

            float localStrengthPosition = spawnInfo.fitness * moveLenght;

            float addPositionY = Strength * localStrengthPosition * 0.05f;
            addPositionY = Mathf.Lerp(addPositionY, 0, randomPositionY);
    
            Vector3 position = new Vector3(spawnInfo.position.x, spawnInfo.position.y + addPositionY, spawnInfo.position.z);

            spawnInfo.position = position;
        }
        
#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            Strength = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength"), Strength, -1f, 1f);
            rect.y += EditorGUIUtility.singleLineHeight;
            YStrengthRandomize = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength Randomize (%)"), YStrengthRandomize, 0f, 100f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }

        public override string GetDisplayName() 
        {
            return "Position";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}
