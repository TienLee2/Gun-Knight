using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using MegaWorld.BrushModify;
using MegaWorld.Edit;
using MegaWorld.PrecisePlace;

namespace MegaWorld
{
    [Serializable]
    public class GeneralDataPackage : ScriptableObject
    {
        public BrushSettings MultipleBrushSettings = new BrushSettings();
        public BrushSettings BrushSettingsForErase = new BrushSettings();
        public BrushSettings BrushSettingsForModify = new BrushSettings();

        public EditSettings EditSettings = new EditSettings();
        public PrecisePlaceToolSettings PresitionPlaceSettings = new PrecisePlaceToolSettings();
        public PinToolSettings PinToolSettings = new PinToolSettings();
        public BrushModifyToolSettings BrushModifyToolSettings = new BrushModifyToolSettings();

        public KeyboardButtonStates KeyboardButtonStates = new KeyboardButtonStates();

        public MegaWorldTools CurrentTool 
        {
            get
            {
                return ToolComponentsStack.GetSelected();
            }
        }

        public ToolComponentsStack ToolComponentsStack = new ToolComponentsStack();

#if UNITY_EDITOR
		private ToolComponentsView _toolComponentsView = null;
		public ToolComponentsView ToolComponentsView
		{
			get
			{
				if(_toolComponentsView == null || _toolComponentsView.Stack == null)
				{
					_toolComponentsView = new ToolComponentsView(ToolComponentsStack);
				}

				return _toolComponentsView;
			}
		}
#endif

        [NonSerialized]
        private StorageCells _storageCells = null;

        public StorageCells StorageCells
        {
            get
            {
                if(_storageCells == null)
                {
                    _storageCells = new StorageCells();
                    _storageCells.RefreshCells(CellSize);
                }

                return _storageCells;
            }
            set
            {
                _storageCells = value;
            }
        }

        public TransformSpace TransformSpace = TransformSpace.Global;
        public bool EnableUndoForGameObject = true;
        public float CellSize = 100;
        public float TextureTargetStrength = 1.0f;
        public float DragFailureRate = 50f;
        public float EraseStrength = 1.0f;

#if UNITY_EDITOR
        public TerrainResourcesControllerEditor TerrainResourcesControllerEditor = new TerrainResourcesControllerEditor();
        public GameObjectControllerEditor GameObjectControllerEditor = new GameObjectControllerEditor();
        public bool ShowCells = false;
#endif
    }
}