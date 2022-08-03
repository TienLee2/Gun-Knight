#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    public static class SelectionTypeUtility 
    {
        public static void DisableAllPrototype(List<Type> typeList)
        {
            foreach (Type type in typeList)
            {
                foreach (PrototypeGameObject prototype in type.ProtoGameObjectList)
                {
                    prototype.selected = false;
                }
                foreach (PrototypeTerrainDetail prototype in type.ProtoTerrainDetailList)
                {
                    prototype.selected = false;
                }
                foreach (PrototypeQuadroItem prototype in type.ProtoQuadroItemList)
                {
                    prototype.selected = false;
                }
            }
        }

        public static void AddType(List<Type> typeList)
        {
            Type newType = ProfileFactory.CreateType("New");
            
            typeList.Add(newType);       
        }

        public static void DeleteSelectedTypes(List<Type> typeList)
        {
            typeList.RemoveAll((type) => type.Selected);
        }

        public static void SelectAllTypes(List<Type> typeList)
        {
            DisableAllPrototype(typeList);

            foreach (Type type in typeList)
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(type, true);
                type.Selected = true;
            }
        }

        public static void DisableAllType(List<Type> typeList)
        {
            foreach (Type type in typeList)
            {
                type.Selected = false;
            }
        }

        public static void SelectType(List<Type> typeList, int typeIndex)
        {
            if(typeIndex < 0 && typeIndex >= typeList.Count)
            {
                return;
            }
            
            foreach (Type type in typeList)
            {
                type.Selected = false;
            }

            DisableAllPrototype(typeList);
            SelectionPrototypeUtility.SetSelectedAllPrototypes(typeList[typeIndex], true);
            typeList[typeIndex].Selected = true;
        }

        public static void SelectTypeAdditive(List<Type> typeList, int typeIndex)
        {
            if(typeIndex < 0 && typeIndex >= typeList.Count)
            {
                return;
            }

            typeList[typeIndex].Selected = !typeList[typeIndex].Selected;
            if(typeList[typeIndex].Selected)
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(typeList[typeIndex], true);
            }
            else
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(typeList[typeIndex], false);
            }
        }
        
        public static void SelectTypeRange(List<Type> typeList, int typeIndex)
        {
            if(typeIndex < 0 && typeIndex >= typeList.Count)
            {
                return;
            }

            int rangeMin = typeIndex;
            int rangeMax = typeIndex;

            for (int i = 0; i < typeList.Count; i++)
            {
                if (typeList[i].Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (typeList[i].Selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(typeList[typeIndex], true);
                typeList[i].Selected = true;
            }
        }

        public static void InsertSelectedType(List<Type> typeList, int index, bool after)
        {
            List<Type> selectedBrushes = new List<Type>();
            typeList.ForEach ((brush) => { if(brush.Selected) selectedBrushes.Add(brush); });

            if(selectedBrushes.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, typeList.Count);

                typeList.Insert(index, null);    // insert null marker
                typeList.RemoveAll (b => b != null && b.Selected); // remove all selected
                typeList.InsertRange(typeList.IndexOf(null), selectedBrushes); // insert selected brushes after null marker
                typeList.RemoveAll ((b) => b == null); // remove null marter
            }
        }
    }
}
#endif