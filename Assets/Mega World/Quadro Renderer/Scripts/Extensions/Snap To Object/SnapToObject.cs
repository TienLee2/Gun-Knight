using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public class SnapToObject : MonoBehaviour
    {
        public QuadroRenderer QuadroRenderer;
        public StorageTerrainCells StorageTerrainCells;

        public LayerMask Layers;
        public float RaycastPositionOffset = 0;
        public float MaxRayDistance = 6500f;
        public float SpawnCheckOffset = 3000;

        void OnEnable()
        {
            DetectNecessaryData();
        }

        public void Snap()
        {
            for (int persistentCellIndex = 0; persistentCellIndex < StorageTerrainCells.PersistentStoragePackage.CellList.Count; persistentCellIndex++)
            {
                Cell persistentCell = StorageTerrainCells.PersistentStoragePackage.CellList[persistentCellIndex];

                StorageTerrainCells.CellModifier.AddModifiedСell(persistentCell);

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentCell.ItemInfoList.Count; persistentInfoIndex++)
                {
                    for (int itemIndex = 0; itemIndex < persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList.Count; itemIndex++)
                    {
                        InstanceData persistentItem = persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex];
                        
                        QuadroPrototype proto = QuadroRenderer.QuadroPrototypesPackage.GetQuadroItem(persistentCell.ItemInfoList[persistentInfoIndex].ID);

                        if(proto == null)
                        {
                            continue;
                        }

                        if(proto.Selected == false)
                        {
                            continue;
                        }

                        RaycastHit hitInfo;
                        Ray ray = new Ray(new Vector3(persistentItem.Position.x, persistentItem.Position.y + SpawnCheckOffset, persistentItem.Position.z), 
                            Vector3.down);

                        if (Physics.Raycast(ray, out hitInfo, MaxRayDistance, Layers))
		                {
                            Vector3 finalPosition = new Vector3(hitInfo.point.x, hitInfo.point.y + RaycastPositionOffset, hitInfo.point.z);
                            persistentItem.Position = finalPosition;
                        }

                        persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex] = persistentItem;
                    }
                }
            }
        }

        public void DetectNecessaryData()
        {
            if (QuadroRenderer == null)
            {
                QuadroRenderer = GetComponentInParent<QuadroRenderer>();
            }

            if(StorageTerrainCells == null)
            {
                StorageTerrainCells = GetComponentInParent<StorageTerrainCells>();
            }
        }

        public bool HasAllNecessaryData()
        {
            if(QuadroRenderer == null)
            {
                return false;
            }

            if(StorageTerrainCells == null)
            {
                return false;
            }

            return true;
        }
    }
}