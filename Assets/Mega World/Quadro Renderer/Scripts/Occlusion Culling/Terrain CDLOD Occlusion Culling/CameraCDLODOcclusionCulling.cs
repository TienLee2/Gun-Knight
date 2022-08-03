using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

namespace QuadroRendererSystem
{
    public class СameraCDLODOcclusionCulling
    {
        public QuadroRendererCamera QuadroRendererCamera;
        
        private float _potentialCellPadding = 100;

        [NonSerialized]
        public List<RenderCell> PotentialMaxDistanceRenderCellList;

        public СameraCDLODOcclusionCulling(QuadroRendererCamera camera)
        {
            this.QuadroRendererCamera = camera; 
        }

        public Vector3 GetCameraPosition()
        {
            return QuadroRendererCamera.Camera.transform.position - QuadroRendererCamera.FloatingOriginOffset;
        }

        public void SetFloatingOriginOffset(Vector3 floatingOriginOffset)
        {
            QuadroRendererCamera.FloatingOriginOffset = floatingOriginOffset;
        }

        public void UpdatePotentialMaxDistanceRenderCell(QuadroRenderer quadroRenderer, TerrainCDLODOcclusionCulling terrainCDLODOcclusionCulling)
        {
            if (PotentialMaxDistanceRenderCellList == null)
            {
                PotentialMaxDistanceRenderCellList = new List<RenderCell>();
            }
            
            if (!QuadroRendererCamera.Camera) 
            {
                return;
            }

            if(terrainCDLODOcclusionCulling.RenderCellQuadTree == null)
            {
                return;
            }

            Vector3 selectedCameraPosition = GetCameraPosition();

            _potentialCellPadding = terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.renderCellSize * 2;
         
            float areaSize = terrainCDLODOcclusionCulling.TerrainCDLODOcclusionCullingSettings.renderCellSize * 2;
            Vector2 position = new Vector2(selectedCameraPosition.x - areaSize / 2f, selectedCameraPosition.z - areaSize / 2f);
            Rect selectedAreaRect = new Rect(position, new Vector2(areaSize, areaSize));                        
            
            PotentialMaxDistanceRenderCellList.Clear();
            terrainCDLODOcclusionCulling.RenderCellQuadTree.Query(selectedAreaRect, PotentialMaxDistanceRenderCellList);
        }

        public void DrawPotentialMaxDistanceRenderCellGizmos()
        {
            if (PotentialMaxDistanceRenderCellList == null) return;

            Gizmos.color = Color.green;

            for (int i = 0; i <= PotentialMaxDistanceRenderCellList.Count - 1; i++)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawWireCube(PotentialMaxDistanceRenderCellList[i].Bounds.center,
                    PotentialMaxDistanceRenderCellList[i].Bounds.size);
            }
        }
    }
}