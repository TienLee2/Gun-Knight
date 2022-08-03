using UnityEngine;

namespace MegaWorld
{
    public class AdvancedSettings : ScriptableObject
    {
        public EditorSettings EditorSettings = new EditorSettings();
        public VisualisationSettings VisualisationSettings = new VisualisationSettings();
    }
}