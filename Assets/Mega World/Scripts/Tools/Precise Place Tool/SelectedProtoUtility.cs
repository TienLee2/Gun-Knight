#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace MegaWorld.PrecisePlace
{
    public static class SelectedProtoUtility 
    {
        public static void SetSelectedProto(Type type)
        {
            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            if(precisionPaintSettings.SelectType == PreciseSelectType.Unit)
            {
                PrecisePlaceTool.SelectedProto = GetPrototypeFromPrecisionUnit(type);
            }
            else
            {
                PrecisePlaceTool.SelectedProto = GetRandomSelectedPrototype(type);
            }
        }

        private static PrototypeGameObject GetPrototypeFromPrecisionUnit(Type type)
        {
            int count = 0;
            foreach (PrototypeGameObject item in type.ProtoGameObjectList)
            {
                if(count == type.GetPrecisionUnit())
                {
                    return item;
                }
                count++;
            }

            return null;
        }

        private static PrototypeGameObject GetRandomSelectedPrototype(Type type)
        {
            if(type.ProtoGameObjectList.Count == 0)
            {
                return null;
            }

            List<PrototypeGameObject> protoList = new List<PrototypeGameObject>();

            foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
            {
                if(proto.selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList[UnityEngine.Random.Range(0, protoList.Count - 1)];
        }
    }
}
#endif