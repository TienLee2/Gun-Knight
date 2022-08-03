using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MegaWorld
{
    public static class Success 
    {
        public static PrototypeGameObject GetMaxSuccessProtoGameObject(List<PrototypeGameObject> protoGameObjectList)
        {
            if(protoGameObjectList.Count == 1)
            {
                return protoGameObjectList[0];
            }

            List<float> successProto = new List<float>();
            
            foreach (PrototypeGameObject proto in protoGameObjectList)
            {
                if(proto.active == false)
                {
                    successProto.Add(-1);
                    continue;
                }
                
                float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                if(randomSuccess > proto.AdditionalSpawnSettings.Success / 100)
                {
                    randomSuccess = proto.AdditionalSpawnSettings.Success / 100;
                }

                successProto.Add(randomSuccess);
            }

            float maxSuccessProto = successProto.Max();
            int maxIndexProto = 0;

            maxIndexProto = successProto.ToList().IndexOf(maxSuccessProto);
            return protoGameObjectList[maxIndexProto];
        }

        public static PrototypeQuadroItem GetMaxSuccessProtoQuadroItem(List<PrototypeQuadroItem> protoQuadroItemList)
        {
            if(protoQuadroItemList.Count == 1)
            {
                return protoQuadroItemList[0];
            }

            List<float> successProto = new List<float>();

            foreach (PrototypeQuadroItem proto in protoQuadroItemList)
            {
                if(proto.active == false)
                {
                    successProto.Add(-1);
                    continue;
                }

                float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                if(randomSuccess > proto.AdditionalSpawnSettings.Success / 100)
                {
                    randomSuccess = proto.AdditionalSpawnSettings.Success / 100;
                }

                successProto.Add(randomSuccess);
            }

            float maxSuccessProto = successProto.Max();
            int maxIndexProto = 0;

            maxIndexProto = successProto.ToList().IndexOf(maxSuccessProto);
            return protoQuadroItemList[maxIndexProto];
        }
    }
}

