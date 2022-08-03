using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class GPUInstancedIndirectLOD
    {
        public ComputeBuffer PositionBuffer;
        public ComputeBuffer PositionShadowBuffer;

        public List<ComputeBuffer> ArgsBufferMergedLODList = new List<ComputeBuffer>();
        public List<ComputeBuffer> ShadowArgsBufferMergedLODList = new List<ComputeBuffer>();
    }

    public class CameraComputeBuffers
    {
        public ComputeBuffer MergeBuffer;

        private readonly uint[] _args = { 0, 0, 0, 0, 0 };

        public List<GPUInstancedIndirectLOD> GPUInstancedIndirectLODList = new List<GPUInstancedIndirectLOD>();

        public CameraComputeBuffers(List<LOD> LODs)
        {
            MergeBuffer = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            MergeBuffer.SetCounterValue(0);

            for (int i = 0; i < LODs.Count; i++)
            {
                GPUInstancedIndirectLOD LOD = new GPUInstancedIndirectLOD();
                LOD.PositionBuffer = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
                LOD.PositionBuffer.SetCounterValue(0);

                LOD.PositionShadowBuffer = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
                LOD.PositionShadowBuffer.SetCounterValue(0);

                GPUInstancedIndirectLODList.Add(LOD);

                SetSubmesh(LODs[i].Mesh, i);
            }
        }

        public void SetSubmesh(Mesh mesh, int LODIndex)
        {
            for (int i = 0; i <= mesh.subMeshCount - 1; i++)
            {
                _args[0] = mesh.GetIndexCount(i);
                _args[2] = mesh.GetIndexStart(i);
                
                ComputeBuffer argsBufferMergedLOD = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);                             
                argsBufferMergedLOD.SetData(_args);
                GPUInstancedIndirectLODList[LODIndex].ArgsBufferMergedLODList.Add(argsBufferMergedLOD);
                
                ComputeBuffer shadowArgsBufferMergedLOD = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);                             
                shadowArgsBufferMergedLOD.SetData(_args);
                GPUInstancedIndirectLODList[LODIndex].ShadowArgsBufferMergedLODList.Add(shadowArgsBufferMergedLOD);
            }
        }

        public void UpdateComputeBufferSize(int newInstanceCount)
        {
            if(newInstanceCount == 0)
            {
                newInstanceCount = 1;
            }
            
            MergeBuffer?.Release();
            MergeBuffer = null;

            for (int i = 0; i < GPUInstancedIndirectLODList.Count; i++)
            {
                GPUInstancedIndirectLODList[i].PositionBuffer?.Release();
                GPUInstancedIndirectLODList[i].PositionBuffer = null;

                GPUInstancedIndirectLODList[i].PositionShadowBuffer?.Release();
                GPUInstancedIndirectLODList[i].PositionShadowBuffer = null;
            }

            MergeBuffer = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            MergeBuffer.SetCounterValue(0);

            for (int i = 0; i < GPUInstancedIndirectLODList.Count; i++)
            {
                GPUInstancedIndirectLODList[i].PositionBuffer = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
                GPUInstancedIndirectLODList[i].PositionBuffer.SetCounterValue(0);

                GPUInstancedIndirectLODList[i].PositionShadowBuffer = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
                GPUInstancedIndirectLODList[i].PositionShadowBuffer.SetCounterValue(0);
            }
        }

        public void DestroyComputeBuffers()
        {
            MergeBuffer?.Release();
            MergeBuffer = null;

            for (int i = 0; i < GPUInstancedIndirectLODList.Count; i++)
            {
                GPUInstancedIndirectLODList[i].PositionBuffer?.Release();
                GPUInstancedIndirectLODList[i].PositionBuffer = null;

                GPUInstancedIndirectLODList[i].PositionShadowBuffer?.Release();
                GPUInstancedIndirectLODList[i].PositionShadowBuffer = null;
            }

            ReleaseArgsBuffers();
        }

        void ReleaseArgsBuffers()
        {
            foreach (GPUInstancedIndirectLOD lod in GPUInstancedIndirectLODList)
            {
                foreach (ComputeBuffer argsBufferMergedLOD in lod.ArgsBufferMergedLODList)
                {
                    argsBufferMergedLOD?.Release();
                }

                foreach (ComputeBuffer argsBufferMergedLOD in lod.ShadowArgsBufferMergedLODList)
                {
                    argsBufferMergedLOD?.Release();
                }

                lod.ArgsBufferMergedLODList.Clear();
                lod.ShadowArgsBufferMergedLODList.Clear();
            }
        }
    }
}