using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class ComputeBufferInfo
    {
        public ComputeBuffer ComputeBuffer;
        public bool Created;
    }

    public class RenderInstances
    {
        public readonly List<NativeList<Matrix4x4>> ItemMatrixList = new List<NativeList<Matrix4x4>>();       
        public readonly List<ComputeBufferInfo> ItemComputeBufferList = new List<ComputeBufferInfo>();

        public RenderInstances(int itemCount)
        {
            ItemMatrixList.Capacity = itemCount;
            for (int i = 0; i < itemCount; i++)
            {
                ItemMatrixList.Add(new NativeList<Matrix4x4>(Allocator.Persistent));
                ItemComputeBufferList.Add(new ComputeBufferInfo());
            }          
        }

        public void ClearInstanceMemory()
        {
            for (int i = 0; i < ItemMatrixList.Count; i++)
            {
                NativeList<Matrix4x4> itemMatrixList = this.ItemMatrixList[i];
                if (itemMatrixList.IsCreated)
                {
                    itemMatrixList.Clear();
                    itemMatrixList.Capacity = 0;
                }
            }
        }
               
        public void DisposeUnmanagedData()
        {         
            for (int i = 0; i < ItemMatrixList.Count; i++)
            {
                ItemMatrixList[i].Dispose();
            }

            ItemMatrixList.Clear(); 

            for (int i = 0; i <= ItemComputeBufferList.Count - 1; i++)
            {
                ComputeBufferInfo computeBufferInfo = ItemComputeBufferList[i];
                if (computeBufferInfo.Created)
                {
                    computeBufferInfo.ComputeBuffer.Dispose();
                    computeBufferInfo.Created = false;
                }
            }
        }
    }
}