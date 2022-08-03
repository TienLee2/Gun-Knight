using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld.BrushModify
{
    public class Scale : ModifyTransformComponent
    {
        public float Strength = 0.3f;
        public float StrengthRandomize = 100;    

        public override void SetInstanceData(ref InstanceData spawnInfo, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal)
        {
            float randomScale = modifyInfo.RandomScale * (StrengthRandomize / 100f);

            float localStrengthScale = spawnInfo.fitness * moveLenght;

            float addScale = Strength * localStrengthScale * 0.005f;
            addScale = Mathf.Lerp(addScale, 0, randomScale);
        
            spawnInfo.scale = new Vector3(spawnInfo.scale.x + addScale, spawnInfo.scale.y + addScale, spawnInfo.scale.z + addScale);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            Strength = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength"), Strength, -1f, 1f);
            rect.y += EditorGUIUtility.singleLineHeight;
            StrengthRandomize = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength Randomize (%)"), StrengthRandomize, 0f, 100f);
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
            return "Scale";
        }

        public override string GetToolTip()
        {
            return "";
        }
#endif
    }
}