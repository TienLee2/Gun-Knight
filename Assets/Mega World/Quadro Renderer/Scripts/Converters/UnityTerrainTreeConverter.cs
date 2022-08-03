using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public class UnityTerrainTreeConverter : MonoBehaviour
    {
        public QuadroRenderer QuadroRenderer;
        public StorageTerrainCells StorageTerrainCells;

        void OnEnable()
        {
            DetectNecessaryData();
        }

        public void ConvertAllUnityTerrainTreeToQuadroRenderer()
        {
            if(Terrain.activeTerrain != null)
            {
                TreePrototype[] treePrototypeArray = Terrain.activeTerrain.terrainData.treePrototypes;
                
                for (int treeProtoIndex = 0; treeProtoIndex < treePrototypeArray.Length; treeProtoIndex++)
                {
                    QuadroRenderer.QuadroPrototypesPackage.AddPrototype(QuadroRenderer, (GameObject)treePrototypeArray[treeProtoIndex].prefab, QuadroRenderer.QuadroRendererCamera.Count, treePrototypeArray[treeProtoIndex].prefab.GetInstanceID());
                }

                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

                    for (int treeIndex = 0; treeIndex < treeInstances.Length; treeIndex++)
                    {
                        TreeInstance tree = treeInstances[treeIndex];

                        QuadroPrototype proto = QuadroRenderer.QuadroPrototypesPackage.GetQuadroItem(treePrototypeArray[tree.prototypeIndex].prefab);

                        if(proto != null)
                        {
                            Vector3 scale = Vector3.one;

                            if(proto.TreeType == TreeType.TreeCreatorTree)
                            {
                                scale = new Vector3(tree.widthScale / 2.2f, tree.heightScale / 2.2f, tree.widthScale / 2.2f);
                            }
                            else
                            {
                                scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
                            }

                            StorageTerrainCells.AddItemInstance(proto.ID, GetWorldPosFromTerrain(terrain, tree.position), scale, Quaternion.Euler(0, tree.rotation, 0));
                        }
                    }

                    terrain.drawTreesAndFoliage = false;
                }
            }
        }

        public void ConvertUnityTerrainTreeToQuadroRenderer()
        {
            if(Terrain.activeTerrain != null)
            {
                TreePrototype[] treePrototypeArray = Terrain.activeTerrain.terrainData.treePrototypes;

                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
                    for (int treeIndex = 0; treeIndex < treeInstances.Length; treeIndex++)
                    {
                        TreeInstance tree = treeInstances[treeIndex];

                        QuadroPrototype proto = QuadroRenderer.QuadroPrototypesPackage.GetQuadroItem(treePrototypeArray[tree.prototypeIndex].prefab);

                        if(proto != null)
                        {
                            Vector3 scale = Vector3.one;

                            if(proto.TreeType == TreeType.TreeCreatorTree)
                            {
                                scale = new Vector3(tree.widthScale / 2.2f, tree.heightScale / 2.2f, tree.widthScale / 2.2f);
                            }
                            else
                            {
                                scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
                            }

                            StorageTerrainCells.AddItemInstance(proto.ID, GetWorldPosFromTerrain(terrain, tree.position), scale, Quaternion.Euler(0, tree.rotation, 0));
                        }
                    }

                    terrain.drawTreesAndFoliage = false;
                }
            }
        }

        public void ConvertQuadroRendererToUnityTerrainTree()
        {
            if(Terrain.activeTerrain != null)
            {
                List<QuadroPrototype> quadroPrototypeList = QuadroRenderer.QuadroPrototypesPackage.PrototypeList;

                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    AddTerrainTreePrototypes(terrain, quadroPrototypeList);

                    terrain.drawTreesAndFoliage = true;
                }

                for (int persistentCellIndex = 0; persistentCellIndex < StorageTerrainCells.PersistentStoragePackage.CellList.Count; persistentCellIndex++)
                {
                    Cell persistentCell = StorageTerrainCells.PersistentStoragePackage.CellList[persistentCellIndex];

                    for (int persistentInfoIndex = 0; persistentInfoIndex < persistentCell.ItemInfoList.Count; persistentInfoIndex++)
                    {
                        QuadroPrototype proto = QuadroRenderer.QuadroPrototypesPackage.GetQuadroItem(persistentCell.ItemInfoList[persistentInfoIndex].ID);

                        if(proto == null)
                        {
                            continue;
                        }

                        for (int itemIndex = 0; itemIndex < persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList.Count; itemIndex++)
                        {
                            InstanceData persistentItem = persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex];

                            Terrain terrain = MegaWorld.Utility.GetTerrain(persistentItem.Position);

                            if(terrain == null)
                            {
                                return;
                            }

                            TreeInstance treeInstance = new TreeInstance();

                            Vector3 normalizedLocalPos = GetNormalizedLocalPosFromTerrain(terrain, persistentItem.Position);
                            
                            SetTreeInstanceInfo(ref treeInstance, GetUnityTerrainIndexFromQuadroID(terrain, proto), normalizedLocalPos, persistentItem, proto);

                            terrain.AddTreeInstance(treeInstance);
                        }
                    }
                }
            }
        }

        public void SetTreeInstanceInfo(ref TreeInstance treeInstance, int terrainProtoIdx, Vector3 normalizedLocalPos, InstanceData persistentItem, QuadroPrototype proto)
        {
            treeInstance.prototypeIndex = terrainProtoIdx;
            treeInstance.position = normalizedLocalPos;

            Vector3 scaleFactor = persistentItem.Scale;

            float rotationY = persistentItem.Rotation.eulerAngles.y;

            if(proto.TreeType == TreeType.TreeCreatorTree)
            {
                treeInstance.widthScale = persistentItem.Scale.x * 2.2f;
                treeInstance.heightScale = persistentItem.Scale.y * 2.2f;  
            }
            else
            {
                treeInstance.widthScale = persistentItem.Scale.x;
                treeInstance.heightScale = persistentItem.Scale.y;  
            }

            treeInstance.rotation = rotationY * (Mathf.PI / 180f);

            treeInstance.color = Color.white;
            treeInstance.lightmapColor = Color.white;
        }

        public int GetUnityTerrainIndexFromQuadroID(Terrain terrain, QuadroPrototype proto)
        {
            if(proto != null)
            {
                for (int treeIndex = 0; treeIndex < terrain.terrainData.treePrototypes.Length; treeIndex++)
                {
                    if (MegaWorld.Utility.IsSameGameObject(terrain.terrainData.treePrototypes[treeIndex].prefab, proto.PrefabObject, false))
                    {
                        return treeIndex;
                    }
                }
            }

            return 0;
        }

        public void AddTerrainTreePrototypes(Terrain terrain, List<QuadroPrototype> quadroPrototypeList)
        {
            bool found = false;

            TreePrototype newTree;
            List<TreePrototype> terrainTrees = new List<TreePrototype>(terrain.terrainData.treePrototypes);
            foreach (QuadroPrototype tree in quadroPrototypeList)
            {
                found = false;
                foreach (TreePrototype tp in terrainTrees)
                {
                    if (MegaWorld.Utility.IsSameGameObject(tp.prefab, tree.PrefabObject, false))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    newTree = new TreePrototype();
                    newTree.prefab = tree.PrefabObject;
                    terrainTrees.Add(newTree);
                }
            }
            
            terrain.terrainData.treePrototypes = terrainTrees.ToArray();
        }

        public void RemoveAllPrototypesFromTerrains()
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                List<TreePrototype> terrainTrees = new List<TreePrototype>();

                terrain.terrainData.treePrototypes = terrainTrees.ToArray();

                terrain.Flush();
            }
        }

        public void UnspawnAllTerrainTree()
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                List<TreeInstance> newTrees = new List<TreeInstance>();
                  
                terrain.terrainData.treeInstances = newTrees.ToArray();
            }
        }

        public Vector3 GetWorldPosFromTerrain(Terrain terrain, Vector3 localPosition)
        {
            Vector3 worldPosition = new Vector3(Mathf.Lerp(0f, terrain.terrainData.size.x, localPosition.x),
                Mathf.Lerp(0f, terrain.terrainData.size.y, localPosition.y),
                Mathf.Lerp(0f, terrain.terrainData.size.z, localPosition.z));

            worldPosition += terrain.GetPosition();

            return worldPosition;
        }

        public Vector3 GetNormalizedLocalPosFromTerrain(Terrain terrain, Vector3 worldPosition)
        {
            Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(worldPosition);
            return new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
        }

        public void DetectNecessaryData()
        {
            if (QuadroRenderer == null)
            {
                QuadroRenderer = GetComponent<QuadroRenderer>();
            }

            if(StorageTerrainCells == null)
            {
                StorageTerrainCells = GetComponent<StorageTerrainCells>();
            }
        }

        public bool HasAllNecessaryData()
        {
            if(QuadroRenderer == null)
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