using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld.Stamper
{
    public static class UnspawnForStamper
    {
        public static void UnspawnAllResourcesTypes(BasicData data)
        {
            foreach (Type type in data.TypeList)
            {
                Unspawn.UnspawnTerrainDetail(type.ProtoTerrainDetailList, false);
                Unspawn.UnspawnQuadroItem(type, false);
                Unspawn.UnspawnGameObject(type, false);
            }
        }

        public static void UnspawnSelectedProto(BasicData data)
        {
            foreach (Type type in data.SelectedVariables.SelectedTypeList)
            {
                Unspawn.UnspawnTerrainDetail(type.ProtoTerrainDetailList, true);
                Unspawn.UnspawnQuadroItem(type, true); 
                Unspawn.UnspawnGameObject(type, true);
            }
        }

        public static void UnspawnTypesForAutoMode(StamperToolControllerSettings settings, Type modifiedType, BasicData data)
        {            
            foreach (Type type in data.SelectedVariables.SelectedTypeList)
            {
                if(modifiedType.ResourceType == ResourceType.TerrainDetail)
                {
                    Unspawn.UnspawnTerrainDetail(type.ProtoTerrainDetailList, false);
                }
                else if(modifiedType.ResourceType == ResourceType.QuadroItem)
                {
                    Unspawn.UnspawnQuadroItem(type, false);
                }
                else if(modifiedType.ResourceType == ResourceType.GameObject)
                {
                    Unspawn.UnspawnGameObject(type, false);
                }
            }
        }
    }
}