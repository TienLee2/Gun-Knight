#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	public class SelectionWindow : EditorWindow
	{
		public static SelectionWindow Window;

		[MenuItem("Window/Mega World/Selection")]
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
			Window = (SelectionWindow)EditorWindow.GetWindow(typeof(SelectionWindow));
            Window.Show();
            Window.titleContent = new GUIContent("Selection");
            Window.Focus();
            Window.ShowUtility();
		}

		void OnInspectorUpdate()
    	{
    	    Repaint();
    	}

		void OnGUI()
        {
			AllAvailableTypes.Refresh();
			MegaWorldPath.DataPackage.SelectedVariables.DeleteNullValueIfNecessary(MegaWorldPath.DataPackage.BasicData.TypeList);
			MegaWorldPath.DataPackage.SelectedVariables.SetAllSelectedParameters(MegaWorldPath.DataPackage.BasicData.TypeList);

			EditorGUI.indentLevel = 0;

			CustomEditorGUI.isInspector = false;

			InternalDragAndDrop.OnBeginGUI();

			OnMainGUI();

			InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
			{
				Repaint();
			}
        }

		void OnMainGUI()
		{
			GUILayout.Space(5);

			CustomEditorGUI.screenRect = this.position;

			MegaWorldPath.DataPackage.BasicData.OnGUI(MegaWorldPath.GeneralDataPackage.CurrentTool);
		}
	}
}
#endif