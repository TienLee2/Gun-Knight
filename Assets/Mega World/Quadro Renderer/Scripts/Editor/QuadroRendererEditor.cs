using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(QuadroRenderer))]
    public partial class QuadroRendererEditor : Editor
    {
        private QuadroRenderer quadroRenderer;

		public bool selectLODSettingsFoldout = true;
		public bool selectFrustumCullingSettingsFoldout = true;
		public bool selectShadowsSettingsFoldout = true;
		public bool selectLODShadowFoldout = true;
		public bool selectPrototypesWindowFoldout = true;
		public bool selectPrototypeFoldout = true;
		public bool selectGeneralRenderSettingsFoldout = true;
		public bool selectRenderModelDataFoldout = true;
		public bool selectSceneFoldout = true;
		public bool selectActionsFoldout = true;
		public bool selectAddComponent = true;

		void SetSceneDirty()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(quadroRenderer.gameObject.scene);
                EditorUtility.SetDirty(quadroRenderer);
            }
        }

        void OnEnable()
        {
            quadroRenderer = (QuadroRenderer)target;
        }

        public override void OnInspectorGUI()
        {			
			quadroRenderer.QuadroPrototypesPackage.SelectedVariables.RefreshSelectedParameters(quadroRenderer.QuadroPrototypesPackage);

			if(quadroRenderer.isActiveAndEnabled == false)
            {
                CustomEditorGUI.WarningBox("Please do not deactivate the component, otherwise there may be errors. If you don't need this component, just remove it.");
                return;
            }

			CustomEditorGUI.BeginChangeCheck();

			CustomEditorGUI.isInspector = true;

            DrawQuadroRendererSystemSettings();

            if (CustomEditorGUI.EndChangeCheck())
            {
				SetSceneDirty();
                EditorUtility.SetDirty(quadroRenderer);
            }
        }

        public void DrawQuadroRendererSystemSettings()
        {			
			CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.fxhu423jjezs");

			SceneSettings();
			
			selectPrototypesWindowFoldout = CustomEditorGUI.Foldout(selectPrototypesWindowFoldout, "Prototypes");

			if(selectPrototypesWindowFoldout)
			{
				EditorGUI.indentLevel++;

				DrawSelectedPrototypeWindow();

				EditorGUI.indentLevel--;
			}
		
			if(quadroRenderer.QuadroPrototypesPackage.SelectedVariables.SelectedProtoIndexList.Count == 1)
			{
				QuadroPrototype selectedPrototype = quadroRenderer.QuadroPrototypesPackage.PrototypeList[quadroRenderer.QuadroPrototypesPackage.SelectedVariables.SelectedProtoIndexList[0]];
				RenderModelInfo renderModelInfo = quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[quadroRenderer.QuadroPrototypesPackage.SelectedVariables.SelectedProtoIndexList[0]];

				selectPrototypeFoldout = CustomEditorGUI.Foldout(selectPrototypeFoldout, selectedPrototype.PrefabObject.name);

				if(selectPrototypeFoldout)
				{
					EditorGUI.indentLevel++;

					DrawWarningTextPrototype(selectedPrototype);
					DrawPrototypeSettings(selectedPrototype, renderModelInfo);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Multi-object editing not supported.");
			}

			DrawActions();
        }

		public void DrawSelectedPrototypeWindow()
        {
            InternalDragAndDrop.OnBeginGUI();

            QuadroRenderEditorSelectedEditor.DrawSelectedWindowForPrototypes(quadroRenderer);
    
    		InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
    		{
    			Repaint();
    		}
        }

		public void SceneSettings()
		{
			selectSceneFoldout = CustomEditorGUI.Foldout(selectSceneFoldout, "Scene Settings");

			if(selectSceneFoldout)
			{
				EditorGUI.indentLevel++;

				GUILayout.BeginVertical();
            	{
					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

						if(CustomEditorGUI.ToggleButton("Quality", quadroRenderer.CurrentTab == CurrentTab.QualitySettings ? true : false, ButtonStyle.General))
						{
							quadroRenderer.CurrentTab = CurrentTab.QualitySettings;
						}

						GUILayout.Space(3);

						if(CustomEditorGUI.ToggleButton("Cameras", quadroRenderer.CurrentTab == CurrentTab.CamerasSettings ? true : false, ButtonStyle.General))
						{
							quadroRenderer.CurrentTab = CurrentTab.CamerasSettings;
						}

						GUILayout.Space(5);
					}
            		GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.Space(5);

				switch (quadroRenderer.CurrentTab)
				{
					case CurrentTab.QualitySettings:
					{
						DrawQualitySettings();
						break;
					}
					case CurrentTab.CamerasSettings:
					{
						DrawCamerasSettings();
						break;
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		public void DrawWarningTextPrototype(QuadroPrototype selectedPrototype)
		{
			if (!string.IsNullOrEmpty(selectedPrototype.WarningText))
            {
                EditorGUILayout.HelpBox(selectedPrototype.WarningText, MessageType.Error);
				
                if (selectedPrototype.WarningText.StartsWith("Can not create instanced version for shader"))
                {
					GUILayout.BeginHorizontal();
                	{
			    		GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
			    		if(CustomEditorGUI.ClickButton("Go to Unity Archive", ButtonStyle.Add))
			    		{
			    			Application.OpenURL("https://unity3d.com/get-unity/download/archive");
			    		}
			    		GUILayout.Space(5);
			    	}
			    	GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                }
            }
		}

		public void DrawPrototypeSettings(QuadroPrototype selectedPrototype, RenderModelInfo renderModelInfo)
        {
            if (selectedPrototype == null)
                return;

			CustomEditorGUI.BeginChangeCheck();

			EditorGUI.BeginDisabledGroup(true);
            CustomEditorGUI.ObjectField(new GUIContent("Prototype"), selectedPrototype, typeof(QuadroPrototype)); 
            EditorGUI.EndDisabledGroup();

			if(QuadroRendererConstants.QuadroRendererSettings.ShowRenderModelData)
			{
				DrawRenderModelSettings(renderModelInfo); 
			}
			
			DrawRenderSettings(selectedPrototype, renderModelInfo);

			if(CustomEditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(selectedPrototype);
			}
        }

		public void DrawRenderSettings(QuadroPrototype selectedPrototype, RenderModelInfo renderModelInfo)
		{
			DrawGeneralRenderSettings(selectedPrototype);
			DrawShadowsSettings(selectedPrototype, renderModelInfo);
			DrawCullingSettings(selectedPrototype);
			DrawLODSettings(selectedPrototype, renderModelInfo);
		}

		public void DrawGeneralRenderSettings(QuadroPrototype selectedPrototype)
		{
			selectGeneralRenderSettingsFoldout = CustomEditorGUI.Foldout(selectGeneralRenderSettingsFoldout, "General Render Settings");

			if(selectGeneralRenderSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				selectedPrototype.GeneralSettings.ActiveRenderer = CustomEditorGUI.Toggle(activeProtoRenderer, selectedPrototype.GeneralSettings.ActiveRenderer);
            	selectedPrototype.GeneralSettings.RenderMode = (RenderMode)CustomEditorGUI.EnumPopup(renderMode, selectedPrototype.GeneralSettings.RenderMode);
				selectedPrototype.GeneralSettings.LightProbeUsage = (LightProbeUsage)CustomEditorGUI.EnumPopup(lightProbeUsage, selectedPrototype.GeneralSettings.LightProbeUsage);

				EditorGUI.indentLevel--;
			}
		}

		public void DrawShadowsSettings(QuadroPrototype selectedPrototype, RenderModelInfo renderModelInfo)
		{
			selectShadowsSettingsFoldout = CustomEditorGUI.Foldout(selectShadowsSettingsFoldout, "Shadows Settings");

			if(selectShadowsSettingsFoldout)
			{
				EditorGUI.indentLevel++;

            	selectedPrototype.ShadowsSettings.IsShadowCasting = CustomEditorGUI.Toggle(isShadowCasting, selectedPrototype.ShadowsSettings.IsShadowCasting);

				if(selectedPrototype.ShadowsSettings.IsShadowCasting && quadroRenderer.QualitySettings.IsShadowCasting)
				{	
					selectedPrototype.ShadowsSettings.UseCustomShadowDistance = CustomEditorGUI.Toggle(useCustomShadowDistance, selectedPrototype.ShadowsSettings.UseCustomShadowDistance);
                	if(selectedPrototype.ShadowsSettings.UseCustomShadowDistance)
                	{
                	    selectedPrototype.ShadowsSettings.ShadowDistance = CustomEditorGUI.FloatField(shadowDistance, selectedPrototype.ShadowsSettings.ShadowDistance);
                	}

					if(selectedPrototype.PrefabObject != null && renderModelInfo.LODs.Count > 1)
                	{
						selectLODShadowFoldout = CustomEditorGUI.Foldout(selectLODShadowFoldout, "LOD Shadow Settings");

						if(selectLODShadowFoldout)
						{
							EditorGUI.indentLevel++;

							List<GUIContent> optionsList = LODs.GetRange(0, renderModelInfo.LODs.Count);
                	    	GUIContent[] options = optionsList.ToArray();
                	    	int index = 0;
                	    	for(int i = 0; i < renderModelInfo.LODs.Count; i++)
                	    	{
								index = i;

                	    	    int lodIndex = (int)selectedPrototype.ShadowsSettings.ShadowLODMap[index];

								selectedPrototype.ShadowsSettings.DrawShadowLODMap[index] = CustomEditorGUI.Toggle(shadowCastingLODs[i], selectedPrototype.ShadowsSettings.DrawShadowLODMap[index]);

								if(selectedPrototype.ShadowsSettings.DrawShadowLODMap[index] == true)
								{
									EditorGUI.indentLevel++;

									selectedPrototype.ShadowsSettings.ShadowLODMap[index] = CustomEditorGUI.Popup(shadowLODMap, lodIndex >= options.Length ? options.Length - 1 : lodIndex, options);

									selectedPrototype.ShadowsSettings.ShadowWithOriginalShaderLODMap[index] = CustomEditorGUI.Toggle(shadowWithOriginalShader, selectedPrototype.ShadowsSettings.ShadowWithOriginalShaderLODMap[index]);

									EditorGUI.indentLevel--;
								}
                	    	}

							EditorGUI.indentLevel--;
						}
                	}
				}

				EditorGUI.indentLevel--;
			}
		}

		public void DrawCullingSettings(QuadroPrototype selectedPrototype)
		{
			selectFrustumCullingSettingsFoldout = CustomEditorGUI.Foldout(selectFrustumCullingSettingsFoldout, "Culling Settings");

			if(selectFrustumCullingSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				selectedPrototype.CullingSettings.MaxDistance = CustomEditorGUI.FloatField(maxDistance, selectedPrototype.CullingSettings.MaxDistance);
				selectedPrototype.CullingSettings.IsFrustumCulling = CustomEditorGUI.Toggle(isFrustumCulling, selectedPrototype.CullingSettings.IsFrustumCulling);

				if(selectedPrototype.CullingSettings.IsFrustumCulling)
				{
					selectedPrototype.CullingSettings.IncreaseBoundingSphere = Mathf.Max(0, CustomEditorGUI.FloatField(increaseBoundingSphere, selectedPrototype.CullingSettings.IncreaseBoundingSphere));

					if (selectedPrototype.ShadowsSettings.IsShadowCasting && quadroRenderer.QualitySettings.IsShadowCasting)
            		{
            		    selectedPrototype.CullingSettings.GetAdditionalShadow = (GetAdditionalShadow)CustomEditorGUI.EnumPopup(getAdditionalShadow, selectedPrototype.CullingSettings.GetAdditionalShadow);

						EditorGUI.indentLevel++;

						switch (selectedPrototype.CullingSettings.GetAdditionalShadow)
						{
							case GetAdditionalShadow.MinCullingDistance:
							{
								selectedPrototype.CullingSettings.MinCullingDistance = CustomEditorGUI.FloatField(MinCullingDistance, selectedPrototype.CullingSettings.MinCullingDistance);
								break;
							}
							case GetAdditionalShadow.IncreaseBoundingSphere:
							{
								selectedPrototype.CullingSettings.IncreaseShadowsBoundingSphere = Mathf.Max(0, CustomEditorGUI.FloatField(increaseShadowsBoundingSphere, 
									selectedPrototype.CullingSettings.IncreaseShadowsBoundingSphere));

								break;
							}
							case GetAdditionalShadow.DirectionLightShadowVisible:
							{
								if(quadroRenderer.QualitySettings.DirectionalLight == null)
								{
									GUILayout.BeginHorizontal();
         							{
										GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
										if(CustomEditorGUI.ClickButton("Find Directional Light", ButtonStyle.Add))
										{
         							        quadroRenderer.FindDirectionalLight();
										}
										GUILayout.Space(5);
									}
									GUILayout.EndHorizontal();

									GUILayout.Space(5);

									quadroRenderer.QualitySettings.DirectionalLight = (Light)CustomEditorGUI.ObjectField(directionalLight, quadroRenderer.QualitySettings.DirectionalLight == null, 
										quadroRenderer.QualitySettings.DirectionalLight, typeof(Light));
								}
								else
								{
            						quadroRenderer.QualitySettings.DirectionalLight = (Light)CustomEditorGUI.ObjectField(directionalLight, quadroRenderer.QualitySettings.DirectionalLight, typeof(Light));
								}
								
								break;
							}
						}

						EditorGUI.indentLevel--;
            		}
				}

				EditorGUI.indentLevel--;
			}
		}

		public void DrawQualitySettings()
		{
			CustomEditorGUI.BeginChangeCheck();

			quadroRenderer.QualitySettings.ActiveRenderer = CustomEditorGUI.Toggle(activeRenderer, quadroRenderer.QualitySettings.ActiveRenderer);

			quadroRenderer.QualitySettings.IsShadowCasting = CustomEditorGUI.Toggle(isShadowCasting, quadroRenderer.QualitySettings.IsShadowCasting);

            quadroRenderer.QualitySettings.MaxRenderDistance = CustomEditorGUI.FloatField(maxRenderDistance, quadroRenderer.QualitySettings.MaxRenderDistance);
			
			quadroRenderer.QualitySettings.LodBias = CustomEditorGUI.Slider(lodBias, quadroRenderer.QualitySettings.LodBias, 0.1f, 5);

			quadroRenderer.TransformOfFloatingOrigin = (Transform)CustomEditorGUI.ObjectField(transformOfFloatingOrigin, quadroRenderer.TransformOfFloatingOrigin, typeof(Transform));

			if (CustomEditorGUI.EndChangeCheck())
            {
                SetSceneDirty();
            }
		}

		public void DrawCamerasSettings()
        {
            CustomEditorGUI.BeginChangeCheck();

            Camera newCamera = (Camera)CustomEditorGUI.ObjectField(new GUIContent("Add Camera"), null, typeof(Camera));

            if (CustomEditorGUI.EndChangeCheck())
            {
                if (newCamera)
                {
                    quadroRenderer.AddCamera(newCamera);
                    SetSceneDirty();
                }
            }

			CustomEditorGUI.BeginChangeCheck();

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           		if(CustomEditorGUI.ClickButton("Find Main Camera"))
				{
					quadroRenderer.AutoFindMainCamera();
				}
				GUILayout.Space(5);
				if(CustomEditorGUI.ClickButton("Find All Camera"))
				{
					quadroRenderer.FindAllCamera();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 

			if (Application.isPlaying == false)
            {
				GUILayout.Space(3);

				if(quadroRenderer.PlayModeSimulation.SimulateAtEditor == false)
				{
					GUILayout.BeginHorizontal();
           			{
           			    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           				if(CustomEditorGUI.ClickButton("Enable Play Mode Simulation", ButtonStyle.Add))
						{
							quadroRenderer.PlayModeSimulation.StartSimulation();
							quadroRenderer.SetInstanceInfoDirty();
						}
           			    GUILayout.Space(3);
           			}
           			GUILayout.EndHorizontal(); 
				}
				else
				{
					GUILayout.BeginHorizontal();
           			{
           			    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           				if(CustomEditorGUI.ClickButton("Disable Play Mode Simulation", ButtonStyle.Remove))
						{
							quadroRenderer.PlayModeSimulation.StopSimulation();
							quadroRenderer.SetInstanceInfoDirty();
						}
           			    GUILayout.Space(3);
           			}
           			GUILayout.EndHorizontal(); 
				}
			}

			bool multipleCameras;
            if (Application.isPlaying)
            {
                multipleCameras = quadroRenderer.QuadroRendererCamera.Count > 1;
            }
            else
            {
                multipleCameras = quadroRenderer.QuadroRendererCamera.Count > 2;
            }

            for (int i = 0; i < quadroRenderer.QuadroRendererCamera.Count; i++)
            {
				QuadroRendererCamera quadroRendererCamera = quadroRenderer.QuadroRendererCamera[i];

				if(quadroRendererCamera.Camera == null)
				{ 
					quadroRendererCamera.FoldoutGUI = CustomEditorGUI.Foldout(quadroRendererCamera.FoldoutGUI, "Camera: NULL");
					continue;
				}

                bool sceneViewCamera = quadroRendererCamera.CameraType == CameraType.SceneView;

				bool cameraEnabled = quadroRenderer.IsCameraActive(quadroRendererCamera);

				string cameraName = quadroRendererCamera.Camera.gameObject.name;

				string foldoutText = string.Format("Camera: {0} {1}",
            	!string.IsNullOrEmpty(cameraName) ? cameraName : i.ToString(),
            	cameraEnabled ? string.Empty : "[Ignored]");

				if(sceneViewCamera)
				{
					quadroRendererCamera.FoldoutGUI = CustomEditorGUI.Foldout(quadroRendererCamera.FoldoutGUI, foldoutText);
				}
				else
				{
					bool clickRemove = false;
					quadroRendererCamera.FoldoutGUI = CustomEditorGUI.FoldoutRemove(quadroRendererCamera.FoldoutGUI, foldoutText, () => 
            		{
            		    quadroRenderer.RemoveCamera(quadroRendererCamera.Camera);
                		SetSceneDirty();
						clickRemove = true;
            		});

					if(clickRemove == true)
					{
						return;
					}
				}
				
				if(quadroRendererCamera.FoldoutGUI)
				{
					EditorGUI.indentLevel++;

					EditorGUI.BeginDisabledGroup(true);
               		CustomEditorGUI.ObjectField(new GUIContent("Camera"), quadroRendererCamera.Camera, typeof(Camera));
               		EditorGUI.EndDisabledGroup();

					if (!sceneViewCamera)
					{
						if (multipleCameras && QuadroRendererConstants.QuadroRendererSettings.RenderSceneCameraInPlayMode == false)
                   		{
                   		    EditorGUILayout.HelpBox(
                   		        "With multiple cameras render direct to camera should normally be enabled. This prevents cameras from seeing each other's objects.",
                   		        MessageType.Warning);
							
							QuadroRendererConstants.QuadroRendererSettings.RenderSceneCameraInPlayMode = CustomEditorGUI.Toggle(new GUIContent("Render Scene Camera in PlayMode"), 
								QuadroRendererConstants.QuadroRendererSettings.RenderSceneCameraInPlayMode);
                   		}

						quadroRendererCamera.Enabled = CustomEditorGUI.Toggle(enabled, quadroRendererCamera.Enabled);
						quadroRendererCamera.IsShadowCasting = CustomEditorGUI.Toggle(isShadowCasting, quadroRendererCamera.IsShadowCasting);
						quadroRendererCamera.LodBias = CustomEditorGUI.Slider(lodBias, quadroRendererCamera.LodBias, 0.1f, 1);
						quadroRendererCamera.CameraCullingMode = (CameraCullingMode)CustomEditorGUI.EnumPopup(cameraCullingMode, quadroRendererCamera.CameraCullingMode);
						quadroRendererCamera.Camera.farClipPlane = Mathf.Max(quadroRendererCamera.Camera.nearClipPlane, CustomEditorGUI.FloatField(maxCameraRenderDistance, quadroRendererCamera.Camera.farClipPlane));
						quadroRendererCamera.Camera.cullingMask = CustomEditorGUI.LayerField(cullingMask, quadroRendererCamera.Camera.cullingMask);

					}
					else
					{
                		EditorGUILayout.HelpBox("The sceneview camera is used during Edit Mode.", MessageType.Info);
					}

					EditorGUI.indentLevel--;
				}
            }

			if (CustomEditorGUI.EndChangeCheck())
            {
                SetSceneDirty();
            }
        }

		public void DrawLODSettings(QuadroPrototype proto, RenderModelInfo renderModelInfo)
        {
            if (renderModelInfo == null) return;
            if (renderModelInfo.LODs.Count < 2) return;

			selectLODSettingsFoldout = CustomEditorGUI.Foldout(selectLODSettingsFoldout, "LOD Settings");

			if(selectLODSettingsFoldout)
			{
				EditorGUI.indentLevel++;

            	if (proto.PrefabObject != null && (proto.PrefabObject.GetComponent<LODGroup>() != null || proto.BillboardSettings.UseGeneratedBillboard))
            	{
					if(QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline())
					{
						proto.LODSettings.SetLODFade(renderModelInfo, CustomEditorGUI.Toggle(LODFade, proto.LODSettings.IsLODCrossFade));

                    	if (proto.LODSettings.IsLODCrossFade)
                    	{
							proto.LODSettings.LodFadeForLastLOD = CustomEditorGUI.Toggle(lodFadeForLastLOD, proto.LODSettings.LodFadeForLastLOD); 
                    	    proto.LODSettings.LodFadeTransitionDistance = CustomEditorGUI.Slider(lodFadeTransitionDistance, proto.LODSettings.LodFadeTransitionDistance, 0f, 20f);
                    	}

            	    	if (proto.LODSettings.IsLODCrossFade)
            	    	{
							CustomEditorGUI.HelpBox("Your shader must have the \"LOD_FADE_CROSSFADE\" keyword and also the LOD Fade algorithm in the shader. You also need the unity_LODFade variable.");
            	    	}
					}

					CustomEditorGUI.Label(new GUIContent("Number of LODs: " + renderModelInfo.LODs.Count));

            		proto.LODSettings.LodBias = CustomEditorGUI.Slider(new GUIContent("LOD Bias"), proto.LODSettings.LodBias, 0.1f, 5f);

					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(EditorGUI.indentLevel * 15);
						LODGUI.DrawLODSettingsStack(proto, renderModelInfo, quadroRenderer);
					}
					GUILayout.EndHorizontal();

					if(UnityEngine.QualitySettings.lodBias != 1)
					{
						EditorGUILayout.HelpBox("Active QualitySettings.lodBias is " + UnityEngine.QualitySettings.lodBias.ToString("F2") + ". Distances are adjusted accordingly.", MessageType.Warning);
					}
					else
					{
						GUILayout.Space(3);
					}
            	}

				EditorGUI.indentLevel--;
			}
		}
		
		//This function is for debugging
		public void DrawRenderModelSettings(RenderModelInfo renderModelInfo)
		{
			selectRenderModelDataFoldout = CustomEditorGUI.Foldout(selectRenderModelDataFoldout, "Render Model Data");

			if(selectRenderModelDataFoldout)
			{
				EditorGUI.indentLevel++;
				
				for (int lodIndex = 0; lodIndex < renderModelInfo.LODs.Count; lodIndex++)
				{
					LOD lod = renderModelInfo.LODs[lodIndex];

					CustomEditorGUI.Label(new GUIContent("LOD Index: " + lodIndex));

					lod.Mesh = (Mesh)CustomEditorGUI.ObjectField(new GUIContent("Mesh"), lod.Mesh, typeof(Mesh));

					for (int matIndex = 0; matIndex < lod.Materials.Count; matIndex++)
					{
						lod.Materials[matIndex] = (Material)CustomEditorGUI.ObjectField(new GUIContent("Material"), lod.Materials[matIndex], typeof(Material));
					}
				}

				EditorGUI.indentLevel--;
			}
		}

        [MenuItem("GameObject/Quadro Renderer/Add Quadro Renderer", false, 14)]
    	public static void AddQuadroRenderer(MenuCommand menuCommand)
    	{
    		//Create the spawner
    		GameObject GameObjectQuadroRenderer = new GameObject("Quadro Renderer");
            QuadroRenderer quadroRenderer = GameObjectQuadroRenderer.AddComponent<QuadroRenderer>();
			TerrainManager terrainManager = GameObjectQuadroRenderer.AddComponent<TerrainManager>();
			terrainManager.AddAllUnityTerrains();
            terrainManager.AddAllPolarisTerrains();
			StorageTerrainCells storageTerrainCells = GameObjectQuadroRenderer.AddComponent<StorageTerrainCells>();
			storageTerrainCells.CreateCells();
			TerrainCellsOcclusionCulling cellsOcclusionCulling = GameObjectQuadroRenderer.AddComponent<TerrainCellsOcclusionCulling>();
			cellsOcclusionCulling.CreateCells();
			quadroRenderer.CreateColliderSystem();
			quadroRenderer.DetectAdditionalData();
    	}

		#region Quality
		public GUIContent activeRenderer = new GUIContent("Active Renderer", "If disabled, no rendering will occur");
		public GUIContent isShadowCasting = new GUIContent("Is Shadow Casting", "Sets whether to cast shadows from objects. If this parameter is disabled, then you can increase the optimization");
		public GUIContent maxRenderDistance = new GUIContent("Max Render Distance", "Sets the maximum distance from the camera that prototypes will be rendered. This setting sets the render distance limit for all prototypes.");
		public GUIContent transformOfFloatingOrigin = new GUIContent("Transform of Floating Origin", "When using floating origin Quadro Renderer needs to know what object defines the root of the world. Quadro Renderer calculates an offset using this transform and applies it in the renderloop at no extra render cost.");
		#endregion Quality

		#region Camera
		public GUIContent enabled = new GUIContent("Enabled", "If disabled, this camera will not be responsible for rendering objects.");
		public GUIContent cullingMask = new GUIContent("Culling Mask", "Includes or omits layers of objects to be rendered by the Camera.");
		public GUIContent cameraCullingMode = new GUIContent("Camera Culling Mode", "Camera culling mode will decide how the camera does culling of objects.");
		public GUIContent maxCameraRenderDistance = new GUIContent("Max Render Distance", "Sets the maximum render distance for this camera.");

		#endregion Camera

		#region General Renderer Settings
		public GUIContent activeProtoRenderer = new GUIContent("Active Renderer", "If disabled, no rendering will occur for this Prototype");
		public GUIContent renderMode = new GUIContent("Render Mode", "Sets how the prototype will be rendered.");
		public GUIContent lightProbeUsage = new GUIContent("Light Probes", "Set how this Renderer receives light from the Light Probe system.");
		#endregion General Renderer Settings

		#region Shadows
        public GUIContent useCustomShadowDistance = new GUIContent("Use Custom Shadow Distance", "If this option is enabled, you can set a custom shadow distance for this prototype. This is particularly helpful since shadows can have the hugest impact on performance. If, for example, your scene fog is blurring objects further away from the camera, then using using a smaller shadow distance can increase the performance while not changing much of visual quality. If this option is not enabled, then Quadro Renderer will use the shadow distance defined in Unity's quality settings.");
		public GUIContent shadowLODMap = new GUIContent("Shadow", "These options appear if the registered prefab has an LOD Group on it. There will be as many options showing here as there are LOD levels on the LOD Group. Using the dropdowns, you can select which LOD level to render the shadows from for each LOD individual level; or choose \"None\" if you don't wish to render shadows for a specific LOD Group. Managing these options correctly can increase the scene performance greatly.");
		public GUIContent shadowWithOriginalShader = new GUIContent("Shadow with Original Shader", "Using this shader helps increase the shadow performance slightly more; especially if the original shader is using a shadow pass generated by a surface shader (using the addshadow directive). In most cases this shader is enough to render the prototype flawlessly; however, if the original shader uses vertex animation and/or alpha cutoff, then the shadows might not look right. Typical cases of this are when adding tree prefabs. In these cases you can enable this option to use the shadow pass of the original shaders of the prefab and the shadows should then look right.");
        public GUIContent shadowDistance = new GUIContent("Shadow Distance");

		public static List<GUIContent> LODs = new List<GUIContent> 
		{
            new GUIContent("LOD 0"), new GUIContent("LOD 1"), new GUIContent("LOD 2"), new GUIContent("LOD 3"),
            new GUIContent("LOD 4"), new GUIContent("LOD 5"), new GUIContent("LOD 6"), new GUIContent("LOD 7")
        };

		public static List<GUIContent> shadowCastingLODs = new List<GUIContent> 
		{
            new GUIContent("LOD 0 Shadow Casting"), new GUIContent("LOD 1 Shadow Casting"), new GUIContent("LOD 2 Shadow Casting"), new GUIContent("LOD 3 Shadow Casting"),
            new GUIContent("LOD 4 Shadow Casting"), new GUIContent("LOD 5 Shadow Casting"), new GUIContent("LOD 6 Shadow Casting"), new GUIContent("LOD 7 Shadow Casting")
        };

        #endregion Shadows



		#region Culling
		public GUIContent maxDistance = new GUIContent("Max Distance", "Defines maximum distance from the camera within which this prototype will be rendered.");
		public GUIContent isFrustumCulling = new GUIContent("Is Frustum Culling", "Specifies whether the objects that are not in the selected camera's view frustum will be rendered or not. If enabled, Quadro Renderer will not render the objects that are outside the selected camera's view frustum. This will increase performance. It is recommended to turn frustum culling on unless there are multiple cameras rendering the scene at the same time.");
		public GUIContent increaseBoundingSphere = new GUIContent("Increase Bounding Sphere", "Objects have a Bounding Box or Bounding Sphere, this can be used for Frustum Culling to determine if the camera can see the object. The Quadro Renderer uses the Bounding Sphere. The Increase Bounding Sphere parameter allows you to increase the Bounding Sphere, this is necessary so that the object does not disappear when the object is expected to have more Scale, and also if the shader bends the object too much.");
        public GUIContent getAdditionalShadow = new GUIContent("Get Additional Shadow", "Allows you to choose how the shadows appear when the shadows are not in Camera Frustum. This parameter is necessary so that you can see the shadows behind the camera, mainly this parameter is necessary when the sun is not directed straight down.");
		public GUIContent MinCullingDistance = new GUIContent("Min Culling Distance", "Defines the minimum distance that any kind of culling will occur. If it is a value higher than 0, the instances with a distance less than the specified value to the Camera will not be culled.");
		public GUIContent increaseShadowsBoundingSphere = new GUIContent("Increase Shadows Bounding", "The Increase Bounding Sphere parameter allows you to increase the Bounding Sphere, then the number of visible shadows becomes larger."); 
		public GUIContent directionalLight = new GUIContent("Directional Light", "This parameter is needed to know the direction of the sun's shadows, this allows only visible sun shadows to be displayed.");
		#endregion Culling

		#region LOD
		public GUIContent LODFade = new GUIContent("LOD Fade", "Enables fade style blending between the LOD levels of this prototype. This can have a minor impact on performance since during fading, both LOD levels will be rendering."); 
        public GUIContent lodFadeForLastLOD = new GUIContent("LOD Fade For Last LOD", "Allows you to use LOD Fade for the last LOD. This allows you to slightly increase FPS because LOD Fade draws additional meshes for other LODs and use LOD Fade for the very first LOD in some cases is not reputed."); 
        public GUIContent lodFadeTransitionDistance = new GUIContent("LOD Fade Transition Distance", "This allows you to adjust the distance where the LOD Fade will end.");
        public GUIContent lodBias = new GUIContent("LOD Bias", "This value effects the LOD level distances per prototype. When it is set to a value less than 1, it favors less detail. A value of more than 1 favors greater detail. You can use this to manipulate the instance LOD distances without changing LOD Group on the original prefab.");
		#endregion LOD
    }
}
