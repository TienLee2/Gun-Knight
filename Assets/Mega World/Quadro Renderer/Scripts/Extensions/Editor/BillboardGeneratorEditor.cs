using UnityEditor;
using UnityEngine;
using System.Linq;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(BillboardGenerator))]
    public class BillboardGeneratorEditor : Editor
    {
        private BillboardGenerator billboardGenerator;

		public bool selectBillboardSettingsFoldout = true;
		public bool materialSettings = true;
		public bool selectPrototypesWindowFoldout = true;
		public bool selectPrototypeFoldout = true;
        public bool selectNecessaryDataFoldout = true;

        void OnEnable()
        {
            billboardGenerator = (BillboardGenerator)target;
        }

        public override void OnInspectorGUI()
        {			
            EditorGUI.BeginChangeCheck();

			CustomEditorGUI.isInspector = true;

			if(QuadroRendererConstants.QuadroRendererSettings.IsStandardRenderPipeline() == false)
			{	
				CustomEditorGUI.WarningBox("Billboard shader does not support HDRP and LWRP. It is planned to add support for these render pipelines soon.");
				return;
			}

			if(billboardGenerator.HasAllNecessaryData() == false)
            {
                DrawNecessaryData();
				CustomEditorGUI.WarningBox("Add all the necessary data to this component");
                return;
            }

			if(billboardGenerator.QuadroRenderer.isActiveAndEnabled == false)
			{
				CustomEditorGUI.WarningBox("QuadroRender component is disabled, please enable it so that Billboard Generator can work.");
				return;
			}

            DrawQuadroRendererSettings();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(billboardGenerator);
                EditorUtility.SetDirty(billboardGenerator.QuadroRenderer);
            }
        }

        public void DrawNecessaryData()
        {			
            selectNecessaryDataFoldout = CustomEditorGUI.Foldout(selectNecessaryDataFoldout, "Necessary Data");

			if(selectNecessaryDataFoldout)
			{
				EditorGUI.indentLevel++;
				
				billboardGenerator.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), billboardGenerator.QuadroRenderer == null, billboardGenerator.QuadroRenderer, typeof(QuadroRenderer));

				EditorGUI.indentLevel--;
			}
        }

        public void DrawQuadroRendererSettings()
        {						
			selectPrototypesWindowFoldout = CustomEditorGUI.Foldout(selectPrototypesWindowFoldout, "Prototypes");

			if(selectPrototypesWindowFoldout)
			{
				EditorGUI.indentLevel++;

				DrawSelectedPrototypeWindow();

				EditorGUI.indentLevel--;
			}
		
			for (int i = 0; i < billboardGenerator.QuadroRenderer.QuadroPrototypesPackage.PrototypeList.Count; i++)
			{
				if(billboardGenerator.QuadroRenderer.QuadroPrototypesPackage.PrototypeList[i].Selected)
				{
					selectPrototypeFoldout = CustomEditorGUI.Foldout(selectPrototypeFoldout, billboardGenerator.QuadroRenderer.QuadroPrototypesPackage.PrototypeList[i].PrefabObject.name);

					if(selectPrototypeFoldout)
					{
						EditorGUI.indentLevel++;

						DrawBillboardSettings(billboardGenerator.QuadroRenderer.QuadroPrototypesPackage.PrototypeList[i], billboardGenerator.QuadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[i]);

						EditorGUI.indentLevel--;
					}
					
					break;
				}
			}
        }

        public void DrawSelectedPrototypeWindow()
        {
            InternalDragAndDrop.OnBeginGUI();

            QuadroRenderEditorSelectedEditor.DrawSelectedWindowForPrototypes(billboardGenerator.QuadroRenderer);
    
    		InternalDragAndDrop.OnEndGUI();

            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
    		{
    			Repaint();
    		}
        }

        public void DrawBillboardSettings(QuadroPrototype selectedPrototype, RenderModelInfo renderModelInfo)
        {
			selectBillboardSettingsFoldout = CustomEditorGUI.Foldout(selectBillboardSettingsFoldout, "Billboard Settings"); 

			if(selectBillboardSettingsFoldout)
			{
				EditorGUI.indentLevel++;

            	selectedPrototype.BillboardSettings.UseGeneratedBillboard = CustomEditorGUI.Toggle(new GUIContent("Use Generated Billboard"), selectedPrototype.BillboardSettings.UseGeneratedBillboard);

            	if (selectedPrototype.BillboardSettings.UseGeneratedBillboard)
            	{
					if(QuadroRendererConstants.QuadroRendererSettings.MaxLODCountSupport != renderModelInfo.LODs.Count)
					{
						selectedPrototype.BillboardSettings.BillboardGeneratorAction = (BillboardGenerationActions)CustomEditorGUI.EnumPopup(new GUIContent("Generator Action"), selectedPrototype.BillboardSettings.BillboardGeneratorAction);
					}

            	    selectedPrototype.BillboardSettings.BillboardQuality = (BillboardQuality)CustomEditorGUI.Popup(new GUIContent("Quality"), (int)selectedPrototype.BillboardSettings.BillboardQuality, selectedPrototype.BillboardSettings.TEXT_BillboardQualityOptions);

            	    switch (selectedPrototype.BillboardSettings.BillboardQuality)
            	    {
            	        case BillboardQuality.Low:
            	            selectedPrototype.BillboardSettings.AtlasResolution = 1024;
            	            break;
            	        case BillboardQuality.Mid:
            	            selectedPrototype.BillboardSettings.AtlasResolution = 2048;
            	            break;
            	        case BillboardQuality.High:
            	            selectedPrototype.BillboardSettings.AtlasResolution = 4096;
            	            break;
            	        case BillboardQuality.VeryHigh:
            	            selectedPrototype.BillboardSettings.AtlasResolution = 8192;
            	            break;
            	    }

            	    selectedPrototype.BillboardSettings.FrameCount = CustomEditorGUI.IntSlider(new GUIContent("Frame Count"), selectedPrototype.BillboardSettings.FrameCount, 8, 32);
            	    selectedPrototype.BillboardSettings.FrameCount = Mathf.NextPowerOfTwo(selectedPrototype.BillboardSettings.FrameCount);

					CustomEditorGUI.BeginChangeCheck();

					materialSettings = CustomEditorGUI.Foldout(materialSettings, "Material Settings"); 

            		if (materialSettings)
            		{
						EditorGUI.indentLevel++;

						selectedPrototype.BillboardSettings.Color = CustomEditorGUI.ColorField(new GUIContent("Color"), selectedPrototype.BillboardSettings.Color);
						selectedPrototype.BillboardSettings.TranslucencyColor = CustomEditorGUI.ColorField(new GUIContent("Translucency Color"), selectedPrototype.BillboardSettings.TranslucencyColor);

            	    	selectedPrototype.BillboardSettings.CutOffOverride = CustomEditorGUI.Slider(new GUIContent("Cutoff Override"), selectedPrototype.BillboardSettings.CutOffOverride, 0.01f, 1.0f);
						selectedPrototype.BillboardSettings.ShadowStrength = CustomEditorGUI.Slider(new GUIContent("Shadow Strength"), selectedPrototype.BillboardSettings.ShadowStrength, 0, 1);

						selectedPrototype.BillboardSettings.NormalInvert = CustomEditorGUI.Toggle(new GUIContent("Normal Invert"), selectedPrototype.BillboardSettings.NormalInvert);
						selectedPrototype.BillboardSettings.BillboardFaceCamPos = CustomEditorGUI.Toggle(new GUIContent("Face Cam. Pos."), selectedPrototype.BillboardSettings.BillboardFaceCamPos);
						selectedPrototype.BillboardSettings.UseHueVariation = CustomEditorGUI.Toggle(new GUIContent("Use Hue Variation"), selectedPrototype.BillboardSettings.UseHueVariation);

						if(selectedPrototype.BillboardSettings.UseHueVariation)
						{
							selectedPrototype.BillboardSettings.HealthyColor = CustomEditorGUI.ColorField(new GUIContent("Healthy Color"), selectedPrototype.BillboardSettings.HealthyColor);
							selectedPrototype.BillboardSettings.DryColor = CustomEditorGUI.ColorField(new GUIContent("Dry Color"), selectedPrototype.BillboardSettings.DryColor);
							selectedPrototype.BillboardSettings.ColorNoiseSpread = CustomEditorGUI.FloatField(new GUIContent("Сolor Noise Spread"), selectedPrototype.BillboardSettings.ColorNoiseSpread);
						}

						EditorGUI.indentLevel--;
					}

            		if (CustomEditorGUI.EndChangeCheck())
            		{
						BillboardUtility.UpdateBillboardMaterial(renderModelInfo.LODs.Last().Materials.Last(), selectedPrototype);
					}

            	    EditorGUI.BeginDisabledGroup(true);
            	    CustomEditorGUI.ObjectField(new GUIContent("Albedo"), selectedPrototype.BillboardSettings.AlbedoAtlasTexture, typeof(GameObject));
            	    CustomEditorGUI.ObjectField(new GUIContent("Normal"), selectedPrototype.BillboardSettings.NormalAtlasTexture, typeof(GameObject));
            	    EditorGUI.EndDisabledGroup();

            	    GUILayout.Space(10);

					if(selectedPrototype.BillboardSettings.AlbedoAtlasTexture != null && selectedPrototype.BillboardSettings.NormalAtlasTexture != null)
					{
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

            			   	if(CustomEditorGUI.ClickButton("Regenerate Billboard", ButtonStyle.Add))
							{
								selectedPrototype.BillboardSettings.DeleteBillboardTextures(selectedPrototype);
								billboardGenerator.QuadroRenderer.SetupItemModels();
                    	        EditorUtility.SetDirty(billboardGenerator.QuadroRenderer);
							}

							GUILayout.Space(5);
            			}
						GUILayout.EndHorizontal();
					}
					else
					{
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());

            			   	if(CustomEditorGUI.ClickButton("Generate Billboard", ButtonStyle.Add))
							{
								billboardGenerator.QuadroRenderer.SetupItemModels();
                    	        EditorUtility.SetDirty(billboardGenerator.QuadroRenderer);
							}

							GUILayout.Space(5);
            			}
						GUILayout.EndHorizontal();
					}

					GUILayout.Space(3);
            	}
				else
				{
            	    if (selectedPrototype.BillboardSettings.AlbedoAtlasTexture != null)
					{
						selectedPrototype.BillboardSettings.DeleteBillboardTextures(selectedPrototype);
						billboardGenerator.QuadroRenderer.SetupItemModels();
					}
				}

				EditorGUI.indentLevel--;
			}
        }
    }
}