using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace QuadroRendererSystem
{
    [BurstCompile(CompileSynchronously = true)]
    public struct MergeCellInstancesJob : IJob
    {
        public NativeList<Matrix4x4> OutputNativeList;
        [ReadOnly]
        public NativeList<Matrix4x4> InputNativeList;

        public void Execute()
        {
            for (int l = 0; l <= InputNativeList.Length - 1; l++)
            {
                OutputNativeList.Add(InputNativeList[l]);
            }
        }
    }    
}