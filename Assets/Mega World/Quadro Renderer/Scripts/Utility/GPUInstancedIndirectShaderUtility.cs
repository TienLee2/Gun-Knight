#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace QuadroRendererSystem
{
    public static class GPUInstancedIndirectShaderUtility 
    {
        [NonSerialized]
        public static string[] s_quadroRendererGPUInstancedIndirectSetup = new string[]
        {
            "#include \"XXX\"\n",
            "#pragma instancing_options procedural:setupQuadroRenderer\n",
            "#pragma multi_compile_instancing\n",
        }; 

        static GPUInstancedIndirectShaderUtility()
        {
            string[] searchFolders = AssetDatabase.FindAssets("QuadroRendererInclude");

            for (int i = 0; i < searchFolders.Length; i++)
            {
                if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("QuadroRendererInclude.cginc")) 
                {
                    string cgincQR = AssetDatabase.GUIDToAssetPath(searchFolders[i]);

                    s_quadroRendererGPUInstancedIndirectSetup[0] = s_quadroRendererGPUInstancedIndirectSetup[0].Replace("XXX", cgincQR);

                    return;
                }
            }
        }

        public static void GenerateInstancedShadersIfNecessary(QuadroPrototype proto)
        {
            MeshRenderer[] meshRenderers = proto.PrefabObject.GetComponentsInChildren<MeshRenderer>();

            string warnings = "";
            
            foreach (MeshRenderer mr in meshRenderers)
            {
                Material[] mats = mr.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null || mats[i].shader == null)
                    {
                        continue;
                    }

                    if (QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.IsShadersInstancedVersionExists(mats[i].shader.name))
                    {
                        QuadroRendererConstants.QuadroRendererSettings.AddShaderVariantToCollection(mats[i]);
                        continue;
                    }

                    if (!Application.isPlaying)
                    {
                        if (IsShaderSupportInstancedIndirect(mats[i].shader))
                        {
                            QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.AddShaderInstance(mats[i].shader.name, mats[i].shader, true);
                            QuadroRendererConstants.QuadroRendererSettings.AddShaderVariantToCollection(mats[i]);
                        }
                        else if(QuadroRendererConstants.QuadroRendererSettings.AutoShaderConversion)
                        {
                            Shader instancedShader = CreateGPUInstancedIndirectShader(mats[i].shader);
                            if (instancedShader != null)
                            {
                                QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.AddShaderInstance(mats[i].shader.name, instancedShader);
                                QuadroRendererConstants.QuadroRendererSettings.AddShaderVariantToCollection(mats[i]);
                            }
                            else 
                            {
                                if (!warnings.Contains(mats[i].shader.name))
                                {
                                    string warningShaderGraph = "{0} is created with ShaderGraph, and Quadro Renderer Setup is not present. Please copy the shader code and paste it to a new shader file. When you set this new shader to your material, Quadro Renderer can run auto-conversion on this shader.";

                                    string originalAssetPath = AssetDatabase.GetAssetPath(mats[i].shader);
                                    if (originalAssetPath.ToLower().EndsWith(".shadergraph"))
                                    {
                                        warnings += string.Format(warningShaderGraph, mats[i].shader.name);
                                    }
                                    else
                                    {
                                        warnings += "Can not create instanced version for shader: " + mats[i].shader.name + ". Standard Shader will be used instead. If you are using a Unity built-in shader, please download the shader to your project from the Unity Archive.";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(warnings))
            {
                if (proto.WarningText != null)
                {
                    proto.WarningText = null;
                    EditorUtility.SetDirty(proto);
                }
            }
            else
            {                
                if (proto.WarningText != warnings)
                {
                    proto.WarningText = warnings;
                    EditorUtility.SetDirty(proto);
                }
            }
        }

        public static bool IsShaderSupportInstancedIndirect(Shader shader)
        {
            if (shader == null || shader.name == QuadroRendererConstants.SHADER_UNITY_INTERNAL_ERROR)
            {
                Debug.LogError("Can not find shader! Please make sure that the material has a shader assigned.");
                return false;
            }
            string originalAssetPath = AssetDatabase.GetAssetPath(shader);
            string originalShaderText = "";
            try
            {
                originalShaderText = System.IO.File.ReadAllText(originalAssetPath);
            }
            catch (Exception)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(originalShaderText))
            {
                return originalShaderText.Contains("QuadroRendererInclude.cginc");
            }
            return false;
        }

        public static List<string> RemoveExistingProceduralSetup(string originalAssetPath)
        {
            string[] originalLines = System.IO.File.ReadAllLines(originalAssetPath);
            List<string> newShaderlines = new List<string>();

            foreach (string line in originalLines)
            {
                if (!line.Contains("#pragma instancing_options")
                    && !line.Contains("QuadroRendererInclude.cginc")
                    && !line.Contains("#pragma multi_compile_instancing"))
                {
                    newShaderlines.Add(line + "\n");
                }
            }

            return newShaderlines; 
        }

        public static List<string> ChangeShaderName(string newShaderName, Shader originalShader, List<string> shaderLines)
        { 
            for (int i = 0; i < shaderLines.Count; i++)
            {
                if (shaderLines[i].Contains(originalShader.name))
                {
                    shaderLines[i] = shaderLines[i].Replace(originalShader.name, newShaderName);
                } 
            }

            return shaderLines;
        } 

        public static List<string> AddGPUInstancedIndirectSupport(List<string> shaderLines)
        {
            for (int lineIndex = 0; lineIndex < shaderLines.Count; lineIndex++)
            {
                if (shaderLines[lineIndex].Contains("HLSLPROGRAM"))
                {
                    for (int i = lineIndex; i < shaderLines.Count; i++)
                    {
                        if (shaderLines[i].Contains("ENDHLSL"))
                        {
                            shaderLines.InsertRange(i, s_quadroRendererGPUInstancedIndirectSetup);
                            break;
                        }
                    }
                }

                if(shaderLines[lineIndex].Contains("CGPROGRAM"))
                {
                    for (int i = lineIndex; i < shaderLines.Count; i++)
                    {
                        if (shaderLines[i].Contains("ENDCG"))
                        {
                            shaderLines.InsertRange(i, s_quadroRendererGPUInstancedIndirectSetup);
                            break;
                        }
                    }
                }
            }
            
            return shaderLines;
        }

        public static byte[] GetBytesFromStrings(List<string> shaderLines)
        {
            string shaderText = "";
            foreach (string line in shaderLines)
            {
                shaderText += line;
            }

            return System.Text.Encoding.UTF8.GetBytes(shaderText);
        }

        public static Shader CreateGPUInstancedIndirectShader(Shader originalShader, bool useOriginal = false)
        { 
            try
            {
                if (originalShader == null || originalShader.name == QuadroRendererConstants.SHADER_UNITY_INTERNAL_ERROR)
                {
                    Debug.LogError("Can not find shader! Please make sure that the material has a shader assigned.");
                    return null;
                }

                Shader originalShaderRef = Shader.Find(originalShader.name);
                string originalAssetPath = AssetDatabase.GetAssetPath(originalShaderRef);

                // can not work with ShaderGraph or other non shader code
                if (!originalAssetPath.EndsWith(".shader"))
                {   
                    return null;
                }

                List<string> shaderLines = new List<string>();

                shaderLines = RemoveExistingProceduralSetup(originalAssetPath);
                
                EditorUtility.DisplayProgressBar("Shader Conversion", "Creating a shader with GPU Instanced Instanced support...", 0.1f);

                string newShaderName = useOriginal ? "" : "QuadroRenderer/" + originalShader.name;
                shaderLines = ChangeShaderName(newShaderName, originalShader, shaderLines);

                shaderLines = AddGPUInstancedIndirectSupport(shaderLines);

                string originalFileName = System.IO.Path.GetFileName(originalAssetPath);
                string newAssetPath = useOriginal ? originalAssetPath : originalAssetPath.Replace(originalFileName, originalFileName.Replace(".shader", "_QuadroRenderer_GPUI.shader"));

                byte[] bytes = GetBytesFromStrings(shaderLines);
                System.IO.FileStream fs = System.IO.File.Create(newAssetPath);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                EditorUtility.DisplayProgressBar("Shader Conversion", "Creating a shader with GPU Instanced Instanced support...", 0.3f);
                AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh(); 

                Shader instancedShader = AssetDatabase.LoadAssetAtPath<Shader>(newAssetPath);
                if (instancedShader == null) 
                { 
                    instancedShader = Shader.Find(newShaderName);
                }

                EditorUtility.ClearProgressBar();

                return instancedShader;
            }
            catch (Exception e)
            {
                if (e is System.IO.DirectoryNotFoundException && e.Message.ToLower().Contains("unity_builtin_extra"))
                    Debug.LogError("\"" + originalShader.name + "\" shader is a built-in shader which is not included in Quadro Renderer package. Please download the original shader file from Unity Archive to enable auto-conversion for this shader. Check prototype details on the Manager for instructions.");
                else
                    Debug.LogException(e);
                EditorUtility.ClearProgressBar();
            }

            return null;
        }
    }
}
#endif