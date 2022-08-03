#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class RaycastSettingsEditor 
    {
        public bool raycastSettingsFoldout = true;

        public void OnGUI(RaycastSettings raycastSettings)
        {
            RaycastSettings(raycastSettings);
        }

        public void RaycastSettings(RaycastSettings raycastSettings)
		{
			raycastSettingsFoldout = CustomEditorGUI.Foldout(raycastSettingsFoldout, "Raycast Settings");

			if(raycastSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				raycastSettings.MaxRayDistance = CustomEditorGUI.FloatField(new GUIContent("Max Ray Distance", "The max distance the ray should check for collisions."), raycastSettings.MaxRayDistance);
				raycastSettings.SpawnCheckOffset = CustomEditorGUI.FloatField(new GUIContent("Spawn Check Offset", "If you want to spawn objects under pawns or inside buildings or in other similar cases. You need to decrease the Spawn Check Offset."), raycastSettings.SpawnCheckOffset);

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif