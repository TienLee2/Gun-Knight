using System;
using UnityEngine;

namespace MegaWorld
{
    [Serializable]
    public abstract class Prototype
    {
        public GameObject prefab;
        public bool selected = false;
        public bool active = true;
    }
}
