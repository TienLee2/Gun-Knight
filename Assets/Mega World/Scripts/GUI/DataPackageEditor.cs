#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
	[Serializable]
    public class DataPackageEditor 
    {
		public SelectionTypeWindow SelectionTypeWindow = new SelectionTypeWindow();
		public SelectionPrototypeWindow SelectionPrototypeWindow = new SelectionPrototypeWindow();

		public void OnGUI(BasicData basicData, MegaWorldTools tool)
		{
			SelectionTypeWindow.OnGUI(basicData);

			SelectionPrototypeWindow.OnGUI(basicData.SelectedVariables, basicData.Clipboard, tool);

			if(basicData.SelectedVariables.HasOneSelectedType())
			{
				SelectionTypeWindow.RenameTypeWindowGUI(basicData.SelectedVariables.SelectedType); 
			}
		}
	}
}
#endif