#if UNITY_EDITOR
using UnityEngine;

namespace MegaWorld.PrecisePlace
{
    public static class PlacedObjectInfoController
    {
        private static PlacedObjectInfo _placedObjectInfo = null;

        public static PlacedObjectInfo PlacedObjectInfo 
        {
            get
            {
                return _placedObjectInfo;
            }
            set
            {
                if(value == null)
                {
                    return;
                }

                if(_placedObjectInfo != null)
                {
                    MegaWorldPath.GeneralDataPackage.StorageCells.AddItemInstance(PrecisePlaceTool.SelectedProto.ID, _placedObjectInfo.gameObject);
                    _placedObjectInfo = value;
                }
                else
                {
                    _placedObjectInfo = value;
                }
            }
        }

        public static void DestroyObjectIfNecessary(Type type)
        {
            if(_placedObjectInfo == null)
            {
                return;
            }

            PrecisePlaceToolSettings precisionPaintSettings = MegaWorldPath.GeneralDataPackage.PresitionPlaceSettings;

            if(_placedObjectInfo.gameObject == null)
            {
                DestroyObject();
            }

            if(precisionPaintSettings.SelectType == PreciseSelectType.Unit)
            {
                if(type.PastPrecisionUnit != type.GetPrecisionUnit())
                {
                    type.PastPrecisionUnit = type.GetPrecisionUnit();

                    DestroyObject();
                }
            }
            else if(precisionPaintSettings.SelectType == PreciseSelectType.RandomRange)
            {  
                if(PrecisePlaceTool.SelectedProto.selected == false)
                {
                    DestroyObject();
                }
            }
        }

        public static void DestroyObject()
        {            
            GameObjectUtility.DestroyImmediate(_placedObjectInfo.gameObject);
            _placedObjectInfo = null;
        }
    }
}
#endif