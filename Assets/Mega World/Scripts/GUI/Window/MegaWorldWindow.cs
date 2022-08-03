#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public partial class MegaWorldWindow : EditorWindow
    {
        private void OnEnable()
        {
            AllAvailableTypes.Refresh();

            hideFlags = HideFlags.HideAndDontSave;

            SceneView.duringSceneGui += OnSceneGUI;

            if (EditorApplication.update != EditorApplicationUpdateCallback)
            {
                EditorApplication.update += EditorApplicationUpdateCallback;
            }

            if (EditorApplication.modifierKeysChanged != ModifierKeysChangedCallback)
            {
                EditorApplication.modifierKeysChanged += ModifierKeysChangedCallback;
            }
        }

        private void OnDisable()
        {
            MegaWorldPath.GeneralDataPackage.ToolComponentsView.DisableAllTools();

            SceneView.duringSceneGui -= OnSceneGUI;

            EditorApplication.update -= EditorApplicationUpdateCallback;
            EditorApplication.modifierKeysChanged -= ModifierKeysChangedCallback;

            EditorUtility.SetDirty(MegaWorldPath.DataPackage);
            EditorUtility.SetDirty(MegaWorldPath.GeneralDataPackage);
        }

        private void EditorApplicationUpdateCallback()
        {
            if (Tools.current != Tool.None)
            {
                if(MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.BrushPaint || 
                    MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.BrushErase || 
                    MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.BrushModify)
                {
                    MegaWorldPath.GeneralDataPackage.ToolComponentsView.DisableAllTools();
                    Repaint();
                }
            }
        }

        void ModifierKeysChangedCallback()
        {
            Repaint();
        }

        private void OnSceneGUI(SceneView sceneView)
        { 
            AllAvailableTypes.Refresh();
            MegaWorldPath.DataPackage.SelectedVariables.DeleteNullValueIfNecessary(MegaWorldPath.DataPackage.BasicData.TypeList);
            MegaWorldPath.DataPackage.SelectedVariables.SetAllSelectedParameters(MegaWorldPath.DataPackage.BasicData.TypeList);
            UpdateSceneViewEvent();

            if (MegaWorldPath.GeneralDataPackage.CurrentTool == MegaWorldTools.None)
            {
                return;
            }

            MegaWorldPath.GeneralDataPackage.ToolComponentsStack.DoSelectedTool();
        }

        public void HandleSceneViewEvent(Event e)
        {
            switch(e.type)
            {
				case EventType.MouseMove:
				{
					MouseCursor.Instance.HandleMouseMoveEvent(e);
                    break;
				}
                case EventType.KeyDown:
				{
					MegaWorldPath.GeneralDataPackage.KeyboardButtonStates.OnKeyboardButtonPressed(e.keyCode);
                    break;
				}
                case EventType.KeyUp:
				{
					MegaWorldPath.GeneralDataPackage.KeyboardButtonStates.OnKeyboardButtonReleased(e.keyCode);
                    break;
				}
            }
        }
    }
} 
#endif