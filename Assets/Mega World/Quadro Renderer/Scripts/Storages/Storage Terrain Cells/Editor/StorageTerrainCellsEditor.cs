using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(StorageTerrainCells))]
    public class StorageTerrainCellsEditor : Editor
    {
        public bool selectDebugSettingsFoldout = true;

        public StorageTerrainCells storageTerrainCells;

        void OnEnable()
        {
            storageTerrainCells = (StorageTerrainCells)target;
        }

        public override void OnInspectorGUI()
        {			
            CustomEditorGUI.isInspector = true;
            
            if(storageTerrainCells.isActiveAndEnabled == false)
            {
                CustomEditorGUI.WarningBox("Please do not deactivate the component, otherwise there may be errors. If you don't need this component, just remove it.");
                return;
            }

            CustomEditorGUI.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.mpwif9g05qgn");

            if(storageTerrainCells.HasAllNecessaryData() == false)
			{
				DrawNecessaryData();
				return;
			}
            else
            {
                storageTerrainCells.PersistentStoragePackage = (PersistentStoragePackage)CustomEditorGUI.ObjectField(package, 
                    storageTerrainCells.PersistentStoragePackage, typeof(PersistentStoragePackage), 1);
            }

            EditorGUI.BeginChangeCheck();

            storageTerrainCells.ActiveRenderer = CustomEditorGUI.Toggle(activeRenderer, storageTerrainCells.ActiveRenderer);
            storageTerrainCells.DontRenderInPlayMode = CustomEditorGUI.Toggle(dontRenderInPlayMode, storageTerrainCells.DontRenderInPlayMode);
            
            DrawPersistentSettings();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(storageTerrainCells);
            }
        }

        public void DrawNecessaryData()
        {
            GUILayout.Space(3); 
            
			storageTerrainCells.QuadroRenderer = (QuadroRenderer)CustomEditorGUI.ObjectField(new GUIContent("Quadro Renderer"), storageTerrainCells.QuadroRenderer == null, storageTerrainCells.QuadroRenderer, typeof(QuadroRenderer));
            
            GUILayout.BeginHorizontal();
            {
                storageTerrainCells.Area = (Area)CustomEditorGUI.ObjectField(new GUIContent("Area"), storageTerrainCells.Area == null, storageTerrainCells.Area, typeof(Area), 1);

                if (CustomEditorGUI.ClickButton("Create"))
                {
                    storageTerrainCells.Area = storageTerrainCells.CreateAreaParent();
                }

                GUILayout.Space(5); 

                GUILayout.EndHorizontal();
            }
			GUILayout.BeginHorizontal();

            storageTerrainCells.PersistentStoragePackage = (PersistentStoragePackage)CustomEditorGUI.ObjectField(package, 
                storageTerrainCells.PersistentStoragePackage == null, storageTerrainCells.PersistentStoragePackage, typeof(PersistentStoragePackage), 1);

            if (CustomEditorGUI.ClickButton("Create"))
            {
                storageTerrainCells.CreateTerrainPersistentCellsPackage();
                storageTerrainCells.CreateCells();
            }

            GUILayout.Space(5); 

            GUILayout.EndHorizontal();
        }

        public void DrawPersistentSettings()
        {
    		storageTerrainCells.CellSize = CustomEditorGUI.FloatField(cellSize, storageTerrainCells.CellSize);

            CustomEditorGUI.Label(new GUIContent("Cell Count: " + storageTerrainCells.PersistentStoragePackage.CellList.Count));

    		DrawDebugSettings();

    		GUILayout.BeginHorizontal();
           	{
           	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           	    if(CustomEditorGUI.ClickButton("Refresh Cells"))
    			{
    				storageTerrainCells.CreateCells();
                    storageTerrainCells.SetupTerrainPersistentCells();
    			}
           	    GUILayout.Space(3);
           	}
           	GUILayout.EndHorizontal();
    	}

        public void DrawDebugSettings()
        {
			selectDebugSettingsFoldout = CustomEditorGUI.Foldout(selectDebugSettingsFoldout, "Debug Settings");

			if(selectDebugSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				storageTerrainCells.ShowCells = CustomEditorGUI.Toggle(showCells, storageTerrainCells.ShowCells); 
                storageTerrainCells.ShowVisibleCells = CustomEditorGUI.Toggle(showVisibleCells, storageTerrainCells.ShowVisibleCells);

				EditorGUI.indentLevel--;
			}
		}

        public GUIContent package = new GUIContent("Package", "A persistent package is a scripted object for storing instances. Data is stored in binary format, this will allow you to read data faster with a large amount of data.");
        public GUIContent activeRenderer = new GUIContent("Active Renderer", "Disable rendering for this component.");
        public GUIContent dontRenderInPlayMode = new GUIContent("Don't Render In Play Mode", "When you are in Play Mode, this component will not render.This parameter is needed because Storage Terrain Cells is not intended for rendering in Play Mode, it is less optimized than, for example, Cells Occlusion Culling.");
        public GUIContent cellSize = new GUIContent("Cell Size", "The cell size determines the size of the cell's internal structure in meters. The smaller cell size will allow for faster spawn creation using tools like MegaWorld. Larger cells give faster initialization times as there are fewer cells and also slightly less CPU time used in the render loop.");
        public GUIContent showCells = new GUIContent("Show Cells", "All cells are shown."); 
        public GUIContent showVisibleCells = new GUIContent("Show Visible Cells", "Shows the cells that will be used for rendering.");
    }
}
