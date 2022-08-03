using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace QuadroRendererSystem
{
    public static class BillboardUtility 
    {
        public static void GeneratePrototypeBillboardIfNecessary(QuadroPrototype proto, RenderModelInfo renderModelInfo, QualitySettings quality, bool forceGenerateBillboard = false)
        {
            if(proto.BillboardSettings.UseGeneratedBillboard == false)
            {
                return;
            }

            if(proto.BillboardSettings.AlbedoAtlasTexture == null || forceGenerateBillboard)
            {
                GeneratePrototypeBillboardData(proto, quality);
            }

            if(QuadroRendererConstants.QuadroRendererSettings.MaxLODCountSupport != renderModelInfo.LODs.Count)
			{
                switch (proto.BillboardSettings.BillboardGeneratorAction)
                {
                    case BillboardGenerationActions.ReplaceLastLOD:
                    {
                        ReplaceLastLODWithBillboard(proto, renderModelInfo);
                        break;
                    }
                    case BillboardGenerationActions.AddNewLOD:
                    {
                        AddNewLODWithBillboard(proto, renderModelInfo, quality);
                        break;
                    }
                }
            }
            else
            {
                ReplaceLastLODWithBillboard(proto, renderModelInfo);
            }
        }

        public static void GeneratePrototypeBillboardData(QuadroPrototype prototype, QualitySettings quality)
        {
            if (prototype.BillboardSettings == null)
                prototype.BillboardSettings = new BillboardSettings();

#if UNITY_2017_2_OR_NEWER
            prototype.BillboardSettings.BillboardFaceCamPos = UnityEngine.XR.XRSettings.enabled;
#else
            prototype.billboard.billboardFaceCamPos = UnityEngine.VR.VRSettings.enabled;
#endif

            GameObject sample = null;
            GameObject billboardCameraPivot = null;
#if UNITY_EDITOR
            string albedoAtlasPath = null;
            string normalAtlasPath = null;
#endif
            try
            {
                // cache the current render texture
                RenderTexture currentRt = RenderTexture.active;

                // calculate frame resolution
                int frameResolution = prototype.BillboardSettings.AtlasResolution / prototype.BillboardSettings.FrameCount;

#if UNITY_EDITOR 
                // use previous texture path if exists
                if (prototype.BillboardSettings.AlbedoAtlasTexture != null)
                    albedoAtlasPath = AssetDatabase.GetAssetPath(prototype.BillboardSettings.AlbedoAtlasTexture);
                if (prototype.BillboardSettings.NormalAtlasTexture != null)
                    normalAtlasPath = AssetDatabase.GetAssetPath(prototype.BillboardSettings.NormalAtlasTexture);
#endif

                // initialize the atlas textures
                prototype.BillboardSettings.AlbedoAtlasTexture = new Texture2D(prototype.BillboardSettings.AtlasResolution, frameResolution);
                prototype.BillboardSettings.NormalAtlasTexture = new Texture2D(prototype.BillboardSettings.AtlasResolution, frameResolution);

                // create render target for atlas frames (both albedo and normal will share the same target)
                RenderTexture frameTarget = RenderTexture.GetTemporary(frameResolution, frameResolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                frameTarget.enableRandomWrite = true;
                frameTarget.Create();

                // instantiate an instance of the prefab to sample
                sample = GameObject.Instantiate(prototype.PrefabObject, Vector3.zero, Quaternion.identity); // TO-DO: apply a rotation offset?
                sample.transform.localScale = Vector3.one;
                sample.hideFlags = HideFlags.DontSave;

                // set all children of the sample to the sample layer and calculate their overall bounds
                int sampleLayer = 31;
                MeshRenderer[] sampleChildrenMRs = sample.GetComponentsInChildren<MeshRenderer>();

                if (sampleChildrenMRs == null || sampleChildrenMRs.Length == 0)
                {
                    Debug.LogError("Cannot create GPU Instancer billboard for " + prototype.PrefabObject.name + " : no mesh renderers found in prototype prefab!");
                    GameObject.DestroyImmediate(sample);
                    prototype.BillboardSettings.UseGeneratedBillboard = false;
                    return;
                }

                Bounds sampleBounds = new Bounds(Vector3.zero, Vector3.zero);

                for (int i = 0; i < sampleChildrenMRs.Length; i++)
                {
                    sampleChildrenMRs[i].gameObject.layer = sampleLayer;

                    // TO-DO: turn this into a generic, add logic to shader bindings:
                    for (int m = 0; m < sampleChildrenMRs[i].sharedMaterials.Length; m++)
                    {
                        if (sampleChildrenMRs[i].sharedMaterials[m].HasProperty("_MainTexture"))
                            sampleChildrenMRs[i].sharedMaterials[m].SetTexture("_MainTex", sampleChildrenMRs[i].sharedMaterials[m].GetTexture("_MainTexture"));
                    }

                    // check if mesh renderer is enabled for this child
                    if (!sampleChildrenMRs[i].enabled)
                        continue;

                    MeshFilter sampleChildMF = sampleChildrenMRs[i].GetComponent<MeshFilter>();
                    if (sampleChildMF == null || sampleChildMF.sharedMesh == null || sampleChildMF.sharedMesh.vertices == null)
                        continue;

                    // encapsulate vertices instead of mesh renderer bounds; the latter are sometimes larger than necessary
                    Vector3[] verts = sampleChildMF.sharedMesh.vertices;
                    for (var v = 0; v < verts.Length; v++)
                        sampleBounds.Encapsulate(sampleChildrenMRs[i].transform.localToWorldMatrix.MultiplyPoint3x4(verts[v]));
                }

                // calculate quad size
                float sampleBoundsMaxSize = Mathf.Max(sampleBounds.size.x, sampleBounds.size.z) * 2;
                sampleBoundsMaxSize = Mathf.Max(sampleBoundsMaxSize, sampleBounds.size.y); // if height is longer, adjust.

                Shader billboardAlbedoBakeShader = Shader.Find(QuadroRendererConstants.SHADER_GPUI_BILLBOARD_ALBEDO_BAKER);
                Shader billboardNormalBakeShader = Shader.Find(QuadroRendererConstants.SHADER_GPUI_BILLBOARD_NORMAL_BAKER);
                Shader.SetGlobalFloat("_GPUIBillboardBrightness", prototype.BillboardSettings.BillboardBrightness);
                Shader.SetGlobalFloat("_GPUIBillboardCutoffOverride", prototype.BillboardSettings.CutOffOverride);
#if UNITY_EDITOR 
                Shader.SetGlobalFloat("_IsLinearSpace", PlayerSettings.colorSpace == ColorSpace.Linear ? 1.0f : 0.0f);
#endif

                // create the billboard snapshot camera
                billboardCameraPivot = new GameObject("GPUI_BillboardCameraPivot");
                Camera billboardCamera = new GameObject().AddComponent<Camera>();
                billboardCamera.transform.SetParent(billboardCameraPivot.transform);

                billboardCamera.gameObject.hideFlags = HideFlags.DontSave;
                billboardCamera.cullingMask = 1 << sampleLayer;
                billboardCamera.clearFlags = CameraClearFlags.SolidColor;
                billboardCamera.backgroundColor = Color.clear;
                billboardCamera.orthographic = true;
                billboardCamera.nearClipPlane = 0f;
                billboardCamera.farClipPlane = sampleBoundsMaxSize;
                billboardCamera.orthographicSize = sampleBoundsMaxSize * 0.5f;
                billboardCamera.allowMSAA = false;
                billboardCamera.enabled = false;
                billboardCamera.renderingPath = RenderingPath.Forward;
                billboardCamera.targetTexture = frameTarget;
                billboardCamera.transform.localPosition = new Vector3(0, sampleBounds.center.y, -sampleBoundsMaxSize / 2);

                float rotateAngle = 360f / prototype.BillboardSettings.FrameCount;

                // render the frames into the atlas textures
                for (int f = 0; f < prototype.BillboardSettings.FrameCount; f++)
                {
                    billboardCameraPivot.transform.rotation = Quaternion.AngleAxis(rotateAngle * f, Vector3.up);
                    RenderTexture.active = frameTarget;
                    billboardCamera.RenderWithShader(billboardAlbedoBakeShader, String.Empty);
                    prototype.BillboardSettings.AlbedoAtlasTexture.ReadPixels(new Rect(0, 0, frameResolution, frameResolution), f * frameResolution, 0);

                    billboardCamera.RenderWithShader(billboardNormalBakeShader, String.Empty);
                    prototype.BillboardSettings.NormalAtlasTexture.ReadPixels(new Rect(0, 0, frameResolution, frameResolution), f * frameResolution, 0);
                }

                // set the result billboard to the prototype
                prototype.BillboardSettings.AlbedoAtlasTexture.Apply();
                prototype.BillboardSettings.NormalAtlasTexture.Apply();

                prototype.BillboardSettings.AlbedoAtlasTexture = DilateBillboardTexture(prototype.BillboardSettings.AlbedoAtlasTexture, prototype.BillboardSettings.FrameCount, false);
                prototype.BillboardSettings.NormalAtlasTexture = DilateBillboardTexture(prototype.BillboardSettings.NormalAtlasTexture, prototype.BillboardSettings.FrameCount, true);

                prototype.BillboardSettings.QuadSize = sampleBoundsMaxSize;
                prototype.BillboardSettings.YPivotOffset = sample.transform.position.y + ((sampleBoundsMaxSize / 2) - (sampleBounds.extents.y) - sampleBounds.min.y);

#if UNITY_EDITOR 
                // save the textures to the project
                if (!System.IO.Directory.Exists(QuadroRendererConstants.PathToPrototypesBillboardTextures))
                {
                    System.IO.Directory.CreateDirectory(QuadroRendererConstants.PathToPrototypesBillboardTextures);
                }

                if (string.IsNullOrEmpty(albedoAtlasPath))
                    albedoAtlasPath = QuadroRendererConstants.PathToPrototypesBillboardTextures + "/" + prototype.PrefabObject.name + "_BillboardAlbedo.png";
                if (string.IsNullOrEmpty(normalAtlasPath)) 
                    normalAtlasPath = QuadroRendererConstants.PathToPrototypesBillboardTextures + "/" + prototype.PrefabObject.name + "_BillboardNormal.png";
                var bytes = prototype.BillboardSettings.AlbedoAtlasTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(albedoAtlasPath, bytes);

                bytes = prototype.BillboardSettings.NormalAtlasTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(normalAtlasPath, bytes);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // allow for larger textures if necessary:
                if (prototype.BillboardSettings.AtlasResolution > 2048)
                {
                    TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(albedoAtlasPath);
                    importer.maxTextureSize = 8192;
                    AssetDatabase.ImportAsset(albedoAtlasPath);
                    importer = (TextureImporter)TextureImporter.GetAtPath(normalAtlasPath);
                    importer.maxTextureSize = 8192;
                    AssetDatabase.ImportAsset(normalAtlasPath);
                }

                // set the atlas references to the newly created asset files.
                prototype.BillboardSettings.AlbedoAtlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoAtlasPath);
                prototype.BillboardSettings.NormalAtlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalAtlasPath);
#endif

                // restore the active render target
                RenderTexture.active = currentRt;
            }
            catch (Exception e)
            {
                Debug.LogError("Error on billboard generation for: " + prototype);
                if (sample)
                    GameObject.DestroyImmediate(sample);
                if (billboardCameraPivot)
                    GameObject.DestroyImmediate(billboardCameraPivot);
                throw e;
            }

            // clean up
            GameObject.DestroyImmediate(sample);
            GameObject.DestroyImmediate(billboardCameraPivot); // this will also release the frameTarget RT
        }

        public static void ReplaceLastLODWithBillboard(QuadroPrototype proto, RenderModelInfo renderModelInfo)
        {
            Mesh billboardMesh = GenerateQuadMesh(proto.BillboardSettings.QuadSize, proto.BillboardSettings.QuadSize,
                                    new Rect(0.0f, 0.0f, 1.0f, 1.0f), true, 0, proto.BillboardSettings.YPivotOffset);
            Material billboardMaterial = GetBillboardMaterial(proto);

            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Mesh = billboardMesh;
            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Materials = new List<Material>{billboardMaterial};
        }

        public static void AddNewLODWithBillboard(QuadroPrototype proto, RenderModelInfo renderModelInfo, QualitySettings quality)
        {
            if(renderModelInfo.LODs.Count == 0)
            {
                return;
            }

            if(renderModelInfo.LODs.Count == 4)
            {
                Debug.LogWarning("Quadro Renderer System only supports 4 LODs, no more LODs can be added");
                return;
            }

            Mesh billboardMesh = GenerateQuadMesh(proto.BillboardSettings.QuadSize, proto.BillboardSettings.QuadSize,
                                    new Rect(0.0f, 0.0f, 1.0f, 1.0f), true, 0, proto.BillboardSettings.YPivotOffset);
            Material billboardMaterial = GetBillboardMaterial(proto);

            renderModelInfo.LODs.Add(new LOD());

            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Mesh = billboardMesh;
            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Materials = new List<Material>{billboardMaterial};
            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Mpb = new MaterialPropertyBlock();
            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].ShadowMPB = new MaterialPropertyBlock();
            renderModelInfo.LODs[renderModelInfo.LODs.Count - 1].Distance = renderModelInfo.LODs[renderModelInfo.LODs.Count - 2].Distance + 100;
        }

        public static Mesh GenerateQuadMesh(float width, float height, Rect? uvRect = null, bool centerPivotAtBottom = false, float pivotOffsetX = 0f, float pivotOffsetY = 0f)
        {
            Mesh mesh = new Mesh();
            mesh.name = "QuadMesh";


            //mesh.vertices = new Vector3[] {
            //    new Vector3(centerPivotAtBottom ? -width/2 : 0, 0, 0),
            //    new Vector3(centerPivotAtBottom ? -width/2 : 0, height, 0),
            //    new Vector3(centerPivotAtBottom ? width/2 : width, height, 0),
            //    new Vector3(centerPivotAtBottom ? width/2 : width, 0, 0)
            //};

            mesh.vertices = new Vector3[]
            {
                new Vector3(centerPivotAtBottom ? -width/2-pivotOffsetX : -pivotOffsetX, -pivotOffsetY, 0), // bottom left
                new Vector3(centerPivotAtBottom ? -width/2-pivotOffsetX : -pivotOffsetX, height-pivotOffsetY, 0), // top left
                new Vector3(centerPivotAtBottom ? width/2-pivotOffsetX : width-pivotOffsetX, height-pivotOffsetY, 0), // top right
                new Vector3(centerPivotAtBottom ? width/2-pivotOffsetX : width-pivotOffsetX, -pivotOffsetY, 0) // bottom right
            };


            if (uvRect != null)
                mesh.uv = new Vector2[]
                {
                    new Vector2(uvRect.Value.x, uvRect.Value.y),
                    new Vector2(uvRect.Value.x, uvRect.Value.y + uvRect.Value.height),
                    new Vector2(uvRect.Value.x + uvRect.Value.width, uvRect.Value.y + uvRect.Value.height),
                    new Vector2(uvRect.Value.x + uvRect.Value.width, uvRect.Value.y)
                };

            //mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 }; // ori
            //mesh.triangles = new int[] { 1, 2, 0, 2, 3, 0 };
            mesh.triangles = new int[] { 0, 1, 3, 1, 2, 3 };

            Vector3 planeNormal = new Vector3(0, 0, -1);
            Vector4 planeTangent = new Vector4(1, 0, 0, -1);

            mesh.normals = new Vector3[4]
            {
                planeNormal,
                planeNormal,
                planeNormal,
                planeNormal
            };

            mesh.tangents = new Vector4[4]
            {
                planeTangent,
                planeTangent,
                planeTangent,
                planeTangent
            };

            Color[] colors = new Color[mesh.vertices.Length];

            for (int i = 0; i < mesh.vertices.Length; i++)
                colors[i] = Color.Lerp(Color.clear, Color.red, mesh.vertices[i].y);

            mesh.colors = colors;

            return mesh;
        }

        public static Material GetBillboardMaterial(QuadroPrototype prototype)
        {
            Material billboardMaterial = new Material(Shader.Find(QuadroRendererConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_TREECREATOR));
            billboardMaterial.enableInstancing = true;
            QuadroRendererConstants.QuadroRendererSettings.AddShaderVariantToCollection(billboardMaterial);

            UpdateBillboardMaterial(billboardMaterial, prototype);

            return billboardMaterial;
        }

        public static void UpdateBillboardMaterial(Material billboardMaterial, QuadroPrototype prototype)
        {
            if (!billboardMaterial) return;   

            billboardMaterial.SetTexture("_AlbedoAtlas", prototype.BillboardSettings.AlbedoAtlasTexture);
            billboardMaterial.SetTexture("_NormalAtlas", prototype.BillboardSettings.NormalAtlasTexture);
            billboardMaterial.SetFloat("_FrameCount", prototype.BillboardSettings.FrameCount);
            billboardMaterial.SetFloat("_Cutoff", prototype.BillboardSettings.CutOffOverride);
            billboardMaterial.SetColor("_Color", prototype.BillboardSettings.Color);

            billboardMaterial.SetColor("_TranslucencyColor", prototype.BillboardSettings.TranslucencyColor);
            billboardMaterial.SetColor("_HealthyColor", prototype.BillboardSettings.HealthyColor);
            billboardMaterial.SetColor("_DryColor", prototype.BillboardSettings.DryColor);
            billboardMaterial.SetFloat("_UseHueVariation", prototype.BillboardSettings.UseHueVariation ? 1 : 0);
            billboardMaterial.SetFloat("_NormalInvert", prototype.BillboardSettings.NormalInvert ? 1 : 0);
            billboardMaterial.SetFloat("_ShadowStrength", prototype.BillboardSettings.ShadowStrength);
            billboardMaterial.SetFloat("_ColorNoiseSpread", prototype.BillboardSettings.ColorNoiseSpread);

            billboardMaterial.DisableKeyword("_BILLBOARDFACECAMPOS_ON");
            if (prototype.BillboardSettings.BillboardFaceCamPos)
            {
                billboardMaterial.EnableKeyword("_BILLBOARDFACECAMPOS_ON");
            }
        }

        public static Texture2D DilateBillboardTexture(Texture2D billboardTexture, int frameCount, bool isNormal)
        {
            ComputeShader dilationCompute = (ComputeShader)Resources.Load(QuadroRendererConstants.COMPUTE_BILLBOARD_RESOURCE_PATH);
            int dilationKernel = dilationCompute.FindKernel(QuadroRendererConstants.COMPUTE_BILLBOARD_DILATION_KERNEL);

            RenderTexture resultTexture = RenderTexture.GetTemporary(billboardTexture.width, billboardTexture.height, 32, RenderTextureFormat.ARGB32);

            resultTexture.enableRandomWrite = true;
            resultTexture.Create();

            dilationCompute.SetTexture(dilationKernel, "result", resultTexture);
            dilationCompute.SetTexture(dilationKernel, "billboardSource", billboardTexture);
            dilationCompute.SetInts("billboardSize", new int[2] { billboardTexture.width, billboardTexture.height });
            dilationCompute.SetInt("frameCount", frameCount);
#if UNITY_EDITOR
            dilationCompute.SetBool("isLinearSpace", PlayerSettings.colorSpace == ColorSpace.Linear);
#endif
            dilationCompute.SetBool("isNormal", isNormal);
            dilationCompute.Dispatch(dilationKernel, Mathf.CeilToInt(billboardTexture.width / (QuadroRendererConstants.COMPUTE_SHADER_THREAD_COUNT_2D * frameCount)), 
                Mathf.CeilToInt(billboardTexture.height / QuadroRendererConstants.COMPUTE_SHADER_THREAD_COUNT_2D), frameCount);
            RenderTexture.active = resultTexture;

            Texture2D result = new Texture2D(billboardTexture.width, billboardTexture.height);
            result.ReadPixels(new Rect(0, 0, billboardTexture.width, billboardTexture.height), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            resultTexture.Release();
            return result;
        }
    }
}
