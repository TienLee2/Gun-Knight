using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    public enum SpawnMode
    {
        Plane,
        Spherical
    }

    public enum FilterType
    {
        SimpleFilter,
        MaskFilter
    }

    public enum ResourceType 
    {  
        GameObject, 
        TerrainDetail,
        TerrainTexture,
        QuadroItem
    }

    [CreateAssetMenu(fileName = "New Type", menuName = "Mega World/Type", order = 0)]
    public class Type : ScriptableObject
    {
        #region Main Properties
        public string TypeName = "Default";
        public bool Selected = false;

        public ResourceType ResourceType = ResourceType.QuadroItem; 

        public List<PrototypeQuadroItem> ProtoQuadroItemList = new List<PrototypeQuadroItem>();
        public List<PrototypeGameObject> ProtoGameObjectList = new List<PrototypeGameObject>();
        public List<PrototypeTerrainDetail> ProtoTerrainDetailList = new List<PrototypeTerrainDetail>(); 
        public List<PrototypeTerrainTexture> ProtoTerrainTextureList = new List<PrototypeTerrainTexture>();
        #endregion

        #region Tools Properties
        public BrushSettings BrushSettings = new BrushSettings();
        public LayerSettings LayerSettings = new LayerSettings();
        public ScatterSettings ScatterSettings = new ScatterSettings();
        public FilterType FilterType = FilterType.MaskFilter;
        public SpawnMode SpawnSurface = SpawnMode.Plane;
        public GameObject TypeParentObject;
        #endregion

        #region Stamper Tool Properties
        public int RandomSeed = 0;
        public bool GenerateRandomSeed = true;
        #endregion

        #region Precision Place Tool Properties
        public int PrecisionUnit = 0;
        public int PastPrecisionUnit = 0;
        #endregion

        #region GUI Properties
        public Vector2 PrototypeWindowsScroll = Vector2.zero;
        public string RenamingName = "Default";
        public bool Renaming = false;
        #endregion

        public void AddPrototypeGameObject(List<GameObject> draggedGameObjects)
        {
            foreach (GameObject draggedGameObject in draggedGameObjects)
            {
                Bounds localBounds = GetInstantiatedBounds(draggedGameObject);
                ProtoGameObjectList.Add(new PrototypeGameObject(draggedGameObject, localBounds.size / 2));
            }        
        }

        public void AddPrototypeTerrainDetail(object[] objectReferences)
        {
#if UNITY_EDITOR
            foreach (UnityEngine.Object draggedObject in objectReferences)
            {
                if (draggedObject is GameObject && 
                    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                    AssetDatabase.Contains(draggedObject))
                {
					PrototypeTerrainDetail prototypeTerrainDetail = new PrototypeTerrainDetail((GameObject)draggedObject);

					ProtoTerrainDetailList.Add(prototypeTerrainDetail);
                }
				if (draggedObject is Texture2D)
                {
					PrototypeTerrainDetail prototypeTerrainDetail = new PrototypeTerrainDetail((Texture2D)draggedObject, draggedObject.name);

					ProtoTerrainDetailList.Add(prototypeTerrainDetail);
                }
            }
#endif
        }

        public void AddPrototypeQuadroItem(List<GameObject> draggedGameObjects)
        {
            foreach (GameObject draggedGameObject in draggedGameObjects)
            {
                Bounds localBounds = GetInstantiatedBounds(draggedGameObject);
                ProtoQuadroItemList.Add(new PrototypeQuadroItem(draggedGameObject, localBounds.size / 2));
            }        
        }

        public static Bounds GetInstantiatedBounds(GameObject prefab)
		{
			GameObject go = Instantiate(prefab);
			go.transform.position = prefab.transform.position;
			Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
			foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
			{
				bounds.Encapsulate(r.bounds);
			}
			foreach (Collider c in go.GetComponentsInChildren<Collider>())
			{
				bounds.Encapsulate(c.bounds);
			}
			DestroyImmediate(go);
			return bounds;
		}

        public List<PrototypeGameObject> GetAllSelectedGameObject()
        {
            List<PrototypeGameObject> localProtoGameObjectList = new List<PrototypeGameObject>();

            foreach (PrototypeGameObject proto in ProtoGameObjectList)
            {
                if(proto.selected)
                {
                    localProtoGameObjectList.Add(proto);
                }
            }

            return localProtoGameObjectList;
        }

        public List<PrototypeQuadroItem> GetAllSelectedQuadroItem()
        {
            List<PrototypeQuadroItem> localProtoQuadroItemList = new List<PrototypeQuadroItem>();

            foreach (PrototypeQuadroItem proto in ProtoQuadroItemList)
            {
                if(proto.selected)
                {
                    localProtoQuadroItemList.Add(proto);
                }
            }

            return localProtoQuadroItemList;
        }

        public void SetPrecisionUnit(int index, int prototypesCount)
        {
            if(index != PrecisionUnit)
            {    
                if(index > prototypesCount - 1)
                {
                    PrecisionUnit = 0;
                }
                else if(index < 0)
                {
                    PrecisionUnit = prototypesCount - 1;
                }
                else
                {
                    PrecisionUnit = index;
                }
            }
        }

        public int GetPrecisionUnit()
        {
            return PrecisionUnit;
        }

        public LayerMask GetCurrentPaintLayers()
        {
            return LayerSettings.GetCurrentPaintLayers(ResourceType);
        }
    }
}