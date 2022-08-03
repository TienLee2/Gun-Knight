using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaWorld
{
    public static class TerrainResourcesController
    {
        public static void AddMissingPrototypesToTerrains(Type type)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                AddMissingPrototypesToTerrain(terrain, type);
            }
        }

        public static void RemoveAllPrototypesFromTerrains(Type type)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                RemoveAllPrototypesFromTerrains(terrain, type);
            }
        }

        public static void AddMissingPrototypesToTerrain(Terrain terrain, Type type)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            switch (type.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    AddTerrainDetailToTerrain(terrain, type.ProtoTerrainDetailList);

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    AddTerrainTexturesToTerrain(terrain, type.ProtoTerrainTextureList);

                    break;
                }
            }

            terrain.Flush();

            SyncTerrainID(terrain, type);
        }

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain, Type type)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }
            
            switch (type.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    List<DetailPrototype> terrainDetails = new List<DetailPrototype>();
                    
                    terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

                    terrain.terrainData.terrainLayers = terrainLayers.ToArray();

                    break;
                }
            }

            terrain.Flush();

            SyncTerrainID(terrain, type);
        }

        public static void SetTerrainDetailSettings(Terrain activeTerrain, PrototypeTerrainDetail protoTerrainDetail)
        {
            if(activeTerrain == null)
            {
                return;
            }

            DetailPrototype[] proto = activeTerrain.terrainData.detailPrototypes;
            			
			for (int index = 0; index < proto.Length; index++)
			{
				bool found = false;

				if(protoTerrainDetail.PrefabType == PrefabType.Mesh)
				{
					if (Utility.IsSameGameObject(proto[index].prototype, protoTerrainDetail.prefab, false))
                    {
                        found = true;
                    }
				}
				else
				{
					if (Utility.IsSameTexture(proto[index].prototypeTexture, protoTerrainDetail.DetailTexture, false))
                    {
                        found = true;
                    }
				}

				if (found)
            	{
            	    proto[index].renderMode = protoTerrainDetail.TerrainDetailSettings.RenderMode;
	
					if (protoTerrainDetail.prefab != null)
            	    {
                        if(proto[index].renderMode == DetailRenderMode.GrassBillboard)
                        {
                            proto[index].renderMode = DetailRenderMode.Grass;
                        }

            	        proto[index].usePrototypeMesh = true;
            	        proto[index].prototype = protoTerrainDetail.prefab;
            	    }
            	    else
            	    {
            	        proto[index].usePrototypeMesh = false;
            	        proto[index].prototypeTexture = protoTerrainDetail.DetailTexture;
            	    }

            	    proto[index].dryColor = protoTerrainDetail.TerrainDetailSettings.DryColour;
            	    proto[index].healthyColor = protoTerrainDetail.TerrainDetailSettings.HealthyColour;
            	    proto[index].maxHeight = protoTerrainDetail.TerrainDetailSettings.MaxHeight;
            	    proto[index].maxWidth = protoTerrainDetail.TerrainDetailSettings.MaxWidth;
            	    proto[index].minHeight = protoTerrainDetail.TerrainDetailSettings.MinHeight;
            	    proto[index].minWidth = protoTerrainDetail.TerrainDetailSettings.MinWidth;
            	    proto[index].noiseSpread = protoTerrainDetail.TerrainDetailSettings.NoiseSpread;
            	}
			}

            activeTerrain.terrainData.detailPrototypes = proto;
        }

        public static void AddTerrainDetailToTerrain(Terrain terrain, List<PrototypeTerrainDetail> protoTerrainDetailList)
        {
            bool found = false;

            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
            foreach (PrototypeTerrainDetail detail in protoTerrainDetailList)
            {
                found = false;
                foreach (DetailPrototype dp in terrainDetails)
                {
                    if (dp.renderMode == detail.TerrainDetailSettings.RenderMode)
                    {
                        if (Utility.IsSameTexture(dp.prototypeTexture, detail.DetailTexture, false))
                        {
                            found = true;
                        }
                        if (Utility.IsSameGameObject(dp.prototype, detail.prefab, false))
                        {
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    newDetail = new DetailPrototype();
                    
                    if (detail.prefab != null)
                    {
                        newDetail.renderMode = detail.TerrainDetailSettings.RenderMode;
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = detail.prefab;
                    }
                    else
                    {
                        newDetail.renderMode = detail.TerrainDetailSettings.RenderMode;
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = detail.DetailTexture;
                    }

                    newDetail.dryColor = detail.TerrainDetailSettings.DryColour;
                    newDetail.healthyColor = detail.TerrainDetailSettings.HealthyColour;
                    newDetail.maxHeight = detail.TerrainDetailSettings.MaxHeight;
                    newDetail.maxWidth = detail.TerrainDetailSettings.MaxWidth;
                    newDetail.minHeight = detail.TerrainDetailSettings.MinHeight;
                    newDetail.minWidth = detail.TerrainDetailSettings.MinWidth;
                    newDetail.noiseSpread = detail.TerrainDetailSettings.NoiseSpread;
                    terrainDetails.Add(newDetail);
                }
            }

            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

            foreach (PrototypeTerrainDetail protoTerrainDetail in protoTerrainDetailList)
            {
                SetTerrainDetailSettings(terrain, protoTerrainDetail);
            }
        }

        public static void AddTerrainTexturesToTerrain(Terrain terrain, List<PrototypeTerrainTexture> protoTerrainTextureList)
        {
            bool found = false;

            TerrainLayer[] currentTerrainLayers = terrain.terrainData.terrainLayers;

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture splat in protoTerrainTextureList)
            {
                found = false;

                if(splat.TerrainLayer == null)
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (Utility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainTextureSettings.Convert());

                        RemoveTerrainLayerAssetFiles(splat.TerrainTextureSettings.DiffuseTexture.name);

                        terrainLayers[terrainLayers.Count - 1] = ProfileFactory.SaveTerrainLayerAsAsset(splat.TerrainTextureSettings.DiffuseTexture.name, terrainLayers.Last());

                        splat.TerrainLayer = terrainLayers[terrainLayers.Count - 1];
                    }
                }
                else
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (Utility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainLayer);
                    }
                }
            }

            terrain.terrainData.terrainLayers = terrainLayers.ToArray();
        }

        private static void RemoveTerrainLayerAssetFiles(string terrainName)
        {
            string megaWorldDirectory = "";
            string terrainLayerDirectory = megaWorldDirectory + "Profiles/TerrainLayers";
            DirectoryInfo info = new DirectoryInfo(terrainLayerDirectory);

            if (info.Exists)
            {
                FileInfo[] fileInfo = info.GetFiles(terrainName + "*.asset");

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    File.Delete(fileInfo[i].FullName);
                }
            }
        }

        public static void SyncTerrainID(Terrain terrain, Type type)
        {
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail detail in type.ProtoTerrainDetailList)
            {
                for (int Id = 0; Id < terrainDetails.Count; Id++)
                {
                    if (terrainDetails[Id].renderMode == detail.TerrainDetailSettings.RenderMode)
                    {
                        if (Utility.IsSameTexture(terrainDetails[Id].prototypeTexture, detail.DetailTexture, false))
                        {
                            detail.TerrainProtoId = Id;
                        }
                        if (Utility.IsSameGameObject(terrainDetails[Id].prototype, detail.prefab, false))
                        {
                            detail.TerrainProtoId = Id;
                        }
                    }
                }
            }
        }

        public static void UpdateOnlyTerrainTexture(Terrain terrain, Type type)
        {
            List<PrototypeTerrainTexture> protoTerrainTextureRemoveList = new List<PrototypeTerrainTexture>();

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture texture in type.ProtoTerrainTextureList)
            {
                bool find = false;

                for (int Id = 0; Id < terrainLayers.Count; Id++)
                {
                    if (Utility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    protoTerrainTextureRemoveList.Add(texture);
                }
            }

            foreach (PrototypeTerrainTexture proto in protoTerrainTextureRemoveList)
            {
                type.ProtoTerrainTextureList.Remove(proto);
            }

            for (int Id = 0; Id < terrainLayers.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainTexture texture in type.ProtoTerrainTextureList)
                {
                    if (Utility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    type.ProtoTerrainTextureList.Add(new PrototypeTerrainTexture(terrainLayers[Id], terrainLayers[Id].diffuseTexture.name));
                }
            }

            SyncTerrainID(terrain, type);
        }

        public static void UpdateOnlyTerrainDetail(Terrain terrain, Type type)
        {
            List<PrototypeTerrainDetail> protoTerrainDetailRemoveList = new List<PrototypeTerrainDetail>();

            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail detail in type.ProtoTerrainDetailList)
            {
                bool find = false;

                for (int Id = 0; Id < terrainDetails.Count; Id++)
                {
                    if (terrainDetails[Id].renderMode == detail.TerrainDetailSettings.RenderMode)
                    {
                        if (Utility.IsSameTexture(terrainDetails[Id].prototypeTexture, detail.DetailTexture, false))
                        {
                            find = true;
                        }
                        if (Utility.IsSameGameObject(terrainDetails[Id].prototype, detail.prefab, false))
                        {
                            find = true;
                        }
                    }
                }

                if(find == false)
                {
                    protoTerrainDetailRemoveList.Add(detail);
                }
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailRemoveList)
            {
                type.ProtoTerrainDetailList.Remove(proto);
            }

            DetailPrototype terrainDetailProto;
            PrototypeTerrainDetail resourceDetailProto;

            for (int Id = 0; Id < terrainDetails.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainDetail detail in type.ProtoTerrainDetailList)
                {
                    if (terrainDetails[Id].renderMode == detail.TerrainDetailSettings.RenderMode)
                    {
                        if (Utility.IsSameTexture(terrainDetails[Id].prototypeTexture, detail.DetailTexture, false))
                        {
                            find = true;
                        }
                        if (Utility.IsSameGameObject(terrainDetails[Id].prototype, detail.prefab, false))
                        {
                            find = true;
                        }
                    }
                }

                if(find == false)
                {
                    terrainDetailProto = terrain.terrainData.detailPrototypes[Id];

                    resourceDetailProto = new PrototypeTerrainDetail();
                    resourceDetailProto.TerrainDetailSettings.RenderMode = terrainDetailProto.renderMode;
                    if (terrainDetailProto.prototype != null)
                    {
                        resourceDetailProto.PrefabType = PrefabType.Mesh;
                        resourceDetailProto.TerrainDetailName = terrainDetailProto.prototype.name;
                        resourceDetailProto.prefab = terrainDetailProto.prototype;
                    }
                    else
                    {
                        resourceDetailProto.PrefabType = PrefabType.Texture;
                        resourceDetailProto.TerrainDetailName = terrainDetailProto.prototypeTexture.name;
                        resourceDetailProto.DetailTexture = terrainDetailProto.prototypeTexture;
                    }
    
                    resourceDetailProto.TerrainDetailSettings.DryColour = terrainDetailProto.dryColor;
                    resourceDetailProto.TerrainDetailSettings.HealthyColour = terrainDetailProto.healthyColor;
                    resourceDetailProto.TerrainDetailSettings.MaxHeight = terrainDetailProto.maxHeight;
                    resourceDetailProto.TerrainDetailSettings.MaxWidth = terrainDetailProto.maxWidth;
                    resourceDetailProto.TerrainDetailSettings.MinHeight = terrainDetailProto.minHeight;
                    resourceDetailProto.TerrainDetailSettings.MinWidth = terrainDetailProto.minWidth;
                    resourceDetailProto.TerrainDetailSettings.NoiseSpread = terrainDetailProto.noiseSpread;
    
                    type.ProtoTerrainDetailList.Add(resourceDetailProto);  
                }
            }

            SyncTerrainID(terrain, type);
        }

        public static void UpdatePrototypesFromTerrain(Terrain terrain, Type type)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Missing active terrain.");
                return;
            }

            switch (type.ResourceType)
            {
                case ResourceType.TerrainTexture:
                {
                    UpdateOnlyTerrainTexture(terrain, type);
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    UpdateOnlyTerrainDetail(terrain, type);
                    break;
                }
            }

            SyncTerrainID(terrain, type);
        }

        public static bool SpawnSupportAvailable(Type type, Terrain terrain)
        {   
            if(type == null || terrain == null)
            {
                return false;
            }

            switch (type.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    for (int i = 0; i < type.ProtoTerrainDetailList.Count; i++)
                    {
                        bool find = false;

                        for (int Id = 0; Id < terrain.terrainData.detailPrototypes.Length; Id++)
                        {
                            if (terrain.terrainData.detailPrototypes[Id].renderMode == type.ProtoTerrainDetailList[i].TerrainDetailSettings.RenderMode)
                            {
                                if (Utility.IsSameTexture(terrain.terrainData.detailPrototypes[Id].prototypeTexture, type.ProtoTerrainDetailList[i].DetailTexture, false))
                                {
                                    find = true;
                                    break;
                                }
                                if (Utility.IsSameGameObject(terrain.terrainData.detailPrototypes[Id].prototype, type.ProtoTerrainDetailList[i].prefab, false))
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }

                        if(find == false)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                case ResourceType.TerrainTexture:
                {
                    for (int i = 0; i < type.ProtoTerrainTextureList.Count; i++)
                    {
                        bool find = false;

                        for (int Id = 0; Id < terrain.terrainData.terrainLayers.Length; Id++)
                        {
                            if (Utility.IsSameTexture(terrain.terrainData.terrainLayers[Id].diffuseTexture, type.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture, false))
                            {
                                find = true;
                                break;
                            }
                        }

                        if(find == false)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public static void LogWarningWithSyncProblem(Type type)
        {
            switch (type.ResourceType)
            {
				case ResourceType.TerrainDetail:
            	{
                    Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
					break;
				}
				case ResourceType.TerrainTexture:
            	{
                    Debug.LogWarning("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
					break;
				}
			}
        }

        public static void SyncAllTerrains(Type type, Terrain terrain)
        {
            if(type == null || terrain == null)
            {
                return;
            }

            if(Terrain.activeTerrains.Length == 0)
            {
                return;
            }

            switch (type.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                    foreach (Terrain item in Terrain.activeTerrains)
                    {
                        item.terrainData.detailPrototypes = terrainDetails.ToArray();
                    }

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    List<TerrainLayer> terrainTextures = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

                    foreach (Terrain item in Terrain.activeTerrains)
                    {
                        item.terrainData.terrainLayers = terrainTextures.ToArray();
                    }

                    break;
                }
            }
        }
    }
}