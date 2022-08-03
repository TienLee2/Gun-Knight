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
    public partial class TerrainCDLODOcclusionCulling
    {
        public void Bake()
        {             
            Rect rectCell;
            Rect rectPersistentCell;

            for (int renderCellIndex = 0; renderCellIndex < TerrainCDLODOcclusionCullingPackage.RenderCells.Count; renderCellIndex++)
            {
                if(TerrainCDLODOcclusionCullingPackage.RenderCells[renderCellIndex].QuadTree == null)
                {
                    continue;
                }

                List<Cell> cellList = new List<Cell>();
                TerrainCDLODOcclusionCullingPackage.RenderCells[renderCellIndex].QuadTree.GetAllCells(cellList);

                for (int i = 0; i <= cellList.Count - 1; i++)
                {
                    Cell cell = cellList[i];

                    rectCell = RectExtension.CreateRectFromBounds(cell.Bounds);

                    cell.ClearCache();
                
#if UNITY_EDITOR
                    if (i % 10 == 0)
                    {
                        if (!Application.isPlaying) 
                        {
                            float progress = (renderCellIndex / (float)(TerrainCDLODOcclusionCullingPackage.RenderCells.Count - 1));
                            EditorUtility.DisplayProgressBar("Render Cell: " + progress + "%" + " (" + renderCellIndex + "/" + TerrainCDLODOcclusionCullingPackage.RenderCells.Count + ")", 
                                " Cell: " + i + "/" + (cellList.Count - 1), progress);
                        }
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
            }
            
#if UNITY_EDITOR 
            if (!Application.isPlaying)  EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(TerrainCDLODOcclusionCullingPackage);
#endif

            SetupCDLODOcclusionCulling();
        }

        public void CreateTerrainCDLODOcclusionCullingPackage()
        {
#if UNITY_EDITOR 
            string pathToStorageData = QuadroRendererConstants.PathToPersistentStorageData + "/" + SceneManager.GetActiveScene().name;

            Directory.CreateDirectory(GeneralPath.pathToResources);
            Directory.CreateDirectory(pathToStorageData);

            string filename = "TerrainCDLODOcclusionCullingPackage.asset";

            string path = QuadroRendererConstants.CombinePath(pathToStorageData, filename);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            TerrainCDLODOcclusionCullingPackage newPackage = ScriptableObject.CreateInstance<TerrainCDLODOcclusionCullingPackage>();

            AssetDatabase.CreateAsset(newPackage, path);

            TerrainCDLODOcclusionCullingPackage = AssetDatabase.LoadAssetAtPath<TerrainCDLODOcclusionCullingPackage>(path);
#endif
        }
    }
}