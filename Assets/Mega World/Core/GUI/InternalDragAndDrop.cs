#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov
{
    static public class InternalDragAndDrop
	{
		enum State
		{
			None,
			DragPrepare,
			DragReady,
			Dragging,
			DragPerform
		}
		
		static object          dragData = null;
		static Vector2         mouseDownPosition;
		static State           state = State.None;
		const float            kDragStartDistance = 7.0f;
		
		public static void OnBeginGUI()
        {
            Event e = Event.current;

            switch(state)
            {
            case State.None:
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        mouseDownPosition = e.mousePosition;
                        state = State.DragPrepare;
                    }
                }
                break;
            case State.DragPrepare:
                {
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        state = State.None;
                    }
                }
                break;
            case State.DragReady:
                {
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        state = State.None;
                    }
                }
                break;
            case State.Dragging:
                {
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        state = State.DragPerform;
                        e.Use();
                    }

                    if (e.type == EventType.MouseDrag)
                    {
                        e.Use();
                    }                       
                }
                break;
            }
        }

        public static void OnEndGUI()
        {
            Event e = Event.current;

            switch(state)
            {
            case State.DragReady:
                if (e.type == EventType.Repaint)
                {
                    state = State.None;
                }
                break;
            case State.DragPrepare:                
                if (e.type == EventType.MouseDrag &&
                    ((mouseDownPosition - e.mousePosition).magnitude > kDragStartDistance))
                {                    
                    state = State.DragReady;
                }
                break;
            case State.DragPerform:
                {
                    if (e.type == EventType.Repaint)
                    {
                        dragData = null;
                        state = State.None;
                    }
                }
                break;
            }
        }

        public static bool IsDragReady()
        {
            return state == State.DragReady;
        }

        public static void StartDrag(object data)
        {
            if (data == null || state != State.DragReady)
			{
				return;
			}

            dragData = data;
            state = State.Dragging;
        }

        public static bool IsDragging()
        {
            return state == State.Dragging;
        }

        public static bool IsDragPerform()
        {
            return state == State.DragPerform;
        }

        public static object GetData()
        {
            return dragData;
        }

        public static Vector2 DragStartPosition()
        {
            return mouseDownPosition;
        }
    }
}
#endif