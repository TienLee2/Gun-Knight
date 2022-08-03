using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuadroRendererSystem
{
    [InitializeOnLoad]
    public class QuadroRendererDefines
    {
        private static readonly string DEFINE_QUADRO_RENDERER = "QUADRO_RENDERER";


#if UNITY_2018_1_OR_NEWER
        public static UnityEditor.PackageManager.Requests.ListRequest packageListRequest;
#endif

        static QuadroRendererDefines()
        {
            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Unknown)
                return;
            List<string> defineList = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
            if (!defineList.Contains(DEFINE_QUADRO_RENDERER))
            {
                defineList.Add(DEFINE_QUADRO_RENDERER);
                string defines = string.Join(";", defineList.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            }

            EditorApplication.update -= GenerateSettings;
            EditorApplication.update += GenerateSettings;

            GenerateSettings();
        }

        static void GenerateSettings()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            SetVersion();

            EditorApplication.update -= GenerateSettings;
        }

        static void SetVersion()
        {
#if UNITY_2018_1_OR_NEWER
            bool forcePackageLoad = false;
#endif
            if (QuadroRendererConstants.QuadroRendererSettings.Version != QuadroRendererConstants.VERSION)
            {
                QuadroRendererConstants.QuadroRendererSettings.Version = QuadroRendererConstants.VERSION;

                RegenerateInstancedIndirectShaders();
                
                EditorUtility.SetDirty(QuadroRendererConstants.QuadroRendererSettings);
#if UNITY_2018_1_OR_NEWER
                forcePackageLoad = true;
#endif
            }

#if UNITY_2018_1_OR_NEWER
            FindCurrentRenderPipeline(forcePackageLoad);
#endif
        }

        public static void RegenerateInstancedIndirectShaders()
        {
            foreach (ShaderInstance item in QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.ShaderInstances)
            {
                item.Regenerate();
            }
        }


#if UNITY_2018_1_OR_NEWER
        public static void FindCurrentRenderPipeline(bool forceNew = false)
        { 
            if (forceNew || !QuadroRendererConstants.QuadroRendererSettings.PackagesLoaded)
            {
                packageListRequest = UnityEditor.PackageManager.Client.List(true);
                QuadroRendererConstants.QuadroRendererSettings.IsHDRP = false;
                QuadroRendererConstants.QuadroRendererSettings.IsLWRP = false;
                EditorApplication.update -= PackageListRequestHandler;
                EditorApplication.update += PackageListRequestHandler;
            }
        }

        private static void PackageListRequestHandler()
        {
            try
            {
                if (packageListRequest != null)
                {
                    if (!packageListRequest.IsCompleted)
                        return;
                    if (packageListRequest.Result != null)
                    {
                        foreach (var item in packageListRequest.Result)
                        {
                            if (item.name.Contains("com.unity.render-pipelines.high-definition"))
                            {
                                QuadroRendererConstants.QuadroRendererSettings.IsHDRP = true;
                                Debug.Log("Quadro Renderer detected HD Render Pipeline.");
                            }
                            else if (item.name.Contains("com.unity.render-pipelines.lightweight"))
                            {
                                QuadroRendererConstants.QuadroRendererSettings.IsLWRP = true;
                                Debug.Log("Quadro Renderer detected LW Render Pipeline.");
                            }
                            else if (item.name.Contains("com.unity.render-pipelines.universal"))
                            {
                                QuadroRendererConstants.QuadroRendererSettings.IsURP = true;
                                Debug.Log("Quadro Renderer detected Universal Render Pipeline.");
                            }
                        }
                        if (QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                            Debug.Log("Quadro Renderer detected Standard Render Pipeline.");
                        EditorUtility.SetDirty(QuadroRendererConstants.QuadroRendererSettings);
                    }
                }
            }
            catch (Exception) { }
            packageListRequest = null;
            QuadroRendererConstants.QuadroRendererSettings.PackagesLoaded = true;
            EditorApplication.update -= PackageListRequestHandler;
        }
#endif
    }
}
