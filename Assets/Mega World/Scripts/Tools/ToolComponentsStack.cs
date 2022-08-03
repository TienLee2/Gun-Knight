using System;
using UnityEngine;
using System.Collections.Generic;

namespace MegaWorld
{
    [Serializable]
    public class ToolComponentsStack 
    {
        [SerializeField]
        public List<ToolComponent> Tools = new List<ToolComponent>();

        public void DoSelectedTool()
        {
            for (int i = 0; i < Tools.Count; i++)
            {
                if(Tools[i].Enabled)
                {
                    Tools[i].DoTool();
                }
            }
        }

        public bool HasSettings(System.Type type)
        {
            foreach (var setting in Tools)
            {
                if (setting.GetType() == type)
                    return true;
            }

            return false;
        }

        public MegaWorldTools GetSelected()
        {
            foreach (ToolComponent tool in Tools)
            {
                if(tool.Enabled)
                {
                    return tool.GetTool();
                }
            }

            return MegaWorldTools.None;
        }
    }
}
