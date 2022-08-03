using System.Collections.Generic;
using System;

namespace MegaWorld.Stamper
{
    [Serializable]
    public class StamperToolControllerSettings
    {
        [NonSerialized] public Type _modifiedType = null;
        [NonSerialized] private PrototypeTerrainDetail _modifiedTerrainDetailPrototype = null;
        
        public Timer TimerForStamperTool = new Timer();
        public bool Visualisation = true;
        public bool AutoSpawn = false;
        public float DelayAutoSpawn = 0f;

        public PrototypeTerrainDetail ModifiedTerrainDetailPrototype
        {
            get
            {
                return _modifiedTerrainDetailPrototype;
            }
        }

        public void SetModifiedTerrainDetailPrototype(PrototypeTerrainDetail terrainDetailPrototype)
        {
            _modifiedTerrainDetailPrototype = terrainDetailPrototype;
        }

        public void SetNullModifiedTerrainDetailPrototype()
        {
            _modifiedTerrainDetailPrototype = null;
        }
    }
}

