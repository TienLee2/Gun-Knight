#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public static class TerrainToolGUIHelper
    {
        public static bool DrawToggleHeaderFoldout(GUIContent title, bool state, ref bool enabled)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.xMin += 13f;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = foldoutRect;
            toggleRect.x = foldoutRect.xMax + 4f;

    		// Background rect should be full-width
    		backgroundRect.xMin = 16f * EditorGUI.indentLevel;
    		backgroundRect.xMin = 0;

    		backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            // Enabled toggle
            enabled = GUI.Toggle(toggleRect, enabled, GUIContent.none, EditorStyles.toggle);

            var e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (toggleRect.Contains(e.mousePosition))
                {
                    enabled = !enabled;
                    e.Use();
                }
                else if (backgroundRect.Contains(e.mousePosition))
                {
                    state = !state;
                    e.Use();
                }
            }

            return state;
        }
    }
}
#endif