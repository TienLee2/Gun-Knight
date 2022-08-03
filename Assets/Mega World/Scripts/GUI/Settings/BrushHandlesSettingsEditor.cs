#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushHandlesSettingsEditor 
    {
        private bool brushHandlesSettingsFoldout = true;

        public void OnGUI(BrushHandlesSettings brushHandlesSettings)
        {
            BrushHandlesSettings(brushHandlesSettings);
        }

        public void BrushHandlesSettings(BrushHandlesSettings brushHandlesSettings)
		{
			brushHandlesSettingsFoldout = CustomEditorGUI.Foldout(brushHandlesSettingsFoldout, "Brush Handles Settings");

			if(brushHandlesSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brushHandlesSettings.BrushHandlesType = (BrushHandlesType)CustomEditorGUI.EnumPopup(new GUIContent("Brush Handles Type"), brushHandlesSettings.BrushHandlesType);
				brushHandlesSettings.DrawSolidDisc = CustomEditorGUI.Toggle(new GUIContent("Draw Solid Disc"), brushHandlesSettings.DrawSolidDisc);

				switch (brushHandlesSettings.BrushHandlesType)
				{
					case BrushHandlesType.Circle:
					{
						brushHandlesSettings.CircleColor = CustomEditorGUI.ColorField(new GUIContent("Сircle Color"), brushHandlesSettings.CircleColor);       				
						brushHandlesSettings.CirclePixelWidth = CustomEditorGUI.Slider(new GUIContent("Сircle Pixel Width"), brushHandlesSettings.CirclePixelWidth, 1f, 5f);

						break;
					}
					case BrushHandlesType.SphereAndCircle:
					{
						brushHandlesSettings.CircleColor = CustomEditorGUI.ColorField(new GUIContent("Сircle Color Cube"), brushHandlesSettings.CircleColor);
						brushHandlesSettings.CirclePixelWidth = CustomEditorGUI.Slider(new GUIContent("Сircle Pixel Width"), brushHandlesSettings.CirclePixelWidth, 1f, 5f);
						brushHandlesSettings.SphereColor = CustomEditorGUI.ColorField(new GUIContent("Sphere Color"), brushHandlesSettings.SphereColor);
						brushHandlesSettings.SpherePixelWidth = CustomEditorGUI.Slider(new GUIContent("Sphere Pixel Width"), brushHandlesSettings.SpherePixelWidth, 1f, 5f);

						break;
					}
				}

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif