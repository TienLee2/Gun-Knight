using UnityEngine;
using System;
using VladislavTsurikov;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaWorld
{
    [Serializable]
    public class SphereCheck
    {
        public bool vegetationMode = true;
        public float size = 3.5f;

        public int priority = 0;
        public float viabilitySize = 4f;
        public float trunkSize = 0.8f;

        public SphereCheck()
        {

        }

        public SphereCheck(SphereCheck other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(SphereCheck other)
        {            
            vegetationMode = other.vegetationMode;
            size = other.size;
            priority = other.priority;
            viabilitySize = other.viabilitySize;
            trunkSize = other.trunkSize;
        }

        public bool OverlapCheck(SphereCheck sphereCheck, Vector3 position, Bounds checkBounds)
        {
            if(vegetationMode)
            {
                if(sphereCheck.priority >= priority)
                {
                    Bounds itemViabilityBounds = new Bounds(position, new Vector3(sphereCheck.viabilitySize, sphereCheck.viabilitySize, sphereCheck.viabilitySize));

                    if(checkBounds.Intersects(itemViabilityBounds))
                    {
                        return true;
                    }
                }

                Bounds itemTrunkBounds = new Bounds(position, new Vector3(sphereCheck.trunkSize, sphereCheck.trunkSize, sphereCheck.trunkSize));

                if(checkBounds.Intersects(itemTrunkBounds))
                {
                    return true;
                }
            }
            else
            {
                Bounds itemBounds = new Bounds(position, new Vector3(sphereCheck.size, sphereCheck.size, sphereCheck.size));

                if(checkBounds.Intersects(itemBounds))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        public static void DrawOverlapСheck(Vector3 position, SphereCheck vegetationCheck)
        {
            if(vegetationCheck.vegetationMode)
            {
                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), vegetationCheck.trunkSize / 2);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, vegetationCheck.trunkSize / 2);

                Handles.color = Color.blue;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), vegetationCheck.viabilitySize / 2);

                Handles.color = Color.blue.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, vegetationCheck.viabilitySize / 2);
            }
            else
            {
                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), vegetationCheck.size / 2);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, vegetationCheck.size / 2);
            }
        }
#endif

        public static Bounds GetBounds(SphereCheck vegetationCheck, Vector3 position, Vector3 scaleFactor)
        {
            Bounds bounds = new Bounds();
            bounds.center = position;   
            bounds.size = new Vector3(vegetationCheck.size * scaleFactor.x, vegetationCheck.size * scaleFactor.x, vegetationCheck.size * scaleFactor.x);

            if(vegetationCheck.vegetationMode)
            {
                bounds.size = new Vector3(vegetationCheck.viabilitySize * scaleFactor.x, vegetationCheck.viabilitySize * scaleFactor.x, vegetationCheck.viabilitySize * scaleFactor.x);
            }

            return bounds;
        }
    }
}
