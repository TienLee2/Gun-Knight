using System.Collections.Generic;
using UnityEngine;
#if GRIFFIN_2020
using Pinwheel.Griffin;
#endif

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public class TerrainManager : MonoBehaviour
    {
        public Area area;

        public List<ITerrain> TerrainList = new List<ITerrain>();
        public List<GameObject> TerrainObjectList = new List<GameObject>();    

		public bool CurrentTerrainsFoldout = false;

        public void SetupTerrainDataManager()
        {
            if (area.AreaBounds.size.magnitude < 1) return;

            RefreshTerrains();
        }

        void OnEnable()
        {
            DetectNecessaryData();

            if(HasAllNecessaryData() == false)
            {
                return;
            }

            SetupTerrainDataManager();

            DeleteNullTerrains();
        }

        public void AddTerrain(GameObject go)
        {
            //TODO only add terrains that overlap area if automatic calculation is disabled
            ITerrain terrain = Utility.GetITerrainDataHelper(go);
            if (terrain != null)
            {
                if (!TerrainObjectList.Contains(go)) TerrainObjectList.Add(go);

                RefreshTerrains();

                CalculateArea();
            }

            VerifyTerrains();
        }

        public void AddTerrains(List<GameObject> terrainList)
        {
            Bounds combinedBounds = new Bounds();
            
            for (int i = 0; i <= terrainList.Count -1; i++)
            {
                ITerrain terrain = Utility.GetITerrainDataHelper(terrainList[i]);
                if (terrain != null)
                {
                    if (!TerrainObjectList.Contains(terrainList[i])) TerrainObjectList.Add(terrainList[i]);                   
                }

                if (i == 0)
                {
                    if (terrain != null) combinedBounds = terrain.TerrainBounds;
                }
                else
                {
                    if (terrain != null)
                        combinedBounds.Encapsulate(terrain.TerrainBounds);
                }
            }
            
            RefreshTerrains();

            CalculateArea();

            VerifyTerrains();
        }

        public void AddAllUnityTerrains()
        {
            Terrain[] terrains = FindObjectsOfType<Terrain>();
            
            List<GameObject> terrainList = new List<GameObject>();
            
            for (int i = 0; i <= terrains.Length - 1; i++)
            {
                UnityTerrain unityTerrain = terrains[i].gameObject.GetComponent<UnityTerrain>();
                if (!unityTerrain)
                {
                    terrains[i].gameObject.AddComponent<UnityTerrain>();
                }

                terrainList.Add(terrains[i].gameObject);
            }
            
            AddTerrains(terrainList);
        }

        public void AddAllPolarisTerrains()
        {
#if GRIFFIN_2020
            GStylizedTerrain[] terrains = FindObjectsOfType<GStylizedTerrain>();
            
            List<GameObject> terrainList = new List<GameObject>();
            
            for (int i = 0; i <= terrains.Length - 1; i++)
            {
                PolarisTerrain terrain = terrains[i].gameObject.GetComponent<PolarisTerrain>();
                if (!terrain)
                {
                    terrains[i].gameObject.AddComponent<PolarisTerrain>();
                }

                terrainList.Add(terrains[i].gameObject);
            }
            
            AddTerrains(terrainList);
#endif
        }

        public void AddMeshTerrains()
        {
            MeshTerrain[] terrains = FindObjectsOfType<MeshTerrain>();
            for (int i = 0; i <= terrains.Length - 1; i++)
            {
                AddTerrain(terrains[i].gameObject);
            }
        }

        public void RemoveAllTerrains()
        {
            List<GameObject> tempTerrainObjectList = new List<GameObject>(); 
            tempTerrainObjectList.AddRange(TerrainObjectList);

            for (int i = 0; i <= tempTerrainObjectList.Count  - 1; i++)
            {
                RemoveTerrain(tempTerrainObjectList[i]);
            }            
        }

        public void RemoveTerrain(GameObject go)
        {
            if (TerrainObjectList.Contains(go)) TerrainObjectList.Remove(go);
            RefreshTerrains();

            ITerrain terrain = Utility.GetITerrainDataHelper(go);          
            CalculateArea();

            VerifyTerrains();
        }

        public void RefreshTerrains()
        {
            VerifyTerrains();
            TerrainList.Clear();
            for (int i = 0; i <= TerrainObjectList.Count - 1; i++)
            {
                ITerrain terrain = Utility.GetITerrainDataHelper(TerrainObjectList[i]);

                if(terrain != null)
                {
                    terrain?.Refresh();
                    TerrainList.Add(terrain);
                }
            }
        }

        public void VerifyTerrains()
        {
            while (TerrainObjectList.Contains(null))
            {
                TerrainObjectList.Remove(null);
            }
        }

        public void DeleteNullTerrains()
        {
            List<GameObject> removeTerrains = new List<GameObject>();
            for (int i = 0; i <= TerrainObjectList.Count - 1; i++)
            {
                ITerrain terrain =
                    Utility.GetITerrainDataHelper(TerrainObjectList[i]);

                if(terrain == null)
                {
                    removeTerrains.Add(TerrainObjectList[i]);
                }
            }

            foreach (GameObject terrain in removeTerrains)
            {
                TerrainObjectList.Remove(terrain);
            }
        }

        public void CalculateArea()
        {
            RefreshTerrains();

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
            for (int i = 0; i <= TerrainObjectList.Count - 1; i++)
            {
                ITerrain terrain = Utility.GetITerrainDataHelper(TerrainObjectList[i]);
                if (terrain != null)
                {
                    if (i == 0)
                    {
                        newBounds = terrain.TerrainBounds;
                    }
                    else
                    {
                        newBounds.Encapsulate(terrain.TerrainBounds);
                    }
                }
            }

            area.transform.position = newBounds.center;
            area.transform.localScale = newBounds.size;

            area.AreaBounds = newBounds;
        }

        public void DetectNecessaryData()
        {
            if(area == null)
            {
                if((Area)FindObjectOfType(typeof(Area)) != null)
                {
                    area = (Area)FindObjectOfType(typeof(Area));
                }
                else
                {
                    area = CreateAreaParent();
                    CalculateArea(); 
                }
            }
        }

        public Area CreateAreaParent()
        {
            GameObject area = new GameObject("Quadro Renderer Area");
            area.transform.localScale = new Vector3(150, 150, 150);
    		area.AddComponent<Area>();
            area.transform.SetParent(transform);

            return area.GetComponent<Area>();
        }

        public bool HasAllNecessaryData()
        {
            if(area == null)
            {
                return false;
            }

            return true;
        }
    }
}