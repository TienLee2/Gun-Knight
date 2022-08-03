#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
	[Serializable]
    public class Clipboard 
    {
        [NonSerialized] private TerrainDetailSettings _copiedTerrainDetailSettings = null;
        [NonSerialized] private SpawnDetailSettings _copiedAdditionalDetailSettings = null;
        [NonSerialized] private AdditionalSpawnSettings _copiedAdditionalSpawnSettings = null;
        [NonSerialized] private FailureSettings _copiedFailureSettings = null;
        [NonSerialized] private OverlapCheckSettings _copiedOverlapCheckSettings = null;
        [NonSerialized] private TransformComponentsStack _copiedTransformComponentsStack = null;
        [NonSerialized] private FilterStack _copiedFilterStack = null;
        [NonSerialized] private SimpleFilterSettings _copiedSimpleFilterSettings = null;
        [NonSerialized] private FlagsSettings _copiedFlagsSettings = null;

        public void ClipboardPrototypeCopyAllSettingsToGameObject(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if(selectedVariables.HasOneSelectedProtoGameObject())
            {	
                PrototypeGameObject selectedProto = selectedVariables.SelectedProtoGameObject;

                if(selectedVariables.SelectedType.FilterType == FilterType.MaskFilter)
                {
                    if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                    {
                        switch (tool)
                        {
                            case MegaWorldTools.BrushErase:
                            {
                                _copiedFilterStack = selectedProto.EraseMaskFilterStack;
                                break;
                            }
                            case MegaWorldTools.BrushModify:
                            {
                                _copiedFilterStack = selectedProto.ModifyMaskFilterStack;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _copiedFilterStack = selectedProto.MaskFilterStack;
                    }
                }
                else
                {
                    if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                    {
                        switch (tool)
                        {
                            case MegaWorldTools.BrushErase:
                            {
                                _copiedSimpleFilterSettings = selectedProto.SimpleEraseFilterSettings;
                                break;
                            }
                            case MegaWorldTools.BrushModify:
                            {
                                _copiedSimpleFilterSettings = selectedProto.SimpleModifyFilterSettings;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _copiedSimpleFilterSettings = selectedProto.SimpleFilterSettings;
                    }
                }

                _copiedAdditionalSpawnSettings = new AdditionalSpawnSettings(selectedProto.AdditionalSpawnSettings);
                _copiedFailureSettings = new FailureSettings(selectedProto.FailureSettings);
                _copiedOverlapCheckSettings = new OverlapCheckSettings(selectedProto.OverlapCheckSettings);
                _copiedFlagsSettings = new FlagsSettings(selectedProto.FlagsSettings);
                _copiedTransformComponentsStack = selectedProto.TransformComponentsStack;
            }
    	}

        public void ClipboardPrototypeCopyAllSettingsToTerrainDetail(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if(selectedVariables.HasOneSelectedProtoTerrainDetail())
            {	
                PrototypeTerrainDetail selectedProto = selectedVariables.SelectedProtoTerrainDetail;

                if(tool == MegaWorldTools.BrushErase)
                {
                    _copiedFilterStack = selectedProto.EraseMaskFilterStack;
                }
                else
                {
                    _copiedFilterStack = selectedProto.MaskFilterStack;
                }

                _copiedFailureSettings = new FailureSettings(selectedProto.FailureSettings);
                _copiedAdditionalDetailSettings = new SpawnDetailSettings(selectedProto.SpawnDetailSettings);
                _copiedTerrainDetailSettings = new TerrainDetailSettings(selectedProto.TerrainDetailSettings);
            }
    	}

        public void ClipboardPrototypeCopyAllSettingsToQuadroItem(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if(selectedVariables.HasOneSelectedProtoQuadroItem())
            {	
                PrototypeQuadroItem selectedProto = selectedVariables.SelectedProtoQuadroItem;

                if(selectedVariables.SelectedType.FilterType == FilterType.MaskFilter)
                {
                    if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                    {
                        switch (tool)
                        {
                            case MegaWorldTools.BrushErase:
                            {
                                _copiedFilterStack = selectedProto.EraseMaskFilterStack;
                                break;
                            }
                            case MegaWorldTools.BrushModify:
                            {
                                _copiedFilterStack = selectedProto.ModifyMaskFilterStack;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _copiedFilterStack = selectedProto.MaskFilterStack;
                    }
                }
                else
                {
                    if(tool == MegaWorldTools.BrushErase || tool == MegaWorldTools.BrushModify)
                    {
                        switch (tool)
                        {
                            case MegaWorldTools.BrushErase:
                            {
                                _copiedSimpleFilterSettings = selectedProto.SimpleEraseFilterSettings;
                                break;
                            }
                            case MegaWorldTools.BrushModify:
                            {
                                _copiedSimpleFilterSettings = selectedProto.SimpleModifyFilterSettings;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _copiedSimpleFilterSettings = selectedProto.SimpleFilterSettings;
                    }
                }

                _copiedAdditionalSpawnSettings = new AdditionalSpawnSettings(selectedProto.AdditionalSpawnSettings);
                _copiedFailureSettings = new FailureSettings(selectedProto.FailureSettings);
                _copiedOverlapCheckSettings = new OverlapCheckSettings(selectedProto.OverlapCheckSettings);
                _copiedTransformComponentsStack = selectedProto.TransformComponentsStack;
            }
    	}

        public void ClipboardPrototypeCopyAllSettingsToTerrainTexture(SelectedTypeVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedProtoTerrainTexture())
            {	
                PrototypeTerrainTexture selectedProto = selectedVariables.SelectedProtoTerrainTexture;

                _copiedFilterStack = selectedProto.MaskFilterStack;
            }
    	}

        public void ClipboardPasteAllSettingsToGameObject(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            switch (tool)
            {
                case MegaWorldTools.StamperTool:
                case MegaWorldTools.BrushPaint:
                {
                    ClipboardPasteFilterMaskStackToGameObject(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToGameObject(selectedVariables, tool);
                    ClipboardPasteAdditionalGameObjectSettings(selectedVariables);
                    ClipboardPasteFailureSettingsToGameObject(selectedVariables);
                    ClipboardPasteOverlapCheckSettingsToGameObject(selectedVariables);
                    ClipboardPasteTransformSettingsSettingsForGameObject(selectedVariables);
                    ClipboardTagsSettings(selectedVariables);

                    break;
                }
                case MegaWorldTools.BrushErase:
                {
                    ClipboardPasteFilterMaskStackToGameObject(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToGameObject(selectedVariables, tool);
                    break;
                }
                case MegaWorldTools.BrushModify:
                {
                    ClipboardPasteFilterMaskStackToGameObject(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToGameObject(selectedVariables, tool);
                    break;
                }
            }
    	}

        public void ClipboardPasteAllSettingsToTerrainDetail(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            switch (tool)
            {
                case MegaWorldTools.StamperTool:
                case MegaWorldTools.BrushPaint:
                {
                    ClipboardPasteFilterMaskStackToTerrainDetail(selectedVariables, tool);
                    ClipboardPasteTerrainDetailSettings(selectedVariables);
                    ClipboardPasteSpawnDetailSettings(selectedVariables);
                    ClipboardPasteFailureSettingsToTerrainDetail(selectedVariables);

                    break;
                }
                case MegaWorldTools.BrushErase:
                {
                    ClipboardPasteFilterMaskStackToTerrainDetail(selectedVariables, tool);
                    break;
                }
                case MegaWorldTools.BrushModify:
                {
                    ClipboardPasteFilterMaskStackToTerrainDetail(selectedVariables, tool);
                    break;
                }
            }
    	}

        public void ClipboardPasteAllSettingsToQuadroItem(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            switch (tool)
            {
                case MegaWorldTools.StamperTool:
                case MegaWorldTools.BrushPaint:
                {
                    ClipboardPasteFilterMaskStackToQuadroItem(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedVariables, tool);
                    ClipboardPasteAdditionalSpawnSettingsToQuadroItem(selectedVariables);
                    ClipboardPasteTransformSettingsSettingsForQuadroItem(selectedVariables);
                    ClipboardPasteFailureSettingsToQuadroItem(selectedVariables);
                    ClipboardPasteOverlapCheckSettingsToQuadroItem(selectedVariables);

                    break;
                }
                case MegaWorldTools.BrushErase:
                {
                    ClipboardPasteFilterMaskStackToQuadroItem(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedVariables, tool);
                    break;
                }
                case MegaWorldTools.BrushModify:
                {
                    ClipboardPasteFilterMaskStackToQuadroItem(selectedVariables, tool);
                    ClipboardPasteSimpleFilterSettingsToQuadroItem(selectedVariables, tool);
                    break;
                }
            }
    	}

        public void ClipboardPasteSimpleFilterSettingsToGameObject(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if (_copiedSimpleFilterSettings == null)
            {
                return;
            }
    
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                switch (tool)
                {
                    case MegaWorldTools.StamperTool:
                    case MegaWorldTools.BrushPaint:
                    {
                        proto.SimpleFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                    case MegaWorldTools.BrushErase:
                    {
                        proto.SimpleEraseFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                    case MegaWorldTools.BrushModify:
                    {
                        proto.SimpleModifyFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                }
            }
    	}

        public void ClipboardPasteTerrainDetailSettings(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedTerrainDetailSettings == null)
            {
                return;
            }
    
            foreach(PrototypeTerrainDetail proto in selectedVariables.SelectedProtoTerrainDetailList) 
            {
                proto.TerrainDetailSettings.CopyFrom(_copiedTerrainDetailSettings);

                foreach (Terrain activeTerrain in Terrain.activeTerrains)
    			{
    				TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, proto);
    			}
            }
    	}

        public void ClipboardPasteSpawnDetailSettings(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedAdditionalDetailSettings == null)
            {
                return;
            }
    
            foreach(PrototypeTerrainDetail proto in selectedVariables.SelectedProtoTerrainDetailList) 
            {
                proto.SpawnDetailSettings.CopyFrom(_copiedAdditionalDetailSettings);
            }
    	}

        public void ClipboardPasteAdditionalGameObjectSettings(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedAdditionalSpawnSettings == null)
            {
                return;
            }
    
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                proto.AdditionalSpawnSettings.CopyFrom(_copiedAdditionalSpawnSettings);
            }
    	}

        public void ClipboardPasteFailureSettingsToGameObject(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedFailureSettings == null)
            {
                return;
            }
    
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                proto.FailureSettings.CopyFrom(_copiedFailureSettings);
            }
    	}

        public void ClipboardPasteFailureSettingsToTerrainDetail(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedFailureSettings == null)
            {
                return;
            }
    
            foreach(PrototypeTerrainDetail proto in selectedVariables.SelectedProtoTerrainDetailList) 
            {
                proto.FailureSettings.CopyFrom(_copiedFailureSettings);
            }
    	}

    	public void ClipboardPasteTransformSettingsSettingsForGameObject(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedTransformComponentsStack == null)
            {
                return;
            }

            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                proto.TransformComponentsStack.CopyTransformComponentsStack(_copiedTransformComponentsStack, selectedVariables.SelectedType);
            }
    	}

        public void ClipboardPasteFilterMaskStackToGameObject(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if (_copiedFilterStack == null)
            {
                return;
            }

            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                switch (tool)
                {
                    case MegaWorldTools.StamperTool:
                    case MegaWorldTools.BrushPaint:
                    {
                        proto.MaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                    case MegaWorldTools.BrushErase:
                    {
                        proto.EraseMaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                    case MegaWorldTools.BrushModify:
                    {
                        proto.ModifyMaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                }
            }
    	}

        public void ClipboardPasteFilterMaskStackToTerrainDetail(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if (_copiedFilterStack == null)
            {
                return;
            }

            foreach(PrototypeTerrainDetail proto in selectedVariables.SelectedProtoTerrainDetailList) 
            {
                switch (tool)
                {
                    case MegaWorldTools.StamperTool:
                    case MegaWorldTools.BrushPaint:
                    {
                        proto.MaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                    case MegaWorldTools.BrushErase:
                    {
                        proto.EraseMaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                }
            }
    	}


        public void ClipboardPasteFilterMaskStackToTerrainTexture(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedFilterStack == null)
            {
                return;
            }

            foreach(PrototypeTerrainTexture proto in selectedVariables.SelectedProtoTerrainTextureList) 
            {
                proto.MaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
            }
    	}

        public void ClipboardPasteFilterMaskStackToQuadroItem(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if (_copiedFilterStack == null)
            {
                return;
            }

            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                switch (tool)
                {
                    case MegaWorldTools.StamperTool:
                    case MegaWorldTools.BrushPaint:
                    {
                        proto.MaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                    case MegaWorldTools.BrushErase:
                    {
                        proto.EraseMaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                    case MegaWorldTools.BrushModify:
                    {
                        proto.ModifyMaskFilterStack.CopyFilterStack(_copiedFilterStack, selectedVariables.SelectedType);
                        break;
                    }
                }
            }
    	}

        public void ClipboardPasteSimpleFilterSettingsToQuadroItem(SelectedTypeVariables selectedVariables, MegaWorldTools tool)
    	{
            if (_copiedSimpleFilterSettings == null)
            {
                return;
            }
    
            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                switch (tool)
                {
                    case MegaWorldTools.StamperTool:
                    case MegaWorldTools.BrushPaint:
                    {
                        proto.SimpleFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                    case MegaWorldTools.BrushErase:
                    {
                        proto.SimpleEraseFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                    case MegaWorldTools.BrushModify:
                    {
                        proto.SimpleModifyFilterSettings.CopyFrom(_copiedSimpleFilterSettings);
                        break;
                    }
                }
            }
    	}

        public void ClipboardTagsSettings(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedFlagsSettings == null)
            {
                return;
            }

            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                proto.FlagsSettings.CopyFrom(_copiedFlagsSettings);
            }
    	}

        public void ClipboardPasteTransformSettingsSettingsForQuadroItem(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedTransformComponentsStack == null)
            {
                return;
            }

            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                proto.TransformComponentsStack.CopyTransformComponentsStack(_copiedTransformComponentsStack, selectedVariables.SelectedType);
            }
    	}

        public void ClipboardPasteFailureSettingsToQuadroItem(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedFailureSettings == null)
            {
                return;
            }
    
            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                proto.FailureSettings.CopyFrom(_copiedFailureSettings);
            }
    	}

        public void ClipboardPasteAdditionalSpawnSettingsToQuadroItem(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedAdditionalSpawnSettings == null)
            {
                return;
            }
    
            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                proto.AdditionalSpawnSettings.CopyFrom(_copiedAdditionalSpawnSettings);
            }
    	}

        public void ClipboardPasteOverlapCheckSettingsToQuadroItem(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedOverlapCheckSettings == null)
            {
                return;
            }
    
            foreach(PrototypeQuadroItem proto in selectedVariables.SelectedProtoQuadroItemList) 
            {
                proto.OverlapCheckSettings.CopyFrom(_copiedOverlapCheckSettings);
            }
    	}

        public void ClipboardPasteOverlapCheckSettingsToGameObject(SelectedTypeVariables selectedVariables)
    	{
            if (_copiedOverlapCheckSettings == null)
            {
                return;
            }
    
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                proto.OverlapCheckSettings.CopyFrom(_copiedOverlapCheckSettings);
            }
    	}
    }
}
#endif