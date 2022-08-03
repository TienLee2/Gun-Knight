#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld.Edit
{
    public class MouseModifyEditTool 
    {
        public MouseModifyQuadroItem MouseModifyQuadroItem = new MouseModifyQuadroItem();
        public MouseModifyGameObject MouseModifyGameObject = new MouseModifyGameObject();

        public void DoTool()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
            {
                MouseModifyGameObject.DoTool();
            }
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
            {
                MouseModifyQuadroItem.DoTool();
            }
        }
    }
}
#endif