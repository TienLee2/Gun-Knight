using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld.Edit
{
    public class GameObjectInfo
    {
        public GameObject Obj;
        public PrototypeGameObject Proto;
        public int CellIndex;

        public GameObjectInfo(GameObject obj, PrototypeGameObject proto, int cellIndex)
        {
            this.Obj = obj;
            this.Proto = proto;
            this.CellIndex  = cellIndex;
        }

        public bool IsValid()
        {
            if(Obj == null)
            {
                return false;
            }
            if(Proto == null)
            {
                return false;
            }

            return true;
        }
    }
}