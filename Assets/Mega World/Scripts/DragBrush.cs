using UnityEngine;
using UnityEditor;
using System;

namespace MegaWorld
{
    public class DragBrush
    {
        public RaycastInfo raycast;
        public RaycastInfo prevRaycast;
        public float dragDistance;
        public Vector3 strokeDirection;
        public Vector3 strokeDirectionRefPoint;

        public void DragMouseRaycast(bool drag, float spacing, Func<Vector3, bool> func)
        {
            if (drag)
            {
                Vector3 hitPoint = raycast.hitInfo.point;
                Vector3 lastHitPoint = prevRaycast.hitInfo.point;

                if(!Utility.IsVector3Equal(hitPoint, lastHitPoint))
                { 
                    Vector3 moveVector = (hitPoint - lastHitPoint);
                    Vector3 moveDirection = moveVector.normalized;
                    float moveLenght = moveVector.magnitude;

                    strokeDirection = (hitPoint - strokeDirectionRefPoint).normalized;

                    if (dragDistance + moveLenght >= spacing)
                    {
                        float d = spacing - dragDistance;
                        Vector3 dragPoint = lastHitPoint + moveDirection * d;
                        dragDistance = 0;
                        moveLenght -= d;

                        func.Invoke(dragPoint);
                        strokeDirectionRefPoint = raycast.hitInfo.point;

                        while (moveLenght >= spacing)
                        {
                            moveLenght -= spacing;
                            dragPoint += moveDirection * spacing;

                            func.Invoke(dragPoint);
                            strokeDirectionRefPoint = raycast.hitInfo.point;
                        }
                    }

                    dragDistance += moveLenght;
                }
            }
            
            prevRaycast = raycast;
        }

#if UNITY_EDITOR
        public bool UpdateDragPosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
            {
                Utility.Raycast(ray, out raycast, type.GetCurrentPaintLayers());
            }

            if(raycast.isHit == false)
            {
                return false;
            }

            if(Quaternion.LookRotation(raycast.hitInfo.normal) == new Quaternion(0, 0, 0, 1))
            {
                return false;
            }

            return true;
        }
#endif
    }
}