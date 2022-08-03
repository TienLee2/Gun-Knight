using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace MegaWorld
{
    public static class Utility
    {        
        public static bool IsVector3Equal (Vector3 a, Vector3 b, float epsilon = 0.001f)
        {            
            return Mathf.Abs (a.x - b.x) < epsilon && Mathf.Abs (a.y - b.y) < epsilon && Mathf.Abs (a.z - b.z) < epsilon;
        }

        public static void ForAllInHierarchy(GameObject gameObject, Action<GameObject> action)
        {
            action(gameObject);

            for (int i = 0; i < gameObject.transform.childCount; i++)
                ForAllInHierarchy(gameObject.transform.GetChild(i).gameObject, action);
        }

        public static void SetSelectedPrototypeListFromAssets<T>(List<T> baseList, List<T> setPrototypeList, System.Type prototypeType) where T : Prototype
        {
            foreach (T asset in baseList)
            {
                if (asset.GetType() == prototypeType)
                {
                    if(asset.selected)
                    {
                        setPrototypeList.Add((T)asset);
                    }
                }
            }
        }

        public static bool IsSameTexture(Texture2D tex1, Texture2D tex2, bool checkID = false)
        {
            if (tex1 == null || tex2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (tex1.GetInstanceID() != tex2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (tex1.name != tex2.name)
            {
                return false;
            }

            if (tex1.width != tex2.width)
            {
                return false;
            }

            if (tex1.height != tex2.height)
            {
                return false;
            }

            return true;
        }

        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }

        public static float WorldToDetailf(float pos, float terrainSize, TerrainData td)
        {
            return (pos / terrainSize) * td.detailResolution;
        }

        public static int WorldToDetail(float pos, float size, TerrainData td)
        {
            return Mathf.RoundToInt(WorldToDetailf(pos, size, td));
        }

        public static int WorldToDetail(float pos, TerrainData td)
        {
            return WorldToDetail(pos, td.size.x, td);
        }

        public static int WorldToSplat(float pos, float terrainSize, TerrainData td)
        {
            return Mathf.RoundToInt(WorldToSplatf(pos, terrainSize, td));
        }

        public static float WorldToSplatf(float pos, float terrainSize, TerrainData td)
        {
            return (pos / terrainSize) * td.alphamapResolution;
        }

        public static void NormalizeArray(float[,] arr, int width, int height, ref float rangeMin, ref float rangeMax)
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = arr[x, y];
                    if (v < min)
                    {
                        min = v;
                    }
                    if (v > max) 
                    {
                        max = v;
                    }
                }
            }

            rangeMin = min;
            rangeMax = max;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = arr[x, y];
                    arr[x, y] = (v - min) / (max - min);
                }
            }
        }

        public static Vector2 GetTerrainWorldPositionFromRange(Vector2 normal, Terrain terrain)
        {
            Vector2 localTerrainPosition = new Vector2(Mathf.Lerp(0, terrain.terrainData.size.x, normal.x), Mathf.Lerp(0, terrain.terrainData.size.z, normal.y));
            return localTerrainPosition + new Vector2(terrain.GetPosition().x, terrain.GetPosition().z);
        }

        public static Vector2 GetNormalizedLocalPos2D(Terrain activeTerrain, Vector3 location)
        {
            if(activeTerrain == null)
            {
                return Vector3.zero;
            }

            Vector3 terrainLocalPos = activeTerrain.transform.InverseTransformPoint(location);
            Vector2 normalizedLocalPos =
                new Vector3(Mathf.InverseLerp(0f, activeTerrain.terrainData.size.x, terrainLocalPos.x),
                    Mathf.InverseLerp(0f, activeTerrain.terrainData.size.z, terrainLocalPos.z));

            return normalizedLocalPos;
        }

        public static Vector2 WorldPointToUV(Vector3 point, Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                return Vector2.zero;
            }
                
            Vector3 localPoint = activeTerrain.transform.InverseTransformPoint(point);
            Vector3 terrainSize = new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z);
            Vector2 uv = new Vector2(
                InverseLerpUnclamped(0, terrainSize.x, localPoint.x),
                InverseLerpUnclamped(0, terrainSize.z, localPoint.z));

            return uv;
        }

        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (a != b)
            {
                return (value - a) / (b - a);
            }
            return 0f;
        }

        public static Terrain GetTerrain(Vector3 location)
        {
            Vector3 terrainMin = new Vector3();
            Vector3 terrainMax = new Vector3();

            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                Terrain terrain = Terrain.activeTerrains[idx];
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }
            return null;
		}

        public static GameObject GetPrefabRoot(GameObject gameObject)
        {
#if UNITY_EDITOR
            if(PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab)
            {
                return gameObject;
            }

            return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
            return gameObject;
#endif
        }

        public static bool Raycast(Ray ray, out RaycastInfo raycastInfo, LayerMask layersMask)
        {
            raycastInfo = new RaycastInfo();
            raycastInfo.isHit = false;

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, layersMask))
		    {
                raycastInfo.hitInfo = hitInfo;
                raycastInfo.isHit = true;

                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public static Vector3 GetRaycastMousePosition(LayerMask layersMask)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastInfo raycast;
            Utility.Raycast(ray, out raycast, layersMask);

            return raycast.hitInfo.point;
        }
