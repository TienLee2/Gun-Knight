using UnityEditor;
using UnityEngine;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    public partial class QuadroRendererEditor : Editor
    {
        public void DrawActions()
		{
			selectActionsFoldout = CustomEditorGUI.Foldout(selectActionsFoldout, "Actions");

			if(selectActionsFoldout)
			{
				EditorGUI.indentLevel++;

				GUILayout.BeginHorizontal();
           		{
           		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
           		    if (CustomEditorGUI.ClickButton("Refresh All Prefabs"))
            		{
						quadroRenderer.SetupItemModels();
            		}
           		    GUILayout.Space(3);
           		}
           		GUILayout.EndHorizontal();

				GUILayout.Space(3);

				GUILayout.BeginHorizontal();
            	{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ClickButton("Regenerate Instanced Indirect Shaders", ButtonStyle.Add))
					{
            	        QuadroRendererDefines.RegenerateInstancedIndirectShaders();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);

				selectAddComponent = CustomEditorGUI.Foldout(selectAddComponent, "Add Component");

				if(selectAddComponent)
				{
					EditorGUI.indentLevel++;

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            		    if(CustomEditorGUI.ClickButton("Add Extension", ButtonStyle.Add))
					    {
					    	AddExtensionMenu();
					    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

					GUILayout.Space(3);

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            		    if(CustomEditorGUI.ClickButton("Add Occlusion Culling", ButtonStyle.Add))
					    {
							AddOcclusionCullingMenu();
					    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

					GUILayout.Space(3);

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            		    if(CustomEditorGUI.ClickButton("Add Convertor", ButtonStyle.Add))
					    {
							AddConvertorMenu();
					    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

					GUILayout.Space(3);

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
            		    if(CustomEditorGUI.ClickButton("Add Storage", ButtonStyle.Add))
					    {
							AddStorageMenu();
					    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
		}

		public void AddOcclusionCullingMenu()
        {
            GenericMenu menu = new GenericMenu();

			if(quadroRenderer.GetComponent<TerrainCellsOcclusionCulling>() == null)
			{
				menu.AddItem(new GUIContent("Terrain Cell Occlusion Culling"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.gameObject.AddComponent<TerrainCellsOcclusionCulling>()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Terrain Cell Occlusion Culling"));
			}

			if(quadroRenderer.GetComponent<TerrainCDLODOcclusionCulling>() == null)
			{
				menu.AddItem(new GUIContent("Terrain CDLOD Occlusion Culling"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.gameObject.AddComponent<TerrainCDLODOcclusionCulling>()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Terrain CDLOD Occlusion Culling"));
			}

			menu.ShowAsContext();
        }

		public void AddConvertorMenu()
        {
            GenericMenu menu = new GenericMenu();

			if(quadroRenderer.GetComponent<UnityTerrainTreeConverter>() == null)
			{
				menu.AddItem(new GUIContent("Unity Terrain Tree Converter"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.gameObject.AddComponent<UnityTerrainTreeConverter>()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Unity Terrain Tree Converter"));
			}

			if(quadroRenderer.GetComponent<GameObjectConverter>() == null)
			{
				menu.AddItem(new GUIContent("GameObject Converter"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.gameObject.AddComponent<GameObjectConverter>()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("GameObject Converter"));
			}

			menu.ShowAsContext();
        }

		public void AddExtensionMenu()
        {
            GenericMenu menu = new GenericMenu();

			if(quadroRenderer.GetComponentInChildren<BillboardGenerator>() == null)
			{
				menu.AddItem(new GUIContent("Billboard Generator"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.CreateBillboardGenerator()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Billboard Generator"));
			}

			if(quadroRenderer.GetComponentInChildren<ColliderSystem>() == null)
			{
				menu.AddItem(new GUIContent("Collider System"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.CreateColliderSystem()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Collider System"));
			}

			if(quadroRenderer.GetComponentInChildren<SnapToObject>() == null)
			{
				menu.AddItem(new GUIContent("Snap To Object"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.CreateSnapToObject()));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Snap To Object"));
			}

			menu.ShowAsContext();
        }

		public void AddStorageMenu()
        {
            GenericMenu menu = new GenericMenu();

    		if(quadroRenderer.GetComponent<StorageTerrainCells>() == null)
    		{
    			menu.AddItem(new GUIContent("Storage Terrain Cells"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => quadroRenderer.gameObject.AddComponent<StorageTerrainCells>()));
    		}
    		else
    		{
    			menu.AddDisabledItem(new GUIContent("Storage Terrain Cells"));
    		}

    		menu.ShowAsContext();
        }
    }
}
