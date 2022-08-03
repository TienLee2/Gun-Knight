using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace QuadroRendererSystem
{
    [Serializable]
    public struct InstanceData
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;
    }

    public class ObjectInstanceData
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public ObjectInstanceData(InstanceData instanceData)
        {
            Position = instanceData.Position;
            Scale = instanceData.Scale;
            Rotation = instanceData.Rotation;
        }
    }

    [Serializable]
    public class ItemInfo
    {
        public int ID;
        
        [SerializeField]
        public List<InstanceData> InstanceDataList = new List<InstanceData>();
        
        [NonSerialized] 
        public NativeArray<InstanceData> NativeInstanceDataArray;

        [NonSerialized]
        public List<ObjectInstanceData> ObjectInstanceData = new List<ObjectInstanceData>();

        public void CopyToNativeArray()
        {           
            NativeInstanceDataArray = new NativeArray<InstanceData>(InstanceDataList.Count, Allocator.Persistent);
            NativeInstanceDataArray.CopyFromFast(InstanceDataList);
        }

        public void AddPersistentItemInstance(ref InstanceData persistentItem)
        {
            InstanceDataList.Add(persistentItem);
        }

        public void DisposeUnmanagedData()
        {
            if (NativeInstanceDataArray.IsCreated)
            {
                NativeInstanceDataArray.Dispose();
            }
        }
    }
}