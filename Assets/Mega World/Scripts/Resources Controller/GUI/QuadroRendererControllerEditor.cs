#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld 
{
    public class QuadroRendererControllerEditor 
    {
        public void QuadroRenderControllerWindowGUI(Type type)
		{
			QuadroRendererController.QuadroRendererControllerFoldout = CustomEditorGUI.Foldout(QuadroRendererController.QuadroRendererControllerFoldout, "Quadro Renderer Controller");

			if(QuadroRendererController.QuadroRendererControllerFoldout)
			{
				EditorGUI.indentLevel++;

				QuadroRendererController.SetCurrentSynchronizationError(type);

				switch (QuadroRendererController.SynchronizationError)
				{
					case SynchronizationError.QuadroRendererNull:
					{
						CustomEditorGUI.WarningBox("There is no Quadro Renderer in the scene. Click the button \"Create Quadro Renderer\"");

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Create Quadro Renderer", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								QuadroRendererController.CreateQuadroRenderer();
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();
						break;
					}
					case SynchronizationError.StorageTerrainCellsNull:
					{
						CustomEditorGUI.WarningBox("There is no Storage Terrain Cells in the scene. Click the button \"Add Storage Terrain Cells\"");

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Add StorageTerrainCells", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								QuadroRendererController.AddStorageTerrainCells();
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
					case SynchronizationError.NotAllProtoAvailable:
					{
						CustomEditorGUI.WarningBox("You need all prototypes of this type to be in Quadro Renderer.");

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Get/Update Item", ButtonStyle.General, ButtonSize.ClickButton))
							{
								QuadroRendererController.UpdateQuadroRenderer(type);
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Add Missing Item", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								QuadroRendererController.AddQuadroItem(type.ProtoQuadroItemList);
							}

							GUILayout.Space(2);

							if(CustomEditorGUI.ClickButton("Remove All Item", ButtonStyle.Remove))
							{
								QuadroRendererController.RemoveAllQuadroItem();
							}

							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();
						break;
					}
					case SynchronizationError.None:
					{
						CustomEditorGUI.HelpBox("Mega World will spawn in \"Storage Terrain Cells\".");

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Get/Update Item", ButtonStyle.General, ButtonSize.ClickButton))
							{
								QuadroRendererController.UpdateQuadroRenderer(type);
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);

						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Add Missing Item", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								QuadroRendererController.AddQuadroItem(type.ProtoQuadroItemList);
							}

							GUILayout.Space(2);

							if(CustomEditorGUI.ClickButton("Remove All Item", ButtonStyle.Remove))
							{
								QuadroRendererController.RemoveAllQuadroItem();
							}

							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();
						break;
					}
				}
				
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif