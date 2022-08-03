#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld.Edit
{
    [Serializable]
    public class UnityHandlesEditTool 
    {
        public UnityHandlesGameObject UnityHandlesGameObject = new UnityHandlesGameObject();
        public UnityHandlesQuadroItem UnityHandlesQuadroItem = new UnityHandlesQuadroItem();

        public void DoTool()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
            {
                UnityHandlesGameObject.DoTool();
            }
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
            {
                UnityHandlesQuadroItem.DoTool();
            }
        }
    }

}
#endif