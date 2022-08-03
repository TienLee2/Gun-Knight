#if UNITY_EDITOR
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    public class ToolComponentsView
    {
        public ToolComponentsStack Stack;

        private static System.Type[] s_moduleTypes;
        private static string[] s_names;
        private static int s_TabsWindowHash = "PPGUI.TabsWindow".GetHashCode();

        static ToolComponentsView()
        {
            var gatheredTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                asm =>
                {
                    System.Type[] asmTypes = null;
                    List<System.Type> typesList = null;

                    try
                    {
                        asmTypes = asm.GetTypes();
                        var whereTypes = asmTypes.Where( t =>
                            {
                                return t != typeof(ToolComponent) && t.BaseType == typeof(ToolComponent);
                            } );
                        
                        if( whereTypes != null )
                        {
                            typesList = new List<System.Type>( whereTypes );
                        }
                    }
                    catch(Exception)
                    {
                        asmTypes = null;
                        typesList = null;
                    }

                    return typesList == null ? new List<System.Type>() : typesList;
                }
            );

            List<System.Type> types = gatheredTypes.ToList();

            List<string> names = new List<string>();

            for(int i = 0; i < types.Count; ++i)
            {
                System.Type filterType = types[i];
                ToolComponent tempFilter = (ToolComponent)ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();
                string toolTip = tempFilter.GetToolTip();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                names.Add(path);
            }

            s_names = names.ToArray();
            s_moduleTypes = types.ToArray();
        }

        public ToolComponentsView(ToolComponentsStack stack)
        {
            Stack = stack;

            if(Stack.Tools.Count == 0)
            {
                System.Type[] types = s_moduleTypes;

                for(int i = 0; i < types.Length; ++i)
                {
                    System.Type filterType = types[i];

                    AddComponent(filterType);
                }
            }
        }

        private void ShowManu()
        {
            GenericMenu contextMenu = new GenericMenu();

            System.Type[] types = s_moduleTypes;

            for(int i = 0; i < types.Length; ++i)
            {
                System.Type filterType = types[i];
                string name = s_names[i];

                bool exists = Stack.HasSettings(filterType);

                if (!exists)
                {
                    contextMenu.AddItem(new GUIContent(name), false, () => AddComponent(filterType));
                }
                else
                {
                    contextMenu.AddDisabledItem(new GUIContent(name));
                }
            }

            contextMenu.ShowAsContext();
        }

        public void DrawSelectedToolSettings()
        {
            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                if(Stack.Tools[i].Enabled)
                {
                    Stack.Tools[i].OnGUI();
                }
            }
        }

        public void DrawToolsWindow()
        {
            Event e = Event.current;

			Color InitialGUIColor = GUI.color;

            int tabWidth = 130;
			int tabHeight = 25;

            int tabCount = Stack.Tools.Count;
            if(tabCount == 0)
            {
                Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(tabHeight));
                Rect tabPlusRect = new Rect(0, lineRect.y, tabWidth/2, tabHeight);

				CustomEditorGUI.RectTab(tabPlusRect, "+", ButtonStyle.Add, tabHeight, 28);

                if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    ShowManu();
                }
                return;
            }

            float windowWidth = EditorGUIUtility.currentViewWidth;
            tabCount = tabCount + 1;

            int tabsPerLine = Mathf.Max(1, Mathf.FloorToInt(windowWidth / tabWidth));
            int tabLines = Mathf.CeilToInt((float)tabCount / tabsPerLine);
            int tabIndex = 0;
			int tabUnderCursor = -1;

            ToolComponent draggingTab = null;
            Rect dragRect = new Rect();
            if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is ToolComponent)
				{
					draggingTab = (ToolComponent)InternalDragAndDrop.GetData();
				}      
            }

            int tabWindowControlID = GUIUtility.GetControlID(s_TabsWindowHash, FocusType.Passive);

			Color color = GUI.color;

            for(int line = 0; line < tabLines; line++)
            {
                Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(tabHeight));

                for(int tab = 0; tab < tabsPerLine && tabIndex < tabCount; tab++, tabIndex++)
                {
                    if(tabIndex == tabCount - 1)
                    {
                        Rect tabPlusRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth/2, tabHeight);

						CustomEditorGUI.RectTab(tabPlusRect, "+", ButtonStyle.Add, tabHeight, 28);

                        if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                        {
                            ShowManu();
                        }
                        break;
                    }

                    Rect tabRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth, tabHeight);

                    // Tab under cursor
                    if(tabRect.Contains(e.mousePosition))
                    {
						tabUnderCursor = tabIndex;
						
                       if (draggingTab != null)
                       {
							EditorGUIUtility.AddCursorRect (tabRect, UnityEditor.MouseCursor.MoveArrow);

                            bool isAfter = (e.mousePosition.x - tabRect.xMin) > tabRect.width/2;

                            dragRect = new Rect(tabRect);

                            if(isAfter)
                            {
                                dragRect.xMin = dragRect.xMax - 2;
                                dragRect.xMax = dragRect.xMax + 2;
                            }
                            else
                            {
                                dragRect.xMax = dragRect.xMin + 2;
                                dragRect.xMin = dragRect.xMin - 2;
                            }

                            if(InternalDragAndDrop.IsDragPerform())
                            {
                                InsertSelected(tabIndex, isAfter);
                            }
                       }
                    }

                    ToolComponent tool = GetComponentAtIndex(tabIndex);

					CustomEditorGUI.RectTab(tabRect, tool.GetDisplayName(), tool.Enabled, tabHeight, ButtonStyle.General);

                    // Tab under cursor
                    if(tabRect.Contains(e.mousePosition))
                    {	
						tabUnderCursor = tabIndex;
                    }
                }
            }

            if(draggingTab != null)
                EditorGUI.DrawRect(dragRect, Color.white);

			switch(e.type)
            {
            	case EventType.MouseDown:
				{
					if(tabUnderCursor != -1 && e.button == 0)
                	{
						GUIUtility.keyboardControl = tabWindowControlID;
            			GUIUtility.hotControl = 0;

						Select(tabUnderCursor);

						e.Use();
					}
            	    break;
				}
                case EventType.ContextClick:
				{
					if(tabUnderCursor != -1)
                	{
						GUIUtility.keyboardControl = tabWindowControlID;
            			GUIUtility.hotControl = 0;

            	    	if(Stack.Tools[tabUnderCursor].Enabled == false)
            	    	{
            	    	    Select(tabUnderCursor);
            	    	} 
						else 
						{
            	    	    TabMenu(tabUnderCursor).ShowAsContext();
            	    	}

						e.Use();
					}
					
            	    break;
				}
			}

            if (InternalDragAndDrop.IsDragReady() && tabUnderCursor != -1)
            {
                InternalDragAndDrop.StartDrag(Stack.Tools[tabUnderCursor]);
            }
        }

        public void DisableAllTools()
        {
            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                Stack.Tools[i].Enabled = false;
                Stack.Tools[i].OnToolDisabled();
            }

            if(QuadroRendererController.StorageTerrainCells != null)
            {
                QuadroRendererController.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
            }
                
