using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public class GameObjectConverter : MonoBehaviour
    {
        public QuadroRenderer quadroRenderer;
        public StorageTerrainCells StorageTerrainCells;
        public GameObject ParentObject;
        public string ParentName = "GameObject Converter";

        void OnEnable()
        {
            DetectNecessaryData();
        }

        public void ConvertGameObjectToQuadroRenderer()
        {
            GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if(MegaWorld.Utility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(sceneObjects[i]);

                if(proto != null)
                {
                    StorageTerrainCells.AddItemInstance(proto.ID, sceneObjects[i].transform.position, sceneObjects[i].transform.localScale, sceneObjects[i].transform.rotation);
                }
            }
        }

        public void ConvertQuadroRendererToGameObject()
        {
            if(Terrain.activeTerrain != null)
            {
                List<QuadroPrototype> quadroPrototypeList = quadroRenderer.QuadroPrototypesPackage.PrototypeList;

                for (int persistentCellIndex = 0; persistentCellIndex < StorageTerrainCells.PersistentStoragePackage.CellList.Count; persistentCellIndex++)
                {
                    Cell persistentCell = StorageTerrainCells.PersistentStoragePackage.CellList[persistentCellIndex];

                    for (int persistentInfoIndex = 0; persistentInfoIndex < persistentCell.ItemInfoList.Count; persistentInfoIndex++)
                    {
                        QuadroPrototype proto = quadroRenderer.QuadroPrototypesPackage.GetQuadroItem(persistentCell.ItemInfoList[persistentInfoIndex].ID);

                        if(proto == null)
                        {
                            continue;
                        }

                        for (int itemIndex = 0; itemIndex < persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList.Count; itemIndex++)
                        {
                            InstanceData persistentItem = persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex];
                            
                            PlaceGameObject(proto, persistentItem.Position, persistentItem.Scale, persistentItem.Rotation);
                        }
                    }
                }
            }
        }

        public void PlaceGameObject(QuadroPrototype proto, Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            GameObject go;

#if UNITY_EDITOR 
            go = PrefabUtility.InstantiatePrefab(proto.PrefabObject) as GameObject;
#else
            go = Instantiate(proto.PrefabObject);
#endif

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            ParentGameObject(proto, go);
        }

        public void ParentGameObject(QuadroPrototype proto, GameObject go)
        {            
            if(ParentObject == null)
            {
                FindParentObject();
            }

            if(ParentObject != null)
            {
                GameObject typeParentObject = ParentObject;

                if (go != null && typeParentObject != null && typeParentObject.transform != null)
                {
                    go.transform.SetParent(typeParentObject.transform, true);
                }
            }
        }

        public void FindParentObject()
        {
            Transform group = null;
            
            GameObject[] sceneRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject root in sceneRoots)
			{
				if(root.name == ParentName) 
                {
					group = root.transform;
                    break;
				}
			} 

            if (group == null)
            {
                GameObject childObject = new GameObject(ParentName);
                group = childObject.transform;
            }

            ParentObject = group.gameObject;
        }

        public void DetectNecessaryData()
        {
            if (quadroRenderer == null)
            {
                quadroRenderer = GetComponent<QuadroRenderer>();
            }

            if(StorageTerrainCells == null)
            {
                StorageTerrainCells = GetComponent<StorageTerrainCells>();
            }
        }

        public bool HasAllNecessaryData()
        {
            if(quadroRenderer == null)
            {
                return false;
            }

            if(StorageTerrainCells == null)
            {
                return false;
            }

            return true;
        }
    }
}