using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace QuadroRendererSystem
{
    public static class Utility 
    {
        public static int GetInstanceCount(List<Cell> cellList, int protoIndex)
        {
            int totalInstanceCount = 0;

            foreach (Cell cell in cellList)
            {
                int count = cell.RenderInstancesList.ItemMatrixList[protoIndex].Length;

                if(count != 0)
                {
                    totalInstanceCount += count;
                } 
            }

            return totalInstanceCount;
        }

        public static ITerrain GetITerrainDataHelper(GameObject go)
        {
            if (go == null) return null;

            MonoBehaviour[] list = go.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in list)
            {
                if (mb is ITerrain)
                {
                    return mb as ITerrain;
                }
            }

            return null;
        }

        public static bool IsInLayer(int layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static float[] GetLODDistances(RenderModelInfo renderModelInfo, float lodBias, float maxDistance, bool forInstancedIndirect = true)
        {
            float[] lodDistances = new float[] 
            {
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000 
            };

            for (int i = 0; i < renderModelInfo.LODs.Count; i++)
            {
                int index = i;

                if(forInstancedIndirect)
                {
                    index = i * 4;
                    if (i >= 4)
                    {   
                        index = (i - 4) * 4 + 1;
                    }
                }
                
                if(i == renderModelInfo.LODs.Count - 1)
                {   
                    lodDistances[index] = maxDistance;
                }   
                else
                {
                    lodDistances[index] = renderModelInfo.LODs[i + 1].Distance * lodBias;
                }
            }

            return lodDistances;
        }

        public static float GetMaxDistance(QuadroPrototype proto, QuadroRendererCamera quadroRendererCamera, QuadroRenderer quadroRenderer)
        {
            float maxDistance = proto.CullingSettings.MaxDistance;

            maxDistance = Mathf.Min(maxDistance, quadroRenderer.QualitySettings.MaxRenderDistance);
            maxDistance = Mathf.Min(maxDistance, quadroRendererCamera.Camera.farClipPlane);

            return maxDistance;
        }

        public static void SetCurrentPrototypes(List<QuadroPrototype> prototypeList, List<GameObject> prefabList, int cameraCount)
        {
            if (prefabList == null)
                return;

#if UNITY_EDITOR 
            bool changed = false;
            foreach (QuadroPrototype prototype in prototypeList)
            {
                if (!prefabList.Contains(prototype.PrefabObject))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
                    changed = true;
                }
            }
            if (changed)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif

            foreach (GameObject go in prefabList)
            {
                if (prototypeList.Exists(p => p.PrefabObject == go))
                    continue;

                prototypeList.Add(GeneratePrefabPrototype(go, cameraCount, go.GetInstanceID()));
            }

#if UNITY_EDITOR 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); 

            if (!Application.isPlaying)
            {
                PrefabDataForQuadroRender[] prefabInstances = GameObject.FindObjectsOfType<PrefabDataForQuadroRender>();
                for (int i = 0; i < prefabInstances.Length; i++)
                {
                    UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstances[i].gameObject);
                    if (prefabRoot != null && ((GameObject)prefabRoot).GetComponent<PrefabDataForQuadroRender>() != null && prefabInstances[i].PrefabPrototype != ((GameObject)prefabRoot).GetComponent<PrefabDataForQuadroRender>().PrefabPrototype)
                    {
                        prefabInstances[i].PrefabPrototype = ((GameObject)prefabRoot).GetComponent<PrefabDataForQuadroRender>().PrefabPrototype;
                    }
                }
            }
#endif
        } 

        public static QuadroPrototype GeneratePrefabPrototype(GameObject go, int cameraCount, int ID)
        {
#if UNITY_EDITOR 
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
#endif

            PrefabDataForQuadroRender prefabScript = go.GetComponent<PrefabDataForQuadroRender>();
            if (prefabScript == null)
#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
                prefabScript = SceneViewDetector.AddComponentToPrefab<PrefabDataForQuadroRender>(go);
#else
                prefabScript = go.AddComponent<PrefabDataForQuadroRender>();
#endif
            if (prefabScript == null)
                return null;

            QuadroPrototype prototype = null;
            if (prefabScript != null)
                prototype = prefabScript.PrefabPrototype;
            if (prototype == null)
            {
                prototype = ScriptableObject.CreateInstance<QuadroPrototype>();
                if (prefabScript != null)
                    prefabScript.PrefabPrototype = prototype;
                prototype.PrefabObject = go;
                prototype.name = go.name + "_" + ID;

                prototype.ID = ID;

#if UNITY_EDITOR 
                if (!Application.isPlaying)
                    EditorUtility.SetDirty(go);
#endif
            }

#if UNITY_EDITOR 
            if (!Application.isPlaying && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(prototype)))
            {
                string assetPath = QuadroRendererConstants.PathToPrototypesPrefab + "/" + prototype.name + ".asset";

                if (!System.IO.Directory.Exists(QuadroRendererConstants.PathToPrototypesPrefab))
                {
                    System.IO.Directory.CreateDirectory(QuadroRendererConstants.PathToPrototypesPrefab);
                }

                AssetDatabase.CreateAsset(prototype, assetPath);
            }

#if UNITY_2018_3_OR_NEWER
            if (!Application.isPlaying && prefabScript != null && prefabScript.PrefabPrototype != prototype)
            {
                GameObject prefabContents = SceneViewDetector.LoadPrefabContents(go);
                prefabContents.GetComponent<PrefabDataForQuadroRender>().PrefabPrototype = prototype;
                SceneViewDetector.UnloadPrefabContents(go, prefabContents);
            }
#endif
#endif

            return prototype;
        }
    }
}