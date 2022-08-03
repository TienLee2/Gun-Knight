using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace MegaWorld
{
    [Serializable]
    public class BasicData
    {
        public List<Type> TypeList = new List<Type>();
        public readonly SelectedTypeVariables SelectedVariables = new SelectedTypeVariables();

#if UNITY_EDITOR
        [NonSerialized]
        public Clipboard Clipboard = new Clipboard();
        public Vector2 TypeWindowsScroll = Vector2.zero;
        public DataPackageEditor DataPackageEditor = new DataPackageEditor();

        public void OnGUI(MegaWorldTools tool)
        {
            DataPackageEditor.OnGUI(this, tool);
        }

        public void SetAllDataDirty()
		{
			foreach (Type type in TypeList)
			{
				EditorUtility.SetDirty(type);
			}
		}
#endif
    }
}