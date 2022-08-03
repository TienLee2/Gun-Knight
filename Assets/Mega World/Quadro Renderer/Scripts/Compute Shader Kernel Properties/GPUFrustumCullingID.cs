using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public static class GPUFrustumCullingID 
    {
        public struct BatchAdd
        {
            public ComputeBuffer VisibleBuffer;
            public ComputeBuffer ShadowBuffer;
        }

        public static ComputeBuffer DummyComputeBuffer;
        public static int FrustumKernelHandle;
        public static ComputeShader FrustumCulling;
        public static int CameraFrustumPlan0;
        public static int CameraFrustumPlan1;
        public static int CameraFrustumPlan2;
        public static int CameraFrustumPlan3;
        public static int CameraFrustumPlan4;
        public static int CameraFrustumPlan5;

        public static int FloatingOriginOffsetID = -1;
        public static int worldSpaceCameraPosID = -1;
        public static int MergeBufferID = -1;

        public static int[] PositionLod = new int[] {-1, -1, -1, -1};
        public static int[] ShadowPositionLod = new int[] {-1, -1, -1, -1};

        public static int PositionsID = -1;
        public static int InstanceCountID = -1;

        public static int BoundingSphereRadiusID = -1;
        public static int IsFrustumCullingID = -1;
 
        public static int VisibleShaderDataBufferID;
               
        public static int LodFadeDistanceID = -1;
        public static int LodFadeForLastLODID = -1;

        public static int UseLODFadeID = -1;
        public static int GetAdditionalShadowID = -1;
        public static int ShadowDistanceID = -1;
        public static int MinCullingDistanceID = -1;
        public static int IncreaseBoundingSphereForShadowsID = -1;

        public static int LodDistancesID = -1;
        public static int LodCountID = -1;
        public static int StartLOD = -1;
        public static int MaxDistanceID = -1;
        public static int MinDistanceID = -1;

        public static int DirectionLightID = -1;
        public static int BoundsSizeID = -1;

        public static float Numthreads = 512f;

        public static void Setup()
        {
            DummyComputeBuffer = new ComputeBuffer(1, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            DummyComputeBuffer.SetCounterValue(0);

            FrustumCulling = (ComputeShader)Resources.Load("QuadroRendererGPUFrustumCullingLOD");
            FrustumKernelHandle = FrustumCulling.FindKernel("QuadroRendererGPUFrustumCullingLOD"); 
            MergeBufferID = Shader.PropertyToID("MergeBuffer");

            FloatingOriginOffsetID = Shader.PropertyToID("floatingOriginOffset");

            worldSpaceCameraPosID = Shader.PropertyToID("worldSpaceCameraPos");

            CameraFrustumPlan0 = Shader.PropertyToID("cameraFrustumPlane0");
            CameraFrustumPlan1 = Shader.PropertyToID("cameraFrustumPlane1");
            CameraFrustumPlan2 = Shader.PropertyToID("cameraFrustumPlane2");
            CameraFrustumPlan3 = Shader.PropertyToID("cameraFrustumPlane3");
            CameraFrustumPlan4 = Shader.PropertyToID("cameraFrustumPlane4");
            CameraFrustumPlan5 = Shader.PropertyToID("cameraFrustumPlane5");

            InstanceCountID = Shader.PropertyToID("instanceCount");
            PositionsID = Shader.PropertyToID("positions");
            PositionLod[0] = Shader.PropertyToID("positionLOD0");
            PositionLod[1] = Shader.PropertyToID("positionLOD1");
            PositionLod[2] = Shader.PropertyToID("positionLOD2");
            PositionLod[3] = Shader.PropertyToID("positionLOD3");
            
            ShadowPositionLod[0] = Shader.PropertyToID("positionShadowLOD0");
            ShadowPositionLod[1] = Shader.PropertyToID("positionShadowLOD1");
            ShadowPositionLod[2] = Shader.PropertyToID("positionShadowLOD2");
            ShadowPositionLod[3] = Shader.PropertyToID("positionShadowLOD3");
                        
            VisibleShaderDataBufferID = Shader.PropertyToID("VisibleShaderDataBufferQuadroRenderer");

            IsFrustumCullingID = Shader.PropertyToID("isFrustumCulling");           
            
            BoundingSphereRadiusID = Shader.PropertyToID("boundingSphereRadius");

            UseLODFadeID = Shader.PropertyToID("useLODFade");
            GetAdditionalShadowID = Shader.PropertyToID("getAdditionalShadow");
            ShadowDistanceID = Shader.PropertyToID("shadowDistance");
            MinCullingDistanceID = Shader.PropertyToID("minCullingDistance");
            IncreaseBoundingSphereForShadowsID = Shader.PropertyToID("increaseBoundingSphereForShadows");

            LodFadeDistanceID = Shader.PropertyToID("LODFadeDistance");
            LodFadeForLastLODID = Shader.PropertyToID("lodFadeForLastLOD");

            LodDistancesID = Shader.PropertyToID("lodDistances");
            LodCountID = Shader.PropertyToID("LODCount");
            StartLOD = Shader.PropertyToID("startLOD");
            MaxDistanceID = Shader.PropertyToID("maxDistance");
            MinDistanceID = Shader.PropertyToID("minDistance");

            DirectionLightID = Shader.PropertyToID("directionLight");
            BoundsSizeID = Shader.PropertyToID("boundsSize");
        }

        public static void SetFrustumCullingPlanes(Camera selectedCamera)
        {
            GeometryUtilityAllocFree.CalculateFrustrumPlanes(selectedCamera);

            Vector4 cameraFrustumPlane0 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[0].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[0].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[0].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[0].distance);
            Vector4 cameraFrustumPlane1 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[1].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[1].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[1].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[1].distance);
            Vector4 cameraFrustumPlane2 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[2].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[2].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[2].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[2].distance);
            Vector4 cameraFrustumPlane3 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[3].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[3].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[3].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[3].distance);
            Vector4 cameraFrustumPlane4 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[4].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[4].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[4].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[4].distance);
            Vector4 cameraFrustumPlane5 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[5].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[5].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[5].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[5].distance);

            FrustumCulling.SetVector(CameraFrustumPlan0, cameraFrustumPlane0);
            FrustumCulling.SetVector(CameraFrustumPlan1, cameraFrustumPlane1);
            FrustumCulling.SetVector(CameraFrustumPlan2, cameraFrustumPlane2);
            FrustumCulling.SetVector(CameraFrustumPlan3, cameraFrustumPlane3);
            FrustumCulling.SetVector(CameraFrustumPlan4, cameraFrustumPlane4); 
            FrustumCulling.SetVector(CameraFrustumPlan5, cameraFrustumPlane5);

            Vector3 worldSpaceCameraPosition = selectedCamera.transform.position;
            Vector4 worldSpaceCameraPos = new Vector4(worldSpaceCameraPosition.x, worldSpaceCameraPosition.y, worldSpaceCameraPosition.z, 1);
            FrustumCulling.SetVector(worldSpaceCameraPosID, worldSpaceCameraPos);
        }

        public static void SetComputeShaderGeneralParams(QuadroRenderer quadroRenderer, QuadroPrototype proto, RenderModelInfo renderModelInfo, QuadroRendererCamera quadroRendererCamera, int cameraIndex, int totalInstanceCount, float lodBias)
        {
            CameraComputeBuffers cameraComputeBuffers = renderModelInfo.CameraComputeBufferList[cameraIndex];

            Vector3 directionLight = quadroRenderer.QualitySettings.DirectionalLight ? quadroRenderer.QualitySettings.DirectionalLight.transform.forward : new Vector3(0, 0, 0);

            float maxDistance = Utility.GetMaxDistance(proto, quadroRendererCamera, quadroRenderer);

            Vector4 floatingOriginOffsetVector4 = new Vector4(quadroRenderer.FloatingOriginOffset.x, quadroRenderer.FloatingOriginOffset.y, quadroRenderer.FloatingOriginOffset.z, 0);

            FrustumCulling.SetVector(FloatingOriginOffsetID, floatingOriginOffsetVector4);

            FrustumCulling.SetBuffer(FrustumKernelHandle, PositionsID, cameraComputeBuffers.MergeBuffer);

            FrustumCulling.SetInt(InstanceCountID, totalInstanceCount);
            FrustumCulling.SetBool(IsFrustumCullingID, quadroRendererCamera.CameraCullingMode == CameraCullingMode.FrustumCulling ? proto.CullingSettings.IsFrustumCulling : false);
    
            FrustumCulling.SetFloat(BoundingSphereRadiusID, renderModelInfo.BoundingSphereRadius + proto.CullingSettings.IncreaseBoundingSphere);
        
            FrustumCulling.SetBool(UseLODFadeID, proto.LODSettings.IsLODCrossFade);
        
            FrustumCulling.SetInt(GetAdditionalShadowID, (int)proto.CullingSettings.GetAdditionalShadow);

            FrustumCulling.SetFloat(ShadowDistanceID, proto.ShadowsSettings.GetShadowDistanceForRendering());
            FrustumCulling.SetFloat(MinCullingDistanceID, proto.CullingSettings.MinCullingDistance);
            FrustumCulling.SetFloat(IncreaseBoundingSphereForShadowsID, proto.CullingSettings.IncreaseShadowsBoundingSphere);
        
            FrustumCulling.SetFloat(LodFadeDistanceID, proto.LODSettings.LodFadeTransitionDistance);
            FrustumCulling.SetBool(LodFadeForLastLODID, proto.LODSettings.LodFadeForLastLOD);

            float[] lodDistances = Utility.GetLODDistances(renderModelInfo, lodBias, maxDistance);

            FrustumCulling.SetFloats(LodDistancesID, lodDistances);
            FrustumCulling.SetInt(LodCountID, renderModelInfo.LODs.Count);
            FrustumCulling.SetFloat(MaxDistanceID, maxDistance);

            FrustumCulling.SetVector(DirectionLightID, directionLight);
            FrustumCulling.SetVector(BoundsSizeID, proto.Bounds.size);
        }

        public static void DispatchQuadroRendererGPUFrustumCulling(QuadroRenderer quadroRenderer, QuadroPrototype proto, RenderModelInfo renderModelInfo, QuadroRendererCamera camera, int cameraIndex, int totalInstanceCount, int threadGroups)
        {
            float lodBias = UnityEngine.QualitySettings.lodBias * quadroRenderer.QualitySettings.LodBias * proto.LODSettings.LodBias * camera.LodBias;

            SetComputeShaderGeneralParams(quadroRenderer, proto, renderModelInfo, camera, cameraIndex, totalInstanceCount, lodBias);

            CameraComputeBuffers cameraComputeBuffers = renderModelInfo.CameraComputeBufferList[cameraIndex];

            List<BatchAdd> batches = new List<BatchAdd>();
            int countDispatch = 0;

            for (int lodIndex = 0; lodIndex < renderModelInfo.LODs.Count; lodIndex++)
            {
                BatchAdd batch = new BatchAdd()
                {
                    VisibleBuffer = cameraComputeBuffers.GPUInstancedIndirectLODList[lodIndex].PositionBuffer,
                    ShadowBuffer = cameraComputeBuffers.GPUInstancedIndirectLODList[lodIndex].PositionShadowBuffer,
                };

                batches.Add(batch);

                if(renderModelInfo.LODs.Count < 4)
                {
                    if(renderModelInfo.LODs.Count - 1 == lodIndex)
                    {
                        Dispatch(batches, proto, renderModelInfo, cameraComputeBuffers, lodBias, threadGroups, countDispatch);
                        batches.Clear();
                        countDispatch++;
                    }
                }
                else if(batches.Count == 4)
                {
                    Dispatch(batches, proto, renderModelInfo, cameraComputeBuffers, lodBias, threadGroups, countDispatch);
                    batches.Clear();
                    countDispatch++;
                }
            }

            if(batches.Count != 0)
            {
                Dispatch(batches, proto, renderModelInfo, cameraComputeBuffers, lodBias, threadGroups, countDispatch);
                batches.Clear();
                countDispatch++;
            }
        }

        public static void Dispatch(List<BatchAdd> batches, QuadroPrototype proto, RenderModelInfo renderModelInfo, CameraComputeBuffers cameraComputeBuffers, float lodBias, int threadGroups,  int countDispatch)
        {
            for (int lodIndex = 0; lodIndex < batches.Count; lodIndex++)
            {
                FrustumCulling.SetBuffer(FrustumKernelHandle, PositionLod[lodIndex], cameraComputeBuffers.GPUInstancedIndirectLODList[countDispatch == 0 ? lodIndex : lodIndex + 4].PositionBuffer);
                FrustumCulling.SetBuffer(FrustumKernelHandle, ShadowPositionLod[lodIndex], cameraComputeBuffers.GPUInstancedIndirectLODList[countDispatch == 0 ? lodIndex : lodIndex + 4].PositionShadowBuffer);
            }
            
            for (int i = batches.Count; i < 4; i++)
            {
                FrustumCulling.SetBuffer(FrustumKernelHandle, PositionLod[i], DummyComputeBuffer);
                FrustumCulling.SetBuffer(FrustumKernelHandle, ShadowPositionLod[i], DummyComputeBuffer);
            }

            float minDistance = 0;

            if(countDispatch > 0)
            {
                if(proto.LODSettings.IsLODCrossFade)
                {
                    minDistance = renderModelInfo.LODs[4].Distance * lodBias - proto.LODSettings.LodFadeTransitionDistance;
                }
                else
                {
                    minDistance = renderModelInfo.LODs[4].Distance * lodBias;
                }
            }

            FrustumCulling.SetInt(StartLOD, countDispatch == 0 ? 0 : 4);
            FrustumCulling.SetFloat(MinDistanceID, minDistance);

            FrustumCulling.Dispatch(FrustumKernelHandle, threadGroups, 1, 1);
        }
    }
}