#endif

        public static Ray GetRayForGlobalSpawnVisualisation(Vector3 checkPoint, Type type)
        {
            return new Ray(new Vector3(checkPoint.x, checkPoint.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.SpawnCheckOffset, checkPoint.z), Vector3.down);
        }

        public static Ray GetCurrentRayForBrushTool(Vector3 checkPoint, Type type)
        {
            if(type.ResourceType == ResourceType.GameObject)
            {
                if(type.ScatterSettings.ScatterAlgorithm == ScatterAlgorithm.RandomPoint)
                {
                    if(type.SpawnSurface == SpawnMode.Spherical)
                    {
                        return WorldPointToRay(checkPoint);
                    }
                    else
                    {                    
                        return new Ray(new Vector3(checkPoint.x, checkPoint.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.SpawnCheckOffset, checkPoint.z), Vector3.down);
                    }
                }
                else
                {
                    return new Ray(new Vector3(checkPoint.x, checkPoint.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.SpawnCheckOffset, checkPoint.z), Vector3.down);
                }
            }
            else
            {
                return new Ray(new Vector3(checkPoint.x, checkPoint.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.SpawnCheckOffset, checkPoint.z), Vector3.down);
            }
        }

        public static Ray WorldPointToRay(Vector3 worldSpacePoint)
        {
            return new Ray(Camera.current.transform.position, (worldSpacePoint - Camera.current.transform.position).normalized);
        }

        public static Vector2 AnimationCurveToRenderTexture(AnimationCurve curve, ref Texture2D tex) 
        {
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = UnityEngine.FilterMode.Bilinear;

            float val = curve.Evaluate(0.0f);
            Vector2 range = new Vector2(val, val);

            Color[] pixels = new Color[tex.width * tex.height];
            pixels[0].r = val;
            for (int i = 1; i < tex.width; i++) {
                float pct = (float)i / (float)tex.width;
                pixels[i].r = curve.Evaluate(pct);
                range[0] = Mathf.Min(range[0], pixels[i].r);
                range[1] = Mathf.Max(range[1], pixels[i].r);
            }
            tex.SetPixels(pixels);
            tex.Apply();

            return range;
        }

        public static Texture2D ToTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
            
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;
            return tex;
        }

        public static Vector2 GetNormalizedCheckPoint(Vector3 dragPoint, Vector3 checkPoint, float radius)
        {
            float size = radius * 2;
            Vector3 startBrush = new Vector3(dragPoint.x - radius, 0, dragPoint.z - radius);
            return new Vector2(Mathf.InverseLerp(startBrush.x, startBrush.x + size, checkPoint.x), Mathf.InverseLerp(startBrush.z, startBrush.z + size, checkPoint.z));
        }

        public static float GetGrayscaleFromWorldPosition(Bounds bounds, Vector3 worldPosition, Texture2D filterMask)
        {
            if(filterMask == null)
            {
                return 0;
            }

            float inverseY = Mathf.InverseLerp(bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z, worldPosition.z);
            float inverseX = Mathf.InverseLerp(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x, worldPosition.x);
            
            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, filterMask.width, inverseY));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, filterMask.height, inverseX));
            
            return filterMask.GetPixel(pixelX, pixelY).grayscale;
        }

        public static float GetGrayscale(Vector2 normal, Texture2D filterMask)
        {
            if(filterMask == null)
            {
                return 0;
            }
            
            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, filterMask.width, normal.y));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, filterMask.height, normal.x));
            
            return filterMask.GetPixel(pixelX, pixelY).grayscale;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) 
        {
            return rotation * (point - pivot) + pivot;
        }

        public static PrototypeGameObject GetCurrentPrototypeGameObject(Type type, GameObject go)
        {
            foreach (PrototypeGameObject proto in type.ProtoGameObjectList)
            {
                if(Utility.IsSameGameObject(proto.prefab, go))
                {
                    return proto;
                }
            }

            return null;
        }

        public static Bounds GetBoundsFromGameObject(GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
            Collider collider;

            if(renderer != null && renderer.enabled && renderer is SkinnedMeshRenderer)
            {
                return renderer.bounds;
            }
            else if (renderer != null && renderer.enabled &&
                meshFilter != null && meshFilter.sharedMesh != null)
            {
                return renderer.bounds;
            }
            else if ((collider = gameObject.GetComponent<Collider>()) != null && collider.enabled)
            {
                return collider.bounds;
            }

            return new Bounds();
        }
    } 
}