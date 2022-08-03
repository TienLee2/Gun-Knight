using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace MegaWorld
{
    [Serializable]
    public class MegaWorldPath : ScriptableObject
    {
        private static AdvancedSettings advancedSettings;
        public static AdvancedSettings AdvancedSettings
        {
            get
            {
                if (advancedSettings == null)
                    advancedSettings = ProfileFactory.GetDefaultAdvancedSettings();
                return advancedSettings;
            }
            set
            {
                advancedSettings = value;
            }
        }

        private static DataPackage dataPackage;
        public static DataPackage DataPackage
        {
            get
            {
                if (dataPackage == null)
                    dataPackage = ProfileFactory.GetDataPackage();
                return dataPackage;
            }
            set
            {
                dataPackage = value;
            }
        }

        private static GeneralDataPackage generalDataPackage;
        public static GeneralDataPackage GeneralDataPackage
        {
            get
            {
                if (generalDataPackage == null)
                    generalDataPackage = ProfileFactory.GetGeneralDataPackage();
                return generalDataPackage;
            }
            set
            {
                generalDataPackage = value;
            }
        }

        private const string ASSETS_PATH = "Assets/";

        public static string pathToMegaWorld = BasePath;
        
        public static string MegaWorld = "MegaWorld";
        public static string DataPackageName = "Data Package";
        public static string GeneralDataPackageName = "General Data Package";
        public static string Types = "Types";
        public static string Resources = "Resources";
        public static string PolarisBrushes = "Polaris Brushes";
        public static string MegaWorldTerrainLayers = "Mega World Terrain Layers";
        public static string AdvancedSettingsName = "Advanced Settings";

        public static string pathToResources = CombinePath("Assets", Resources);
        public static string pathToResourcesMegaWorld = CombinePath(pathToResources, MegaWorld);
        public static string pathToDataPackage = CombinePath(pathToResourcesMegaWorld, DataPackageName);
        public static string pathToGeneralDataPackage = CombinePath(pathToResourcesMegaWorld, GeneralDataPackageName);
        public static string pathToType = CombinePath(pathToDataPackage, Types);
        public static string pathToAdvancedSettings = CombinePath(pathToResourcesMegaWorld, AdvancedSettingsName);
        public static string terrainLayerStoragePath = CombinePath("Assets", MegaWorldTerrainLayers);


        private static string s_basePath;

        public static string BasePath
        { 
            get
            {
#if UNITY_EDITOR

                if (!string.IsNullOrEmpty(s_basePath)) return s_basePath;
                var obj = CreateInstance<MegaWorldPath>();
                UnityEditor.MonoScript s = UnityEditor.MonoScript.FromScriptableObject(obj);
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
                DestroyImmediate(obj);
                var fileInfo = new FileInfo(assetPath);
                UnityEngine.Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
                UnityEngine.Debug.Assert(fileInfo.Directory.Parent != null, "fileInfo.Directory.Parent != null");
                DirectoryInfo baseDir = fileInfo.Directory.Parent;
                UnityEngine.Debug.Assert(baseDir != null, "baseDir != null");
                Assert.AreEqual("Mega World", baseDir.Name);
                string baseDirPath = ReplaceBackslashesWithForwardSlashes(baseDir.ToString());
                int index = baseDirPath.LastIndexOf(ASSETS_PATH, StringComparison.Ordinal);
                Assert.IsTrue(index >= 0);
                baseDirPath = baseDirPath.Substring(index);
                s_basePath = baseDirPath;

                pathToMegaWorld = s_basePath;

                return s_basePath;

#else
                return "";
#endif
            }
        }

        public static string ReplaceBackslashesWithForwardSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}