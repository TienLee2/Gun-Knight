using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class PrototypeGameObject : Prototype
    {
		public int ID
		{
			get
			{
				return prefab.GetInstanceID();
			}
		}

		public Vector3 Extents = Vector3.one;

        public AdditionalSpawnSettings AdditionalSpawnSettings = new AdditionalSpawnSettings();
        public FailureSettings FailureSettings = new FailureSettings();
		public OverlapCheckSettings OverlapCheckSettings = new OverlapCheckSettings();
        public FlagsSettings FlagsSettings = new FlagsSettings();
        public TransformComponentsStack TransformComponentsStack = new TransformComponentsStack();

		public SimpleFilterSettings SimpleFilterSettings = new SimpleFilterSettings();
		public SimpleFilterSettings SimpleEraseFilterSettings = new SimpleFilterSettings();
		public SimpleFilterSettings SimpleModifyFilterSettings = new SimpleFilterSettings();

        public FilterStack EraseMaskFilterStack = new FilterStack();
        public FilterStack MaskFilterStack = new FilterStack();
        public FilterStack ModifyMaskFilterStack = new FilterStack();
		
		[NonSerialized]
		public PastTransform pastTransform;
		public float PositionOffset = 0;

		public FilterContext FilterContext;
		public Texture2D FilterMaskTexture2D;

#if UNITY_EDITOR
		private TransformComponentsView _transformComponentView = null;
		public TransformComponentsView TransformComponentView
		{
			get
			{
				if(_transformComponentView == null || _transformComponentView.transformComponentsStack == null )
				{
					_transformComponentView = new TransformComponentsView(new GUIContent("Transform Components Settings"), TransformComponentsStack);
				}

				return _transformComponentView;
			}
		}

		private FilterStackView _eraseMaskFilterStackView = null;
		public FilterStackView EraseMaskFilterStackView
		{
			get
			{
				if( _eraseMaskFilterStackView == null || _eraseMaskFilterStackView.m_filterStack == null )
				{
					_eraseMaskFilterStackView = new FilterStackView(new GUIContent("Erase Mask Filters Settings"), EraseMaskFilterStack );
				}

				return _eraseMaskFilterStackView;
			}
		}

		private FilterStackView _maskFilterStackView = null;
		public FilterStackView MaskFilterStackView
		{
			get
			{
				if( _maskFilterStackView == null || _maskFilterStackView.m_filterStack == null )
				{
					_maskFilterStackView = new FilterStackView(new GUIContent("Mask Filters Settings"), MaskFilterStack );
				}

				return _maskFilterStackView;
			}
		}
		
		private FilterStackView _modifyFilterStackView = null;
		public FilterStackView ModifyFilterStackView
		{
			get
			{
				if( _modifyFilterStackView == null || _modifyFilterStackView.m_filterStack == null )
				{
					_modifyFilterStackView = new FilterStackView(new GUIContent("Modify Filters Settings"), ModifyMaskFilterStack);
				}

				return _modifyFilterStackView;
			}
		}
#endif

		public PrototypeGameObject(GameObject gameObject)
        {
            prefab = gameObject;
            pastTransform = new PastTransform(gameObject.transform);
        }

        public PrototypeGameObject(GameObject gameObject, Vector3 extents)
        {
            prefab = gameObject;
            pastTransform = new PastTransform(gameObject.transform);
            this.Extents = extents;
        }
    }
}