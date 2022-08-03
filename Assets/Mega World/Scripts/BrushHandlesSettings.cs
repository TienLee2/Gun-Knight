using UnityEngine;
using System;

namespace MegaWorld
{
    public enum BrushHandlesType 
    {  
        Circle, 
        SphereAndCircle, 
    }
    
    [Serializable]
    public class BrushHandlesSettings 
    {
        public BrushHandlesType BrushHandlesType = BrushHandlesType.SphereAndCircle;
        public bool DrawSolidDisc = true;
        public Color SphereColor = new Color(0.64f, 0.75f, 0.22f, 0.75f);
        public float SpherePixelWidth = 3f;
        public Color CircleColor = new Color(0.64f, 0.75f, 0.22f, 1);
        public float CirclePixelWidth = 5f;


#if UNITY_EDITOR
        public BrushHandlesSettingsEditor BrushHandlesSettingsEditor = new BrushHandlesSettingsEditor();

        public void OnGUI()
        {
            BrushHandlesSettingsEditor.OnGUI(this);
        }
#endif
    }
}

