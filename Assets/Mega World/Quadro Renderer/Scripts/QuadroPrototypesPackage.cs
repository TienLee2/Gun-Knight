using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    [Serializable]
    public class QuadroPrototypesPackage
    {
        public Vector2 PrototypeWindowsScroll = Vector2.zero;

        [SerializeField]
        public List<QuadroPrototype> PrototypeList = new List<QuadroPrototype>();
        public List<RenderModelInfo> RenderModelInfoList = new List<RenderModelInfo>();
        public List<GameObject> PrefabList;

        public SelectedVariables SelectedVariables = new SelectedVariables();

        #region Clipboard variables
        [NonSerialized] private GeneralSettings _copiedGeneralSettings = null;
        [NonSerialized] private CullingSettings _copiedCullingSettings = null;
        [NonSerialized] private ShadowsSettings _copiedShadowsSettings = null;
        [NonSerialized] private LODSettings _copiedLODSettings = null;
        [NonSerialized] private BillboardSettings _copiedBillboardSettings = null;
        #endregion

#if UNITY_EDITOR
        public void CheckPrototypeChanges(QuadroRenderer quadroRenderer, int cameraCount)
        {
            QuadroRendererConstants.QuadroRendererSettings.SetDefaultShaderBindings();

            if (PrefabList == null)
            {
                PrefabList = new List<GameObject>();
            }
                
            PrefabList.RemoveAll(p => p == null);
            PrefabList.RemoveAll(p => p.GetComponent<PrefabDataForQuadroRender>() == null);
            PrototypeList.RemoveAll(p => p == null);
            PrototypeList.RemoveAll(p => !PrefabList.Contains(p.PrefabObject));

            if (PrefabList.Count != PrototypeList.Count)
            {
                Utility.SetCurrentPrototypes(PrototypeList, PrefabList, cameraCount);
            }

            if (QuadroRendererConstants.QuadroRendererSettings != null && QuadroRendererConstants.QuadroRendererSettings.ShaderBindings != null)
            {
                QuadroRendererConstants.QuadroRendererSettings.ShaderBindings.ClearEmptyShaderInstances();
                foreach (QuadroPrototype prototype in PrototypeList)
                {
                    if (prototype.PrefabObject != null)
                    {
                        GPUInstancedIndirectShaderUtility.GenerateInstancedShadersIfNecessary(prototype); 

                        if (string.IsNullOrEmpty(prototype.WarningText))
                        {
                            if (prototype.PrefabObject.GetComponentInChildren<MeshRenderer>() == null)
                            {
                                prototype.WarningText = "Prefab object does not contain any Mesh Renderers.";
                            }
                        }
                    }
                }
            }

            if(PrototypeList.Count != RenderModelInfoList.Count)
            {
                quadroRenderer.SetupItemModels();
            }
        }
#endif

        public static void DetermineTreePrototypeType(QuadroPrototype proto)
        {
            if (proto.PrefabObject != null)
            {
                if (proto.PrefabObject.GetComponent<MeshFilter>() != null && proto.PrefabObject.GetComponent<MeshRenderer>() != null
                    && proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials != null
                    && proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials.Length > 0)
                {
                    // Tree Creator
                    if (proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name.Contains("Tree Creator"))
                    {
                        proto.TreeType = TreeType.TreeCreatorTree;
                        return;
                    }

                    // SpeedTree 7 with single renderer
                    if (proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE
                        || proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_GPUI_SPEED_TREE
                        || proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_URP)
                    {
                        proto.TreeType = TreeType.SpeedTree;
                        return;
                    }

                    // SpeedTree 8 with single renderer
                    if (proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_8 ||
                        proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_GPUI_SPEED_TREE_8 ||
                        proto.PrefabObject.GetComponent<MeshRenderer>().sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_8_URP)
                    {
                        proto.TreeType = TreeType.SpeedTree8;
                        if (QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                            ImportSpeedTree8Shader();
                        return;
                    }
                }

                if (proto.PrefabObject.GetComponent<LODGroup>() != null) // SpeedTree with LOD Group
                {
                    if (proto.PrefabObject.GetComponent<LODGroup>().GetLODs() != null && proto.PrefabObject.GetComponent<LODGroup>().GetLODs().Length > 0
                        && proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers != null && proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers.Length > 0
                        && proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials != null
                        && proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials.Length > 0)
                    {
                        // SpeedTree 7 with LOD Group
                        if (proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE ||
                            proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_GPUI_SPEED_TREE ||
                            proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_URP)
                        {
                            proto.TreeType = TreeType.SpeedTree;
                            return;
                        }

                        // SpeedTree 8 with LOD Group
                        if (proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_8 ||
                            proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_GPUI_SPEED_TREE_8 ||
                            proto.PrefabObject.GetComponent<LODGroup>().GetLODs()[0].renderers[0].sharedMaterials[0].shader.name == QuadroRendererConstants.SHADER_UNITY_SPEED_TREE_8_URP)
                        {
                            proto.TreeType = TreeType.SpeedTree8;
                            if (QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
                                ImportSpeedTree8Shader();
                            return;
                        }

                    }
                }
            }

            proto.TreeType = TreeType.None;
        }

        public static void ImportSpeedTree8Shader()
        {
#if UNITY_EDITOR
            EditorApplication.update -= ImportSpeedTree8ShaderPopup;
            EditorApplication.update += ImportSpeedTree8ShaderPopup;
#endif
        }

        public static void ImportSpeedTree8ShaderPopup()
        {
#if UNITY_EDITOR
            if (Shader.Find(QuadroRendererConstants.SHADER_GPUI_SPEED_TREE_8) == null)
            {
                if (System.IO.File.Exists(QuadroRendererConstants.GetDefaultPath() + "Extras/GPUI_SpeedTree8_Support.unitypackage"))
                {
                    if (EditorUtility.DisplayDialog("GPUI SpeedTree8 Support", "You have added a SpeedTree8 tree.\n\nDo you wish to import the GPUI support for this shader?\n\nThis operation can take some time depending on your system.", "YES", "NO"))
                    {
                        Debug.Log("GPUI is importing SpeedTree8 shader...");
                        AssetDatabase.ImportPackage(QuadroRendererConstants.GetDefaultPath() + "Extras/GPUI_SpeedTree8_Support.unitypackage", true);
                    }
                }
                else
                    Debug.LogError("GPUI can not find GPUI_SpeedTree8_Support.unitypackage");
            }
            EditorApplication.update -= ImportSpeedTree8ShaderPopup;
#endif
        }
        
        public QuadroPrototype GetQuadroItem(int ID)
        {
            for (var i = 0; i <= PrototypeList.Count - 1; i++)
            {
                if (PrototypeList[i].ID == ID) return PrototypeList[i];
            }

            return null;
        }

        public QuadroPrototype GetQuadroItem(GameObject prefab)
        {
            foreach (QuadroPrototype proto in PrototypeList)
            {
                if (MegaWorld.Utility.IsSameGameObject(prefab, proto.PrefabObject, false))
                {
                    return proto;
                }
            }

            return null;
        }

        public bool PrefabAlreadyExists(GameObject prefab)
        {
            foreach (QuadroPrototype proto in PrototypeList)
            {
                if (MegaWorld.Utility.IsSameGameObject(prefab, proto.PrefabObject, false))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddPrototype(QuadroRenderer quadroRenderer, GameObject go, int cameraCount, int ID)
        {
            if(quadroRenderer.QuadroPrototypesPackage.PrefabAlreadyExists(go))
            {
                return;
            }

            GameObject prefabObject = (GameObject)go;

#if UNITY_EDITOR
            prefabObject = SceneViewDetector.GetCorrespongingPrefabOfVariant(prefabObject);
#endif

            QuadroPrototype proto = Utility.GeneratePrefabPrototype(prefabObject, cameraCount, ID);

            if(proto != null)
            {
                if (!PrefabList.Contains(prefabObject))
                {
                    PrefabList.Add(prefabObject);
                }

#if UNITY_EDITOR 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.SetDirty(proto);
#endif

                PrototypeList.Add(proto);

                DetermineTreePrototypeType(proto);

                if(proto.TreeType == TreeType.TreeCreatorTree)
                {
                    if(quadroRenderer.BillboardGenerator == null)
                    {
                        quadroRenderer.gameObject.AddComponent<BillboardGenerator>();
			            quadroRenderer.DetectAdditionalData();
                    }

                    proto.BillboardSettings.UseGeneratedBillboard = true;
                    proto.BillboardSettings.BillboardGeneratorAction = BillboardGenerationActions.AddNewLOD;   
                    proto.BillboardSettings.NormalInvert = true;
                    proto.BillboardSettings.Color = new Color(0.83f, 0.83f, 0.83f, 1);
                }
            }
            
            quadroRenderer.RefreshQuadroRenderer();
        }

        public void DeleteAssetPrototype(QuadroPrototype prototype)
        {
            PrefabList.Remove(prototype.PrefabObject);

#if UNITY_EDITOR
            SceneViewDetector.RemoveComponentFromPrefab<PrefabDataForQuadroRender>(prototype.PrefabObject);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
#endif

        }

        public void DeleteSelectedPrototypes(QuadroRenderer quadroRenderer)
        {
            foreach (QuadroPrototype proto in PrototypeList)
            {
                if(proto.Selected)
                {
                    DeleteAssetPrototype(proto);
                }
            }

            PrototypeList.RemoveAll((proto) => proto == null);

            quadroRenderer.RefreshQuadroRenderer();
        }

        public void SelectPrototype(int prototypeIndex)
        {
            SetSelectedAllPrototypes(false);

            if(prototypeIndex < 0 && prototypeIndex >= PrototypeList.Count)
            {
                return;
            }

            PrototypeList[prototypeIndex].Selected = true;
        }

        public void SelectPrototypeAdditive(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= PrototypeList.Count)
            {
                return;
            }
        
            PrototypeList[prototypeIndex].Selected = !PrototypeList[prototypeIndex].Selected;
        }

        public void SelectPrototypeRange(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= PrototypeList.Count)
            {
                return;
            }

            int rangeMin = prototypeIndex;
            int rangeMax = prototypeIndex;

            for (int i = 0; i < PrototypeList.Count; i++)
            {
                if (PrototypeList[i].Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (PrototypeList[i].Selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                PrototypeList[i].Selected = true;
            }
        }

        public void SetSelectedAllPrototypes(bool select)
        {
            foreach (QuadroPrototype proto in PrototypeList)
            {
                proto.Selected = select;
            }
        }

        public void DisposeData()
        {
            for (int i = 0; i <= RenderModelInfoList.Count - 1; i++)
            {
                RenderModelInfoList[i]?.UnmanagedData();
            }

            RenderModelInfoList.Clear();
        }

        public void CopyAllSettingsFromQuadroPrototype(QuadroPrototype selectedProto)
		{            
            _copiedGeneralSettings = new GeneralSettings(selectedProto.GeneralSettings);
            _copiedCullingSettings = new CullingSettings(selectedProto.CullingSettings);
            _copiedShadowsSettings = new ShadowsSettings(selectedProto.ShadowsSettings);
            _copiedLODSettings = new LODSettings(selectedProto.LODSettings);
            _copiedBillboardSettings = new BillboardSettings(selectedProto.BillboardSettings);
		}

        public void PasteAllSettingsToQuadroPrototype(QuadroRenderer quadroRenderer, RenderModelInfo renderModelInfo)
		{
            PasteGeneralSettingsToQuadroPrototype();
            PasteCullingSettingsToQuadroPrototype();
            PasteShadowsSettingsToQuadroPrototype();
            PasteLODSettingsToQuadroPrototype(renderModelInfo);
            PasteBillboardSettingsToQuadroPrototype(quadroRenderer);
		}

        public void PasteGeneralSettingsToQuadroPrototype()
		{
            if (_copiedGeneralSettings == null)
            {
                return;
            }
				
            foreach(QuadroPrototype proto in PrototypeList) 
            {
                if(proto.Selected)
                {
                    proto.GeneralSettings.CopyFrom(_copiedGeneralSettings);
                }
            }
		}

        public void PasteCullingSettingsToQuadroPrototype()
		{
            if (_copiedCullingSettings == null)
            {
                return;
            }
				
            foreach(QuadroPrototype proto in PrototypeList) 
            {
                if(proto.Selected)
                {
                    proto.CullingSettings.CopyFrom(_copiedCullingSettings);
                }
            }
		}

        public void PasteShadowsSettingsToQuadroPrototype()
		{
            if (_copiedShadowsSettings == null)
            {
                return;
            }
				
            foreach(QuadroPrototype proto in PrototypeList) 
            {
                if(proto.Selected)
                {
                    proto.ShadowsSettings.CopyFrom(_copiedShadowsSettings);
                }
            }
		}

        public void PasteLODSettingsToQuadroPrototype(RenderModelInfo renderModelInfo)
		{
            if (_copiedLODSettings == null)
            {
                return;
            }
				
            foreach(QuadroPrototype proto in PrototypeList) 
            {
                if(proto.Selected)
                {
                    proto.LODSettings.CopyFrom(_copiedLODSettings, renderModelInfo);
                }
            }
		}

        public void PasteBillboardSettingsToQuadroPrototype(QuadroRenderer quadroRenderer)
		{
            if (_copiedBillboardSettings == null)
            {
                return;
            }
				
            foreach(QuadroPrototype proto in PrototypeList) 
            {
                if(proto.Selected)
                {
                    proto.BillboardSettings.CopyFrom(_copiedBillboardSettings);
                }
            }

            quadroRenderer.SetupItemModels();
		}
    }
}