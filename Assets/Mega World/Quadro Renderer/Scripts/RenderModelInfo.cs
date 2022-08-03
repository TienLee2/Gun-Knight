using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class RenderModelInfo
    {
        public QuadroPrototype QuadroPrototype;
        public GameObject Model;
        public float BoundingSphereRadius;
        public Material MaterialShadowsOnly;
        public List<LOD> LODs;
        public List<CameraComputeBuffers> CameraComputeBufferList = new List<CameraComputeBuffers>();

        public RenderModelInfo(QuadroPrototype proto, int cameraCount)
        {
            this.QuadroPrototype = proto;
            
            Model = proto.PrefabObject;

            MaterialShadowsOnly = new Material(Shader.Find(QuadroRendererConstants.SHADER_SHADOWS_ONLY));
            MaterialShadowsOnly.enableInstancing = true; 

            if(proto.PrefabObject.GetComponent<LODGroup>() != null)
            {                   
                GenerateLODsFromLODGroup(proto);
            }
            else
            {
                AddOneLOD(proto);
            }

            proto.Bounds = MeshUtility.CalculateBoundsInstantiate(Model);      

            BoundingSphereRadius = proto.Bounds.extents.magnitude;

            CreateCameraBuffers(cameraCount); 
        }

        public void AddOneLOD(QuadroPrototype prototype)
        {
            if (LODs == null)
            {
                LODs = new List<LOD>();
            }
            else
            {
                LODs.Clear();
            }

            MeshRenderer meshRenderer = prototype.PrefabObject.GetComponent<MeshRenderer>();

            List<Material> instanceMaterials = CreateMaterialsWithInstancedIndirect(meshRenderer.sharedMaterials, prototype);

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(mpb);
            MaterialPropertyBlock shadowMPB = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(shadowMPB);

            AddLod(0, meshRenderer.GetComponent<MeshFilter>().sharedMesh, instanceMaterials, mpb, shadowMPB);
        }

        public void GenerateLODsFromLODGroup(QuadroPrototype prototype)
        {
            LODGroup lodGroup = prototype.PrefabObject.GetComponent<LODGroup>();

            if (LODs == null)
            {
                LODs = new List<LOD>();
            }
            else
            {
                LODs.Clear();
            }
                
            for (int LODIndex = 0; LODIndex < lodGroup.GetLODs().Length; LODIndex++)
            {
                if (lodGroup.GetLODs()[LODIndex].renderers != null)
                {
                    Renderer renderer = lodGroup.GetLODs()[LODIndex].renderers[0];

                    List<Material> instanceMaterials = CreateMaterialsWithInstancedIndirect(renderer.sharedMaterials, prototype);

                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(mpb);
                    MaterialPropertyBlock shadowMPB = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(shadowMPB);

                    AddLod(LODIndex, renderer.GetComponent<MeshFilter>().sharedMesh, instanceMaterials, mpb, shadowMPB);
                }
            }
        }

        public void AddLod(int LODIndex, Mesh mesh, List<Material> materials, MaterialPropertyBlock mpb, MaterialPropertyBlock shadowMPB)
        {
            if (mesh == null)
            {
                Debug.LogError("Can't add renderer: mesh is null. Make sure that all the MeshFilters on the objects has a mesh assigned.");
                return;
            }

            if (materials == null || materials.Count == 0)
            {
                Debug.LogError("Can't add renderer: no materials. Make sure that all the MeshRenderers have their materials assigned.");
                return;
            }

            LOD lod = new LOD()
            {
                Mesh = mesh,
                Materials = materials,
                Mpb = mpb,
                ShadowMPB = shadowMPB,
            };

            if(LODIndex == 0)
            {
                lod.Distance = 0;
            }
            else
            {
                lod.Distance = GetLODDistance(Model, LODIndex - 1);
            }

            LODs.Add(lod);
        }

        public void CreateCameraBuffers(int cameraCount)
        {
            DisposeCameraBuffers();

            CameraComputeBufferList = new List<CameraComputeBuffers>();
            
            for (int i = 0; i <= cameraCount - 1; i++)
            {
                CameraComputeBuffers cameraComputeBuffers = new CameraComputeBuffers(LODs);

                CameraComputeBufferList.Add(cameraComputeBuffers);
            }
        }

        void DisposeCameraBuffers()
        {
            if(CameraComputeBufferList == null)
            {
                return;
            }

            for (int i = 0; i < CameraComputeBufferList.Count; i++)
            {
                CameraComputeBufferList[i].DestroyComputeBuffers();
            }

            CameraComputeBufferList.Clear();
        }

        public void UnmanagedData()
        {
            DisposeCameraBuffers();
        }

        private static float GetLODDistance(GameObject rootModel, int lodIndex)
        {
            LODGroup lodGroup = rootModel.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                UnityEngine.LOD[] lods = lodGroup.GetLODs();
                if (lodIndex >= 0 && lodIndex < lods.Length)
                {
                    return (lodGroup.size / lods[lodIndex].screenRelativeTransitionHeight);
                }
            }
            return -1;
        }

        private List<Material> CreateMaterialsWithInstancedIndirect(Material[] sharedMaterials, QuadroPrototype proto)
        {
            List<Material> instanceMaterials = new List<Material>();
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                Material mat = sharedMaterials[i];

                if(QuadroRendererConstants.QuadroRendererSettings.AutoShaderConversion)
                {
                    mat = QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.GetInstancedMaterial(sharedMaterials[i]);
                }

                if(proto.LODSettings.IsLODCrossFade && QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                {
                    mat.EnableKeyword("LOD_FADE_CROSSFADE"); 
                }
                else
                {
                    mat.DisableKeyword("LOD_FADE_CROSSFADE"); 
                }

                instanceMaterials.Add(mat);
            }

            return instanceMaterials;
        }

        public void SetLODFadeKeyword(bool lodFade)
        {
            foreach (LOD lod in LODs)
            {
                foreach (Material mat in lod.Materials)
                {
                    if(lodFade)
                    {
                        mat.EnableKeyword("LOD_FADE_CROSSFADE"); 
                    }
                    else
                    {
                        mat.DisableKeyword("LOD_FADE_CROSSFADE"); 
                    }
                }
            }
        }

        public ComputeBuffer GetLODVisibleBuffer(int LODIndex, int cameraIndex, bool shadows)
        {
            if (shadows)
            {
                return CameraComputeBufferList[cameraIndex].GPUInstancedIndirectLODList[LODIndex].PositionShadowBuffer;
            }
            else
            {
                return CameraComputeBufferList[cameraIndex].GPUInstancedIndirectLODList[LODIndex].PositionBuffer;
            }                      
        }

        public List<ComputeBuffer> GetLODArgsBufferList(int cameraIndex, int LODIndex, bool shadows)
        {
            if (shadows)
            {
                return CameraComputeBufferList[cameraIndex].GPUInstancedIndirectLODList[LODIndex].ShadowArgsBufferMergedLODList;
            }
            else
            {
                return CameraComputeBufferList[cameraIndex].GPUInstancedIndirectLODList[LODIndex].ArgsBufferMergedLODList;
            }
        }
    }
}