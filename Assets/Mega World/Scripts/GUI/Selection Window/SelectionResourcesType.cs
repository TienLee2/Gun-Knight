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
    public static class SelectionResourcesType 
    {
        public static void DrawResourcesType(Type type)
        {
			string name = GetName(type.ResourceType);

            Rect switchRect = GUILayoutUtility.GetRect(130, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            CustomEditorGUI.RectTab(switchRect, name, false, 20);

            Event e = Event.current;

            if(switchRect.Contains(e.mousePosition))
            {	
				switch(e.type)
           		{
           			case EventType.MouseDown:
					{
						ResourcesTypeMenu(type);
                        break;
					}
				}
            }
        }

        private static void ResourcesTypeMenu(Type type)
        {
            GenericMenu menu = new GenericMenu();

            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                menu.AddItem(new GUIContent(GetName(resourceType)), type.ResourceType == resourceType, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
                    type.ResourceType = resourceType
                ));
            }

            menu.ShowAsContext();
        }

        public static string GetName(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.QuadroItem:
                {
                    return "Quadro Item";
                }
                case ResourceType.GameObject:
                {
                    return "GameObject";
                }
                case ResourceType.TerrainDetail:
                {
                    return "Terrain Detail";
                }
                case ResourceType.TerrainTexture:
                {
                    return "Terrain Texture";
                }
            }

            return "NULL";
        }
    }
}
#endif