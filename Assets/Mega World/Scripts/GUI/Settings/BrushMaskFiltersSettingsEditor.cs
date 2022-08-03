#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushMaskFiltersSettingsEditor 
    {
        private bool maskFiltersSettingsFoldout = true;

        public void OnGUI(MaskFiltersSettings brushMaskFiltersSettings)
        {
            BrushMaskFiltersSettings(brushMaskFiltersSettings);
        }

        public void BrushMaskFiltersSettings(MaskFiltersSettings brushMaskFiltersSettings)
		{
			maskFiltersSettingsFoldout = CustomEditorGUI.Foldout(maskFiltersSettingsFoldout, "Mask Filters Settings");

			if(maskFiltersSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brushMaskFiltersSettings.ColorSpace = (ColorSpaceForBrushMaskFilter)CustomEditorGUI.EnumPopup(new GUIContent("Color Space"), brushMaskFiltersSettings.ColorSpace);
				
				switch (brushMaskFiltersSettings.ColorSpace)
				{
					case ColorSpaceForBrushMaskFilter.СustomColor:
					{
						brushMaskFiltersSettings.Color = CustomEditorGUI.ColorField(new GUIContent("Color"), brushMaskFiltersSettings.Color);
						brushMaskFiltersSettings.EnableStripe = CustomEditorGUI.Toggle(new GUIContent("Enable Stripe"), brushMaskFiltersSettings.EnableStripe);

						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUI.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);
						
						break;
					}
					case ColorSpaceForBrushMaskFilter.Colorful:
					{							
						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUI.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);

						break;
					}
					case ColorSpaceForBrushMaskFilter.Heightmap:
					{
						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUI.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);

						break;
					}
				}

				brushMaskFiltersSettings.CustomAlpha = CustomEditorGUI.Slider(new GUIContent("Alpha"), brushMaskFiltersSettings.CustomAlpha, 0, 1);
				brushMaskFiltersSettings.EnableDefaultPreviewMaterial = CustomEditorGUI.Toggle(new GUIContent("Enable Default Preview Material"), brushMaskFiltersSettings.EnableDefaultPreviewMaterial);
				
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif