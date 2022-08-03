using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public struct RaycastInfo
    {
        public RaycastHit hitInfo;
        public bool isHit;

        public Vector3 point
        {
            get
            {
                return hitInfo.point;
            }
        }

        public Vector3 normal
        {
            get
            {
                return hitInfo.normal;
            }
        }

        public bool IntersectsHitPlane(Ray ray, out Vector3 hitPoint)
        {
            float rayDistance;
            Plane plane = new Plane(hitInfo.normal, hitInfo.point);
            if (plane.Raycast(ray, out rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                return true;
            }
            hitPoint = Vector3.zero;
            return false;
        }
    }
}
    