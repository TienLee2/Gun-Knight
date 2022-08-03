using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public static class ProfileFactory
    {        
        public static Type CreateType(string targetName)
        {
#if UNITY_EDITOR
                Directory.CreateDirectory(MegaWorldPath.pathToDataPackage);
                Directory.CreateDirectory(MegaWorldPath.pathToType);

                var path = string.Empty;

                path += targetName + " Type.asset";

                path = MegaWorldPath.CombinePath(MegaWorldPath.pathToType, path);
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                var profile = ScriptableObject.CreateInstance<Type>();
                AssetDatabase.CreateAsset(profile, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return profile;

#else
                return null;
#endif
        }

        public static TerrainLayer SaveTerrainLayerAsAsset(string textureName, TerrainLayer terrainLayer)
        {
#if UNITY_EDITOR
                Directory.CreateDirectory(MegaWorldPath.terrainLayerStoragePath);

                string path = MegaWorldPath.terrainLayerStoragePath + "/" + textureName + ".asset";

                path = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(terrainLayer, path);
                AssetDatabase.SaveAssets();

                return AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);

#else
                return null;
#endif
        }

        public static AdvancedSettings GetDefaultAdvancedSettings()
        {
            AdvancedSettings advancedSettings = Resources.Load<AdvancedSettings>(MegaWorldPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.AdvancedSettingsName));
                
            if (advancedSettings == null)
            {
                advancedSettings = ScriptableObject.CreateInstance<AdvancedSettings>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(MegaWorldPath.pathToResourcesMegaWorld))
                    {
                        System.IO.Directory.CreateDirectory(MegaWorldPath.pathToResourcesMegaWorld);
                    }

                    AssetDatabase.CreateAsset(advancedSettings, MegaWorldPath.pathToAdvancedSettings + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }
            
            return advancedSettings;
        }

        public static DataPackage GetDataPackage()
        {
            string pathToDataPackageForResourcesLoad = MegaWorldPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.DataPackageName);

            DataPackage dataPackage = Resources.Load<DataPackage>(MegaWorldPath.CombinePath(pathToDataPackageForResourcesLoad, MegaWorldPath.DataPackageName));
            
            if (dataPackage == null)
            {
                dataPackage = ScriptableObject.CreateInstance<DataPackage>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToDataPackage))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToDataPackage);
                }

                AssetDatabase.CreateAsset(dataPackage, MegaWorldPath.CombinePath(MegaWorldPath.pathToDataPackage, MegaWorldPath.DataPackageName) + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return dataPackage;
        }

        public static GeneralDataPackage GetGeneralDataPackage()
        {
            string pathToDataPackageForResourcesLoad = MegaWorldPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.GeneralDataPackageName);

            GeneralDataPackage generalDataPackage = Resources.Load<GeneralDataPackage>(MegaWorldPath.CombinePath(pathToDataPackageForResourcesLoad, MegaWorldPath.GeneralDataPackageName));
            
            if (generalDataPackage == null)
            {
                generalDataPackage = ScriptableObject.CreateInstance<GeneralDataPackage>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToGeneralDataPackage))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToGeneralDataPackage);
                }

                AssetDatabase.CreateAsset(generalDataPackage, MegaWorldPath.CombinePath(MegaWorldPath.pathToGeneralDataPackage, MegaWorldPath.GeneralDataPackageName) + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return generalDataPackage;
        }
    }
}
