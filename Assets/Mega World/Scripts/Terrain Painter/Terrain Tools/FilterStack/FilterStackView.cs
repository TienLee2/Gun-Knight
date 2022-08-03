#if UNITY_EDITOR
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.Experimental.TerrainAPI;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    public class FilterStackView
    {
        private static class Styles
        {
            public static Texture2D move;
            
            static Styles()
            {
                move = Resources.Load<Texture2D>("Images/move");
            }
        }
        
        private static System.Type[] s_filterTypes;
        private static GUIContent[] s_displayNames;
        private static string[] s_paths;

        static FilterStackView()
        {
            var gatheredFilterTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                asm =>
                {
                    System.Type[] asmTypes = null;
                    List<System.Type> types = null;

                    try
                    {
                        asmTypes = asm.GetTypes();
                        var whereTypes = asmTypes.Where( t =>
                            {
                                return t != typeof(Filter) && t.BaseType == typeof(Filter);
                            } );
                        
                        if( whereTypes != null )
                        {
                            types = new List<System.Type>( whereTypes );
                        }
                    }
                    catch(Exception)
                    {
                        asmTypes = null;
                        types = null;
                    }

                    return types == null ? new List<System.Type>() : types;
                }
            );

            List<System.Type> filterTypes = gatheredFilterTypes.ToList();

            List<string> paths = new List<string>();
            List<GUIContent> displayNames = new List<GUIContent>();

            for(int i = 0; i < filterTypes.Count; ++i)
            {
                System.Type filterType = filterTypes[i];
                Filter tempFilter = ( Filter )ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();
                string toolTip = tempFilter.GetToolTip();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                paths.Add(path);
                displayNames.Add(new GUIContent(path.Substring(separatorIndex, path.Length - separatorIndex), toolTip));
            }

            s_paths = paths.ToArray();
            s_displayNames = displayNames.ToArray();
            s_filterTypes = filterTypes.ToArray();
        }

        /*===========================================================================================
        
            Instance Members

        ===========================================================================================*/

        public bool headerFoldout = true;

        public ReorderableList     m_reorderableList;
        public FilterStack         m_filterStack;
        public GenericMenu         m_ContextMenu;
        public GUIContent          m_label;
        public bool                m_Dragging;

        public FilterStackView(GUIContent label, FilterStack filterStack)
        {
            m_label = label;
            m_filterStack = filterStack;
            m_reorderableList = new ReorderableList( filterStack.Filters, filterStack.Filters.GetType(), true, true, true, true );

            SetupCallbacks();
        }

        private void Init(Type type)
        {
            m_ContextMenu = new GenericMenu();

            string[] paths = s_paths;
            System.Type[] filterTypes = s_filterTypes;

            for(int i = 0; i < filterTypes.Length; ++i)
            {
                System.Type filterType = filterTypes[i];
                string path = paths[ i ];

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                m_ContextMenu.AddItem( new GUIContent(path), false, () => AddFilter(filterType, type));
            }
        }

        private void SetupCallbacks()
        {
            // setup the callbacks
            m_reorderableList.drawHeaderCallback = DrawHeaderCB;
            m_reorderableList.drawElementCallback = DrawElementCB;
            m_reorderableList.elementHeightCallback = ElementHeightCB;
            m_reorderableList.onAddCallback = AddCB;
            m_reorderableList.onRemoveCallback = RemoveFilter;
            //need this line because there is a bug in editor source. ReorderableList.cs : 708 - 709
#if UNITY_2019_2_OR_NEWER
            m_reorderableList.onMouseDragCallback = MouseDragCB;
#endif
        }

        public void OnGUI(Rect maskRect, Type type)
        {
            Init(type);

            m_reorderableList.DoList(maskRect);
        }

        void RemoveFilter( ReorderableList list )
        {
            RemoveFilter(list.index);
        }

        private void DrawHeaderCB(Rect rect)
        {
            if(CustomEditorGUI.isInspector == false)
            {
                rect.x -= 15;
            }

            headerFoldout = EditorGUI.Foldout(rect, headerFoldout, m_label.text, true);
        }

        private float ElementHeightCB(int index)
        {
            Filter filter = GetFilterAtIndex(index);

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(filter == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!filter.FoldoutGUI)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                height += filter.GetElementHeight(index);
                return height;
            }
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float iconSize = 14f;

            bool isSelected = m_reorderableList.index == index;

            Color bgColor;

            if(EditorGUIUtility.isProSkin)
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if(isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if(EditorGUIUtility.isProSkin)
                {
                    ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;
            
            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements GUI rect
            if(containsMouse && Event.current.type == EventType.MouseDown)
            {
                m_reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;
            
            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            Filter filter = GetFilterAtIndex( index );
            
            if(filter == null)
            {
                return;
            }

            bool changed = false;
            
            Rect moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );

            // draw move handle rect
            EditorGUIUtility.AddCursorRect(moveRect, UnityEditor.MouseCursor.Pan);
            GUI.DrawTexture(moveRect, Styles.move, ScaleMode.StretchToFill);

            Rect toggleRect = totalRect;

            toggleRect.x += 20;
            toggleRect.height = EditorGUIUtility.singleLineHeight * 1.3f;
            toggleRect.width = 30;

            Rect headerRect = toggleRect;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            if(CustomEditorGUI.isInspector)
            {
                headerRect.x += 30;
            }
            else
            {
                headerRect.x += 20;
            }

            EditorGUI.BeginChangeCheck();
            {
                filter.Enabled = EditorGUI.Toggle(toggleRect, "", filter.Enabled);

                GUI.color = new Color(1f, 1f, 1f, 1f);

                filter.FoldoutGUI = EditorGUI.Foldout(headerRect, filter.FoldoutGUI, filter.GetDisplayName(), true);
            }

            changed |= EditorGUI.EndChangeCheck();
            
            // update dragging state
            if(containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !m_Dragging && isFocused)
                {
                    m_Dragging = true;
                    m_reorderableList.index = index;
                }
            }

            if(m_Dragging)
            {
                if(Event.current.type == EventType.MouseUp)
                {
                    m_Dragging = false;
                }
            }

            using( new EditorGUI.DisabledScope( !m_filterStack.Filters[index].Enabled) )
            {
                float rectX = 50;

                if(CustomEditorGUI.isInspector == false)
                {
                    rectX = 53;
                }

                totalRect.x += rectX;
                totalRect.y += 2;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                totalRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                
                GUI.color = prevColor;
                Undo.RecordObject(filter, "Filter Changed");

                if(filter.FoldoutGUI)
                {
                    EditorGUI.BeginChangeCheck();
                    filter.DoGUI(totalRect, index);
                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if(changed)
            {
                EditorUtility.SetDirty(filter);
            }

            GUI.color = prevColor;
        }

        void AddFilter(System.Type type, Type typeOfMegaWorld)
        {
            var effect = CreateNewFilter(type);
            m_filterStack.Filters.Add(effect);

            AssetDatabase.AddObjectToAsset(effect, typeOfMegaWorld);   

            AssetDatabase.SaveAssets();
        }

        void RemoveFilter(int id)
        {
            var effect = m_filterStack.Filters[id];
            m_filterStack.Filters.RemoveAt(id);

            Undo.DestroyObjectImmediate(effect);

            AssetDatabase.SaveAssets();
        }

        Filter CreateNewFilter(System.Type type)
        {
            var filter = (Filter)ScriptableObject.CreateInstance(type);
            filter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            filter.name = type.Name;
            return filter;
        }

        private Filter GetFilterAtIndex(int index)
        {
            return m_filterStack.Filters[index];
        }

        private int GetFilterIndex(string name)
        {
            for(int i = 0; i < s_paths.Length; ++i)
            {
                if(name.CompareTo(s_paths[i]) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void MouseDragCB(ReorderableList list)
        {
            
        }

        private void AddCB(ReorderableList list)
        {
            m_ContextMenu.ShowAsContext();
        }

        private void OnMenuItemSelected()
        {
            
        }
    }
}
#endif