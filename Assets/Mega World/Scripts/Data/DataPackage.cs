using UnityEngine;
using UnityEditor;

namespace MegaWorld
{
    public class DataPackage : ScriptableObject
    {
        public BasicData BasicData = new BasicData();

        public SelectedTypeVariables SelectedVariables
        {
            get
            {
                return BasicData.SelectedVariables;
            }
        }

#if UNITY_EDITOR
        public void SetAllDataDirty()
		{
            EditorUtility.SetDirty(this);
			BasicData.SetAllDataDirty();
		}
#endif
    }
}