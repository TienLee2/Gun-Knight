#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class BrushSettingsEditor 
    {
		public BrushJitterSettingsEditor brushJitterSettingsEditor = new BrushJitterSettingsEditor();
        public bool brushSettingsFoldout = true;

        public void OnGUI(BrushSettings brush, string content)
        {
            BrushSettingsWindowGUI(brush, content);
        }

        public void BrushSettingsWindowGUI(BrushSettings brush, string content)
		{
			brushSettingsFoldout = CustomEditorGUI.Foldout(brushSettingsFoldout, content);

			if(brushSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				GeneralBrushSettings(brush);

				EditorGUI.indentLevel--;
			}
		}

		public void GeneralBrushSettings(BrushSettings brush)
		{
			brush.SpacingMode = (SpacingMode)CustomEditorGUI.EnumPopup(spacingMode, brush.SpacingMode);

			if(brush.SpacingMode == SpacingMode.Drag)
			{
				EditorGUI.indentLevel++;

				brush.SpacingEqualsType = (SpacingEqualsType)CustomEditorGUI.EnumPopup(spacingEqualsType, brush.SpacingEqualsType);

				if (brush.SpacingMode == SpacingMode.Drag)
				{
					if(brush.SpacingEqualsType == SpacingEqualsType.Custom)
					{
						brush.Spacing = CustomEditorGUI.FloatField(spacing, brush.Spacing);
					}
				}

				EditorGUI.indentLevel--;
			}

			brush.MaskType = (MaskType)CustomEditorGUI.EnumPopup(maskType, brush.MaskType);
			
			switch (brush.MaskType)
			{
				case MaskType.Custom:
				{
					brush.CustomMasks.OnGUI();

					break;
				}
				case MaskType.Procedural:
				{
					brush.ProceduralMask.OnGUI();

					break;
				}
			}

			brushJitterSettingsEditor.OnGUI(brush, brush.BrushJitterSettings);
		}

		[NonSerialized]
		private GUIContent spacingMode = new GUIContent("Spacing Mode", "Allows you to disable or enable Brush Drag.");
		[NonSerialized]
		private GUIContent spacingEqualsType = new GUIContent("Spacing Equals", "Allows you to set what size the Spacing will be.");
		[NonSerialized]
		private GUIContent spacingRange = new GUIContent("Spacing Range", "Sets limits on possible Spacing.");
		[NonSerialized]
		private GUIContent spacing = new GUIContent("Spacing", "Controls the distance between brush marks.");
		[NonSerialized]
		private GUIContent maskType = new GUIContent("Mask Type", "Allows you to choose which brush mask will be used.");
    }
}
#endif
