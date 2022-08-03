using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace QuadroRendererSystem
{
    [BurstCompile(CompileSynchronously = true)]
#if UNITY_2019_1_OR_NEWER
    public struct LoadPersistentStorageToMatrixWideJob : IJobParallelForDefer
#else
    public struct LoadPersistentStorageToMatrixWideJob : IJobParallelFor
#endif
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<InstanceData> InstanceList;
        public NativeArray<Matrix4x4> MatrixList;

        public void Execute(int index)
        {
            MatrixList[index] = Matrix4x4.TRS(InstanceList[index].Position, InstanceList[index].Rotation, InstanceList[index].Scale);
        }
    }
}