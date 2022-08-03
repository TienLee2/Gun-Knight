using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuadroRendererSystem
{
    public class GPUInstancedRenderer 
    {
        private int _unityLODFadeID;
        private readonly Matrix4x4[] _renderArray = new Matrix4x4[1000];
        private readonly Vector4[] _renderLodFadeArray = new Vector4[1000];
        
        public void SetupInstancedRenderMaterialPropertiesIDs()
        {
            _unityLODFadeID = Shader.PropertyToID("unity_LODFade");
        }

        public void Render(QuadroRenderer quadroRenderer, int protoIndex, QuadroRendererCamera camera, Camera targetCamera)
        {
            QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.PrototypeList[protoIndex];

            ShadowCastingMode shadowCastingMode = quadroRenderer.QualitySettings.GetShadowCastingMode();

            if (camera.IsShadowCasting == false || proto.ShadowsSettings.IsShadowCasting == false)
            {
                shadowCastingMode = ShadowCastingMode.Off;
            }

            LayerMask layer = proto.PrefabObject.layer;

            RenderModelInfo renderModelInfo = quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[protoIndex];

            CameraRenderData сameraRender = camera.CameraRender;

            for (int LODIndex = 0; LODIndex < сameraRender.LODCount; LODIndex++)
            {
                NativeList<Matrix4x4> LODMatrixList = сameraRender.ItemLODMatrixList[LODIndex].ItemMatrixList[protoIndex];
                NativeList<Vector4> LODFadeist = сameraRender.ItemLODFadeList[LODIndex].ItemLodFadeList[protoIndex];

                RenderItemLOD(LODMatrixList, LODFadeist, renderModelInfo, proto, LODIndex, targetCamera, ShadowCastingMode.Off, layer);

                if(shadowCastingMode == ShadowCastingMode.On && proto.ShadowsSettings.DrawShadowLODMap[LODIndex])
                {
                    NativeList<Matrix4x4> LODShadowMatrixList = сameraRender.ItemLODShadowMatrixList[LODIndex].ItemMatrixList[protoIndex];

                    RenderItemLOD(LODShadowMatrixList, LODFadeist, renderModelInfo, proto, proto.ShadowsSettings.ShadowLODMap[LODIndex], targetCamera, 
                        ShadowCastingMode.ShadowsOnly, layer, proto.ShadowsSettings.ShadowWithOriginalShaderLODMap[LODIndex]);
                }
            }
        }

        public void RenderItemLOD(NativeList<Matrix4x4> matrixList, NativeList<Vector4> lodFadeList, RenderModelInfo renderModelInfo, 
            QuadroPrototype proto, int lodIndex, Camera targetCamera, ShadowCastingMode shadowCastingMode, LayerMask layer, bool shadowWithOriginalShader = true)
        {          
            if(matrixList.Length == 0) 
            {
                return;
            }

            LOD lod = renderModelInfo.LODs[lodIndex];

            int count = Mathf.CeilToInt(matrixList.Length / 1000f);
            int totalCount = matrixList.Length;
            for (int l = 0; l <= count - 1; l++)
            {
                int copyCount = 1000;
                if (totalCount < 1000) copyCount = totalCount;

                NativeSlice<Matrix4x4> matrixSlice = new NativeSlice<Matrix4x4>(matrixList, l * 1000, copyCount);          
                matrixSlice.CopyToFast(_renderArray);

                Mesh mesh = lod.Mesh;
                MaterialPropertyBlock materialPropertyBlock = lod.Mpb;

                materialPropertyBlock.Clear(); 

                if (proto.LODSettings.IsLODCrossFade && lodFadeList.Length == matrixList.Length)
                {
                    NativeSlice<Vector4> lodFadeSlice = new NativeSlice<Vector4>(lodFadeList, l * 1000, copyCount);
                    lodFadeSlice.CopyToFast(_renderLodFadeArray);     
                    materialPropertyBlock.SetVectorArray(_unityLODFadeID, _renderLodFadeArray); 
                } 

                for (int m = 0; m < lod.Materials.Count; m++)
                {
                    Material material = lod.Materials[m];

                    int submeshIndex = Mathf.Min(m, lod.Mesh.subMeshCount - 1);

                    Graphics.DrawMeshInstanced(mesh, submeshIndex,
                        shadowWithOriginalShader ? material : renderModelInfo.MaterialShadowsOnly, 
                        _renderArray, 
                        copyCount,
                        materialPropertyBlock, 
                        shadowCastingMode, true, layer,
                        targetCamera,
                        proto.GeneralSettings.LightProbeUsage);
                }
                
                totalCount -= 1000;
            }
        }
    }
}