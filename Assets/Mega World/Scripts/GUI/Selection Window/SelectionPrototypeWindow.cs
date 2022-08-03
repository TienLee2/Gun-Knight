#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using VladislavTsurikov;
using VladislavTsurikov.Editor;
using MegaWorld.PrecisePlace;

namespace MegaWorld
{
	public class SelectionPrototypeWindow 
    {
		private static bool s_selectPrototypeFoldout = true;
		private static int s_protoIconWidth  = 80;
        private static int s_protoIconHeight = 95;
		private static float s_prototypeWindowHeight = 100f;

		
		[NonSerialized]
        public SelectedTypeVariables SelectedTypeVariables;

		[NonSerialized]
        public Type Type;

		[NonSerialized]
        public Clipboard Clipboard;

		[NonSerialized]
        public MegaWorldTools CurrentTool;

        public void OnGUI(SelectedTypeVariables selectedTypeVariables, Clipboard clipboard, MegaWorldTools currentTool)
        {
            SelectedTypeVariables = selectedTypeVariables;
            Type = selectedTypeVariables.SelectedType;
            Clipboard = clipboard;
            CurrentTool = currentTool;

            DrawPrototypes();
        }

        public void DrawPrototypes()
        {
			EditorGUILayout.BeginHorizontal();
            s_selectPrototypeFoldout = CustomEditorGUI.Foldout(s_selectPrototypeFoldout, "Prototypes");
			
			if(SelectedTypeVariables.HasOneSelectedType())
			{
				SelectionResourcesType.DrawResourcesType(Type);
			}
			
            EditorGUILayout.EndHorizontal();
            
			if(s_selectPrototypeFoldout)
			{
				DrawSelectedWindowForPrototypes();	
			}
        }

        public void DrawSelectedWindowForPrototypes()
		{
			++EditorGUI.indentLevel;

			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, s_prototypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForPrototypes(SelectedTypeVariables, windowRect, InitialGUIColor, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				switch (Type.ResourceType)
            	{
            	    case ResourceType.GameObject:
            	    {
						DrawProtoGameObjectIcons(Type, e, Type.ProtoGameObjectList, windowRect);
            	        break;
            	    }
					case ResourceType.TerrainDetail:
					{
						DrawProtoTerrainDetailIcons(Type, e, Type.ProtoTerrainDetailList, windowRect);
						break;
					}
            	    case ResourceType.TerrainTexture:
            	    {
						DrawProtoTerrainTextureIcons(Type, e, Type.ProtoTerrainTextureList, windowRect);
            	        break;
            	    }
					case ResourceType.QuadroItem:
					{
						DrawProtoQuadroItemIcons(Type, e, Type.ProtoQuadroItemList, windowRect);
						break;
					}
            	}
			}

			SelectedWindowUtility.DrawResizeBar(e, s_protoIconHeight, ref s_prototypeWindowHeight);

			if(dragAndDrop)
			{
				switch (Type.ResourceType)
            	{
            	    case ResourceType.GameObject:
            	    {
						DropOperationForProtoGameObject(Type, e, virtualRect);
            	        break;
            	    }
					case ResourceType.TerrainDetail:
					{
						DropOperationForProtoTerrainDetail(Type, e, virtualRect);
						break;
					}
            	    case ResourceType.TerrainTexture:
            	    {
						DropOperationForProtoTerrainTexture(Type, e, virtualRect);
            	        break;
            	    }
					case ResourceType.QuadroItem:
					{
						DropOperationForProtoQuadroItem(Type, e, virtualRect);
						break;
					}
            	}
			} 

			--EditorGUI.indentLevel;
		}

		private void DrawProtoGameObjectIcons(Type type, Event e, List<PrototypeGameObject> protoGameObjectList, Rect brushWindowRect)
		{
			PrototypeGameObject draggingPrototype = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is PrototypeGameObject)
				{
					draggingPrototype = (PrototypeGameObject)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, protoGameObjectList.Count, s_protoIconWidth, s_protoIconHeight);

			Vector2 brushWindowScrollPos = type.PrototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			Rect dragRect = new Rect(0, 0, 0, 0);

			for (int protoIndexForGUI = 0; protoIndexForGUI < protoGameObjectList.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, s_protoIconWidth, s_protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIconOfPrototypeGameObject(protoGameObjectList, type, protoIndexForGUI, out textColor, out rectColor);
				SetIconInfo(protoGameObjectList[protoIndexForGUI].prefab, out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, s_protoIconWidth, false, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoGameObjectList[protoIndexForGUI]);
            		}

