using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class ShaderBindings : ScriptableObject
    {
        public List<ShaderInstance> ShaderInstances;

        private static readonly List<string> StandardUnityShaders = new List<string> 
        {
            QuadroRendererConstants.SHADER_UNITY_STANDARD, 
            QuadroRendererConstants.SHADER_UNITY_STANDARD_SPECULAR,
            QuadroRendererConstants.SHADER_UNITY_STANDARD_ROUGHNESS, 
            QuadroRendererConstants.SHADER_UNITY_SPEED_TREE, 
            QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_8,
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_BARK, 
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_BARK_OPTIMIZED,
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_LEAVES, 
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_OPTIMIZED,
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_FAST, 
            QuadroRendererConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_FAST_OPTIMIZED,
            QuadroRendererConstants.SHADER_UNITY_TREE_SOFT_OCCLUSION_BARK, 
            QuadroRendererConstants.SHADER_UNITY_TREE_SOFT_OCCLUSION_LEAVES
        };

        private static readonly List<string> StandardUnityShadersGPUI = new List<string> 
        {
            QuadroRendererConstants.SHADER_GPUI_STANDARD, 
            QuadroRendererConstants.SHADER_GPUI_STANDARD_SPECULAR,
            QuadroRendererConstants.SHADER_GPUI_STANDARD_ROUGHNESS,
            QuadroRendererConstants.SHADER_GPUI_SPEED_TREE, 
            QuadroRendererConstants.SHADER_GPUI_SPEED_TREE_8,
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_BARK, 
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_BARK_OPTIMIZED,
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_LEAVES, 
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_OPTIMIZED,
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_FAST, 
            QuadroRendererConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_FAST_OPTIMIZED,
            QuadroRendererConstants.SHADER_GPUI_TREE_SOFT_OCCLUSION_BARK, 
            QuadroRendererConstants.SHADER_GPUI_TREE_SOFT_OCCLUSION_LEAVES
        };

        private static readonly List<string> ExtraGPUIShaders = new List<string> 
        {
            QuadroRendererConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_TREECREATOR,
        };
        
        public Shader GetInstancedShader(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
                return null;

            if (ShaderInstances == null)
                ShaderInstances = new List<ShaderInstance>();

            foreach (ShaderInstance si in ShaderInstances)
            {
                if (si.Name.Equals(shaderName))
                    return si.InstancedShader;
            }

            if (StandardUnityShaders.Contains(shaderName))
            {
                return Shader.Find(StandardUnityShadersGPUI[StandardUnityShaders.IndexOf(shaderName)]);
            }
            
            if (StandardUnityShadersGPUI.Contains(shaderName))
            {
                return Shader.Find(shaderName);
            }

            if (ExtraGPUIShaders.Contains(shaderName))
            {
                return Shader.Find(shaderName);
            }

            if (!shaderName.Equals(QuadroRendererConstants.SHADER_UNITY_STANDARD))
            {
                if (Application.isPlaying)
                    Debug.LogWarning("Can not find instanced shader for : " + shaderName + ". Using Standard shader instead.");
                return GetInstancedShader(QuadroRendererConstants.SHADER_UNITY_STANDARD);
            }

            Debug.LogWarning("Can not find instanced shader for : " + shaderName);
            return null;
        }

        public Material GetInstancedMaterial(Material originalMaterial)
        {
            if (originalMaterial == null || originalMaterial.shader == null)
            {
                Debug.LogWarning("One of the Quadro prototypes is missing material reference! Check the Material references in MeshRenderer.");
                return new Material(GetInstancedShader(QuadroRendererConstants.SHADER_UNITY_STANDARD));
            }

            if(QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.IsShaderOriginalInstanced(originalMaterial.shader.name))
            {
                return originalMaterial;
            }

            Material instancedMaterial = new Material(GetInstancedShader(originalMaterial.shader.name));
            instancedMaterial.CopyPropertiesFromMaterial(originalMaterial);
            instancedMaterial.name = originalMaterial.name + "_QuadroRenderer_GPUI";
            instancedMaterial.enableInstancing = true;

            return instancedMaterial;
        }

        public void ResetShaderInstances()
        {
            if (ShaderInstances == null)
                ShaderInstances = new List<ShaderInstance>();
            else
                ShaderInstances.Clear();

#if UNITY_EDITOR 
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void ClearEmptyShaderInstances()
        {
            if (ShaderInstances != null)
            {
#if UNITY_EDITOR 
                bool modified = false;

                modified |= ShaderInstances.RemoveAll(si => si == null || si.InstancedShader == null || string.IsNullOrEmpty(si.Name)) > 0;

                if (modified)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public void AddShaderInstance(string name, Shader instancedShader, bool isOriginalInstanced = false, string extensionCode = null)
        {
            ShaderInstances.Add(new ShaderInstance(name, instancedShader, isOriginalInstanced, extensionCode));
#if UNITY_EDITOR 
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public bool IsShadersInstancedVersionExists(string shaderName)
        {
            if (StandardUnityShaders.Contains(shaderName) || StandardUnityShadersGPUI.Contains(shaderName) || ExtraGPUIShaders.Contains(shaderName))
            {
                return true;
            }
                
            foreach (ShaderInstance si in ShaderInstances)
            {
                if (si.Name.Equals(shaderName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsShaderOriginalInstanced(string shaderName)
        {
            foreach (ShaderInstance si in ShaderInstances)
            {
                if (si.Name.Equals(shaderName))
                {
                    if(si.IsOriginalInstanced)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}