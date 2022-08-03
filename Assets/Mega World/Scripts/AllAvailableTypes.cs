using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    public static class AllAvailableTypes
	{
        public static List<Type> allTypeList = new List<Type>();

        public static void Refresh()
		{
            string pathToDataPackage = MegaWorldPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.DataPackageName);
            string pathToType = MegaWorldPath.CombinePath(pathToDataPackage, MegaWorldPath.Types);

            allTypeList = new List<Type>(Resources.LoadAll<Type>(pathToType));
		}

        public static PrototypeGameObject GetCurrentPrototypeGameObject(GameObject go)
        {
            foreach (Type type in allTypeList)
            {
                foreach (PrototypeGameObject prototypeGameObject in type.ProtoGameObjectList)
                {
                    if(Utility.IsSameGameObject(prototypeGameObject.prefab, go))
                    {
                        return prototypeGameObject;
                    }
                }
            }

            return null;
        }

        public static PrototypeGameObject GetCurrentPrototypeGameObject(int ID)
        {
            foreach (Type type in allTypeList)
            {
                foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
                {
                    if(proto.ID == ID)
                    {
                        return proto;
                    }
                }
            }
            
            return null;
        }

        public static PrototypeQuadroItem GetCurrentQuadroItem(int ID)
        {
            foreach (Type type in allTypeList)
            {
                foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
                {
                    if(proto.ID == ID)
                    {
                        return proto;
                    }
                }
            }
            
            return null;
        }

        public static PrototypeQuadroItem GetCurrentQuadroItem(Type type, int ID)
        {
            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                if(proto.ID == ID)
                {
                    return proto;
                }
            }
            
            return null;
        }

        public static Type GetType(PrototypeGameObject proto)
        {
            foreach (Type type in allTypeList)
            {
                foreach (PrototypeGameObject prototypeGameObject in type.ProtoGameObjectList)
                {
                    if(proto == prototypeGameObject)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
	}
}