					if (draggingPrototype != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
                		    SelectionPrototypeUtility.InsertSelectedProtoGameObject(type, protoIndexForGUI, isAfter);
                		}
					}

					HandleSelectPrototypeGameObject(type, protoIndexForGUI, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, s_protoIconWidth, s_protoIconHeight, ref y, ref x);
			}

            if(draggingPrototype != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			type.PrototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private void DrawProtoTerrainDetailIcons(Type type, Event e, List<PrototypeTerrainDetail> protoTerrainDetailList, Rect brushWindowRect)
		{
			PrototypeTerrainDetail draggingPrototypeTerrainDetail = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is PrototypeTerrainDetail)
				{
					draggingPrototypeTerrainDetail = (PrototypeTerrainDetail)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, protoTerrainDetailList.Count, s_protoIconWidth, s_protoIconHeight);

			Vector2 brushWindowScrollPos = type.PrototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < protoTerrainDetailList.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, s_protoIconWidth, s_protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(protoTerrainDetailList[protoIndexForGUI].selected, protoTerrainDetailList[protoIndexForGUI].active, out textColor, out rectColor);
				SetIconInfoForDetail(protoTerrainDetailList[protoIndexForGUI], out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, s_protoIconWidth, true, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoTerrainDetailList[protoIndexForGUI]);
            		}

					if (draggingPrototypeTerrainDetail != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
                		    SelectionPrototypeUtility.InsertSelectedProtoTerrainDetail(type, protoIndexForGUI, isAfter);
                		}
					}

					HandleSelectPrototypeTerrainDetail(type, protoIndexForGUI, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, s_protoIconWidth, s_protoIconHeight, ref y, ref x);
			}

            if(draggingPrototypeTerrainDetail != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			type.PrototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private void DrawProtoTerrainTextureIcons(Type type, Event e, List<PrototypeTerrainTexture> protoTerrainTextureList, Rect brushWindowRect)
		{
			PrototypeTerrainTexture draggingPrototypeTerrainTexture = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is PrototypeTerrainTexture)
				{
					draggingPrototypeTerrainTexture = (PrototypeTerrainTexture)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, protoTerrainTextureList.Count, s_protoIconWidth, s_protoIconHeight);

			Vector2 brushWindowScrollPos = type.PrototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < protoTerrainTextureList.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, s_protoIconWidth, s_protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(protoTerrainTextureList[protoIndexForGUI].selected, protoTerrainTextureList[protoIndexForGUI].active, out textColor, out rectColor);
				SetIconInfoForTexture(protoTerrainTextureList[protoIndexForGUI], out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, s_protoIconWidth, false, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoTerrainTextureList[protoIndexForGUI]);
            		}

					if (draggingPrototypeTerrainTexture != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
                		    SelectionPrototypeUtility.InsertSelectedProtoTerrainTexture(type, protoIndexForGUI, isAfter);
                		}
					}

					HandleSelectPrototypeTerrainTexture(type, protoIndexForGUI, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, s_protoIconWidth, s_protoIconHeight, ref y, ref x);
			}

            if(draggingPrototypeTerrainTexture != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			type.PrototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private void DrawProtoQuadroItemIcons(Type type, Event e, List<PrototypeQuadroItem> protoQuadroItemList, Rect brushWindowRect)
		{
			PrototypeQuadroItem draggingPrototypeQuadroItem = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is PrototypeQuadroItem)
				{
					draggingPrototypeQuadroItem = (PrototypeQuadroItem)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, protoQuadroItemList.Count, s_protoIconWidth, s_protoIconHeight);

			Vector2 brushWindowScrollPos = type.PrototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < protoQuadroItemList.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, s_protoIconWidth, s_protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(protoQuadroItemList[protoIndexForGUI].selected, protoQuadroItemList[protoIndexForGUI].active, out textColor, out rectColor);
				SetIconInfo(protoQuadroItemList[protoIndexForGUI].prefab, out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, s_protoIconWidth, false, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoQuadroItemList[protoIndexForGUI]);
            		}

					if (draggingPrototypeQuadroItem != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
                		    SelectionPrototypeUtility.InsertSelectedProtoQuadroItem(type, protoIndexForGUI, isAfter);
                		}
					}

					HandleSelectPrototypeQuadroItem(type, protoIndexForGUI, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, s_protoIconWidth, s_protoIconHeight, ref y, ref x);
			}

			if(draggingPrototypeQuadroItem != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			type.PrototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

        private void SetColorForIconOfPrototypeGameObject(List<PrototypeGameObject> protoGameObjectList, Type type, int currentPrototypeIndexForGUI, out Color textColor, out Color rectColor)
		{
			if(CurrentTool == MegaWorldTools.PrecisePlace && MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings.SelectType == PreciseSelectType.Unit)
			{
				if(type.GetPrecisionUnit() == currentPrototypeIndexForGUI)
				{
					rectColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);

					if (EditorGUIUtility.isProSkin)
            		{
						textColor = EditorColors.Instance.orangeNormal; 
            		}
            		else
            		{
						textColor = EditorColors.Instance.orangeDark;
					}
				}
				else
				{
					SetColorForIcon(protoGameObjectList[currentPrototypeIndexForGUI].selected, protoGameObjectList[currentPrototypeIndexForGUI].active, out textColor, out rectColor);
				}
			}
			else
			{
				SetColorForIcon(protoGameObjectList[currentPrototypeIndexForGUI].selected, protoGameObjectList[currentPrototypeIndexForGUI].active, out textColor, out rectColor);
			}
		}

        private void SetIconInfoForDetail(PrototypeTerrainDetail protoTerrainDetail, out Texture2D preview, out string name)
		{
			if(protoTerrainDetail.PrefabType == PrefabType.Mesh)
			{
            	if (protoTerrainDetail.prefab != null)
            	{
            	    preview = MegaWorldGUIUtility.GetPrefabPreviewTexture(protoTerrainDetail.prefab);      
					name = protoTerrainDetail.TerrainDetailName;
            	}
				else
				{
					preview = null;
					name = "Missing Prefab";
				}
			}
			else
			{
            	if (protoTerrainDetail.DetailTexture != null)
            	{
            	    preview = protoTerrainDetail.DetailTexture;      
					name = protoTerrainDetail.TerrainDetailName;
            	}
				else
				{
					preview = null;
					name = "Missing Texture";
				}
			}
		}

		private void SetIconInfoForTexture(PrototypeTerrainTexture protoTerrainTexture, out Texture2D preview, out string name)
		{
            if (protoTerrainTexture.TerrainTextureSettings.DiffuseTexture != null)
            {
                preview = protoTerrainTexture.TerrainTextureSettings.DiffuseTexture;      
				name = protoTerrainTexture.TerrainTextureName;
            }
			else
			{
				preview = null;
				name = "Missing Texture";
			}
		}

        private bool IsNecessaryToDrawIconsForPrototypes(SelectedTypeVariables selectedTypeVariables, Rect brushWindowRect, Color InitialGUIColor, ref bool dragAndDrop)
		{
			if(selectedTypeVariables.HasOneSelectedType() == false)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "To Draw Prototype, you need to select one type");
				dragAndDrop = false;
				return false;
			}	

			Type type = selectedTypeVariables.SelectedType;

			switch (type.ResourceType)
            {
                case ResourceType.GameObject:
                {
					if(type.ProtoGameObjectList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Drag & Drop Prefabs Here");
						dragAndDrop = true;
						return false;
					}
                    break;
                }
				case ResourceType.TerrainDetail:
				{
					if(type.ProtoTerrainDetailList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Drag & Drop Prefabs or Textures Here");
						dragAndDrop = true;
						return false;
					}
					break;
				}
            	case ResourceType.TerrainTexture:
            	{
					if(type.ProtoTerrainTextureList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Drag & Drop Texture or Terrain Layers Here");
						dragAndDrop = true;
						return false;
					}
            	    break;
            	}
				case ResourceType.QuadroItem:
				{
					if(type.ProtoQuadroItemList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Drag & Drop Prefabs Here");
						dragAndDrop = true;
						return false;
					}
					break;
            	}
            }

			dragAndDrop = true;
			return true;
		}

        private void DropOperationForProtoGameObject(Type type, Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

						List<GameObject> draggedGameObjects = new List<GameObject>();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject && 
                                PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                                AssetDatabase.Contains(draggedObject))
                            {
								draggedGameObjects.Add((GameObject)draggedObject);   
                            }
                        }

						if(draggedGameObjects.Count > 0)
						{
							type.AddPrototypeGameObject(draggedGameObjects);
						}
                    }
                    e.Use();
                } 
			}
		}

		private void DropOperationForProtoTerrainDetail(Type type, Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

						type.AddPrototypeTerrainDetail(DragAndDrop.objectReferences);
                    }
                    e.Use();
                } 
			}
		}

		private void DropOperationForProtoTerrainTexture(Type type, Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
							if (draggedObject is Texture2D)
                            {
								type.ProtoTerrainTextureList.Add(new PrototypeTerrainTexture((Texture2D)draggedObject, draggedObject.name));
                            }
							else if(draggedObject is TerrainLayer)
							{
								type.ProtoTerrainTextureList.Add(new PrototypeTerrainTexture((TerrainLayer)draggedObject, draggedObject.name));
							}
                        }
                    }
                    e.Use();
                } 
			}
		}

		private void DropOperationForProtoQuadroItem(Type type, Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

						List<GameObject> draggedGameObjects = new List<GameObject>();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject && 
                                PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                                AssetDatabase.Contains(draggedObject))
                            {
								draggedGameObjects.Add((GameObject)draggedObject);   
                            }
                        }

						if(draggedGameObjects.Count > 0)
						{
							type.AddPrototypeQuadroItem(draggedGameObjects);
						}
                    }
                    e.Use();
                } 
			}
		}

        public void HandleSelectPrototypeGameObject(Type type, int prototypeIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectionPrototypeUtility.SelectPrototypeAdditive(type, prototypeIndex, ResourceType.GameObject);               
						}
						else if (e.shift)
						{          
							SelectionPrototypeUtility.SelectPrototypeRange(type, prototypeIndex, ResourceType.GameObject);                
						}
						else 
						{
							SelectionPrototypeUtility.SelectPrototype(type, prototypeIndex, ResourceType.GameObject);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					if(prototypeIndex != -1)
					{
						WindowMenu.PrototypeGameObjectMenu(type, SelectedTypeVariables, Clipboard, CurrentTool, prototypeIndex).ShowAsContext();

						e.Use();
					}

            		break;
				}
			}
		}

		public void HandleSelectPrototypeTerrainDetail(Type type, int prototypeIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectionPrototypeUtility.SelectPrototypeAdditive(type, prototypeIndex, ResourceType.TerrainDetail);               
						}
						else if (e.shift)
						{          
							SelectionPrototypeUtility.SelectPrototypeRange(type, prototypeIndex, ResourceType.TerrainDetail);                
						}
						else 
						{
							SelectionPrototypeUtility.SelectPrototype(type, prototypeIndex, ResourceType.TerrainDetail);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					WindowMenu.PrototypeTerrainDetailMenu(type, SelectedTypeVariables, Clipboard, CurrentTool, prototypeIndex).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

		public void HandleSelectPrototypeTerrainTexture(Type type, int prototypeIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectionPrototypeUtility.SelectPrototypeAdditive(type, prototypeIndex, ResourceType.TerrainTexture);               
						}
						else if (e.shift)
						{          
							SelectionPrototypeUtility.SelectPrototypeRange(type, prototypeIndex, ResourceType.TerrainTexture);                
						}
						else 
						{
							SelectionPrototypeUtility.SelectPrototype(type, prototypeIndex, ResourceType.TerrainTexture);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					WindowMenu.PrototypeTerrainTextureMenu(type, SelectedTypeVariables, Clipboard, CurrentTool, prototypeIndex).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

		public void HandleSelectPrototypeQuadroItem(Type type, int prototypeIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectionPrototypeUtility.SelectPrototypeAdditive(type, prototypeIndex, ResourceType.QuadroItem);               
						}
						else if (e.shift)
						{          
							SelectionPrototypeUtility.SelectPrototypeRange(type, prototypeIndex, ResourceType.QuadroItem);                
						}
						else 
						{
							SelectionPrototypeUtility.SelectPrototype(type, prototypeIndex, ResourceType.QuadroItem);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					WindowMenu.PrototypeQuadroItemMenu(type, SelectedTypeVariables, Clipboard, CurrentTool, prototypeIndex).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

        private void SetColorForIcon(bool selected, bool active, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = active ? EditorColors.Instance.ToggleButtonActiveColor : EditorColors.Instance.redNormal;
			}
			else
			{
				rectColor = active ? EditorColors.Instance.ToggleButtonInactiveColor : EditorColors.Instance.redDark;
			}
		}

        private void SetIconInfo(GameObject prefab, out Texture2D preview, out string name)
		{
            if (prefab != null)
            {
                preview = MegaWorldGUIUtility.GetPrefabPreviewTexture(prefab);      
				name = prefab.name;
            }
			else
			{
				preview = null;
				name = "Missing Prefab";
			}
		}
    }
}
#endif