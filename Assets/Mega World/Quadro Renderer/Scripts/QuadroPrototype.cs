using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class QuadroPrototype : ScriptableObject
    {       
        public bool Selected = false;
        public GameObject PrefabObject;
        public Bounds Bounds;
        public int ID;

        public GeneralSettings GeneralSettings = new GeneralSettings();
        public ShadowsSettings ShadowsSettings = new ShadowsSettings();
        public CullingSettings CullingSettings = new CullingSettings();
        public LODSettings LODSettings = new LODSettings();
        public BillboardSettings BillboardSettings = new BillboardSettings();

        public TreeType TreeType;
        
        public string WarningText;

        public override string ToString()
        {
            if (PrefabObject != null)
                return PrefabObject.name;
            return base.ToString();
        }
    }
}
