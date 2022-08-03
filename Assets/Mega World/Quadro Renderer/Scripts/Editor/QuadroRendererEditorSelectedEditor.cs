using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    public static class QuadroRenderEditorSelectedEditor 
    {
		public static int protoIconWidth  = 80;
        public static int protoIconHeight = 95;
		public static float prototypeWindowHeight = 100f;

        public static void DrawSelectedWindowForPrototypes(QuadroRenderer quadroRenderer)
    	{
    		bool dragAndDrop = false;

    		Color InitialGUIColor = GUI.color;

    		Event e = Event.current;

    		Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, prototypeWindowHeight)) );
    		windowRect = EditorGUI.IndentedRect(windowRect);

    		Rect virtualRect = new Rect(windowRect);

    		if(IsNecessaryToDrawIconsForPrototypes(quadroRenderer, windowRect, InitialGUIColor, ref dragAndDrop) == true)
    		{
    			SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

    			DrawProtoQuadroItemIcons(quadroRenderer, e, quadroRenderer.QuadroPrototypesPackage.PrototypeList, windowRect);

    			SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
    		}
    		else
    		{
    			SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
    		}

    		if(dragAndDrop)
    		{
    			DropOperationForProtoQuadroItem(quadroRenderer, e, virtualRect);
    		} 
    	}

        private static bool IsNecessaryToDrawIconsForPrototypes(QuadroRenderer quadroRenderer, Rect brushWindowRect, Color InitialGUIColor, ref bool dragAndDrop)
    	{
    		if(quadroRenderer.QuadroPrototypesPackage.PrototypeList.Count == 0)
    		{
    			SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Has no prototypes");
    			dragAndDrop = true;
    			return false;
    		}

    		dragAndDrop = true;
    		return true;
    	}

        private static void DropOperationForProtoQuadroItem(QuadroRenderer quadroRenderer, Event e, Rect virtualRect)
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
                            if (draggedObject is GameObject && 
                                PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                                AssetDatabase.Contains(draggedObject))
                            {
                                quadroRenderer.QuadroPrototypesPackage.AddPrototype(quadroRenderer, (GameObject)draggedObject, quadroRenderer.QuadroRendererCamera.Count, draggedObject.GetInstanceID());
                            }
                        }
                    }
                    e.Use();
                } 
    		}
    	}

        private static void DrawProtoQuadroItemIcons(QuadroRenderer quadroRenderer, Event e, List<QuadroPrototype> protoList, Rect brushWindowRect)
    	{
    		Rect dragRect = new Rect(0, 0, 0, 0);

    		Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, protoList.Count, protoIconWidth, protoIconHeight);

    		Vector2 brushWindowScrollPos = quadroRenderer.QuadroPrototypesPackage.PrototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

    		int y = (int)virtualRect.yMin;
    		int x = (int)virtualRect.xMin;

    		for (int protoIndexForGUI = 0; protoIndexForGUI < protoList.Count; protoIndexForGUI++)
    		{
    			Rect brushIconRect = new Rect(x, y, protoIconWidth, protoIconHeight);

    			Color textColor;
    			Color rectColor;

    			Texture2D preview;
    			string name;

    			SetColorForIcon(protoList[protoIndexForGUI], protoList[protoIndexForGUI].Selected, out textColor, out rectColor);
    			SetIconInfo(protoList[protoIndexForGUI].PrefabObject, out preview, out name);

    			SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, protoIconWidth, false, () =>
    			{
    				if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoList[protoIndexForGUI]);
            		}

    				HandleSelectQuadroItem(quadroRenderer, protoIndexForGUI, e);
    			});

    			SelectedWindowUtility.SetNextXYIcon(virtualRect, protoIconWidth, protoIconHeight, ref y, ref x);
    		}

    		quadroRenderer.QuadroPrototypesPackage.PrototypeWindowsScroll = brushWindowScrollPos;

    		GUI.EndScrollView();
    	}

    	public static void HandleSelectQuadroItem(QuadroRenderer quadroRenderer, int prototypeIndex, Event e)
    	{
    		switch (e.type)
    		{
    			case EventType.MouseDown:
    			{
    				if(e.button == 0)
    				{										
    					if (e.control)
						{    
							quadroRenderer.QuadroPrototypesPackage.SelectPrototypeAdditive(prototypeIndex);               
						}
						else if (e.shift)
						{          
							quadroRenderer.QuadroPrototypesPackage.SelectPrototypeRange(prototypeIndex);                
						}
						else 
						{
							quadroRenderer.QuadroPrototypesPackage.SelectPrototype(prototypeIndex);
						}

            	    	e.Use();
    				}

            	    break;
    			}
    			case EventType.ContextClick:
    			{
    				PrototypeMenu(quadroRenderer, prototypeIndex).ShowAsContext();

    				e.Use();

            		break;
    			}
    		}
    	}

    	public static GenericMenu PrototypeMenu(QuadroRenderer quadroRenderer, int selectedProtoIndex)
        {
            GenericMenu menu = new GenericMenu();
    
            GameObject prefab = quadroRenderer.QuadroPrototypesPackage.PrototypeList[selectedProtoIndex].PrefabObject;

            if(prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.DeleteSelectedPrototypes(quadroRenderer)));

    		menu.AddSeparator ("");

    		menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.CopyAllSettingsFromQuadroPrototype(quadroRenderer.QuadroPrototypesPackage.PrototypeList[selectedProtoIndex])));
            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteAllSettingsToQuadroPrototype(quadroRenderer, quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[selectedProtoIndex])));

    		menu.AddItem(new GUIContent("Paste Settings/General Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteGeneralSettingsToQuadroPrototype()));
    		menu.AddItem(new GUIContent("Paste Settings/Culling Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteCullingSettingsToQuadroPrototype()));
    		menu.AddItem(new GUIContent("Paste Settings/Shadows Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteShadowsSettingsToQuadroPrototype()));
    		menu.AddItem(new GUIContent("Paste Settings/LOD Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteLODSettingsToQuadroPrototype(quadroRenderer.QuadroPrototypesPackage.RenderModelInfoList[selectedProtoIndex])));
			menu.AddItem(new GUIContent("Paste Settings/Billboard Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.QuadroPrototypesPackage.PasteBillboardSettingsToQuadroPrototype(quadroRenderer)));
            
			return menu;
        }

		private static void SetColorForIcon(QuadroPrototype prototype, bool selected, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = string.IsNullOrEmpty(prototype.WarningText) ? EditorColors.Instance.ToggleButtonActiveColor : EditorColors.Instance.redNormal;
			}
			else
			{
				rectColor = string.IsNullOrEmpty(prototype.WarningText) ? EditorColors.Instance.ToggleButtonInactiveColor : EditorColors.Instance.redDark;
			}
		}

        private static void SetIconInfo(GameObject prefab, out Texture2D preview, out string name)
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