using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public partial class StorageTerrainCells
    {
        public void AddItemInstance(int ID, Vector3 worldPosition, Vector3 scale, Quaternion rotation)
        {
            if (!QuadroRenderer || CellQuadTree == null) return;

            Rect positionRect = new Rect(new Vector2(worldPosition.x, worldPosition.z), Vector2.zero);

            List<Cell> overlapCellList = new List<Cell>();                 
            CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;
                PersistentStoragePackage.AddItemInstance(cellIndex, ID, worldPosition, scale, rotation);

                CellModifier.AddModifiedСell(overlapCellList[i], false, true);
            }
        }
        
        public Cell GetPersistentCell(int index)
        {
            if (index < PersistentStoragePackage.CellList.Count)
            {
                return PersistentStoragePackage.CellList[index]; 
            }                

            return null;
        }
        
        public void CreateTerrainPersistentCellsPackage()
        {
#if UNITY_EDITOR
            string pathToStorageData = QuadroRendererConstants.PathToPersistentStorageData + "/" + SceneManager.GetActiveScene().name;

            Directory.CreateDirectory(GeneralPath.pathToResources);
            Directory.CreateDirectory(pathToStorageData);
            
            string filename = "TerrainPersistentCellsPackage.asset";

            string path = QuadroRendererConstants.CombinePath(pathToStorageData, filename);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            PersistentStoragePackage newPackage = ScriptableObject.CreateInstance<PersistentStoragePackage>();

            AssetDatabase.CreateAsset(newPackage, path);

            PersistentStoragePackage = AssetDatabase.LoadAssetAtPath<PersistentStoragePackage>(path);
#endif
        }
    }
}
