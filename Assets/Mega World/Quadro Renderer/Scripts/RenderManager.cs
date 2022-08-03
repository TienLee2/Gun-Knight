using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;

namespace QuadroRendererSystem
{
    public class RenderManager 
    {
        public GPUInstancedRenderer GPUInstancedRenderer = new GPUInstancedRenderer();
        public GPUInstancedIndirectRenderer GPUInstancedIndirectRenderer = new GPUInstancedIndirectRenderer();

        public void Render(QuadroRenderer quadroRenderer)
        {
            if(quadroRenderer.enabled == false)
            {
                return;
            }

            for (int cameraIndex = 0; cameraIndex <= quadroRenderer.QuadroRendererCamera.Count - 1; cameraIndex++)
            {            
                if(quadroRenderer.QuadroRendererCamera[cameraIndex].Camera == null)
                {
                    continue;
                }

                if (!quadroRenderer.IsCameraActive(quadroRenderer.QuadroRendererCamera[cameraIndex])) 
                {
                    continue;
                }   

                Camera targetCamera = quadroRenderer.QuadroRendererCamera[cameraIndex].GetRenderingCamera(quadroRenderer);

                for (int protoIndex = 0; protoIndex <= quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count - 1; protoIndex++)
                {
                    QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];

                    if(proto.GeneralSettings.ActiveRenderer == false)
                    {
                        continue;
                    }

                    if (Utility.IsInLayer(quadroRenderer.QuadroRendererCamera[cameraIndex].Camera.cullingMask, proto.PrefabObject.layer) == false)
                    {
                        continue;
                    }

                    switch (proto.GeneralSettings.RenderMode)
                    {
                        case RenderMode.GPUInstanced:
                        {
                            GPUInstancedRenderer.Render(quadroRenderer, protoIndex, quadroRenderer.QuadroRendererCamera[cameraIndex], targetCamera);
                            break;
                        }
                        case RenderMode.GPUInstancedIndirect:
                        {
                            GPUInstancedIndirectRenderer.Render(quadroRenderer, protoIndex, cameraIndex, targetCamera);
                            break;
                        }
                    }
                }
            }
        }

        public void UpdateRendererData(QuadroRenderer quadroRenderer, QuadroRendererCamera quadroRendererCamera, int totalInstanceCount, int protoIndex, List<Cell> cellList)
        {
            quadroRendererCamera.CameraRender.ClearRenderData(protoIndex);

            if(totalInstanceCount == 0)
            {
                return;
            }

            QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];

            NativeList<Matrix4x4> itemMatrixList = quadroRendererCamera.CameraRender.ItemMergeMatrixList[protoIndex];

            if(proto.GeneralSettings.ActiveRenderer == false)
            {
                return;
            }

            if (Utility.IsInLayer(quadroRendererCamera.Camera.cullingMask, proto.PrefabObject.layer) == false)
            {
                return;
            }

            RenderModelInfo renderModelInfo = quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[protoIndex];

            NativeArray<Plane> frustumPlanes = new NativeArray<Plane>(6, Allocator.Persistent);

            Plane[] FrustumPlaneArray = new Plane[6];
    
            GeometryUtility.CalculateFrustumPlanes(quadroRendererCamera.Camera, FrustumPlaneArray);
    
            frustumPlanes.CopyFromFast(FrustumPlaneArray);

            for (int index = 0; index < cellList.Count; index++)
            {
                Cell cell = cellList[index];
                    
                MergeCellInstancesJob mergeCellInstancesJob =
                    new MergeCellInstancesJob
                    {
                        OutputNativeList = itemMatrixList,
                        InputNativeList = cell.RenderInstancesList.ItemMatrixList[protoIndex]
                    };
                    
                mergeCellInstancesJob.Schedule().Complete(); 
            }

            CameraRenderData cameraRender = quadroRendererCamera.CameraRender;

            float maxDistance = Utility.GetMaxDistance(proto, quadroRendererCamera, quadroRenderer);
    
            ShadowCastingMode shadowCastingMode = quadroRenderer.QualitySettings.GetShadowCastingMode();
    
            if (proto.ShadowsSettings.IsShadowCasting == false)
            {
                shadowCastingMode = ShadowCastingMode.Off;
            }

            Vector3 directionLight = quadroRenderer.QualitySettings.DirectionalLight ? quadroRenderer.QualitySettings.DirectionalLight.transform.forward : new Vector3(0, 0, 0);
            
            float lodBias = UnityEngine.QualitySettings.lodBias * quadroRenderer.QualitySettings.LodBias * proto.LODSettings.LodBias * quadroRendererCamera.LodBias;

            NativeArray<float> lodDistances = new NativeArray<float>(16, Allocator.Persistent);

            lodDistances.CopyFromFast(Utility.GetLODDistances(renderModelInfo, lodBias, maxDistance, false));
            
