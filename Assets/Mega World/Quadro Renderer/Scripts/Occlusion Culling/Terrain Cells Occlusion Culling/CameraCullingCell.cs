using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    public class СameraCellOcclusionCulling
    {
        public QuadroRendererCamera QuadroRendererCamera;
        public JobCellOcclusionCulling JobOcclusionCulling;
        
        private float PotentialCellPadding = 100;
        private float LastDistance;
        public Vector3 PotentialCellsCenterPosition = new Vector3(0, -10000, 0);

        [NonSerialized]
        public List<Cell> PotentialVisibleCellList;
        

        public СameraCellOcclusionCulling()
        {

        }

        public void ScheduleCull(QuadroRenderer quadroRenderer, QuadTree<Cell> cellQuadTree, float cellSize, Vector3 floatingOriginOffset, bool forceUpdate)
        {
            if (!QuadroRendererCamera.Camera) return;

            if (JobOcclusionCulling == null || JobOcclusionCulling.TargetCamera == null) 
            {
                CreateCullingGroup();
            }

            JobHandle cullingHandle = default(JobHandle);

            QuadroRendererCamera.FloatingOriginOffset = floatingOriginOffset;
            JobOcclusionCulling.SetFloatingOriginOffset(floatingOriginOffset);

            UpdatePotentialVisibleCells(quadroRenderer, cellQuadTree, cellSize, forceUpdate);

            JobOcclusionCulling.Cull(cullingHandle).Complete();
        }

        void UpdatePotentialVisibleCells(QuadroRenderer quadroRenderer, QuadTree<Cell> cellQuadTree, float cellSize, bool forceUpdate)
        {
            Vector3 selectedCameraPosition = QuadroRendererCamera.GetCameraPosition();

            PotentialCellPadding = cellSize * 2;

            bool needsUpdate = forceUpdate;
            if (PotentialVisibleCellList == null)
            {
                PotentialVisibleCellList = new List<Cell>();
                needsUpdate = true;
            }

            float distance = Vector3.Distance(PotentialCellsCenterPosition, selectedCameraPosition);
            if (distance > cellSize 
                || Math.Abs(LastDistance - QuadroRendererCamera.GetMaxDistance(quadroRenderer)) > 0.1f)
            {
                needsUpdate = true;
                PotentialCellsCenterPosition = selectedCameraPosition;
                LastDistance = QuadroRendererCamera.GetMaxDistance(quadroRenderer);
            }

            if (needsUpdate)
            {         
                JobOcclusionCulling.VisibleCellIndexList.Clear();

                float areaSize = QuadroRendererCamera.GetMaxDistance(quadroRenderer) * 2 + PotentialCellPadding; 

                Vector2 position = new Vector2(selectedCameraPosition.x - areaSize / 2f, selectedCameraPosition.z - areaSize / 2f);
                Rect selectedAreaRect = new Rect(position, new Vector2(areaSize, areaSize));   
                
                PotentialVisibleCellList.Clear();
                cellQuadTree.Query(selectedAreaRect, PotentialVisibleCellList);

                UpdateCullingGroup(quadroRenderer, cellSize);
            }
        }

        void UpdateCullingGroup(QuadroRenderer quadroRenderer, float cellSize)
        {
            JobOcclusionCulling.MaxDistance = QuadroRendererCamera.GetMaxDistance(quadroRenderer) + cellSize;
            JobOcclusionCulling.CameraCullingMode = QuadroRendererCamera.CameraCullingMode;
            
            JobOcclusionCulling.BundingSphereInfoList.Clear();
            if (JobOcclusionCulling.BundingSphereInfoList.Capacity < PotentialVisibleCellList.Count)
            {
                JobOcclusionCulling.BundingSphereInfoList.Capacity = PotentialVisibleCellList.Count;
            }
       
            for (int i = 0; i <= PotentialVisibleCellList.Count - 1; i++)
            {
                BoundingSphere boundingSphere = PotentialVisibleCellList[i].GetBoundingSphere();
                
                BoundingSphereInfo boundingSphereInfo = new BoundingSphereInfo
                {
                    BoundingSphere = boundingSphere,
                };
                JobOcclusionCulling.BundingSphereInfoList.Add(boundingSphereInfo);
            }
        }

        void CreateCullingGroup()
        {
            JobOcclusionCulling = new JobCellOcclusionCulling {TargetCamera = QuadroRendererCamera.Camera};
        }

        public BoundingSphereInfo GetBoundingSphereInfo(int potentialVisibleCellIndex)
        {
            return JobOcclusionCulling.BundingSphereInfoList[potentialVisibleCellIndex];
        }

        public void DisposeData()
        {    
            if(PotentialVisibleCellList != null)
            {
                PotentialVisibleCellList.Clear();
            }

            if(JobOcclusionCulling != null)
            {
                JobOcclusionCulling.Dispose();
                JobOcclusionCulling = null;
            }

            PotentialCellsCenterPosition = new Vector3(0,-10000,0);
        }
    }
}