#if UNITY_EDITOR
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld.BrushModify
{
    public class ModifyTransformComponentsView
    {
        private static class Styles
        {
            public static Texture2D move;
            
            static Styles()
            {
                move = Resources.Load<Texture2D>("Images/move");
            }
        }

        private static System.Type[] _transformComponentTypes;
        private static GUIContent[] s_displayNames;
        private static string[] s_paths;

        static ModifyTransformComponentsView()
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
                                return t != typeof(ModifyTransformComponent) && t.BaseType == typeof(ModifyTransformComponent);
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
                ModifyTransformComponent tempFilter = (ModifyTransformComponent)ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();
                string toolTip = tempFilter.GetToolTip();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                paths.Add(path);
                displayNames.Add(new GUIContent(path.Substring(separatorIndex, path.Length - separatorIndex), toolTip));
            }

            s_paths = paths.ToArray();
            s_displayNames = displayNames.ToArray();
            _transformComponentTypes = filterTypes.ToArray();
        }

        /*===========================================================================================
        
            Instance Members

        ===========================================================================================*/

        public bool headerFoldout = true;

        public ReorderableList m_reorderableList;
        public ModifyTransformComponentsStack transformComponentsStack;
        private GenericMenu m_contextMenu;
        public GUIContent m_label;
        public bool m_Dragging;

        public ModifyTransformComponentsView(GUIContent label, ModifyTransformComponentsStack transformComponentsStack)
        {
            m_label = label;
            this.transformComponentsStack = transformComponentsStack;
            m_reorderableList = new ReorderableList( transformComponentsStack.TransformComponents, transformComponentsStack.TransformComponents.GetType(), true, true, true, true );

            SetupCallbacks();
        }

        private void Init(GeneralDataPackage generalDataPackage)
        {
            m_contextMenu = new GenericMenu();

            string[] paths = s_paths;
            System.Type[] transformComponent = _transformComponentTypes;

            for(int i = 0; i < transformComponent.Length; ++i)
            {
                System.Type filterType = transformComponent[i];
                string path = paths[i];

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                bool exists = transformComponentsStack.HasSettings(filterType);

                if (!exists)
                {
                    m_contextMenu.AddItem( new GUIContent(path), false, () => AddTransformComponent(filterType, generalDataPackage));
                }
                else
                {
                    m_contextMenu.AddDisabledItem(new GUIContent(path));
                }
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
            // need this line because there is a bug in editor source. ReorderableList.cs : 708 - 709
#if UNITY_2019_2_OR_NEWER
            m_reorderableList.onMouseDragCallback = MouseDragCB;
#endif
        }

        public void OnGUI(Rect maskRect, GeneralDataPackage generalDataPackage)
        {
            Init(generalDataPackage);

            m_reorderableList.DoList(maskRect);
        }

        void RemoveFilter( ReorderableList list )
        {
            RemoveTransformComponent(list.index);
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
            ModifyTransformComponent component = GetComponentAtIndex(index);

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(component == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!component.FoldoutGUI)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                height += component.GetElementHeight(index);
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

            ModifyTransformComponent component = GetComponentAtIndex(index);
            
            if(component == null)
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
                component.Enabled = EditorGUI.Toggle(toggleRect, "", component.Enabled);

                GUI.color = new Color(1f, 1f, 1f, 1f);

                component.FoldoutGUI = EditorGUI.Foldout(headerRect, component.FoldoutGUI, component.GetDisplayName(), true);
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

            using( new EditorGUI.DisabledScope( !transformComponentsStack.TransformComponents[index].Enabled) )
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
                Undo.RecordObject(component, "Filter Changed");

                if(component.FoldoutGUI)
                {
                    EditorGUI.BeginChangeCheck();
                    component.DoGUI(totalRect, index);
                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if(changed)
            {
                EditorUtility.SetDirty(component);
            }

            GUI.color = prevColor;
        }

        public void AddTransformComponent(System.Type type, GeneralDataPackage generalDataPackage)
        {
            var transformComponent = CreateNewTransformComponent(type);
            transformComponentsStack.TransformComponents.Add(transformComponent);

            AssetDatabase.AddObjectToAsset(transformComponent, generalDataPackage);   

            AssetDatabase.SaveAssets();
        }

        public void RemoveTransformComponent(int id)
        {
            var transformComponent = transformComponentsStack.TransformComponents[id];
            transformComponentsStack.TransformComponents.RemoveAt(id);

            Undo.DestroyObjectImmediate(transformComponent);

            AssetDatabase.SaveAssets();
        }

        public ModifyTransformComponent CreateNewTransformComponent(System.Type type)
        {
            var transformComponent = (ModifyTransformComponent)ScriptableObject.CreateInstance(type);
            transformComponent.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            transformComponent.name = type.Name;
            return transformComponent;
        }

        private ModifyTransformComponent GetComponentAtIndex(int index)
        {
            return transformComponentsStack.TransformComponents[index];
        }

        private int GetTransformComponentIndex(string name)
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
            m_contextMenu.ShowAsContext();
        }

        private void OnMenuItemSelected()
        {
            
        }
    }
}
#endif