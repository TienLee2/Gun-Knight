using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using System.Collections.Generic;

namespace MegaWorld
{
    public static class GetFitnessUtility
    {
        public static float GetFitnessForQuadroItem(Type type, Bounds bounds, PrototypeQuadroItem proto, RaycastInfo raycastInfo, bool updateMaskTexture = false)
        {
            if(DoUseMaskFilter(type))
            {
                return GetFitnessFromMaskFilter(bounds, proto.MaskFilterStack, proto.FilterMaskTexture2D, raycastInfo);
            }
            else
            {
                return GetFitnessFromSimpleFilter(proto.SimpleFilterSettings, raycastInfo);
            }
        }

        public static float GetFitnessForGameObject(Type type, Bounds bounds, PrototypeGameObject proto, RaycastInfo raycastInfo, bool updateMaskTexture = false)
        {
            if(DoUseMaskFilter(type))
            {
                return GetFitnessFromMaskFilter(bounds, proto.MaskFilterStack, proto.FilterMaskTexture2D, raycastInfo);
            }
            else
            {
                return GetFitnessFromSimpleFilter(proto.SimpleFilterSettings, raycastInfo);
            }
        }

        public static float GetFitnessFromSimpleFilter(SimpleFilterSettings simpleFilterSettings, RaycastInfo raycastInfo)
        {
            return simpleFilterSettings.GetFitness(raycastInfo.point, raycastInfo.normal);
        }

        public static float GetFitnessFromMaskFilter(Bounds bounds, FilterStack stack, Texture2D filterMask, RaycastInfo raycastInfo)
        {
            if(stack.Filters.Count != 0)
            {
                return Utility.GetGrayscaleFromWorldPosition(bounds, raycastInfo.hitInfo.point, filterMask);
            }

            return 1;
        }

        public static bool DoUseMaskFilter(Type type)
        {
            if(type.SpawnSurface != SpawnMode.Spherical && type.FilterType == FilterType.MaskFilter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}