using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public static class MergeInstancedIndirectBuffersID
    {
        public struct BatchAdd
        {
            public ComputeBuffer Instances;
            public int Count;
        }

        public static ComputeBuffer DummyComputeBuffer;
        public static ComputeShader MergeBufferShader;

        public static int[] Count = new int[32];
        public static int[] Instances = new int[32];

        public static int[] AddInstances;
        public static int[] MaxNumberCellsKernel;
        public static int[] Threads;
        
        public static int AddInstances1;
        public static int AddInstances2;
        public static int AddInstances4;
        public static int AddInstances8;
        public static int AddInstances16;
        public static int AddInstances32;

        public static void Setup()
        {
            DummyComputeBuffer = new ComputeBuffer(1, (16 * 4) + 16, ComputeBufferType.Default);

            MergeBufferShader = (ComputeShader)Resources.Load("QuadroRendererMergeInstancedIndirectBuffers");

            for (int index = 0; index < Count.Length; ++index)
            {
                Count[index] = Shader.PropertyToID("Count" + index.ToString());
            }
            
            for (int index = 0; index < Instances.Length; ++index)
            {
                Instances[index] = Shader.PropertyToID("Instances" + index.ToString());
            }

            MaxNumberCellsKernel = new int[6]
            {
                1,
                2, 
                4,
                8,
                16,
                32
            };

            Threads = new int[6]
            {
                64,
                64,
                64,
                64,
                64,
                128
            };

            AddInstances1 = MergeBufferShader.FindKernel("AddInstances1");
            AddInstances2 = MergeBufferShader.FindKernel("AddInstances2");
            AddInstances4 = MergeBufferShader.FindKernel("AddInstances4");
            AddInstances8 = MergeBufferShader.FindKernel("AddInstances8");
            AddInstances16 = MergeBufferShader.FindKernel("AddInstances16");
            AddInstances32 = MergeBufferShader.FindKernel("AddInstances32");
            AddInstances = new int[32]
            {
                AddInstances1,
                AddInstances2,
                AddInstances4,
                AddInstances4,
                AddInstances8,
                AddInstances8,
                AddInstances8,
                AddInstances8,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances16,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32,
                AddInstances32
            };
        }

        public static void MergeBuffer(List<Cell> instancedIndirectCellList, CameraComputeBuffers cameraComputeBuffers, int itemIndex)
        {
            cameraComputeBuffers.MergeBuffer.SetCounterValue(0);

            List<BatchAdd> batchAddList = new List<BatchAdd>();
            for (int cellIndex = 0; cellIndex < instancedIndirectCellList.Count; cellIndex++)
            {
                Cell cell = instancedIndirectCellList[cellIndex];

                int instanceCount = instancedIndirectCellList[cellIndex].RenderInstancesList.ItemMatrixList[itemIndex].Length;

                BatchAdd batch = new BatchAdd()
                {
                    Instances = cell.RenderInstancesList.ItemComputeBufferList[itemIndex].ComputeBuffer,
                    Count = instanceCount,
                };

                batchAddList.Add(batch);

                if(instancedIndirectCellList.Count < 32)
                {
                    if(instancedIndirectCellList.Count - 1 == cellIndex)
                    {
                        MergeBuffer(batchAddList, cameraComputeBuffers);
                        batchAddList.Clear();
                    }
                }
                else
                {
                    if(batchAddList.Count == 32)
                    {
                        MergeBuffer(batchAddList, cameraComputeBuffers);
                        batchAddList.Clear();
                    }
                }
            }

            if(batchAddList.Count != 0)
            {
                MergeBuffer(batchAddList, cameraComputeBuffers);
            }
        }

        public static void MergeBuffer(List<BatchAdd> batches, CameraComputeBuffers cameraComputeBuffers)
        {
            int addInstance = AddInstances[batches.Count - 1];
            int instanceCount = 0;
            for (int index = 0; index < batches.Count; index++)
            {
                instanceCount = Mathf.Max(instanceCount, batches[index].Count);
                MergeBufferShader.SetInt(Count[index], batches[index].Count);
                MergeBufferShader.SetBuffer(addInstance, Instances[index], batches[index].Instances);
            }
            
            for (int length = batches.Count; length < MaxNumberCellsKernel[addInstance]; length++)
            {
                MergeBufferShader.SetInt(Count[length], 0);
                MergeBufferShader.SetBuffer(addInstance, Instances[length], DummyComputeBuffer);
            }

            int threadGroups = Mathf.CeilToInt((float)instanceCount / Threads[addInstance]);
            if (threadGroups == 0) 
            {
                return;
            }

            MergeBufferShader.SetBuffer(addInstance, GPUFrustumCullingID.MergeBufferID, cameraComputeBuffers.MergeBuffer);

            MergeBufferShader.Dispatch(addInstance, threadGroups, 1, 1);
        }
    }
}