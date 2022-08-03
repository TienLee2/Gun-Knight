#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	public class ToolsWindow : EditorWindow
	{
		public static ToolsWindow Window;

		[MenuItem("Window/Mega World/Tools")]
        static void Init()
        {
			OpenWindow();
        }

		private void OnEnable() 
		{
			Window = this;
		}
		
		public static void OpenWindow()
		{
			Window = (ToolsWindow)EditorWindow.GetWindow(typeof(ToolsWindow));
            Window.Show();
            Window.titleContent = new GUIContent("Tools");
            Window.Focus();
            Window.ShowUtility();
		}

		void OnInspectorUpdate()
    	{
    	    Repaint();
    	}

		void OnGUI()
        {
			EditorGUI.indentLevel = 0;

			CustomEditorGUI.isInspector = false;

			InternalDragAndDrop.OnBeginGUI();

			MegaWorldPath.GeneralDataPackage.ToolComponentsView.DrawToolsWindow();

			InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
			{
				Repaint();
			}
        }
	}
}
#endif