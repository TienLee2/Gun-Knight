using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public struct DebugColorsetSettings 
    {
        public List<Color> ColorList;
        [SerializeField]
        private int _currentColorListIndex;
        public int CurrentColorListIndex
        {
            get
            {
                return _currentColorListIndex;
            }
        }

        public DebugColorsetSettings(List<Color> colorList, int currentColorListIndex)
        {   
            this.ColorList = colorList;
            this._currentColorListIndex = currentColorListIndex;
        }

        

        public void IncreaseIndex()
        {
            _currentColorListIndex += 1;

            if(_currentColorListIndex > ColorList.Count - 1)
            {
                _currentColorListIndex = 0;
            }
        }
    }
    
    [Serializable]
    public class TerrainCDLODOcclusionCullingSettings
    {
        public CellLODsSetings cellLODsSetings = new CellLODsSetings();
        public float renderCellSize = 2000;
        public bool showVisibleCells;
        public bool showRenderCell;

        public DebugColorsetSettings debugColorSetSettings;

        public TerrainCDLODOcclusionCullingSettings()
        {
            List<Color> colorList = new List<Color>(3) {Color.red, Color.green, Color.blue};

            debugColorSetSettings = new DebugColorsetSettings(colorList, 0);
        }
    }
}