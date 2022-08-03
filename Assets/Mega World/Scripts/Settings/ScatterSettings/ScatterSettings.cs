using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public enum GlobalSpawnType 
    { 
        Grid,
        RandomPoint,
        RandomClustered
    }

    public enum ScatterAlgorithm 
    { 
        RandomPoint, 
        Grid,
    }

    [Serializable]
    public class ScatterSettings 
    {   
        public ScatterAlgorithm ScatterAlgorithm = ScatterAlgorithm.Grid;
        public GlobalSpawnType StamperScatterAlgorithm = GlobalSpawnType.Grid;

        public Grid Grid = new Grid();
        public RandomPoint RandomPoint = new RandomPoint();
        public RandomClustered RandomClustered = new RandomClustered();

#if UNITY_EDITOR
        public ScatterSettingsEditor ScatterSettingsEditor = new ScatterSettingsEditor();

        public void OnGUI(MegaWorldTools tools)
        {
            ScatterSettingsEditor.OnGUI(this, tools);
        }
#endif

        public void SetGlobalSpawnScatterPosition(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            switch (StamperScatterAlgorithm)
			{
				case GlobalSpawnType.Grid:
				{
                    Grid.SetPosition(type, painterVariables, func);

                    break;
                }
                case GlobalSpawnType.RandomPoint:
				{
                    RandomPoint.SetPositionForStamper(type, painterVariables, func);

                    break;
                }
                case GlobalSpawnType.RandomClustered:
				{
                    RandomClustered.SetPosition(type, painterVariables, func);

                    break;
                }
            }
        }

        public void SetBrushScatterPosition(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            switch (ScatterAlgorithm)
            {
                case ScatterAlgorithm.RandomPoint:
                {
                    RandomPoint.SetPosition(type, painterVariables, func);
    
                    break;
                }
                case ScatterAlgorithm.Grid:
                {
                    Grid.SetPosition(type, painterVariables, func);
    
                    break;
                }
            }
        }
    }
}

