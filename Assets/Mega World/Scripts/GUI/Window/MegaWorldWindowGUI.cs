#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	public partial class MegaWorldWindow : EditorWindow
	{

		[MenuItem("Window/Mega World/Main")]
        static void Init()
        {
            MegaWorldWindow window = (MegaWorldWindow)EditorWindow.GetWindow(typeof(MegaWorldWindow));
            window.Show();
            window.titleContent = new GUIContent("Mega World");
            window.Focus();
            window.ShowUtility();
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
			UpdateSceneViewEvent();

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

		private void HandleKeyboardEvents(Event e)
        {
			if(e.keyCode == KeyCode.Escape && e.modifiers == 0)
            {
				if(MegaWorldPath.GeneralDataPackage.CurrentTool != MegaWorldTools.None)
				{
                    Tools.current = Tool.Move;
					MegaWorldPath.GeneralDataPackage.ToolComponentsView.DisableAllTools();
				}

                Repaint();
            }
		}

		private void UpdateSceneViewEvent()
		{
			Event e = Event.current;

			if (e.Equals(KeyDeleteEvent()))
			{
				Unspawn.UnspawnAllSelectedProto();
			}

			HandleSceneViewEvent(e);
			HandleKeyboardEvents(e);
		}

		void OnMainGUI()
		{
			GUILayout.Space(5);

			CustomEditorGUI.screenRect = this.position;

			if(ToolsWindow.Window == null)
			{
				MegaWorldPath.GeneralDataPackage.ToolComponentsView.DrawToolsWindow();
			}

			if(MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.None)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
           		EditorGUILayout.LabelField("No Tool Selected");
           		EditorGUILayout.EndVertical();

				return;
			}

			MegaWorldPath.GeneralDataPackage.ToolComponentsView.DrawSelectedToolSettings();

			MegaWorldPath.DataPackage.SetAllDataDirty();
		}

		public static Event KeyDeleteEvent()
        {
            Event retEvent = Event.KeyboardEvent("^" + "backspace");
            return retEvent;
        }
	}
}
#endif