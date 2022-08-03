#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(Area))]
    public class AreaEditor : Editor
    {
        private Area area;
        
        void OnEnable()
        {
            area = (Area)target;

            if(area.AreaBounds == null)
            {
                area.SetAreaBounds();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            CustomEditorGUI.isInspector = true;

            DrawAreaSettings(area);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(area, "Made changes");
                EditorUtility.SetDirty(area);
            }
        }

		public void DrawAreaHandlesSettings(Area area)
		{
			area.HandlesSettingsFoldout = CustomEditorGUI.Foldout(area.HandlesSettingsFoldout, "Handles Settings");

			if(area.HandlesSettingsFoldout)
			{
                EditorGUI.indentLevel++;

                area.HandleSettingsMode = (HandleSettingsMode)CustomEditorGUI.EnumPopup(new GUIContent("Handle Settings Mode"), area.HandleSettingsMode);

                if (area.HandleSettingsMode == HandleSettingsMode.Custom)
                {
                    area.ColorCube = CustomEditorGUI.ColorField(new GUIContent("Color Cube"), area.ColorCube);
                    area.PixelWidth = CustomEditorGUI.Slider(new GUIContent("Pixel Width"), area.PixelWidth, 1f, 5f);
                    area.Dotted = CustomEditorGUI.Toggle(new GUIContent("Dotted"), area.Dotted);
                }

                area.DrawHandleIfNotSelected = CustomEditorGUI.Toggle(new GUIContent("Draw Handle If Not Selected"), area.DrawHandleIfNotSelected);

                EditorGUI.indentLevel--;
            }
		}

        public void DrawAreaSettings(Area area)
        {
			if(area == null)
			{
				return;
			}

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
                if(CustomEditorGUI.ClickButton("Fit To Terrain Size"))
			    {
			    	area.FitToTerrainSize();
			    }
                GUILayout.Space(3);
            }
            GUILayout.EndHorizontal();

			GUILayout.Space(3);
            
			DrawAreaHandlesSettings(area);
            
            area.SetBoundsIfNecessary();
        }
    }
}
#endif