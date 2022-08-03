using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Mathematics;
using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class CDLODQuadTree
    {
        public Cell Cell;

        [SerializeReference]
        public List<CDLODQuadTree> QuadTree = null;
        public float CurrentRange;

        public CDLODQuadTree(Bounds bounds, int amountChildren, DebugColorsetSettings debugCullingSettings, bool parent = true)
        {
            if(parent == true)
            {
                this.Cell = new Cell(bounds, debugCullingSettings.ColorList[debugCullingSettings.CurrentColorListIndex]);

                SubCell(amountChildren, debugCullingSettings);
            }
            else
            {
                if(amountChildren == 1)
                {
                    this.Cell = new Cell(bounds, debugCullingSettings.ColorList[debugCullingSettings.CurrentColorListIndex]);
                }
                else
                {
                    this.Cell = new Cell(bounds, debugCullingSettings.ColorList[debugCullingSettings.CurrentColorListIndex]);

                    SubCell(amountChildren - 1, debugCullingSettings);
                }
            }
    	}

        public bool LODSelect(float[] ranges, int lodLevel, Plane[] frustumPlaneArray, Vector3 cameraPosition, bool isFrustumCulling, List<Cell> cellList) 
        {
            if(lodLevel > ranges.Length -1)
            {
                return false;
            }

            CurrentRange = ranges[lodLevel];

            if(isFrustumCulling)
            {
                if(!GeometryUtility.TestPlanesAABB(frustumPlaneArray, Cell.Bounds))
                {
                    return true;
                }
            }            

            if (!InSphere(ranges[lodLevel], cameraPosition)) 
            {
                return false;
            }

            if(lodLevel == 0) 
            {
                cellList.Add(Cell);

                return true;
            } 
            else 
            {
                if(!InSphere(ranges[lodLevel-1], cameraPosition)) 
                {
                    cellList.Add(Cell);
                } 
                else 
                {
                    CDLODQuadTree child;

                    if(QuadTree == null)
                    {
                        return false;
                    }

                    for (int i = 0; i < QuadTree.Count; i++)
                    {
                        child = QuadTree[i];
                        if (!child.LODSelect(ranges, lodLevel-1, frustumPlaneArray, cameraPosition, isFrustumCulling, cellList)) 
                        {
                            child.CurrentRange = CurrentRange;
                            cellList.Add(child.Cell);
                        }
                    }
                }
                return true;
            }
        }

        public bool InSphere(float radius, Vector3 position) 
        {
            float distance = math.distance(Cell.Bounds.center, position);
            if (distance < radius * 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SubCell(int amountChildren, DebugColorsetSettings debugCullingSettings)
        {
            debugCullingSettings.IncreaseIndex();

            float halfSizeX = Cell.Bounds.size.x / 2f;
    		float halfSizeZ = Cell.Bounds.size.z / 2f;

            Vector3 center1 = new Vector3(Cell.Bounds.center.x - Cell.Bounds.size.x / 4f, 0, Cell.Bounds.center.z + Cell.Bounds.size.x / 4f);
            Vector3 center2 = new Vector3(Cell.Bounds.center.x + Cell.Bounds.size.x / 4f, 0, Cell.Bounds.center.z + Cell.Bounds.size.x / 4f);
            Vector3 center3 = new Vector3(Cell.Bounds.center.x - Cell.Bounds.size.x / 4f, 0, Cell.Bounds.center.z - Cell.Bounds.size.x / 4f);
            Vector3 center4 = new Vector3(Cell.Bounds.center.x + Cell.Bounds.size.x / 4f, 0, Cell.Bounds.center.z - Cell.Bounds.size.x / 4f);
            
            Vector3 size = new Vector3(halfSizeX, 40, halfSizeZ);

            QuadTree = new List<CDLODQuadTree>(4);

            QuadTree.Add(new CDLODQuadTree(new Bounds(center1, size), amountChildren, debugCullingSettings, false));
            QuadTree.Add(new CDLODQuadTree(new Bounds(center2, size), amountChildren, debugCullingSettings, false));
            QuadTree.Add(new CDLODQuadTree(new Bounds(center3, size), amountChildren, debugCullingSettings, false));
            QuadTree.Add(new CDLODQuadTree(new Bounds(center4, size), amountChildren, debugCullingSettings, false));
        }

    	public void Clear()
    	{		
    		for(int i  = 0; i < QuadTree.Count; i++)
    		{
    			if(QuadTree[i] != null)
    			{
    				QuadTree[i].Clear();
    				QuadTree[i] = null;
    			}
    		}
    	}

        public void GetAllCells(List<Cell> cellsList)
        {
            if(Cell == null)
            {
                return;
            }

            cellsList.Add(Cell);

            if(QuadTree == null)
            {
                return;
            }

            for(int i  = 0; i < QuadTree.Count; i++)
    		{
    			if(QuadTree[i] != null)
    			{
    				QuadTree[i].GetAllCells(cellsList);
    			}
    		}
        }
    }
}