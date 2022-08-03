using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
	[CustomEditor(typeof(Type))]
    public class TypeEditor : Editor
    {
        private Type type;

		public SelectionPrototypeWindow SelectionPrototypeWindow = new SelectionPrototypeWindow();
		public readonly SelectedTypeVariables SelectedVariables = new SelectedTypeVariables();
        public Clipboard Clipboard = new Clipboard();

        private void OnEnable()
        {
            type = (Type)target;
        }

        public override void OnInspectorGUI()
        {
            CustomEditorGUI.isInspector = true;
            OnGUI();
        }

		public void OnGUI()
		{
			List<Type> types = new List<Type>() {type};

			SelectedVariables.SetAllSelectedParameters(types);
            SelectedVariables.SelectedTypeList.Add(type);
            SelectedVariables.SelectedType = type;

			SelectionPrototypeWindow.OnGUI(SelectedVariables, Clipboard, MegaWorldPath.GeneralDataPackage.CurrentTool);
		}
	}
}