            FrustumCullingLODJob lodJob =
                new FrustumCullingLODJob
                {
                    BoundingSphereRadius = renderModelInfo.BoundingSphereRadius + proto.CullingSettings.IncreaseBoundingSphere,
                    MatrixList = itemMatrixList,
                    ItemLOD0MatrixList = cameraRender.ItemLODMatrixList[0].ItemMatrixList[protoIndex],
                    ItemLOD1MatrixList = cameraRender.ItemLODMatrixList[1].ItemMatrixList[protoIndex],
                    ItemLOD2MatrixList = cameraRender.ItemLODMatrixList[2].ItemMatrixList[protoIndex],
                    ItemLOD3MatrixList = cameraRender.ItemLODMatrixList[3].ItemMatrixList[protoIndex],
                    ItemLOD4MatrixList = cameraRender.ItemLODMatrixList[4].ItemMatrixList[protoIndex],
                    ItemLOD5MatrixList = cameraRender.ItemLODMatrixList[5].ItemMatrixList[protoIndex],
                    ItemLOD6MatrixList = cameraRender.ItemLODMatrixList[6].ItemMatrixList[protoIndex],
                    ItemLOD7MatrixList = cameraRender.ItemLODMatrixList[7].ItemMatrixList[protoIndex],
                    ItemLOD0ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[0].ItemMatrixList[protoIndex],
                    ItemLOD1ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[1].ItemMatrixList[protoIndex],
                    ItemLOD2ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[2].ItemMatrixList[protoIndex],
                    ItemLOD3ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[3].ItemMatrixList[protoIndex],
                    ItemLOD4ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[4].ItemMatrixList[protoIndex],
                    ItemLOD5ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[5].ItemMatrixList[protoIndex],
                    ItemLOD6ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[6].ItemMatrixList[protoIndex],
                    ItemLOD7ShadowMatrixList = cameraRender.ItemLODShadowMatrixList[7].ItemMatrixList[protoIndex],
                    LOD0FadeList = cameraRender.ItemLODFadeList[0].ItemLodFadeList[protoIndex],
                    LOD1FadeList = cameraRender.ItemLODFadeList[1].ItemLodFadeList[protoIndex],
                    LOD2FadeList = cameraRender.ItemLODFadeList[2].ItemLodFadeList[protoIndex],
                    LOD3FadeList = cameraRender.ItemLODFadeList[3].ItemLodFadeList[protoIndex],
                    LOD4FadeList = cameraRender.ItemLODFadeList[4].ItemLodFadeList[protoIndex],
                    LOD5FadeList = cameraRender.ItemLODFadeList[5].ItemLodFadeList[protoIndex],
                    LOD6FadeList = cameraRender.ItemLODFadeList[6].ItemLodFadeList[protoIndex],
                    LOD7FadeList = cameraRender.ItemLODFadeList[7].ItemLodFadeList[protoIndex],
                    ShadowDistance = proto.ShadowsSettings.GetShadowDistanceForRendering(),
                    GetAdditionalShadow = proto.CullingSettings.GetAdditionalShadow,
                    MinCullingDistance = proto.CullingSettings.MinCullingDistance,
                    IncreaseBoundingSphereForShadows = proto.CullingSettings.IncreaseShadowsBoundingSphere,
                    FrustumPlanes = frustumPlanes,
                    CameraPosition = quadroRendererCamera.Camera.transform.position,
                    IsFrustumCulling = quadroRendererCamera.CameraCullingMode == CameraCullingMode.FrustumCulling ? proto.CullingSettings.IsFrustumCulling : false,
                    MaxDistance = maxDistance,
                    LodDistances = lodDistances,
                    LODCount = renderModelInfo.LODs.Count,
                    LODFadeDistance = proto.LODSettings.LodFadeTransitionDistance,
                    LodFadeForLastLOD = proto.LODSettings.LodFadeForLastLOD,
                    UseLODFade = proto.LODSettings.IsLODCrossFade,
                    FloatingOriginOffset = quadroRenderer.FloatingOriginOffset,
                    DirectionLight = directionLight,               
                    BoundsSize = proto.Bounds.size               
                };
    
            lodJob.Schedule().Complete();

            frustumPlanes.Dispose();
            lodDistances.Dispose();
        }

        public List<Cell> GetCellsWithItem(List<Cell> instancedIndirectCellList, int protoIndex, out int totalInstanceCount)
        {
            totalInstanceCount = 0;
            List<Cell> currentCellList = new List<Cell>();

            foreach (Cell cell in instancedIndirectCellList)
            {
                int count = cell.RenderInstancesList.ItemMatrixList[protoIndex].Length;

                if(count != 0)
                {
                    currentCellList.Add(cell);
                    totalInstanceCount += count;
                } 
            }

            return currentCellList;
        }
    }
}