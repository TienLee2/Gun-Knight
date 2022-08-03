#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld.Stamper
{
    [Serializable]
    public class AreaEditor
    {
        public void OnGUI(StamperTool stamper, Area area)
        {
            DrawAreaSettings(area, stamper);
        }

        public void DrawAreaHandlesSettings(Area area)
    	{
    		area.HandlesSettingsFoldout = CustomEditorGUI.Foldout(area.HandlesSettingsFoldout, "Handles Settings");

    		if(area.HandlesSettingsFoldout)
    		{
                EditorGUI.indentLevel++;

                area.HandleSettingsMode = (HandleSettingsMode)CustomEditorGUI.EnumPopup(handleSettingsMode, area.HandleSettingsMode);

                if (area.HandleSettingsMode == HandleSettingsMode.Custom)
                {
                    area.ColorCube = CustomEditorGUI.ColorField(colorCube, area.ColorCube);
                    area.PixelWidth = CustomEditorGUI.Slider(pixelWidth, area.PixelWidth, 1f, 5f);
                    area.Dotted = CustomEditorGUI.Toggle(dotted, area.Dotted);
                }

                area.DrawHandleIfNotSelected = CustomEditorGUI.Toggle(drawHandleIfNotSelected, area.DrawHandleIfNotSelected);

                EditorGUI.indentLevel--;
            }
    	}

        public void DrawAreaSettings(Area area, StamperTool stamper)
        {
    		area.AreaSettingsFoldout = CustomEditorGUI.Foldout(area.AreaSettingsFoldout, "Area Settings");

    		if(area.AreaSettingsFoldout)
    		{
    			EditorGUI.indentLevel++;

    			GUILayout.BeginHorizontal();
            	{
            	    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            	    if(CustomEditorGUI.ClickButton("Fit To Terrain Size"))
    			    {
    			    	area.FitToTerrainSize(stamper);
    			    }
            	    GUILayout.Space(3);
            	}
            	GUILayout.EndHorizontal();

    			GUILayout.Space(3);

				area.UseSpawnCells = CustomEditorGUI.Toggle(new GUIContent("Use Spawn Cells"), area.UseSpawnCells);

				if(area.UseSpawnCells)
				{
					CustomEditorGUI.HelpBox("It is recommended to enable \"Use Cells\" when your terrain is more than 4 km * 4 km. This parameter creates smaller cells, \"Stamper Tool\" will spawn each cell in turn. Why this parameter is needed, too long spawn delay can disable Unity.");

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            		    if(CustomEditorGUI.ClickButton("Refresh Cells"))
    				    {
    				    	area.CreateCells();
    				    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

    				GUILayout.Space(3);

					area.CellSize = CustomEditorGUI.FloatField(cellSize, area.CellSize);
					CustomEditorGUI.Label(new GUIContent("Cell Count: " + area.CellList.Count));
					area.ShowCells = CustomEditorGUI.Toggle(showCells, area.ShowCells); 
				}
				else
				{
					area.UseMask = CustomEditorGUI.Toggle(new GUIContent("Use Mask"), area.UseMask);

            		if(area.UseMask)
            		{
                	    area.MaskType = (MaskType)CustomEditorGUI.EnumPopup(new GUIContent("Mask Type"), area.MaskType);

            		    switch (area.MaskType)
			    	    {
			    	    	case MaskType.Custom:
			    	    	{
			    	    		area.CustomMasks.OnGUI();

			    	    		break;
			    	    	}
			    	    	case MaskType.Procedural:
			    	    	{
			    	    		area.ProceduralMask.OnGUI();

			    	    		break;
			    	    	}
			    	    }
            		}
				}

    			DrawAreaHandlesSettings(area);

    			EditorGUI.indentLevel--;
    		}
        }

		public GUIContent cellSize = new GUIContent("Cell Size", "Sets the cell size in meters.");
		public GUIContent showCells = new GUIContent("Show Cells", "Shows all available cells."); 

		public GUIContent handleSettingsMode = new GUIContent("Handle Settings Mode", "Allows you to choose the settings for how the Area will be displayed.");
		public GUIContent colorCube = new GUIContent("Color Cube", "Area color");
		public GUIContent pixelWidth = new GUIContent("Pixel Width", "How wide the cube line will be");
		public GUIContent dotted = new GUIContent("Dotted", "Displays cube lines by line segments.");
        public GUIContent drawHandleIfNotSelected = new GUIContent("Draw Handle If Not Selected", "If enabled, the visualization and the Area itself will be displayed if you do not have Stamper selected.");
    }
}
#endif