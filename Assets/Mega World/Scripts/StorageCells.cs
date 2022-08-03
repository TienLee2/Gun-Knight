using System.Collections.Generic;
using UnityEngine;
using System;
using VladislavTsurikov;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaWorld
{
    public class StorageCells
    {
        public QuadTree<MegaWorldCell> cellQuadTree;

        [NonSerialized]
        public List<MegaWorldCell> cellList = new List<MegaWorldCell>();
        public List<MegaWorldCell> modifiedСells = new List<MegaWorldCell>();

        public void RefreshCells(float cellSize)
        {
            GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

            cellList = new List<MegaWorldCell>();

            CreateCells(GetWorldBounds(sceneObjects), cellSize);
            Populate(sceneObjects);
        }

        public void Populate(GameObject[] sceneObjects)
        {
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if(Utility.GetPrefabRoot(sceneObjects[i]) == null)
                {
                    continue;
                }
                
                if(Utility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                PrototypeGameObject proto = AllAvailableTypes.GetCurrentPrototypeGameObject(sceneObjects[i]);

                if(proto != null)
                {
                    AddItemInstance(proto.ID, sceneObjects[i]);
                }
            }
        }

        public void CreateCells(Bounds worldBounds, float cellSize)
        {
            cellList.Clear(); 
            
            Bounds expandedBounds = new Bounds(worldBounds.center, worldBounds.size);
            expandedBounds.Expand(new Vector3(cellSize * 2f, 0, cellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            cellQuadTree = new QuadTree<MegaWorldCell>(expandedRect);

            int cellXCount = Mathf.CeilToInt(worldBounds.size.x / cellSize);
            int cellZCount = Mathf.CeilToInt(worldBounds.size.z / cellSize);

            Vector2 corner = new Vector2(worldBounds.center.x - worldBounds.size.x / 2f,
                worldBounds.center.z - worldBounds.size.z / 2f);

            Bounds bounds = new Bounds();

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(cellSize * x + corner.x, cellSize * z + corner.y),
                        new Vector2(cellSize, cellSize));

                    bounds = RectExtension.CreateBoundsFromRect(rect, worldBounds.center.y, worldBounds.size.y);

                    MegaWorldCell cell = new MegaWorldCell(bounds);

                    cellList.Add(cell);
                    cell.index = cellList.Count - 1;
                    cellQuadTree.Insert(cell);
                }
            }
        }

        public Bounds GetWorldBounds(GameObject[] sceneObjects)
        {
            Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if(Utility.GetPrefabRoot(sceneObjects[i]) == null)
                {
                    continue;
                }

                if(Utility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                if (!sceneObjects[i].activeInHierarchy)
                {
                    continue;
                }

                worldBounds.Encapsulate(Utility.GetBoundsFromGameObject(sceneObjects[i]));
            } 

            return worldBounds;
        }

        public void AddItemInstance(int ID, GameObject go)
        {
            if (cellQuadTree == null || go == null) 
            {
                return;
            }

            Rect positionRect = new Rect(new Vector2(go.transform.position.x, go.transform.position.z), Vector2.zero);

            List<MegaWorldCell> overlapCellList = new List<MegaWorldCell>();                 
            cellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].index;
                if (cellList.Count > cellIndex)
                {
                    cellList[cellIndex].AddItemInstance(ID, go);
                }
            }
        }

        public void RemoveNullGameObjects(bool forModifiedСells)
        {
            if(forModifiedСells)
            {
                for (int i = 0; i < modifiedСells.Count; i++)
                {                  
                    foreach (GameObjectInfo gameObjectInfo in modifiedСells[i].persistentInfoList)
                    {   
                        gameObjectInfo.itemList.RemoveAll((go) => go == null);
                    }
                }

                modifiedСells.Clear();
            }
            else
            {
                for (int i = 0; i < cellList.Count; i++)
                {                  
                    foreach (GameObjectInfo gameObjectInfo in cellList[i].persistentInfoList)
                    {
                        gameObjectInfo.itemList.RemoveAll((go) => go == null);
                    }
                }
            }
        }

        public void RepositionObject(GameObject go, int ID, int cellIndex)
        {
            AddItemInstance(ID, go);

            foreach (GameObjectInfo gameObjectInfo in cellList[cellIndex].persistentInfoList)
            {
                if(gameObjectInfo.ID == ID)
                {
                    gameObjectInfo.itemList.Remove(go);
                    break;
                }
            }
        }

        public void IntersectBounds(Bounds bounds, Func<GameObjectInfo, int, bool> func)
        {
            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<MegaWorldCell> overlapCellList = new List<MegaWorldCell>();                 
            MegaWorldPath.GeneralDataPackage.StorageCells.cellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].index;

                List<GameObjectInfo> persistentInfoList = MegaWorldPath.GeneralDataPackage.StorageCells.cellList[cellIndex].persistentInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    GameObjectInfo gameObjectInfo = persistentInfoList[persistentInfoIndex];

                    func.Invoke(gameObjectInfo, cellIndex);
                }
            }
        }

#if UNITY_EDITOR
        public void ShowCells()
        {
            Handles.color = Color.yellow;

            for (int i = 0; i < cellList.Count; i++)
            {                  
                Handles.DrawWireCube(cellList[i].bounds.center, cellList[i].bounds.size);
            }
        }
#endif
        
    }

    [Serializable]
    public class GameObjectInfo
    {
        public int ID;

        [SerializeField]
        public List<GameObject> itemList = new List<GameObject>();

        public void AddPersistentItemInstance(ref GameObject go)
        {
            itemList.Add(go);
        }
    }

    [Serializable]
    public class MegaWorldCell : IHasRect
    {
        public Bounds bounds;
        public List<GameObjectInfo> persistentInfoList = new List<GameObjectInfo>();
        public int index;

        public MegaWorldCell(Bounds bounds)
        {
            this.bounds = bounds;
        }

        public Rect Rectangle
        {
            get
            {
                return RectExtension.CreateRectFromBounds(bounds);
            }
            set
            {
                bounds = RectExtension.CreateBoundsFromRect(value);
            }
        }

        public void AddItemInstance(int ID, GameObject go)
        {
            GameObjectInfo persistentInfo = GetPersistentInfo(ID);
            if (persistentInfo == null)
            {
                persistentInfo = new GameObjectInfo 
                {
                    ID = ID
                };
                persistentInfoList.Add(persistentInfo);
            }

            persistentInfo.AddPersistentItemInstance(ref go);
        }

        public GameObjectInfo GetPersistentInfo(int ID)
        {
            for (int i = 0; i <= persistentInfoList.Count - 1; i++)
            {
                if (persistentInfoList[i].ID == ID) 
                {
                    return persistentInfoList[i];
                }
            }

            return null;
        }

        public void RemoveNullGameObjects()
        {
            foreach (GameObjectInfo gameObjectInfo in persistentInfoList)
            {   
                gameObjectInfo.itemList.RemoveAll((go) => go == null);
            }
        }

        public void RemoveItemInstances(int ID)
        {
            GameObjectInfo persistentInfo = GetPersistentInfo(ID);
            if (persistentInfo != null)
            {
                persistentInfo.itemList.Clear();
                persistentInfo.itemList.Capacity = 0;
            }
        }
    }
}