using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public class QuadroRendererSettings : ScriptableObject
    {
        public float Version;

        public ShaderBindings ShaderBindings;
        public ShaderVariantCollection ShaderVariantCollection;

        public bool PackagesLoaded;
        public bool IsHDRP;
        public bool IsLWRP;
        public bool IsURP;

        [HideInInspector]
        [NonSerialized]
        public int MaxLODCountSupport = 8;

        public bool AutoShaderConversion = true;
        public bool ShowRenderModelData = false;
        public bool RenderSceneCameraInPlayMode = false;
        public bool RenderDirectToCamera = false;

        public static QuadroRendererSettings GetDefaultQuadroRendererSettings()
        {
            QuadroRendererSettings gpuiSettings = Resources.Load<QuadroRendererSettings>(MegaWorld.MegaWorldPath.CombinePath(QuadroRendererConstants.QuadroRenderer, QuadroRendererConstants.QuadroRendererSettingsName));

            if (gpuiSettings == null)
            {
                gpuiSettings = ScriptableObject.CreateInstance<QuadroRendererSettings>();
#if UNITY_EDITOR 
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(QuadroRendererConstants.PathToResourcesQuadroRenderer))
                    {
                        System.IO.Directory.CreateDirectory(QuadroRendererConstants.PathToResourcesQuadroRenderer);
                    }

                    AssetDatabase.CreateAsset(gpuiSettings, QuadroRendererConstants.PathToQuadroRendererSettings + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            gpuiSettings.SetDefaultShaderBindings();
            gpuiSettings.SetDefaultShaderVariantCollection();
            
            return gpuiSettings;
        }

        #region Shader Bindings
        public void SetDefaultShaderBindings()
        {
            if (ShaderBindings == null)
            {
                ShaderBindings = GetDefaultShaderBindings();
            }
        }

        public static ShaderBindings GetDefaultShaderBindings()
        {
            ShaderBindings shaderBindings = Resources.Load<ShaderBindings>(MegaWorld.MegaWorldPath.CombinePath(QuadroRendererConstants.QuadroRenderer, QuadroRendererConstants.ShaderBindings));

            if (shaderBindings == null)
            {
                shaderBindings = ScriptableObject.CreateInstance<ShaderBindings>();
                shaderBindings.ResetShaderInstances();
#if UNITY_EDITOR 
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(QuadroRendererConstants.PathToResourcesQuadroRenderer))
                    {
                        System.IO.Directory.CreateDirectory(QuadroRendererConstants.PathToResourcesQuadroRenderer);
                    }

                    AssetDatabase.CreateAsset(shaderBindings, QuadroRendererConstants.PathToShaderBindings + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return shaderBindings;
        }
        #endregion Shader Bindings

        #region Shader Variant Collection
        public void SetDefaultShaderVariantCollection()
        {
            if (ShaderVariantCollection == null)
            {
                ShaderVariantCollection = GetDefaultShaderVariantCollection();
            }
            SetDefaultShaderVariants();
        }

        public static ShaderVariantCollection GetDefaultShaderVariantCollection()
        {
            ShaderVariantCollection shaderVariantCollection = Resources.Load<ShaderVariantCollection>(MegaWorld.MegaWorldPath.CombinePath(QuadroRendererConstants.QuadroRenderer, QuadroRendererConstants.SHADER_VARIANT_COLLECTION_DEFAULT_NAME));

            if (shaderVariantCollection == null)
            {
                shaderVariantCollection = new ShaderVariantCollection();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(QuadroRendererConstants.PathToResourcesQuadroRenderer))
                    {
                        System.IO.Directory.CreateDirectory(QuadroRendererConstants.PathToResourcesQuadroRenderer);
                    }

                    AssetDatabase.CreateAsset(shaderVariantCollection, QuadroRendererConstants.PathToShaderVariantCollection + ".shadervariants");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return shaderVariantCollection;
        }

        public void SetDefaultShaderVariants()
        {
            AddShaderVariantToCollection(QuadroRendererConstants.SHADER_SHADOWS_ONLY);
        }

        public void AddShaderVariantToCollection(string shaderName)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && ShaderBindings != null && ShaderVariantCollection != null && !string.IsNullOrEmpty(shaderName))
            {
                Shader instancedShader = Shader.Find(shaderName);
                if (instancedShader != null)
                {
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = instancedShader;
                    //shaderVariant.passType = PassType.Normal;
                    ShaderVariantCollection.Add(shaderVariant);
                    // To add only the shader without the passtype or keywords, remove the specific variant but the shader remains
                    ShaderVariantCollection.Remove(shaderVariant);
                }
            }
#endif
        }

        public virtual void AddShaderVariantToCollection(Material material)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && ShaderBindings != null && ShaderVariantCollection != null && !string.IsNullOrEmpty(material.shader.name) && material)
            {
                Shader shader = AutoShaderConversion ? ShaderBindings.GetInstancedShader(material.shader.name) : material.shader;
                if (shader != null)
                {
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = shader;
                    shaderVariant.keywords = material.shaderKeywords;
                    ShaderVariantCollection.Add(shaderVariant);
                }
            }
#endif
        }
        #endregion Shader Variant Collection

        public bool IsStandardRenderPipeline()
        {
            return !IsLWRP && !IsHDRP && !IsURP;
        }
    }
}