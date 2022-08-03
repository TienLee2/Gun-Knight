using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class PlacedObjectInfo
    {
        public RaycastInfo RaycastInfo;
        public GameObject gameObject;
        public Bounds bounds;
    }
}
    