using System.Collections.Generic;
using UnityEngine;
using System;
using QuadroRendererSystem;

namespace MegaWorld.Edit
{
    public class QuadroItemInfo
    {
        public ObjectInstanceData Obj;
        public PrototypeQuadroItem Proto;
        public int CellIndex;

        public QuadroItemInfo(ObjectInstanceData obj, PrototypeQuadroItem proto, int cellIndex)
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