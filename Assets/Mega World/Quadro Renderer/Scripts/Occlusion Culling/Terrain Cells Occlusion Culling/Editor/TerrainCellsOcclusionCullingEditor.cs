using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(TerrainCellsOcclusionCulling))]
    public class TerrainCellsOcclusionCullingEditor : Editor
    {
        public bool selectDebugSettingsFoldout = true;
        public TerrainCellsOcclusionCulling terrainCellsOcclusionCulling;

        void OnEnable()
        {
            terrainCellsOcclusionCulling = (TerrainCellsOcclusionCulling)target;
        }

        public override void OnInspectorGUI()
        {			
            CustomEditorGUI.isInspector = true;

            if(terrainCellsOcclusionCulling.isActiveAndEnabled == false)
            {
                CustomEditorGUI.WarningBox("Please do not deactivate the component, otherwise there may be errors. If you don't need this component, just remove it.");
                return;
            }

            CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.6ake2c4tymli");

            if(terrainCellsOcclusionCulling.HasAllNecessaryData() == false)
			{
				DrawNecessaryData();
				return;
			}
            else
            {
                terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage = (PersistentStoragePackage)CustomEditorGUI.ObjectField(package, 
                    terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage, typeof(PersistentStoragePackage), 1);
            }

            EditorGUI.BeginChangeCheck();

            terrainCellsOcclusionCulling.ActiveRenderer = CustomEditorGUI.Toggle(activeRenderer, terrainCellsOcclusionCulling.ActiveRenderer);
			terrainCellsOcclusionCulling.BakeAndRenderInPlayMode = CustomEditorGUI.Toggle(bakeAndRenderInPlayMode, terrainCellsOcclusionCulling.BakeAndRenderInPlayMode);

            CellOcclusionCulling(); 

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(terrainCellsOcclusionCulling);
            }
        }

        public void DrawNecessaryData()
        {			
            GUILayout.Space(3); 
            
			terrainCellsOcclusionCulling.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), terrainCellsOcclusionCulling.QuadroRenderer == null, terrainCellsOcclusionCulling.QuadroRenderer, typeof(QuadroRenderer));
            terrainCellsOcclusionCulling.TerrainManager = (TerrainManager)CustomEditorGUI.ObjectField(new GUIContent("Terrain Manager"), terrainCellsOcclusionCulling.TerrainManager == null, terrainCellsOcclusionCulling.TerrainManager, typeof(TerrainManager ));
            terrainCellsOcclusionCulling.StorageTerrainCells = (StorageTerrainCells)CustomEditorGUI.ObjectField(new GUIContent("Storage Terrain Cells"), terrainCellsOcclusionCulling.StorageTerrainCells == null, terrainCellsOcclusionCulling.StorageTerrainCells, typeof(StorageTerrainCells ));

			GUILayout.BeginHorizontal();

            terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage = (PersistentStoragePackage)CustomEditorGUI.ObjectField(package, 
                terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage == null, terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage, typeof(PersistentStoragePackage), 1);

            if (CustomEditorGUI.ClickButton("Create"))
            {
                terrainCellsOcclusionCulling.CreateTerrainCellsOcclusionCullingPackage();
                terrainCellsOcclusionCulling.CreateCells();
            }

            GUILayout.Space(5); 

            GUILayout.EndHorizontal();
        }

        public void CellOcclusionCulling()
		{
            terrainCellsOcclusionCulling.ExcludeСellsByMinHeight = CustomEditorGUI.Toggle(excludeСellsByMinHeight, terrainCellsOcclusionCulling.ExcludeСellsByMinHeight);
            GUI.enabled = terrainCellsOcclusionCulling.ExcludeСellsByMinHeight;
            terrainCellsOcclusionCulling.MinHeight = CustomEditorGUI.FloatField(minHeight, terrainCellsOcclusionCulling.MinHeight);
            GUI.enabled = true;

			terrainCellsOcclusionCulling.CellSize = CustomEditorGUI.FloatField(cellSize, terrainCellsOcclusionCulling.CellSize);

            CustomEditorGUI.Label(new GUIContent("Cell Count: " + terrainCellsOcclusionCulling.TerrainCellsOcclusionCullingPackage.CellList.Count));
			
			DrawDebugPersistentSettings();

			GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Bake From Storage Terrain Cells"))
				{
					terrainCellsOcclusionCulling.Bake();
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
					terrainCellsOcclusionCulling.CreateCells();
                    terrainCellsOcclusionCulling.SetupCellOcclusionCulling();
				}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal(); 
		}
        
        public void DrawDebugPersistentSettings()
        {
			selectDebugSettingsFoldout = CustomEditorGUI.Foldout(selectDebugSettingsFoldout, "Debug Settings");

			if(selectDebugSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				terrainCellsOcclusionCulling.ShowCells = CustomEditorGUI.Toggle(showCells, terrainCellsOcclusionCulling.ShowCells);
			    terrainCellsOcclusionCulling.ShowVisibleCells = CustomEditorGUI.Toggle(showVisibleCells, terrainCellsOcclusionCulling.ShowVisibleCells);

				EditorGUI.indentLevel--;
			}
		}

        public GUIContent package = new GUIContent("Package", "A persistent package is a scripted object for storing instances. Data is stored in binary format, this will allow you to read data faster with a large amount of data.");
        public GUIContent activeRenderer = new GUIContent("Active Renderer", "Disable rendering for this component.");
        public GUIContent bakeAndRenderInPlayMode = new GUIContent("Bake And Render In Play Mode", "When you are in Play Mode, it bakes instances from Storage Terrain Cells and turns on the Active Renderer option.");
        public GUIContent excludeСellsByMinHeight = new GUIContent("Exclude Cells By Min Height", "Can be used for setups where you have no underwater vegetation or rocks. It will remove internal cells where the entire cell is under sea level.");
        public GUIContent minHeight = new GUIContent("Min Height", "it is recommended to set the sea height. This will prevent you from creating cells under water, where there will be no rendering and will increase the FPS.");
        public GUIContent cellSize = new GUIContent("Cell Size", "The cell size determines the size of the cell's internal structure in meters. Larger cells give faster initialization times as there are fewer cells and also slightly less CPU time used in the render loop. Larger cells provide faster initialization times as there are fewer cells and also slightly less CPU time used in the render loop. Too large a cell size can reduce optimization, and too low a cell size can also reduce optimization. It is recommended to set the cell size, that when your terrain is very large, when your terrain is small, set a lower value.");
        public GUIContent showCells = new GUIContent("Show Cells", "All cells are shown."); 
        public GUIContent showVisibleCells = new GUIContent("Show Visible Cells", "Shows the cells that will be used for rendering.");
    }
}