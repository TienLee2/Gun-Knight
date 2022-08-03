using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    [PreferBinarySerialization]
    [Serializable]
    public class PersistentStoragePackage : ScriptableObject
    {
        public List<Cell> CellList = new List<Cell>();

        public void RemoveItemInstances(QuadroRenderer quadroRenderer, int ID)
        {
            for (int i = 0; i <= CellList.Count - 1; i++)
            {
                CellList[i].RemoveItemInstances(ID);
                CellList[i].ConvertCellPersistentStorageToRenderData(quadroRenderer);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void AddItemInstance(int cellIndex, int ID, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            if (CellList.Count > cellIndex)
            {
                CellList[cellIndex].AddItemInstance(ID, position, scale, rotation);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