#if UNITY_EDITOR
           Selection.objects = new UnityEngine.Object[0];
           Tools.current = Tool.None;
#endif
        }

        public void Select(int index)
        {
            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                Stack.Tools[i].Enabled = false;
                Stack.Tools[i].OnToolDisabled();
            }

            Stack.Tools[index].Enabled = true;

            if(QuadroRendererController.StorageTerrainCells != null)
            {
                QuadroRendererController.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
            }
                
#if UNITY_EDITOR
           Selection.objects = new UnityEngine.Object[0];
           Tools.current = Tool.None;
#endif
        }

        public void InsertSelected(int index, bool after)
        {
            List<ToolComponent> selectedTools = new List<ToolComponent>();
            Stack.Tools.ForEach ((brush) => { if(brush.Enabled) selectedTools.Add(brush); });

            if(selectedTools.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, Stack.Tools.Count);

                Stack.Tools.Insert(index, null);    // insert null marker
                Stack.Tools.RemoveAll (b => b != null && b.Enabled); // remove all selected
                Stack.Tools.InsertRange(Stack.Tools.IndexOf(null), selectedTools); // insert selected brushes after null marker
                Stack.Tools.RemoveAll ((b) => b == null); // remove null marter
            }

            index += after ? 1 : 0;
            index = Mathf.Clamp(index, 0, Stack.Tools.Count);
        }

        private GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new GenericMenu();

            if(Stack.Tools.Count > 1)
                menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => { RemoveComponent(currentTabIndex); }));
            else
                menu.AddDisabledItem(new GUIContent("Delete"));    

            return menu;
        }

        public void AddComponent(System.Type type)
        {
            var component = CreateNewComponent(type);
            Stack.Tools.Add(component);

            AssetDatabase.AddObjectToAsset(component, MegaWorldPath.GeneralDataPackage);   

            AssetDatabase.SaveAssets();
        }

        public ToolComponent CreateNewComponent(System.Type type)
        {
            var component = (ToolComponent)ScriptableObject.CreateInstance(type);
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            component.name = type.Name;
            return component;
        }

        public void RemoveComponent(int index)
        {
            var component = Stack.Tools[index];
            Stack.Tools.RemoveAt(index);

            Undo.DestroyObjectImmediate(component);

            AssetDatabase.SaveAssets();
        }

        private ToolComponent GetComponentAtIndex(int index)
        {
            return Stack.Tools[index];
        }
    }
}
#endif