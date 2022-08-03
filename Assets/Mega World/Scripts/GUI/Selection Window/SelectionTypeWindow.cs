#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	[Serializable]
    public class SelectionTypeWindow 
    {
        private bool SelectTypeFoldout = true;
		private int TypeIconWidth  = 80;
        private int TypeIconHeight = 95;
		private float TypeWindowHeight = 100f;

		[NonSerialized]
        public BasicData Data;

        public void OnGUI(BasicData data)
        {
            Data = data;

            DrawTypes();
        }
	
        public void DrawTypes()
		{
			SelectTypeFoldout = CustomEditorGUI.Foldout(SelectTypeFoldout, "Types");

			if(SelectTypeFoldout)
			{
				DrawSelectedWindowForTypes();
			}
		}

		private void DrawSelectedWindowForTypes()
		{
			++EditorGUI.indentLevel;

			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, TypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForTypes(windowRect, InitialGUIColor, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				DrawTypesIcons(e, windowRect);

				SelectedWindowUtility.DrawResizeBar(e, TypeIconHeight, ref TypeWindowHeight);
			}
			else
			{
				SelectedWindowUtility.DrawResizeBar(e, TypeIconHeight, ref TypeWindowHeight);
			}

			if(dragAndDrop == true)
			{
				DropOperationForType(e, virtualRect);
			}

			switch (e.type)
			{
				case EventType.ContextClick:
				{
            		if(virtualRect.Contains(e.mousePosition))
            		{
						WindowMenu.TypesWindowMenu(Data).ShowAsContext();
            		    e.Use();
            		}
            		break;
				}
			}

			--EditorGUI.indentLevel;
		}

		public void DrawTypesIcons(Event e, Rect brushWindowRect)
		{
			Type draggingType = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Type)
				{
					draggingType = (Type)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, Data.TypeList.Count, TypeIconWidth, TypeIconHeight);

			Vector2 brushWindowScrollPos = Data.TypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			Rect dragRect = new Rect(0, 0, 0, 0);

			for (int typeIndex = 0; typeIndex < Data.TypeList.Count; typeIndex++)
			{
				Rect brushIconRect = new Rect(x, y, TypeIconWidth, TypeIconHeight);

				Color textColor;
				Color rectColor;

				SetColorForTypeIcon(Data.TypeList[typeIndex], out textColor, out rectColor);
				DrawIconRectForType(e, Data.TypeList[typeIndex], brushIconRect, brushWindowRect, textColor, rectColor, brushWindowScrollPos, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && typeIndex != -1)
            		{
            		    InternalDragAndDrop.StartDrag(Data.TypeList[typeIndex]);
            		}

					if (draggingType != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
            	        {
            	            SelectionTypeUtility.InsertSelectedType(Data.TypeList, typeIndex, isAfter);
            	        }
					}

					HandleSelectType(Data.TypeList, typeIndex, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, TypeIconWidth, TypeIconHeight, ref y, ref x);
			}

            if(draggingType != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			Data.TypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private void SetColorForTypeIcon(Type type, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(type.Renaming)
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

			else if(type.Selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

        private void DrawIconRectForType(Event e, Type type, Rect brushIconRect, Rect brushWindowRect, Color textColor, Color rectColor, Vector2 brushWindowScrollPos, UnityAction clickAction)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUI.GetStyle(StyleName.LabelTextForIcon);
			GUIStyle LabelTextForSelectedArea = CustomEditorGUI.GetStyle(StyleName.LabelTextForSelectedArea);

            Rect brushIconRectScrolled = new Rect(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if(brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if(brushIconRect.Contains(e.mousePosition))
				{
					clickAction.Invoke();
				}

				EditorGUI.DrawRect(brushIconRect, rectColor);
                    
				// Prefab preview 
                if(e.type == EventType.Repaint)
            	{
            	    Rect previewRect = new Rect(brushIconRect.x+2, brushIconRect.y+2, brushIconRect.width-4, brushIconRect.width-4);
            	    Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

            	    Rect[] icons =
            	    {   new Rect(previewRect.x, previewRect.y, previewRect.width / 2 - 1, previewRect.height / 2 - 1),
            	        new Rect(previewRect.x + previewRect.width / 2, previewRect.y, previewRect.width / 2, previewRect.height / 2 - 1),
            	        new Rect(previewRect.x, previewRect.y + previewRect.height/2, previewRect.width / 2 - 1, previewRect.height / 2),
            	        new Rect(previewRect.x + previewRect.width / 2, previewRect.y + previewRect.height / 2, previewRect.width / 2, previewRect.height / 2)
            	    };

					Texture2D[] textures = new Texture2D[4];

					switch (type.ResourceType)
					{
						case ResourceType.GameObject:
						{
							for(int i = 0, j = 0; i < type.ProtoGameObjectList.Count && j < 4; i++)
            	    		{
            	    		    if(type.ProtoGameObjectList[i].prefab != null)
            	    		    {
            	        			textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(type.ProtoGameObjectList[i].prefab);
            	    		        j++;
            	    		    }
            	    		}
							break;
						}
						case ResourceType.TerrainDetail:
						{
							for(int i = 0, j = 0; i < type.ProtoTerrainDetailList.Count && j < 4; i++)
            	    		{
								if(type.ProtoTerrainDetailList[i].PrefabType == PrefabType.Mesh)
								{
									if(type.ProtoTerrainDetailList[i].prefab != null)
            	    		    	{
            	    		    	    textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(type.ProtoTerrainDetailList[i].prefab);
            	    		    	    j++;
            	    		    	}
								}
								else
								{
									if(type.ProtoTerrainDetailList[i].DetailTexture != null)
            	    		    	{
										textures[j] = type.ProtoTerrainDetailList[i].DetailTexture;
            	    		    	    j++;
            	    		    	}
								}
            	    		}

							break;
						}
						case ResourceType.TerrainTexture:
						{
							for(int i = 0, j = 0; i < type.ProtoTerrainTextureList.Count && j < 4; i++)
            	    		{
								if(type.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture != null)
            	    		    {
									textures[j] = type.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture;
            	    		        j++;
            	    		    }
            	    		}

							break;
						}
						case ResourceType.QuadroItem:
						{
							for(int i = 0, j = 0; i < type.ProtoQuadroItemList.Count && j < 4; i++)
            	    		{
								if(type.ProtoQuadroItemList[i].prefab != null)
            	    		    {
            	    		        textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(type.ProtoQuadroItemList[i].prefab);
            	    		        j++;
            	    		    }
            	    		}

							break;
						}
					}

            	    for(int i = 0; i < 4; i++)
            	    {
            	        if(textures[i] != null)
            	        {
							EditorGUI.DrawPreviewTexture(icons[i], textures[i]);
            	        }
            	        else
						{
							EditorGUI.DrawRect(icons[i], dimmedColor);
						}
            	    }

					LabelTextForIcon.normal.textColor = textColor;
					LabelTextForIcon.Draw(brushIconRect, SelectedWindowUtility.GetShortNameForIcon(type.TypeName, TypeIconWidth), false, false, false, false);
            	}
			}
		}

		private bool IsNecessaryToDrawIconsForTypes(Rect brushWindowRect, Color InitialGUIColor, ref bool dragAndDrop)
		{
			if(Data.TypeList.Count == 0)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Drag & Drop Type Here");
				dragAndDrop = true;
				return false;
			}

			dragAndDrop = true;
			return true;
		}

		private void DropOperationForType(Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains (e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
	
                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();
	
						List<Type> typeList = new List<Type>();
	
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Type)
                            {
								if(draggedObject)
								{
									typeList.Add((Type)draggedObject);   
								}    
                            }
                        }
	
						if(typeList.Count > 0)
						{
							foreach (Type type in typeList)
							{
								if(Data.TypeList.Contains(type) == false)
								{
									Data.TypeList.Add(type);  
								}
							}  
						}
                    }
                    e.Use();
                } 
            }
		}

		public void HandleSelectType(List<Type> typeList, int typeIndex, Event e)
		{
			switch(e.type)
            {
            	case EventType.MouseDown:
				{
					if(e.button == 0)
                    {
						if (e.control)
                        {    
							SelectionTypeUtility.SelectTypeAdditive(typeList, typeIndex);               
                        }
                        else if (e.shift)
                        {          
							SelectionTypeUtility.SelectTypeRange(typeList, typeIndex);                
                        }
						else 
						{
							SelectionTypeUtility.DisableAllType(typeList);
							SelectionTypeUtility.SelectType(typeList, typeIndex);    
                        }

            	        e.Use();
					}
            	    break;
				}
				case EventType.ContextClick:
				{
					if (Data.TypeList[typeIndex].Selected)
            		{
            		    WindowMenu.TypeMenu(Data, typeIndex).ShowAsContext();
            		}
            		else
            		{
						SelectionTypeUtility.DisableAllType(typeList);
						SelectionTypeUtility.SelectType(typeList, typeIndex);    
            		}
            		
            		e.Use();

            		break;
				}
			}
		}

        public static void RenameTypeWindowGUI(Type type) 
		{
			if(type.Renaming == false)
			{
				return;
			}

			GUIStyle barStyle = CustomEditorGUI.GetStyle(StyleName.ActiveBar);
			GUIStyle labelStyle = CustomEditorGUI.GetStyle(StyleName.LabelButton);
			GUIStyle labelTextStyle = CustomEditorGUI.GetStyle(StyleName.LabelText);

			Color InitialGUIColor = GUI.color;

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(5);

				if (EditorGUIUtility.isProSkin)
                {
                    CustomEditorGUI.Button(type.TypeName, labelStyle, barStyle, EditorColors.Instance.orangeNormal, EditorColors.Instance.orangeDark.WithAlpha(0.3f), 20);
                }
                else
                {
                    CustomEditorGUI.Button(type.TypeName, labelStyle, barStyle, EditorColors.Instance.orangeDark, EditorColors.Instance.orangeNormal.WithAlpha(0.3f), 20);
                }				

				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			{
                GUILayout.BeginHorizontal();
                {
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace() + 15);
					
					GUILayout.Label(new GUIContent("Rename to"), labelTextStyle);

					GUI.color = EditorColors.Instance.orangeNormal;

                    type.RenamingName = EditorGUILayout.TextField(GUIContent.none, type.RenamingName); //rename to field
					
                    GUI.color = InitialGUIColor;

                    if (CustomEditorGUI.DrawIcon(StyleName.IconButtonOk, EditorColors.Instance.Green) || Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp) //rename OK button
                    {
						type.TypeName = type.RenamingName;
						type.Renaming = false;

						string assetPath =  AssetDatabase.GetAssetPath(type);
 						AssetDatabase.RenameAsset(assetPath, type.RenamingName);
 						AssetDatabase.SaveAssets();

						Event.current.Use();
                    }

                    if (CustomEditorGUI.DrawIcon(StyleName.IconButtonCancel, EditorColors.Instance.Red)) //rename CANCEL button
                    {
						type.RenamingName = type.TypeName;
						type.Renaming = false;

                        Event.current.Use();
                    }

					GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
			}
			GUILayout.EndVertical();
			GUI.color = InitialGUIColor;
		}
    }
}
#endif