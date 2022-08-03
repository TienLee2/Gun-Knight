using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
	public class SelectedTypeVariables
	{
		public List<Type> SelectedTypeList = new List<Type>();

		public List<PrototypeGameObject> SelectedProtoGameObjectList = new List<PrototypeGameObject>();
		public List<PrototypeTerrainDetail> SelectedProtoTerrainDetailList = new List<PrototypeTerrainDetail>();
		public List<PrototypeTerrainTexture> SelectedProtoTerrainTextureList = new List<PrototypeTerrainTexture>();
        public List<PrototypeQuadroItem> SelectedProtoQuadroItemList = new List<PrototypeQuadroItem>();

		public Type SelectedType;
		public PrototypeGameObject SelectedProtoGameObject;
		public PrototypeTerrainDetail SelectedProtoTerrainDetail;
		public PrototypeTerrainTexture SelectedProtoTerrainTexture;
		public PrototypeQuadroItem SelectedProtoQuadroItem;

		public void SetAllSelectedParameters(List<Type> typeList)
		{
			ClearSelectedList();
			SetSelectedList(typeList);
			SetSelected();
		}

		public void ClearSelectedList()
		{
			SelectedTypeList.Clear();
			SelectedProtoGameObjectList.Clear();
			SelectedProtoTerrainDetailList.Clear();
			SelectedProtoTerrainTextureList.Clear();
			SelectedProtoQuadroItemList.Clear();
		}

		public void SetSelected()
		{
			SetSelectedType();
			SetSelectedProtoGameObject();
			SetSelectedProtoTerrainDetail();
			SetSelectedProtoTerrainTexture();

			SetSelectedProtoQuadroItem();
		}

		public void SetSelectedList(List<Type> typeList)
		{
		    for (int indexType = 0; indexType < typeList.Count; indexType++)
		    {
		    	if(typeList[indexType].Selected)
		    	{
					Type selectedType = typeList[indexType];
					SelectedTypeList.Add(selectedType);

					switch (selectedType.ResourceType)
					{
						case ResourceType.GameObject:
            			{
							Utility.SetSelectedPrototypeListFromAssets(selectedType.ProtoGameObjectList, SelectedProtoGameObjectList, typeof(PrototypeGameObject));
            			    break;
            			}
            			case ResourceType.TerrainDetail:
            			{
							Utility.SetSelectedPrototypeListFromAssets(selectedType.ProtoTerrainDetailList, SelectedProtoTerrainDetailList, typeof(PrototypeTerrainDetail));
            			    break;
            			}
						case ResourceType.TerrainTexture:
            			{
							Utility.SetSelectedPrototypeListFromAssets(selectedType.ProtoTerrainTextureList, SelectedProtoTerrainTextureList, typeof(PrototypeTerrainTexture));
            			    break;
            			}
						case ResourceType.QuadroItem:
						{
							Utility.SetSelectedPrototypeListFromAssets(selectedType.ProtoQuadroItemList, SelectedProtoQuadroItemList, typeof(PrototypeQuadroItem));
							break;
						}
					}
		    	}
		    }
		}

		public void SetSelectedType()
		{
			if(SelectedTypeList.Count == 1)
			{
				SelectedType = SelectedTypeList[SelectedTypeList.Count - 1];
			}
			else
			{
				SelectedType = null;
			}
		}

		public void SetSelectedProtoGameObject()
		{
			if(SelectedProtoGameObjectList.Count == 1)
			{
				SelectedProtoGameObject = SelectedProtoGameObjectList[SelectedProtoGameObjectList.Count - 1];
			}
			else
			{
				SelectedProtoGameObject = null;
			}
		}

		public void SetSelectedProtoQuadroItem()
		{
			if(SelectedProtoQuadroItemList.Count == 1)
			{
				SelectedProtoQuadroItem = SelectedProtoQuadroItemList[SelectedProtoQuadroItemList.Count - 1];
			}
			else
			{
				SelectedProtoQuadroItem = null;
			}
		}

		public void SetSelectedProtoTerrainDetail()
		{
			if(SelectedProtoTerrainDetailList.Count == 1)
			{
				SelectedProtoTerrainDetail = SelectedProtoTerrainDetailList[SelectedProtoTerrainDetailList.Count - 1];
			}
			else
			{
				SelectedProtoTerrainDetail = null;
			}
		}

		public void SetSelectedProtoTerrainTexture()
		{
			if(SelectedProtoTerrainTextureList.Count == 1)
			{
				SelectedProtoTerrainTexture = SelectedProtoTerrainTextureList[SelectedProtoTerrainTextureList.Count - 1];
			}
			else
			{
				SelectedProtoTerrainTexture = null;
			}
		}

		public bool OnlyQuadroItemSelected()
        {
            if(SelectedProtoGameObjectList.Count != 0) 
			{
				return false;
			}
			if(SelectedProtoTerrainDetailList.Count != 0)
			{
				return false;
			}
			if(SelectedProtoTerrainTextureList.Count != 0)
			{
				return false;
			}

			return true;
        }

		public bool HasOneSelectedType()
		{
			if(SelectedType == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasOneSelectedProtoGameObject()
		{
			if(SelectedProtoGameObject == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool HasOneSelectedProtoTerrainDetail()
		{
			if(SelectedProtoTerrainDetail == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasOneSelectedProtoTerrainTexture()
		{
			if(SelectedProtoTerrainTexture == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool HasOneSelectedResourceProto()
		{
			if(SelectedProtoGameObject != null || SelectedProtoTerrainDetail != null || SelectedProtoTerrainTexture != null || SelectedProtoQuadroItem != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasOneSelectedProtoQuadroItem()
		{
			if(SelectedProtoQuadroItem == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasSelectedResourceGameObject()
		{
			foreach (Type type in SelectedTypeList)
			{
				if(type.ResourceType == ResourceType.GameObject)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasSelectedResourceQuadroItem()
		{
			foreach (Type type in SelectedTypeList)
			{
				if(type.ResourceType == ResourceType.QuadroItem)
				{
					return true;
				}
			}

			return false;
		}

		public void DeleteNullValueIfNecessary(List<Type> typeList)
		{
			foreach (Type type in typeList)
			{
				if(type == null)
				{
					typeList.Remove(type);
					return;
				}
			}
		}
	}
}