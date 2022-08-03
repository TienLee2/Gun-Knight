using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(TerrainCDLODOcclusionCulling))]
    public class TerrainCDLODOcclusionCullingEditor : Editor
    {
        public bool selectOcclusionCullingDebugSettingsFoldout = true;
        public bool selectCellLODsFoldout = true;

        public TerrainCDLODOcclusionCulling terrainCDLODOcclusionCulling;

        void OnEnable()
        {
            terrainCDLODOcclusionCulling = (TerrainCDLODOcclusionCulling)target;
        }

        public override void OnInspectorGUI()
        {			
			CustomEditorGUI.isInspector = true;
			
            if(terrainCDLODOcclusionCulling.isActiveAndEnabled == false)
            {
                CustomEditorGUI.WarningBox("Please do not deactivate the component, otherwise there may be errors. If you don't need this component, just remove it.");
                return;
            }

			CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.wsmw12rkw524");

			if(terrainCDLODOcclusionCulling.HasAllNecessaryData() == false)
			{
				DrawNecessaryData();
				return;
			}
			else
			{
				terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingPackage = (TerrainCDLODOcclusionCullingPackage)CustomEditorGUI.ObjectField(package, 
					terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingPackage, typeof(TerrainCDLODOcclusionCullingPackage), 1);
			}

            EditorGUI.BeginChangeCheck();

            terrainCDLODOcclusionCulling.ActiveRenderer = CustomEditorGUI.Toggle(activeRenderer, terrainCDLODOcclusionCulling.ActiveRenderer);
			terrainCDLODOcclusionCulling.BakeAndRenderInPlayMode = CustomEditorGUI.Toggle(bakeAndRenderInPlayMode, terrainCDLODOcclusionCulling.BakeAndRenderInPlayMode);

            DrawCDLODOcclusionCulling();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(terrainCDLODOcclusionCulling);
            }
        }

        public void DrawNecessaryData()
        {			
			GUILayout.Space(3); 
			
			terrainCDLODOcclusionCulling.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), terrainCDLODOcclusionCulling.QuadroRenderer == null, terrainCDLODOcclusionCulling.QuadroRenderer, typeof(QuadroRenderer));
            terrainCDLODOcclusionCulling.TerrainManager = (TerrainManager)CustomEditorGUI.ObjectField(new GUIContent("Terrain Manager"), terrainCDLODOcclusionCulling.TerrainManager == null, terrainCDLODOcclusionCulling.TerrainManager, typeof(TerrainManager ));
            terrainCDLODOcclusionCulling.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), terrainCDLODOcclusionCulling.StorageTerrainCells == null, terrainCDLODOcclusionCulling.StorageTerrainCells, typeof(StorageTerrainCells ));

            GUILayout.BeginHorizontal();

            terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingPackage = (TerrainCDLODOcclusionCullingPackage)CustomEditorGUI.ObjectField(new GUIContent("Package"), terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingPackage == null, terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingPackage, typeof(TerrainCDLODOcclusionCullingPackage), 1);

            if (CustomEditorGUI.ClickButton("Create"))
            {
                terrainCDLODOcclusionCulling.CreateTerrainCDLODOcclusionCullingPackage();
                terrainCDLODOcclusionCulling.CreateRenderCells();
            }

            GUILayout.Space(5); 

            GUILayout.EndHorizontal();
        }

        public void DrawCDLODOcclusionCulling()
		{
			terrainCDLODOcclusionCulling.ExcludeСellsByMinHeight = CustomEditorGUI.Toggle(excludeСellsByMinHeight, terrainCDLODOcclusionCulling.ExcludeСellsByMinHeight);
            GUI.enabled = terrainCDLODOcclusionCulling.ExcludeСellsByMinHeight;
            terrainCDLODOcclusionCulling.MinHeight = CustomEditorGUI.FloatField(minHeight, terrainCDLODOcclusionCulling.MinHeight);
            GUI.enabled = true;

			terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.renderCellSize = CustomEditorGUI.FloatField(renderCellSize, terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.renderCellSize);

			DrawCellLODsSetings();
			DrawOcclusionCullingDebugSettings();

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Bake From Storage Terrain Cells"))
				{
					terrainCDLODOcclusionCulling.Bake();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Refresh Cells"))
				{
					terrainCDLODOcclusionCulling.CreateRenderCells();
					terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.cellLODsSetings.UpdateLODRanges();
					terrainCDLODOcclusionCulling.SetupCDLODOcclusionCulling();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
		}

        public void DrawCellLODsSetings()
		{
			selectCellLODsFoldout = CustomEditorGUI.Foldout(selectCellLODsFoldout, "Cell LODs Settings");

			if(selectCellLODsFoldout)
			{
				EditorGUI.indentLevel++;

			    terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.cellLODsSetings.Lod0Distance = CustomEditorGUI.Slider(Lod0Distance, terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.cellLODsSetings.Lod0Distance, 15, 600);
				terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.cellLODsSetings.LodLevels = CustomEditorGUI.IntSlider(lodLevels, terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.cellLODsSetings.LodLevels, 1, 5);

				EditorGUI.indentLevel--;
			}
		}

        public void DrawOcclusionCullingDebugSettings()
        {
			selectOcclusionCullingDebugSettingsFoldout = CustomEditorGUI.Foldout(selectOcclusionCullingDebugSettingsFoldout, "Debug Settings");

			if(selectOcclusionCullingDebugSettingsFoldout)
			{
				EditorGUI.indentLevel++;
								
				terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.showVisibleCells = CustomEditorGUI.Toggle(showVisibleCells, terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.showVisibleCells);

				EditorGUI.indentLevel--;
			}
		}

		public GUIContent package = new GUIContent("Package", "A persistent package is a scripted object for storing instances. Data is stored in binary format, this will allow you to read data faster with a large amount of data.");
        public GUIContent activeRenderer = new GUIContent("Active Renderer", "Disable rendering for this component.");
		public GUIContent bakeAndRenderInPlayMode = new GUIContent("Bake And Render In Play Mode", "When you are in Play Mode, it bakes instances from Storage Terrain Cells and turns on the Active Renderer option.");
        public GUIContent excludeСellsByMinHeight = new GUIContent("Exclude Cells By Min Height", "Can be used for setups where you have no underwater vegetation or rocks. It will remove internal cells where the entire cell is under sea level.");
        public GUIContent minHeight = new GUIContent("Min Height", "it is recommended to set the sea height. This will prevent you from creating cells under water, where there will be no rendering and will increase the FPS.");
        public GUIContent renderCellSize = new GUIContent("Render Cell Size", "Set this value to the maximum render distance of the Quadro Renderer. The render cells contain a QuadTree, the smaller the cell, the less LOD Levels are needed.");
		public GUIContent Lod0Distance = new GUIContent("LOD 0 Distance", "Distance of the first lod in which the cells will be displayed. ");
		public GUIContent lodLevels = new GUIContent("LOD Levels", "Number of LODs in Quad Tree");
		public GUIContent showVisibleCells = new GUIContent("Show Visible Cells", "Shows the cells that will be used for rendering.");
    }
}