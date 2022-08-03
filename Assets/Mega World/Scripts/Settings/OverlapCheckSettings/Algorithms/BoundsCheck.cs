using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaWorld
{
    public enum BoundsCheckType 
    { 
        Custom,
        BoundsPrefab
    }
    
    [Serializable]
    public class BoundsCheck
    {
        public BoundsCheckType boundsType = BoundsCheckType.BoundsPrefab;
        public bool uniformBoundsSize = false;
        public Vector3 boundsSize = Vector3.one;
        public float multiplyBoundsSize = 1;

        public BoundsCheck()
        {

        }

        public BoundsCheck(BoundsCheck other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(BoundsCheck other)
        {            
            boundsType = other.boundsType;
            uniformBoundsSize = other.uniformBoundsSize;
            multiplyBoundsSize = other.multiplyBoundsSize;
        }

        public bool OverlapCheck(BoundsCheck boundsCheck, Vector3 extents, Vector3 position, Vector3 scale, Bounds objectBounds)
        {
            Bounds currentBounds = BoundsCheck.GetBounds(boundsCheck, position, scale, extents);

            if(objectBounds.Intersects(currentBounds))
            {
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public static void DrawIntersectionСheckType(Vector3 position, Vector3 scale, Vector3 extents, BoundsCheck boundsCheck)
        {
            Bounds bounds = BoundsCheck.GetBounds(boundsCheck, position, scale, extents);
            Handles.color = Color.red;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }
#endif

        public static Bounds GetBounds(BoundsCheck boundsCheck, Vector3 position, Vector3 scaleFactor, Vector3 extents)
        {
            

            Vector3 boundsSize = Vector3.zero;

            if(boundsCheck.boundsType == BoundsCheckType.Custom)
            {
                if(boundsCheck.uniformBoundsSize)
                {
                    boundsSize.x = boundsCheck.boundsSize.x;
                    boundsSize.y = boundsCheck.boundsSize.x;
                    boundsSize.z = boundsCheck.boundsSize.x;
                }
                else
                {
                    boundsSize = boundsCheck.boundsSize;
                }
            }
            else if(boundsCheck.boundsType == BoundsCheckType.BoundsPrefab)
            {
                boundsSize.x = scaleFactor.x * (extents.x * 2);
                boundsSize.y = scaleFactor.y * (extents.y * 2);
                boundsSize.z = scaleFactor.z * (extents.z * 2);
            }

            

            boundsSize.x *= boundsCheck.multiplyBoundsSize;
            boundsSize.y *= boundsCheck.multiplyBoundsSize;
            boundsSize.z *= boundsCheck.multiplyBoundsSize;

            position = new Vector3(position.x , position.y + (boundsSize.y / 2), position.z);

            Bounds bounds = new Bounds();
            bounds.center = position;   
            bounds.size = new Vector3(boundsSize.x, boundsSize.y, boundsSize.z);

            return bounds;
        }
    }
}