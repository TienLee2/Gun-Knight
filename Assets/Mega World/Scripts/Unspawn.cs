using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuadroRendererSystem;

namespace MegaWorld
{
    public static class Unspawn 
    {
        public static void UnspawnAllSelectedProto()
        {
            foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
            {
                UnspawnTerrainDetail(type.ProtoTerrainDetailList, true);
                UnspawnQuadroItem(type, true);
                UnspawnGameObject(type, true);
            }
        }

        public static void UnspawnGameObject(Type type, bool unspawnSelected)
        {
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

            for (int goIndex = 0; goIndex < allGameObjects.Length; goIndex++)
            {
                PrototypeGameObject proto = Utility.GetCurrentPrototypeGameObject(type, allGameObjects[goIndex]);

                if(proto != null)
                {
                    if(unspawnSelected)
                    {
                        if(proto.selected == false)
                        {
                            continue;
                        }
                    }

                    UnityEngine.Object.DestroyImmediate(allGameObjects[goIndex]);
                }
            }

            MegaWorldPath.GeneralDataPackage.StorageCells.RemoveNullGameObjects(false);
        }

        public static void UnspawnTerrainDetail(List<PrototypeTerrainDetail> protoTerrainDetailList, bool unspawnSelected)
        {
            for (int i = 0; i < protoTerrainDetailList.Count; i++)
            {
                if(unspawnSelected)
                {
                    if(protoTerrainDetailList[i].selected == false)
                    {
                        continue;
                    }
                }

                foreach (var terrain in Terrain.activeTerrains)
                {
                    if(terrain.terrainData.detailResolution == 0)
                    {
                        continue;
                    }

                    if(protoTerrainDetailList[i].TerrainProtoId > terrain.terrainData.detailPrototypes.Length - 1)
                    {
                        Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    }
                    else
                    {
                        terrain.terrainData.SetDetailLayer(0, 0, protoTerrainDetailList[i].TerrainProtoId, new int[terrain.terrainData.detailWidth, terrain.terrainData.detailWidth]);
                    }
                }
            }
        }

        public static void UnspawnQuadroItem(Type type, bool unspawnSelected)
        {
            for (int index = 0; index < type.ProtoQuadroItemList.Count; index++)
            {
                PrototypeQuadroItem proto = type.ProtoQuadroItemList[index];

                if(unspawnSelected)
                {
                    if(proto.selected == false)
                    {
                        continue;
                    }
                }

                StorageTerrainCellsAPI.RemoveItemInstances(QuadroRendererController.StorageTerrainCells, proto.ID);
            }
        }
    }
}