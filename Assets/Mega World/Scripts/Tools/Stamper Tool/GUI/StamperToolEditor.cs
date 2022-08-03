#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace MegaWorld.Stamper
{
    [CustomEditor(typeof(StamperTool))]
    public class StamperToolEditor : Editor
    {
		private StamperTool _stamperTool;

        #region UI Settings
		private bool SelectTypeSettingsFoldout = true;
		private bool SelectPrototypeSettingsFoldout = true;
        public bool StamperToolControllerFoldout = true;
		#endregion

        void OnEnable()
        {
            _stamperTool = (StamperTool)target;

            if(_stamperTool.Area.Bounds == null)
            {
                _stamperTool.Area.SetAreaBounds(_stamperTool);
            }
        }

        public override void OnInspectorGUI()
        {
			InternalDragAndDrop.OnBeginGUI();

			OnGUI();

			InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
			{
				Repaint();
			}
        }

        public void OnGUI()
		{		
			CustomEditorGUI.isInspector = true;

			_stamperTool.Data.OnGUI(MegaWorldTools.StamperTool);

			_stamperTool.Area.OnGUI(_stamperTool);
			StamperToolControllerWindowGUI();
			
			if(_stamperTool.Data.SelectedVariables.HasOneSelectedType())
			{
				switch (_stamperTool.Data.SelectedVariables.SelectedType.ResourceType)
				{
					case ResourceType.GameObject:
					{
						if(_stamperTool.Data.SelectedVariables.HasSelectedResourceGameObject())
            			{
            			    MegaWorldPath.GeneralDataPackage.GameObjectControllerEditor.OnGUI();
            			}

						DrawSettingsForGameObject();
						break;
					}
					case ResourceType.TerrainDetail:
					{
						DrawSettingsForTerrainDetail();
						break;
					}
					case ResourceType.TerrainTexture:
					{
						DrawSettingsForTerrainTexture();
						break;
					}
					case ResourceType.QuadroItem:
					{
						DrawSettingsForQuadroItem();
						break;
					}
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one type to display type settings");     
			}
			
			_stamperTool.Data.SetAllDataDirty();
		}

        private void DrawSettingsForTerrainDetail()
		{
			SelectTypeSettingsFoldout = CustomEditorGUI.Foldout(SelectTypeSettingsFoldout, "Type Settings");

			if(SelectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				CustomEditorGUI.BeginChangeCheck();

				MegaWorldPath.GeneralDataPackage.TerrainResourcesControllerEditor.OnGUI(_stamperTool.Data.SelectedVariables.SelectedType);

				if(CustomEditorGUI.EndChangeCheck())
				{
					_stamperTool.AutoSpawn(_stamperTool.Data.SelectedVariables.SelectedType);
				}

				EditorGUI.indentLevel--;
			}

			if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoTerrainDetail())
			{
				PrototypeTerrainDetail proto = _stamperTool.Data.SelectedVariables.SelectedProtoTerrainDetail;

				SelectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(SelectPrototypeSettingsFoldout, proto.TerrainDetailName);

				if(SelectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					CustomEditorGUI.BeginChangeCheck();

					proto.SpawnDetailSettings.OnGUI(MegaWorldTools.StamperTool);
					DrawMaskFilterSettings(proto.MaskFilterStackView, _stamperTool.Data.SelectedVariables.SelectedType);
					
					proto.FailureSettings.OnGUI();

					if(CustomEditorGUI.EndChangeCheck())
					{
						_stamperTool.AutoSpawn(proto);
					}

					proto.TerrainDetailSettings.OnGUI(proto, MegaWorldTools.StamperTool);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings."); 
			}
		}

        private void DrawSettingsForGameObject()
		{
			CustomEditorGUI.BeginChangeCheck();

			SelectTypeSettingsFoldout = CustomEditorGUI.Foldout(SelectTypeSettingsFoldout, "Type Settings");

			if(SelectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				Type type = _stamperTool.Data.SelectedVariables.SelectedType;

				type.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), type.FilterType);
				type.GenerateRandomSeed = CustomEditorGUI.Toggle(new GUIContent("Generate Random Seed"), type.GenerateRandomSeed);
				if(type.GenerateRandomSeed == false)
				{
					EditorGUI.indentLevel++;
					
					type.RandomSeed = CustomEditorGUI.IntField(new GUIContent("Random Seed"), type.RandomSeed);

					EditorGUI.indentLevel--;
				}

				type.ScatterSettings.OnGUI(MegaWorldTools.StamperTool);
				type.LayerSettings.OnGUI();
				
				EditorGUI.indentLevel--;
			}

			if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoGameObject())
			{
				PrototypeGameObject proto = _stamperTool.Data.SelectedVariables.SelectedProtoGameObject;

				SelectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(SelectPrototypeSettingsFoldout, proto.prefab.name);

				if(SelectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					CustomEditorGUI.BeginChangeCheck();

					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.StamperTool);

					switch (_stamperTool.Data.SelectedVariables.SelectedType.FilterType)
					{
						case FilterType.SimpleFilter:
						{
							proto.SimpleFilterSettings.OnGUI("Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.MaskFilterStackView, _stamperTool.Data.SelectedVariables.SelectedType);
							break;
						}
					}

					proto.FailureSettings.OnGUI();
					proto.OverlapCheckSettings.OnGUI();
					TransformSettingWindowGUI(proto.TransformComponentView, _stamperTool.Data.SelectedVariables.SelectedType);
					proto.FlagsSettings.OnGUI();
			
					if(CustomEditorGUI.EndChangeCheck())
					{
						_stamperTool.AutoSpawn(_stamperTool.Data.SelectedVariables.SelectedType);
					}				

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings."); 
			}

			if(CustomEditorGUI.EndChangeCheck())
			{
				_stamperTool.AutoSpawn(_stamperTool.Data.SelectedVariables.SelectedType);
			}
		}

        private void DrawSettingsForQuadroItem()
		{
			CustomEditorGUI.BeginChangeCheck();

			SelectTypeSettingsFoldout = CustomEditorGUI.Foldout(SelectTypeSettingsFoldout, "Type Settings");

			if(SelectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				Type type = _stamperTool.Data.SelectedVariables.SelectedType;

				type.FilterType = (FilterType)CustomEditorGUI.EnumPopup(new GUIContent("Filter Type"), type.FilterType);
				type.GenerateRandomSeed = CustomEditorGUI.Toggle(new GUIContent("Generate Random Seed"), type.GenerateRandomSeed);
				if(type.GenerateRandomSeed == false)
				{
					EditorGUI.indentLevel++;
					
					type.RandomSeed = CustomEditorGUI.IntField(new GUIContent("Random Seed"), type.RandomSeed);

					EditorGUI.indentLevel--;
				}

				QuadroRendererController.QuadroRendererControllerEditor.QuadroRenderControllerWindowGUI(type);
				type.ScatterSettings.OnGUI(MegaWorldTools.StamperTool);
				type.LayerSettings.OnGUI();

				EditorGUI.indentLevel--;
			}

			if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoQuadroItem())
			{
				PrototypeQuadroItem proto = _stamperTool.Data.SelectedVariables.SelectedProtoQuadroItem;

				SelectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(SelectPrototypeSettingsFoldout, proto.prefab.name);

				if(SelectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					proto.AdditionalSpawnSettings.OnGUI(MegaWorldTools.StamperTool);

					switch (_stamperTool.Data.SelectedVariables.SelectedType.FilterType)
					{
						case FilterType.SimpleFilter:
						{
							proto.SimpleFilterSettings.OnGUI("Simple Filter Settings");
							break;
						}
						case FilterType.MaskFilter:
						{
							DrawMaskFilterSettings(proto.MaskFilterStackView, _stamperTool.Data.SelectedVariables.SelectedType);
							
							break;
						}
					}

					proto.FailureSettings.OnGUI();
					proto.OverlapCheckSettings.OnGUI();
					TransformSettingWindowGUI(proto.TransformComponentView, _stamperTool.Data.SelectedVariables.SelectedType);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");
			}

			if(CustomEditorGUI.EndChangeCheck())
			{
				_stamperTool.AutoSpawn(_stamperTool.Data.SelectedVariables.SelectedType);
			}
		}

        private void DrawSettingsForTerrainTexture()
		{
			CustomEditorGUI.BeginChangeCheck();

			SelectTypeSettingsFoldout = CustomEditorGUI.Foldout(SelectTypeSettingsFoldout, "Type Settings");

			if(SelectTypeSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				MegaWorldPath.GeneralDataPackage.TerrainResourcesControllerEditor.OnGUI(_stamperTool.Data.SelectedVariables.SelectedType);

				EditorGUI.indentLevel--;
			}

			if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoTerrainTexture() == true)
			{
				PrototypeTerrainTexture proto = _stamperTool.Data.SelectedVariables.SelectedProtoTerrainTexture;
				
				SelectPrototypeSettingsFoldout = CustomEditorGUI.Foldout(SelectPrototypeSettingsFoldout, proto.TerrainTextureName);

				if(SelectPrototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawMaskFilterSettings(proto.MaskFilterStackView, _stamperTool.Data.SelectedVariables.SelectedType);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUI.HelpBox("Select one prototype to display prototype settings.");   
			}

			if(CustomEditorGUI.EndChangeCheck())
			{
				_stamperTool.AutoSpawn(_stamperTool.Data.SelectedVariables.SelectedType);
			}
		}

        public void StamperToolControllerWindowGUI()
		{
			StamperToolControllerFoldout = CustomEditorGUI.Foldout(StamperToolControllerFoldout, "Stamper Tool Controller");

			if(StamperToolControllerFoldout)
			{
				EditorGUI.indentLevel++;

				_stamperTool.StamperToolControllerSettings.Visualisation = CustomEditorGUI.Toggle(visualisation, _stamperTool.StamperToolControllerSettings.Visualisation);

				if(_stamperTool.Area.UseSpawnCells == false)
				{
					_stamperTool.StamperToolControllerSettings.AutoSpawn = CustomEditorGUI.Toggle(autoSpawn, _stamperTool.StamperToolControllerSettings.AutoSpawn);

					if(_stamperTool.StamperToolControllerSettings.AutoSpawn)
					{
						EditorGUI.indentLevel++;
						_stamperTool.StamperToolControllerSettings.DelayAutoSpawn = CustomEditorGUI.Slider(delayAutoSpawn, _stamperTool.StamperToolControllerSettings.DelayAutoSpawn, 0, 3);
						EditorGUI.indentLevel--;
						
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								UnspawnForStamper.UnspawnAllResourcesTypes(_stamperTool.Data);
								_stamperTool.Spawn();
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);
					}
					else
					{
						DrawSpawnControls();
					}
				}
				else
				{
					CustomEditorGUI.HelpBox("Auto Spawn does not support when \"Use Spawn Cells\" is enabled in \"Area Settings\".");
	
					DrawSpawnWithCellsControls();
				}

				if (_stamperTool.SpawnProgress == 0)
				{
					if(_stamperTool.Data.SelectedVariables.SelectedProtoGameObjectList.Count != 0 || _stamperTool.Data.SelectedVariables.SelectedProtoTerrainDetailList.Count != 0
						|| _stamperTool.Data.SelectedVariables.SelectedProtoQuadroItemList.Count != 0)
					{
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
							if(CustomEditorGUI.ClickButton("Unspawn Selected Prototypes", ButtonStyle.Remove, ButtonSize.ClickButton))
							{
								if (EditorUtility.DisplayDialog("WARNING!",
									"Are you sure you want to remove all resource instances that have been selected from the scene?",
									"OK", "Cancel"))
								{
									UnspawnForStamper.UnspawnSelectedProto(_stamperTool.Data);
								}
							}

							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		private void DrawSpawnControls()
        {
            if (_stamperTool.SpawnProgress > 0f && _stamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_stamperTool.Data.SelectedVariables.SelectedProtoTerrainTextureList.Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
						if(CustomEditorGUI.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							UnspawnForStamper.UnspawnAllResourcesTypes(_stamperTool.Data);
							_stamperTool.Spawn();
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ClickButton("Spawn", ButtonStyle.Add))
					{
						_stamperTool.Spawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		private void DrawSpawnWithCellsControls()
        {
			if (_stamperTool.SpawnProgress > 0f && _stamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_stamperTool.Data.SelectedVariables.SelectedProtoTerrainTextureList.Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
						if(CustomEditorGUI.ClickButton("Refresh", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							if(_stamperTool.Area.CellList.Count == 0)
							{
								_stamperTool.Area.CreateCells();
							}

							UnspawnForStamper.UnspawnAllResourcesTypes(_stamperTool.Data);
							_stamperTool.SpawnWithCells(_stamperTool.Area.CellList);
						}
	
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
	
					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
					if(CustomEditorGUI.ClickButton("Spawn", ButtonStyle.Add))
					{
						if(_stamperTool.Area.CellList.Count == 0)
						{
							_stamperTool.Area.CreateCells();
						}

						_stamperTool.SpawnWithCells(_stamperTool.Area.CellList);
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		public void DrawMaskFilterSettings(FilterStackView maskFilterStackView, Type type)
		{
			if(maskFilterStackView.headerFoldout)
			{
				Rect maskRect = EditorGUILayout.GetControlRect(true, maskFilterStackView.m_reorderableList.GetHeight());
				maskRect = EditorGUI.IndentedRect(maskRect);

				EditorGUI.BeginChangeCheck();

				maskFilterStackView.OnGUI(maskRect, type);
			
				if(EditorGUI.EndChangeCheck())
				{
					CustomEditorGUI.changeCheck = true;
					_stamperTool.StamperVisualisation.UpdateMask = true;
				}
			}
			else
			{
				maskFilterStackView.headerFoldout = CustomEditorGUI.Foldout(maskFilterStackView.headerFoldout, maskFilterStackView.m_label.text);
			}
		}

		public void TransformSettingWindowGUI(TransformComponentsView transformComponentsView, Type type)
		{
			if(transformComponentsView.headerFoldout)
			{
				Rect rect = EditorGUILayout.GetControlRect(true, transformComponentsView.m_reorderableList.GetHeight());
				rect = EditorGUI.IndentedRect(rect);

				EditorGUI.BeginChangeCheck();

				transformComponentsView.OnGUI(rect, type);

				if(EditorGUI.EndChangeCheck())
				{
					CustomEditorGUI.changeCheck = true;
				}
			}
			else
			{
				transformComponentsView.headerFoldout = CustomEditorGUI.Foldout(transformComponentsView.headerFoldout, transformComponentsView.m_label.text);
			}
		}

        public void CancelSpawn()
        {
            _stamperTool.CancelSpawn = true;
            _stamperTool.SpawnComplete = true;
            _stamperTool.SpawnProgress = 0f;
			EditorUtility.ClearProgressBar();
        }

		void ProgressBar(string label, float value)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }

        [MenuItem("GameObject/MegaWorld/Add Stamper", false, 14)]
    	public static void AddStamper(MenuCommand menuCommand)
    	{
    		GameObject stamper = new GameObject("Stamper");
            stamper.transform.localScale = new Vector3(150, 150, 150);
    		stamper.AddComponent<StamperTool>();
    		Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
    		Selection.activeObject = stamper;
    	}

		[NonSerialized]
		public GUIContent visualisation = new GUIContent("Visualisation", "Allows you to see the Mask Filter Settings visualization.");
		[NonSerialized]
		public GUIContent autoSpawn = new GUIContent("Auto Spawn", "Allows you to do automatic deletion and then spawn when you changed the settings.");
		[NonSerialized]
		public GUIContent delayAutoSpawn = new GUIContent("Delay Auto Spawn", "Respawn delay in seconds.");
    }
}
#endif