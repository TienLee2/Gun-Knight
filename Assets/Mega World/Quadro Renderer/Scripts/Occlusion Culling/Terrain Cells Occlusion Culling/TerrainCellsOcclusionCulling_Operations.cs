using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public partial class TerrainCellsOcclusionCulling
    {
        public void Bake()
        { 
            Rect rectCell;
            Rect rectPersistentCell;
            
            for (int i = 0; i <= TerrainCellsOcclusionCullingPackage.CellList.Count - 1; i++)
            {
                Cell cell = TerrainCellsOcclusionCullingPackage.CellList[i];

                rectCell = RectExtension.CreateRectFromBounds(cell.Bounds);

                cell.ClearCache();
                
#if UNITY_EDITOR 
                if (i % 10 == 0)
                {
                    if (!Application.isPlaying) EditorUtility.DisplayProgressBar("Bake", "Cell " + i + "/" + (TerrainCellsOcclusionCullingPackage.CellList.Count - 1), i/((float)TerrainCellsOcclusionCullingPackage.CellList.Count - 1));
                }
#endif

                for (int persistentCellIndex = 0; persistentCellIndex < StorageTerrainCells.PersistentStoragePackage.CellList.Count; persistentCellIndex++)
                {
                    Cell persistentCell = StorageTerrainCells.PersistentStoragePackage.CellList[persistentCellIndex];

                    rectPersistentCell = RectExtension.CreateRectFromBounds(persistentCell.Bounds);

                    if(rectPersistentCell.Overlaps(rectCell)) 
                    {
                        for (int entityInfoIndex = 0; entityInfoIndex < persistentCell.ItemInfoList.Count; entityInfoIndex++)
                        {
                            for (int entityIndex = 0; entityIndex < persistentCell.ItemInfoList[entityInfoIndex].InstanceDataList.Count; entityIndex++)
                            {
                                InstanceData persistentItem = persistentCell.ItemInfoList[entityInfoIndex].InstanceDataList[entityIndex];

                                if(cell.Rectangle.Contains(new Vector2(persistentItem.Position.x, persistentItem.Position.z))) 
                                {
                                    cell.AddItemInstance(persistentCell.ItemInfoList[entityInfoIndex].ID, persistentItem.Position, persistentItem.Scale, persistentItem.Rotation);
                                }
                            }
                        }
                    }
                }
            }            
            
#if UNITY_EDITOR
            if (!Application.isPlaying)  EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(TerrainCellsOcclusionCullingPackage);
#endif

            SetupCellOcclusionCulling();
        }

        public void CreateTerrainCellsOcclusionCullingPackage()
        {
#if UNITY_EDITOR
            string pathToStorageData = QuadroRendererConstants.PathToPersistentStorageData + "/" + SceneManager.GetActiveScene().name;

            Directory.CreateDirectory(GeneralPath.pathToResources);
            Directory.CreateDirectory(pathToStorageData);

            string filename = "TerrainCellsOcclusionCullingPackage.asset";

            string path = QuadroRendererConstants.CombinePath(pathToStorageData, filename);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            PersistentStoragePackage newPackage = ScriptableObject.CreateInstance<PersistentStoragePackage>();

            AssetDatabase.CreateAsset(newPackage, path);

            TerrainCellsOcclusionCullingPackage = AssetDatabase.LoadAssetAtPath<PersistentStoragePackage>(path);
#endif
        }
    }
}