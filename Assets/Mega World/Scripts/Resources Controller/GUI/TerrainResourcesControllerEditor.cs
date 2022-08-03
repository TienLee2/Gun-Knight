#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld 
{
	[Serializable]
    public class TerrainResourcesControllerEditor 
    {
		[Serializable]
		public class Layer //: ScriptableObject
		{
			public bool selected = false;
			public TerrainLayer AssignedLayer = null;
		}

		private List<Layer> paletteLayers = new List<Layer>();
        
		private int protoIconWidth  = 64;
        private int protoIconHeight = 76;
		private float prototypeWindowHeight = 100f;

		private Vector2 prototypeWindowsScroll = Vector2.zero;
		private bool terrainResourcesControllerFoldout = true;

		void UpdateLayerPalette(Terrain terrain)
		{
			if (terrain == null)
			{
				return;
			}

			bool[] selectedList = new bool[paletteLayers.Count];
			for(int i = 0; i < paletteLayers.Count; i++)
			{
				if (paletteLayers[i] != null)
				{
					selectedList[i] = paletteLayers[i].selected;
				}				
			}

			paletteLayers.Clear();

			int index = 0;
			foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
			{
				if(layer != null) 
				{
					Layer paletteLayer = new Layer();//ScriptableObject.CreateInstance<Layer>();
					paletteLayer.AssignedLayer = layer; 
					paletteLayer.selected = selectedList.ElementAtOrDefault(index);
					paletteLayers.Add(paletteLayer); 
					index++;
				}
			}
		}

		public void OnGUI(Type type)
		{
			terrainResourcesControllerFoldout = CustomEditorGUI.Foldout(terrainResourcesControllerFoldout, "Terrain Resources Controller");

			if(terrainResourcesControllerFoldout)
			{
				EditorGUI.indentLevel++;

				if(Terrain.activeTerrains.Length != 0)
				{	
					if(TerrainResourcesController.SpawnSupportAvailable(type, Terrain.activeTerrain) == false)
					{
						switch (type.ResourceType)
            			{
							case ResourceType.TerrainDetail:
               				{
								CustomEditorGUI.WarningBox("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
								break;
							}
							case ResourceType.TerrainTexture:
               				{
								CustomEditorGUI.WarningBox("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
								break;
							}
						}
					}
	
					string getResourcesFromTerrain = "Get/Update Resources From Terrain";
	
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
						GUILayout.BeginVertical();
						{
							if(CustomEditorGUI.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
							{
								TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, type);
							}
	
							GUILayout.Space(3);
	
							GUILayout.BeginHorizontal();
							{
								if(CustomEditorGUI.ClickButton("Add Missing Resources", ButtonStyle.Add))
								{
									TerrainResourcesController.AddMissingPrototypesToTerrains(type);
								}
	
								GUILayout.Space(2);
	
								if(CustomEditorGUI.ClickButton("Remove All Resources", ButtonStyle.Remove))
								{
									if (EditorUtility.DisplayDialog("WARNING!",
										"Are you sure you want to remove all Terrain Resources from the scene?",
										"OK", "Cancel"))
									{
										TerrainResourcesController.RemoveAllPrototypesFromTerrains(type);
									}
								}
							}
							GUILayout.EndHorizontal();
						}
						GUILayout.EndVertical();
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					if(type.ResourceType == ResourceType.TerrainTexture)
					{
						CustomEditorGUI.Header("Active Terrain: Layer Palette");   

						if(Terrain.activeTerrain != null)
						{
							DrawSelectedWindowForTerrainResources(Terrain.activeTerrain, type);
						}
					}
	
					GUILayout.Space(3);
					
				}
				else
				{
					CustomEditorGUI.WarningBox("There is no active terrain in the scene.");
				}

				EditorGUI.indentLevel--;
			}
		}

		public void DrawSelectedWindowForTerrainResources(Terrain terrain, Type type)
		{
			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, prototypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForPrototypes(windowRect, InitialGUIColor, terrain, type, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				UpdateLayerPalette(terrain);
				DrawProtoTerrainTextureIcons(e, type, paletteLayers, windowRect);

				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
			else
			{
				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
		}

		private bool IsNecessaryToDrawIconsForPrototypes(Rect brushWindowRect, Color InitialGUIColor, Terrain terrain, Type type, ref bool dragAndDrop)
		{
			switch (type.ResourceType)
            {
				case ResourceType.TerrainDetail:
				{
					if(terrain.terrainData.detailPrototypes.Length == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Detail\" on terrain");
						dragAndDrop = true;
						return false;
					}
					break;
				}
            	case ResourceType.TerrainTexture:
            	{
					if(terrain.terrainData.terrainLayers.Length == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Layers\" on terrain");
						dragAndDrop = true;
						return false;
					}
            	    break;
            	}
            }

			dragAndDrop = true;
			return true;
		}

		private void DrawProtoTerrainTextureIcons(Event e, Type type, List<Layer> paletteLayers, Rect brushWindowRect)
		{
			Layer draggingPrototypeTerrainTexture = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Layer)
				{
					draggingPrototypeTerrainTexture = (Layer)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, paletteLayers.Count, protoIconWidth, protoIconHeight);

			Vector2 brushWindowScrollPos = prototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < paletteLayers.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, protoIconWidth, protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(paletteLayers[protoIndexForGUI].selected, out textColor, out rectColor);
				SetIconInfoForTexture(paletteLayers[protoIndexForGUI].AssignedLayer, out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, protoIconWidth, false, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(paletteLayers[protoIndexForGUI]);
            		}

					if (draggingPrototypeTerrainTexture != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
							InsertSelectedLayer(protoIndexForGUI, isAfter, type);
							TerrainResourcesController.SyncTerrainID(Terrain.activeTerrain, type);
                		}
					}

					HandleSelectLayer(protoIndexForGUI, type, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, protoIconWidth, protoIconHeight, ref y, ref x);
			}

            if(draggingPrototypeTerrainTexture != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			prototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private void SetColorForIcon(bool selected, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

		private void SetIconInfoForTexture(TerrainLayer protoTerrainTexture, out Texture2D preview, out string name)
		{
            if (protoTerrainTexture.diffuseTexture != null)
            {
                preview = protoTerrainTexture.diffuseTexture;      
				name = protoTerrainTexture.name;
            }
			else
			{
				preview = null;
				name = "Missing Texture";
			}
		}

		public void HandleSelectLayer(int prototypeIndex, Type type, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectLayerAdditive(prototypeIndex);               
						}
						else if (e.shift)
						{          
							SelectLayerRange(prototypeIndex);                
						}
						else 
						{
							SelectLayer(prototypeIndex);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					PrototypeTerrainTextureMenu(type).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

		public void SelectLayer(int prototypeIndex)
        {
            SetSelectedAllLayer(false);

            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }

            paletteLayers[prototypeIndex].selected = true;
        }

        public void SelectLayerAdditive(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }
        
            paletteLayers[prototypeIndex].selected = !paletteLayers[prototypeIndex].selected;
        }

        public void SelectLayerRange(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }

            int rangeMin = prototypeIndex;
            int rangeMax = prototypeIndex;

            for (int i = 0; i < paletteLayers.Count; i++)
            {
                if (paletteLayers[i].selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (paletteLayers[i].selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                paletteLayers[i].selected = true;
            }
		}

		public void SetSelectedAllLayer(bool select)
        {
            foreach (Layer proto in paletteLayers)
            {
                proto.selected = select;
            }
        }

        public void InsertSelectedLayer(int index, bool after, Type type)
        {
            List<Layer> selectedProto = new List<Layer>();
            paletteLayers.ForEach ((proto) => { if(proto.selected) selectedProto.Add(proto); });

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, paletteLayers.Count);

                paletteLayers.Insert(index, null);    // insert null marker
                paletteLayers.RemoveAll (b => b != null && b.selected); // remove all selected
                paletteLayers.InsertRange(paletteLayers.IndexOf(null), selectedProto); // insert selected brushes after null marker
                paletteLayers.RemoveAll ((b) => b == null); // remove null marter
            }

			SetToTerrainLayers(type);
        }

		public void SetToTerrainLayers(Type type)
		{
			List<TerrainLayer> layers = new List<TerrainLayer>();

			foreach (Layer item in paletteLayers)
			{
				layers.Add(item.AssignedLayer);
			}

#if UNITY_2019_2_OR_NEWER
			Terrain.activeTerrain.terrainData.SetTerrainLayersRegisterUndo(layers.ToArray(), "Reorder Terrain Layers");
#else
			Terrain.activeTerrain.terrainData.terrainLayers = layers.ToArray();
#endif

			TerrainResourcesController.SyncAllTerrains(type, Terrain.activeTerrain);
		}

		GenericMenu PrototypeTerrainTextureMenu(Type type)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => DeleteSelectedProtoTerrainTexture(type)));

			menu.AddSeparator ("");
			
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SetSelectedAllLayer(true)));

            return menu;
        }

        public void DeleteSelectedProtoTerrainTexture(Type type)
        {
            paletteLayers.RemoveAll((prototype) => prototype.selected);
			SetToTerrainLayers(type);
        }
	}
}
#endif