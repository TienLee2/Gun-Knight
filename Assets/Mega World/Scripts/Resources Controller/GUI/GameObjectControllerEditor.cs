#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld 
{
    [Serializable]
    public class GameObjectControllerEditor
    {
        public bool gameObjectControllerFoldout = false;

        public void OnGUI()
		{
            gameObjectControllerFoldout = CustomEditorGUI.Foldout(gameObjectControllerFoldout, "GameObject Controller");

			if(gameObjectControllerFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject = CustomEditorGUI.Toggle(new GUIContent("Enable Undo", "If enabled, Undo support for GameObject will work, but problems may arise in optimizing the spawn speed"), 
					MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject);

                if(MegaWorldPath.GeneralDataPackage.EnableUndoForGameObject)
			    {
					CustomEditorGUI.HelpBox("MegaWorld currently cannot automatically clear the Undo buffer, if you do not clear the large Undo history then the spawn speed will be reduced.");

			    	GUILayout.BeginHorizontal();
                	{
			    		GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
			    		if(CustomEditorGUI.ClickButton("Clear buffer \"Undo\"", ButtonStyle.Remove))
			    		{
			    			Undo.ClearAll();
			    		}
			    		GUILayout.Space(5);
			    	}
			    	GUILayout.EndHorizontal();
			    }

				StorageCells storageCells = MegaWorldPath.GeneralDataPackage.StorageCells;

				MegaWorldPath.GeneralDataPackage.CellSize = CustomEditorGUI.FloatField(new GUIContent("Cell Size", ""), MegaWorldPath.GeneralDataPackage.CellSize);
				MegaWorldPath.GeneralDataPackage.ShowCells = CustomEditorGUI.Toggle(new GUIContent("Show Cells", ""), MegaWorldPath.GeneralDataPackage.ShowCells);

                CustomEditorGUI.HelpBox("If you manually changed the position of the GameObject without using MegaWorld, please click on this button, otherwise, for example, Brush Erase will not be able to delete the changed GameObject.");

                GUILayout.BeginHorizontal();
                {
			    	GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
			    	if(CustomEditorGUI.ClickButton("Refresh Cells", ButtonStyle.Add))
			    	{
						storageCells.RefreshCells(MegaWorldPath.GeneralDataPackage.CellSize);
			    	}
			    	GUILayout.Space(5);
			    }
			    GUILayout.EndHorizontal();

				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif