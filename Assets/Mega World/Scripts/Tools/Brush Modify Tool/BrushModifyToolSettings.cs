using UnityEngine;
using System;

namespace MegaWorld.BrushModify
{
    [Serializable]
    public class BrushModifyToolSettings 
    {
        public ModifyTransformComponentsStack ModifyTransformComponentsStack = new ModifyTransformComponentsStack ();

#if UNITY_EDITOR
		private ModifyTransformComponentsView _modifyTransformComponentsView = null;
		public ModifyTransformComponentsView ModifyTransformComponentsView
		{
			get
			{
				if( _modifyTransformComponentsView == null || _modifyTransformComponentsView.transformComponentsStack == null )
				{
					_modifyTransformComponentsView = new ModifyTransformComponentsView(new GUIContent("Modify Transform Components"), ModifyTransformComponentsStack);
				}

				return _modifyTransformComponentsView;
			}
		}
#endif

        public bool ModifyByLayer;
        public LayerMask ModifyLayers;

        #if UNITY_EDITOR
        public BrushModifyToolSettingsEditor ModifySettingsEditor = new BrushModifyToolSettingsEditor();

        public void OnGUI()
        {
            ModifySettingsEditor.OnGUI(this);
        }
        #endif    
    }
}