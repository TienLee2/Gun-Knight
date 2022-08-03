using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    public partial class GPUInstancedIndirectRenderer 
    {
        public void DisposeComputeShaders()
        {
            MergeInstancedIndirectBuffersID.DummyComputeBuffer?.Dispose();
            GPUFrustumCullingID.DummyComputeBuffer?.Dispose();
        }

        public void UpdateGPUBuffer(QuadroRenderer quadroRenderer, List<Cell> instancedIndirectCellList, int totalInstanceCount, int protoIndex, int cameraIndex)
        {
            QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];
            RenderModelInfo renderModelInfo = quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[protoIndex];  

            QuadroRendererCamera quadroRendererCamera = quadroRenderer.QuadroRendererCamera[cameraIndex];   
            CameraComputeBuffers cameraComputeBuffers = renderModelInfo.CameraComputeBufferList[cameraIndex];

            GPUFrustumCullingID.SetFrustumCullingPlanes(quadroRendererCamera.Camera); 

            ClearRenderData(cameraComputeBuffers, renderModelInfo);

            if (instancedIndirectCellList.Count == 0 || totalInstanceCount == 0)
            {
                return;
            }

            if (totalInstanceCount > cameraComputeBuffers.MergeBuffer.count)
            {
                cameraComputeBuffers.UpdateComputeBufferSize(totalInstanceCount + 5000);
            }

            int threadGroupsFrustum = Mathf.CeilToInt(totalInstanceCount / GPUFrustumCullingID.Numthreads);

            if (threadGroupsFrustum == 0) return;

            MergeInstancedIndirectBuffersID.MergeBuffer(instancedIndirectCellList, cameraComputeBuffers, protoIndex);

            GPUFrustumCullingID.DispatchQuadroRendererGPUFrustumCulling(quadroRenderer, proto, renderModelInfo, quadroRendererCamera, cameraIndex, totalInstanceCount, threadGroupsFrustum);

            for (int LODIndex = 0; LODIndex < renderModelInfo.LODs.Count; LODIndex++)
            {
                for (int n = 0; n < renderModelInfo.LODs[LODIndex].Mesh.subMeshCount; n++)
                {
                    ComputeBuffer.CopyCount(cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionBuffer, cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].ArgsBufferMergedLODList[n], sizeof(uint) * 1);
                    ComputeBuffer.CopyCount(cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionShadowBuffer, cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].ShadowArgsBufferMergedLODList[n], sizeof(uint) * 1);
                }
            }
        }

        public void ClearRenderData(CameraComputeBuffers cameraComputeBuffers, RenderModelInfo renderModelInfo)
        {
            for (int LODIndex = 0; LODIndex < renderModelInfo.LODs.Count; LODIndex++)
            {
                cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionBuffer.SetCounterValue(0); 
                cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionShadowBuffer.SetCounterValue(0); 

                for (int n = 0; n < renderModelInfo.LODs[LODIndex].Mesh.subMeshCount; n++)
                {
                    ComputeBuffer.CopyCount(cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionBuffer, cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].ArgsBufferMergedLODList[n], sizeof(uint) * 1);
                    ComputeBuffer.CopyCount(cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].PositionShadowBuffer, cameraComputeBuffers.GPUInstancedIndirectLODList[LODIndex].ShadowArgsBufferMergedLODList[n], sizeof(uint) * 1);
                }
            }
        } 

        public void Render(QuadroRenderer quadroRenderer, int protoIndex, int cameraIndex, Camera targetCamera)
        {
            QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];
            QuadroRendererCamera quadroRendererCamera = quadroRenderer.QuadroRendererCamera[cameraIndex];
            RenderModelInfo renderModelInfo = quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[protoIndex];       
            
            ShadowCastingMode shadowCastingMode = quadroRenderer.QualitySettings.GetShadowCastingMode();

            float maxDistance = Utility.GetMaxDistance(proto, quadroRendererCamera, quadroRenderer);

            float boundsDistance = maxDistance * 2 + renderModelInfo.BoundingSphereRadius;
            Bounds cellBounds = new Bounds(quadroRendererCamera.Camera.transform.position, new Vector3(boundsDistance, boundsDistance, boundsDistance));

            if (proto.ShadowsSettings.IsShadowCasting == false)
            {
                shadowCastingMode = ShadowCastingMode.Off;
            }
            
            LayerMask layer = proto.PrefabObject.layer;

            LOD lod;
            Material material;
            int submeshIndex = 0;

            for (int LODIndex = 0; LODIndex < renderModelInfo.LODs.Count; LODIndex++)
            {
                lod = renderModelInfo.LODs[LODIndex];

                lod.Mpb.Clear();
                ComputeBuffer visibleBuffer = renderModelInfo.GetLODVisibleBuffer(LODIndex, cameraIndex, false);
                lod.Mpb.SetBuffer(GPUFrustumCullingID.VisibleShaderDataBufferID, visibleBuffer);

                if (shadowCastingMode == ShadowCastingMode.On && proto.ShadowsSettings.DrawShadowLODMap[LODIndex] == true)
                {
                    lod.ShadowMPB.Clear();
                    ComputeBuffer shadowVisibleBuffer = renderModelInfo.GetLODVisibleBuffer(proto.ShadowsSettings.ShadowLODMap[LODIndex], cameraIndex, true);
                    lod.ShadowMPB.SetBuffer(GPUFrustumCullingID.VisibleShaderDataBufferID, shadowVisibleBuffer);
                }

                for (int m = 0; m < lod.Materials.Count; m++)
                {
                    material = lod.Materials[m];

                    submeshIndex = Math.Min(m, lod.Mesh.subMeshCount - 1);

                    List<ComputeBuffer> argsBufferList = renderModelInfo.GetLODArgsBufferList(cameraIndex, LODIndex, false);

                    Graphics.DrawMeshInstancedIndirect(lod.Mesh, submeshIndex,
                        material,
                        cellBounds,
                        argsBufferList[submeshIndex],
                        0,
                        lod.Mpb,
                        ShadowCastingMode.Off, true, layer,
                        targetCamera
#if UNITY_2018_1_OR_NEWER
                        , proto.GeneralSettings.LightProbeUsage
#endif
                        );

                    if (shadowCastingMode == ShadowCastingMode.On && proto.ShadowsSettings.DrawShadowLODMap[LODIndex] == true)
                    {
                        List<ComputeBuffer> shadowArgsBufferList = renderModelInfo.GetLODArgsBufferList(cameraIndex, LODIndex, true);

                        bool shadowWithOriginalShader = proto.ShadowsSettings.ShadowWithOriginalShaderLODMap[LODIndex];

                        Graphics.DrawMeshInstancedIndirect(lod.Mesh, submeshIndex,
                            shadowWithOriginalShader ? material : renderModelInfo.MaterialShadowsOnly,
                            cellBounds,
                            shadowArgsBufferList[submeshIndex],
                            0,
                            lod.ShadowMPB,
                            ShadowCastingMode.ShadowsOnly, true, layer,
                            targetCamera
#if UNITY_2018_1_OR_NEWER
                            , proto.GeneralSettings.LightProbeUsage
#endif
                        );
                    }
                }
            }
        }
    }
}