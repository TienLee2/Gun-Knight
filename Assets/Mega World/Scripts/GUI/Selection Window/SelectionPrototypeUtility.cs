#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public static class SelectionPrototypeUtility
    {
        public static void DeleteSelectedProtoGameObject(Type type)
        {
            type.ProtoGameObjectList.RemoveAll((prototype) => prototype.selected);
        }

        public static void DeleteSelectedProtoTerrainDetail(Type type)
        {
            type.ProtoTerrainDetailList.RemoveAll((prototype) => prototype.selected);
        }

        public static void DeleteSelectedProtoTerrainTexture(Type type)
        {
            type.ProtoTerrainTextureList.RemoveAll((prototype) => prototype.selected);
        }

        public static void DeleteSelectedProtoQuadroItem(Type type)
        {
            type.ProtoQuadroItemList.RemoveAll((prototype) => prototype.selected);
        }

        public static void SelectPrototype(Type type, int prototypeIndex, ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.GameObject:
                {
                    SetSelectedAllPrototypes(type, false);

                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoGameObjectList.Count)
                    {
                        return;
                    }

                    type.ProtoGameObjectList[prototypeIndex].selected = true;
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    SetSelectedAllPrototypes(type, false);

                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainDetailList.Count)
                    {
                        return;
                    }

                    type.ProtoTerrainDetailList[prototypeIndex].selected = true;
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    SetSelectedAllPrototypes(type, false);

                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainTextureList.Count)
                    {
                        return;
                    }

                    type.ProtoTerrainTextureList[prototypeIndex].selected = true;
                    break;
                }
                case ResourceType.QuadroItem:
                {
                    SetSelectedAllPrototypes(type, false);

                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoQuadroItemList.Count)
                    {
                        return;
                    }

                    type.ProtoQuadroItemList[prototypeIndex].selected = true;
                    break;
                }
            }
        }

        public static void SelectPrototypeAdditive(Type type, int prototypeIndex, ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.GameObject:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoGameObjectList.Count)
                    {
                        return;
                    }

                    type.ProtoGameObjectList[prototypeIndex].selected = !type.ProtoGameObjectList[prototypeIndex].selected;
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainDetailList.Count)
                    {
                        return;
                    }
        
                    type.ProtoTerrainDetailList[prototypeIndex].selected = !type.ProtoTerrainDetailList[prototypeIndex].selected;
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainTextureList.Count)
                    {
                        return;
                    }
        
                    type.ProtoTerrainTextureList[prototypeIndex].selected = !type.ProtoTerrainTextureList[prototypeIndex].selected;
                    break;
                }
                case ResourceType.QuadroItem:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoQuadroItemList.Count)
                    {
                        return;
                    }
        
                    type.ProtoQuadroItemList[prototypeIndex].selected = !type.ProtoQuadroItemList[prototypeIndex].selected;
                    break;
                }
            }
        }

        public static void SelectPrototypeRange(Type type, int prototypeIndex, ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.GameObject:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoGameObjectList.Count)
                    {
                        return;
                    }

                    int rangeMin = prototypeIndex;
                    int rangeMax = prototypeIndex;

                    for (int i = 0; i < type.ProtoGameObjectList.Count; i++)
                    {
                        if (type.ProtoGameObjectList[i].selected)
                        {
                            rangeMin = Mathf.Min(rangeMin, i);
                            rangeMax = Mathf.Max(rangeMax, i);
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        if (type.ProtoGameObjectList[i].selected != true)
                        {
                            break;
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        type.ProtoGameObjectList[i].selected = true;
                    }
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainDetailList.Count)
                    {
                        return;
                    }

                    int rangeMin = prototypeIndex;
                    int rangeMax = prototypeIndex;

                    for (int i = 0; i < type.ProtoTerrainDetailList.Count; i++)
                    {
                        if (type.ProtoTerrainDetailList[i].selected)
                        {
                            rangeMin = Mathf.Min(rangeMin, i);
                            rangeMax = Mathf.Max(rangeMax, i);
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        if (type.ProtoTerrainDetailList[i].selected != true)
                        {
                            break;
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        type.ProtoTerrainDetailList[i].selected = true;
                    }
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoTerrainTextureList.Count)
                    {
                        return;
                    }

                    int rangeMin = prototypeIndex;
                    int rangeMax = prototypeIndex;

                    for (int i = 0; i < type.ProtoTerrainTextureList.Count; i++)
                    {
                        if (type.ProtoTerrainTextureList[i].selected)
                        {
                            rangeMin = Mathf.Min(rangeMin, i);
                            rangeMax = Mathf.Max(rangeMax, i);
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        if (type.ProtoTerrainTextureList[i].selected != true)
                        {
                            break;
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        type.ProtoTerrainTextureList[i].selected = true;
                    }
                    break;
                }
                case ResourceType.QuadroItem:
                {
                    if(prototypeIndex < 0 && prototypeIndex >= type.ProtoQuadroItemList.Count)
                    {
                        return;
                    }

                    int rangeMin = prototypeIndex;
                    int rangeMax = prototypeIndex;

                    for (int i = 0; i < type.ProtoQuadroItemList.Count; i++)
                    {
                        if (type.ProtoQuadroItemList[i].selected)
                        {
                            rangeMin = Mathf.Min(rangeMin, i);
                            rangeMax = Mathf.Max(rangeMax, i);
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        if (type.ProtoQuadroItemList[i].selected != true)
                        {
                            break;
                        }
                    }

                    for (int i = rangeMin; i <= rangeMax; i++) 
                    {
                        type.ProtoQuadroItemList[i].selected = true;
                    }
                    break;
                }
            }
        }

        public static void SetSelectedAllPrototypes(Type type, bool select)
        {
            foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
            {
                proto.selected = select;
            }

            foreach (PrototypeTerrainDetail proto in type.ProtoTerrainDetailList)
            {
                proto.selected = select;
            }

            foreach (PrototypeTerrainTexture proto in type.ProtoTerrainTextureList)
            {
                proto.selected = select;
            }

            foreach (PrototypeQuadroItem proto in type.ProtoQuadroItemList)
            {
                proto.selected = select;
            }
        }

        public static void SetSelectSpecificAllPrototypes(Type type, ResourceType resourceType, bool select)
        {
            switch (resourceType)
            {
                case ResourceType.GameObject:
                {
                    foreach (Prototype item in type.ProtoGameObjectList)
                    {
                        item.selected = select;
                    }
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    foreach (PrototypeTerrainDetail item in type.ProtoTerrainDetailList)
                    {
                        item.selected = select;
                    }
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    foreach (PrototypeTerrainTexture item in type.ProtoTerrainTextureList)
                    {
                        item.selected = select;
                    }
                    break;
                }
                case ResourceType.QuadroItem:
                {
                    foreach (PrototypeQuadroItem item in type.ProtoQuadroItemList)
                    {
                        item.selected = select;
                    }
                    break;
                }
            }
        }

        public static void InsertSelectedProtoQuadroItem(Type type, int index, bool after)
        {
            List<PrototypeQuadroItem> selectedProto = new List<PrototypeQuadroItem>();
            type.ProtoQuadroItemList.ForEach ((Action<PrototypeQuadroItem>)((proto) => { if(proto.selected) selectedProto.Add(proto); }));

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, type.ProtoQuadroItemList.Count);

                type.ProtoQuadroItemList.Insert(index, null); 
                type.ProtoQuadroItemList.RemoveAll (b => b != null && b.selected); 
                type.ProtoQuadroItemList.InsertRange(type.ProtoQuadroItemList.IndexOf(null), selectedProto); 
                type.ProtoQuadroItemList.RemoveAll ((b) => b == null); 
            }
        }

        public static void InsertSelectedProtoGameObject(Type type, int index, bool after)
        {
            List<PrototypeGameObject> selectedProto = new List<PrototypeGameObject>();
            type.ProtoGameObjectList.ForEach ((Action<PrototypeGameObject>)((proto) => { if(proto.selected) selectedProto.Add(proto); }));

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, type.ProtoGameObjectList.Count);

                type.ProtoGameObjectList.Insert(index, null); 
                type.ProtoGameObjectList.RemoveAll (b => b != null && b.selected); 
                type.ProtoGameObjectList.InsertRange(type.ProtoGameObjectList.IndexOf(null), selectedProto); 
                type.ProtoGameObjectList.RemoveAll ((b) => b == null); 
            }
        }

        public static void InsertSelectedProtoTerrainDetail(Type type, int index, bool after)
        {
            List<PrototypeTerrainDetail> selectedProto = new List<PrototypeTerrainDetail>();
            type.ProtoTerrainDetailList.ForEach ((Action<PrototypeTerrainDetail>)((proto) => { if(proto.selected) selectedProto.Add(proto); }));

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, type.ProtoTerrainDetailList.Count);

                type.ProtoTerrainDetailList.Insert(index, null); 
                type.ProtoTerrainDetailList.RemoveAll (b => b != null && b.selected); 
                type.ProtoTerrainDetailList.InsertRange(type.ProtoTerrainDetailList.IndexOf(null), selectedProto); 
                type.ProtoTerrainDetailList.RemoveAll ((b) => b == null);
            }
        }

        public static void InsertSelectedProtoTerrainTexture(Type type, int index, bool after)
        {
            List<PrototypeTerrainTexture> selectedProto = new List<PrototypeTerrainTexture>();
            type.ProtoTerrainTextureList.ForEach ((Action<PrototypeTerrainTexture>)((proto) => { if(proto.selected) selectedProto.Add(proto); }));

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, type.ProtoTerrainTextureList.Count);

                type.ProtoTerrainTextureList.Insert(index, null); 
                type.ProtoTerrainTextureList.RemoveAll (b => b != null && b.selected); 
                type.ProtoTerrainTextureList.InsertRange(type.ProtoTerrainTextureList.IndexOf(null), selectedProto); 
                type.ProtoTerrainTextureList.RemoveAll ((b) => b == null); 
            }
        }
    }
}
#endif