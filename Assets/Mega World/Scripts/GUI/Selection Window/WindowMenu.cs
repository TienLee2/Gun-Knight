#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;

namespace MegaWorld
{
    public static class WindowMenu 
    {
        public static GenericMenu TypesWindowMenu(BasicData data)
        {
            GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Add Type"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.AddType(data.TypeList))); 

            return menu;
        }

		public static  GenericMenu TypeMenu(BasicData data, int currentTypeIndexForGUI)
        {
            GenericMenu menu = new GenericMenu();

			Type type = data.TypeList[currentTypeIndexForGUI];

            if(type != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(type)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.DeleteSelectedTypes(data.TypeList)));
			menu.AddItem(new GUIContent("Rename"), data.TypeList[currentTypeIndexForGUI].Renaming, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => data.TypeList[currentTypeIndexForGUI].Renaming = !data.TypeList[currentTypeIndexForGUI].Renaming));

            return menu;
        }

		public static GenericMenu PrototypeGameObjectMenu(Type type, SelectedTypeVariables selectedTypeVariables, Clipboard clipboard, MegaWorldTools currentTool, int currentPrototypeIndexForGUI)
        {
            GenericMenu menu = new GenericMenu();

			PrototypeGameObject localProto = type.ProtoGameObjectList[currentPrototypeIndexForGUI];
			
            GameObject prefab = localProto.prefab;

            if(prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoGameObject(type)));

            if(currentTool == MegaWorldTools.BrushPaint
            || currentTool == MegaWorldTools.StamperTool
            || currentTool == MegaWorldTools.PrecisePlace
            || currentTool == MegaWorldTools.BrushErase
            || currentTool == MegaWorldTools.BrushModify)
            {
                menu.AddSeparator ("");
			    if(selectedTypeVariables.HasOneSelectedProtoGameObject())
			    {
			    	menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPrototypeCopyAllSettingsToGameObject(selectedTypeVariables, currentTool)));
			    }

                menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteAllSettingsToGameObject(selectedTypeVariables, currentTool)));
            }
            
			switch (currentTool)
            {
                case MegaWorldTools.BrushPaint:
				case MegaWorldTools.StamperTool:
                {
                    menu.AddItem(new GUIContent("Paste Settings/Additional GameObject Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteAdditionalGameObjectSettings(selectedTypeVariables)));
					
                    if(type.FilterType == FilterType.MaskFilter)
                    {
					    menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToGameObject(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToGameObject(selectedTypeVariables, currentTool)));
                    }	

					menu.AddItem(new GUIContent("Paste Settings/Failure Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFailureSettingsToGameObject(selectedTypeVariables)));
            		menu.AddItem(new GUIContent("Paste Settings/Overlap Check Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteOverlapCheckSettingsToGameObject(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/Transform Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteTransformSettingsSettingsForGameObject(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/TagsSettings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardTagsSettings(selectedTypeVariables)));
                    
                    menu.AddSeparator ("");
			        menu.AddItem(new GUIContent("Apply Templates/Big Trees"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { GameObjectTemplates.ApplyBigTreesTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Small Trees"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { GameObjectTemplates.ApplySmallTreesTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Cliffs/Big Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { GameObjectTemplates.ApplyBigCliffsTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Cliffs/Small Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { GameObjectTemplates.ApplySmallCliffsTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { GameObjectTemplates.ApplyRocksTemplate(type, proto);})));

					break;
                }
                case MegaWorldTools.PrecisePlace:
                {
                    if(type.FilterType == FilterType.MaskFilter)
                    {
					    menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToGameObject(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToGameObject(selectedTypeVariables, currentTool)));
                    }	

            		menu.AddItem(new GUIContent("Paste Settings/Overlap Check Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteOverlapCheckSettingsToGameObject(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/Transform Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteTransformSettingsSettingsForGameObject(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/TagsSettings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardTagsSettings(selectedTypeVariables)));

                    break;
                }
                case MegaWorldTools.BrushErase:
                {
					if(type.FilterType == FilterType.MaskFilter)
                    {
					    menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToGameObject(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToGameObject(selectedTypeVariables, currentTool)));
                    }	
					
                    break;
                }
                case MegaWorldTools.BrushModify:
                {
					if(type.FilterType == FilterType.MaskFilter)
                    {
					    menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToGameObject(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToGameObject(selectedTypeVariables, currentTool)));
                    }	
				
                    break;
                }
            }
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(type, true)));
			menu.AddItem(new GUIContent("Active"), localProto.active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { proto.active = !proto.active;})));

            return menu;
        }

		public static GenericMenu PrototypeTerrainDetailMenu(Type type, SelectedTypeVariables selectedTypeVariables, Clipboard clipboard, MegaWorldTools currentTool, int currentPrototypeIndexForGUI)
        {
            GenericMenu menu = new GenericMenu();
			
			PrototypeTerrainDetail localProto = type.ProtoTerrainDetailList[currentPrototypeIndexForGUI];

			if(localProto.DetailTexture != null)
           	{
           	    menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.DetailTexture)));
           	    menu.AddSeparator ("");
           	}
			else if(localProto.prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoTerrainDetail(type)));

			menu.AddSeparator ("");
			if(selectedTypeVariables.HasOneSelectedProtoTerrainDetail())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPrototypeCopyAllSettingsToTerrainDetail(selectedTypeVariables, currentTool)));
			}
                
            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteAllSettingsToTerrainDetail(selectedTypeVariables, currentTool)));

			switch (currentTool)
            {
                case MegaWorldTools.BrushPaint:
				case MegaWorldTools.StamperTool:
                {
                    menu.AddItem(new GUIContent("Paste Settings/Spawn Detail Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSpawnDetailSettings(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToTerrainDetail(selectedTypeVariables, currentTool)));
					menu.AddItem(new GUIContent("Paste Settings/Failure Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFailureSettingsToTerrainDetail(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/Terrain Detail Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteTerrainDetailSettings(selectedTypeVariables)));
                    
					break;
                }
                case MegaWorldTools.BrushErase:
                {
					menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToTerrainDetail(selectedTypeVariables, currentTool)));
					
                    break;
                }
            }
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(type, true)));
			menu.AddItem(new GUIContent("Active"), localProto.active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoTerrainDetailList.ForEach ((proto) => { proto.active = !proto.active;})));

            return menu;
        }

		public static GenericMenu PrototypeTerrainTextureMenu(Type type, SelectedTypeVariables selectedTypeVariables, Clipboard clipboard, MegaWorldTools сurrentTool, int currentPrototypeIndexForGUI)
        {
            GenericMenu menu = new GenericMenu();

			PrototypeTerrainTexture localProto = type.ProtoTerrainTextureList[currentPrototypeIndexForGUI];

			if(localProto.TerrainLayer != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.TerrainLayer)));
                menu.AddSeparator ("");
            }
			else if(localProto.TerrainTextureSettings.DiffuseTexture)
			{
				menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.TerrainTextureSettings.DiffuseTexture)));
                menu.AddSeparator ("");
			}

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoTerrainTexture(type)));

			menu.AddSeparator ("");
			if(selectedTypeVariables.HasOneSelectedProtoTerrainTexture())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPrototypeCopyAllSettingsToTerrainTexture(selectedTypeVariables)));
			}
                
			menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToTerrainTexture(selectedTypeVariables)));
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(type, true)));
			menu.AddItem(new GUIContent("Active"), localProto.active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoTerrainTextureList.ForEach ((proto) => { proto.active = !proto.active;})));

            return menu;
        }

		public static GenericMenu PrototypeQuadroItemMenu(Type type, SelectedTypeVariables selectedTypeVariables, Clipboard clipboard, MegaWorldTools currentTool, int currentPrototypeIndexForGUI)
        {			
            GenericMenu menu = new GenericMenu();

			PrototypeQuadroItem localProto = type.ProtoQuadroItemList[currentPrototypeIndexForGUI];
			
            GameObject prefab = localProto.prefab;

            if(prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoQuadroItem(type)));

            if(currentTool == MegaWorldTools.BrushPaint
            || currentTool == MegaWorldTools.StamperTool
            || currentTool == MegaWorldTools.BrushErase
            || currentTool == MegaWorldTools.BrushModify)
            {
                menu.AddSeparator ("");
			    if(selectedTypeVariables.HasOneSelectedProtoQuadroItem())
			    {
			    	menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPrototypeCopyAllSettingsToQuadroItem(selectedTypeVariables, currentTool)));
			    }

                menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteAllSettingsToQuadroItem(selectedTypeVariables, currentTool)));
            }

            switch (currentTool)
            {
                case MegaWorldTools.BrushPaint:
				case MegaWorldTools.StamperTool:
                {
                    menu.AddItem(new GUIContent("Paste Settings/Additional Tree Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteAdditionalSpawnSettingsToQuadroItem(selectedTypeVariables)));
					if(type.FilterType == FilterType.MaskFilter)
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToQuadroItem(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedTypeVariables, currentTool)));
                    }
					menu.AddItem(new GUIContent("Paste Settings/Transform Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteTransformSettingsSettingsForQuadroItem(selectedTypeVariables)));
					menu.AddItem(new GUIContent("Paste Settings/Failure Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFailureSettingsToQuadroItem(selectedTypeVariables)));
            		menu.AddItem(new GUIContent("Paste Settings/Overlap Check Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteOverlapCheckSettingsToQuadroItem(selectedTypeVariables)));
                    
                    menu.AddSeparator ("");
			        menu.AddItem(new GUIContent("Apply Templates/Big Trees"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { QuadroItemTemplates.ApplyBigTreesTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Small Trees"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { QuadroItemTemplates.ApplySmallTreesTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Cliffs/Big Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { QuadroItemTemplates.ApplyBigCliffsTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Cliffs/Small Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { QuadroItemTemplates.ApplySmallCliffsTemplate(type, proto);})));
			        menu.AddItem(new GUIContent("Apply Templates/Rocks"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { QuadroItemTemplates.ApplyRocksTemplate(type, proto);})));
                    break;
                }
                case MegaWorldTools.BrushErase:
                {
                    if(type.FilterType == FilterType.MaskFilter)
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToQuadroItem(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedTypeVariables, currentTool)));
                    }					
                    break;
                }
                case MegaWorldTools.BrushModify:
                {
                    if(type.FilterType == FilterType.MaskFilter)
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteFilterMaskStackToQuadroItem(selectedTypeVariables, currentTool)));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Paste Settings/Simple Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => clipboard.ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedTypeVariables, currentTool)));
                    }				
                    break;
                }
            }

			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(type, true)));
			menu.AddItem(new GUIContent("Active"), localProto.active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoQuadroItemList.ForEach ((proto) => { proto.active = !proto.active;})));

            return menu;
        }
    }
}
#endif