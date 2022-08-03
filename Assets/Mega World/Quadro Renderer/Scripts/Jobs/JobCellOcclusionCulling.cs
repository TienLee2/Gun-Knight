using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace QuadroRendererSystem
{
    public struct BoundingSphereInfo
    {
        public BoundingSphere BoundingSphere;
        public bool Visibility;
    }

    [BurstCompile(CompileSynchronously = true)]
    struct BoundingSphereVisibleJob : IJobParallelForFilter
    {
        [ReadOnly]
        public NativeArray<BoundingSphereInfo> BoundingSphereInfoList;

        public bool Execute(int index)
        {
            if (BoundingSphereInfoList[index].Visibility)
            {
                return true;
            }

            return false;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    struct BoundingSphereCullJob : IJobParallelFor
    {
        public NativeArray<BoundingSphereInfo> BoundingSphereInfoList;

        [ReadOnly]
        public float MaxDistance;

        [ReadOnly]
        public NativeArray<Plane> FrustumPlanes;
        public Vector3 TargetCameraPosition;
        public Vector3 FloatingOriginOffset;
        public bool IsFrustumCulling;

        public void Execute(int index)
        {
            BoundingSphereInfo boundingSphereInfo = BoundingSphereInfoList[index];
            boundingSphereInfo.BoundingSphere.position += FloatingOriginOffset;

            float distance = math.distance(boundingSphereInfo.BoundingSphere.position, TargetCameraPosition);

            boundingSphereInfo.Visibility = false;
            
            if(distance < MaxDistance)
            {
                boundingSphereInfo.Visibility = IsFrustumCulling ? SphereInFrustum(boundingSphereInfo.BoundingSphere) : true;
            }
            
            boundingSphereInfo.BoundingSphere.position -= FloatingOriginOffset;
            BoundingSphereInfoList[index] = boundingSphereInfo;
        }

        bool SphereInFrustum(BoundingSphere boundingSphere)
        {            
            for (int i = 0; i <= FrustumPlanes.Length - 1; i++)
            {
                float dist = FrustumPlanes[i].normal.x * boundingSphere.position.x + FrustumPlanes[i].normal.y * boundingSphere.position.y + FrustumPlanes[i].normal.z * boundingSphere.position.z + FrustumPlanes[i].distance;
                if (dist < -boundingSphere.radius)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class JobCellOcclusionCulling
    {
        public float MaxDistance;
        public CameraCullingMode CameraCullingMode = CameraCullingMode.FrustumCulling;
        public NativeList<BoundingSphereInfo> BundingSphereInfoList;
        public NativeList<int> VisibleCellIndexList;

        public NativeArray<Plane> FrustumPlanes;
        private static readonly Plane[] FrustumPlaneArray = new Plane[6];

        public Camera TargetCamera { set; get; }

        private Vector3 _floatingOriginOffset = new Vector3(0,0,0);

        public JobCellOcclusionCulling()
        {
            BundingSphereInfoList = new NativeList<BoundingSphereInfo>(Allocator.Persistent);
            FrustumPlanes = new NativeArray<Plane>(6, Allocator.Persistent);
            VisibleCellIndexList = new NativeList<int>(Allocator.Persistent);
        }

        public void SetFloatingOriginOffset(Vector3 floatingOriginOffset)
        {
            _floatingOriginOffset = floatingOriginOffset;
        }

        public Vector3 GetTargetCameraPosition()
        {
            return TargetCamera.transform.position;
        }

        public void Dispose()
        {
            if (BundingSphereInfoList.IsCreated) BundingSphereInfoList.Dispose();
            if (FrustumPlanes.IsCreated) FrustumPlanes.Dispose();
            if (VisibleCellIndexList.IsCreated) VisibleCellIndexList.Dispose();
        }

        public JobHandle Cull(JobHandle dependsOn)
        {
            if (TargetCamera == null)
            {
                return dependsOn;
            }

            if (BundingSphereInfoList.Length == 0)
            {
                return dependsOn;
            }

            GeometryUtility.CalculateFrustumPlanes(TargetCamera, FrustumPlaneArray);
            for (int i = 0; i <= 5; i++)
            {
                FrustumPlanes[i] = FrustumPlaneArray[i];
            }
            Vector3 targetCameraPosition = GetTargetCameraPosition();

            BoundingSphereCullJob boundingSphereCullJob =
                new BoundingSphereCullJob
                {
                    BoundingSphereInfoList = BundingSphereInfoList,
                    TargetCameraPosition = targetCameraPosition,
                    MaxDistance = MaxDistance,
                    FrustumPlanes = FrustumPlanes,
                    FloatingOriginOffset = _floatingOriginOffset,
                    IsFrustumCulling = CameraCullingMode == CameraCullingMode.FrustumCulling
                };
           
            int length = BundingSphereInfoList.Length;
            
            VisibleCellIndexList.Clear();
            JobHandle handle = boundingSphereCullJob.Schedule(length, 32, dependsOn);

            BoundingSphereVisibleJob boundingSphereVisibleJob = new BoundingSphereVisibleJob { BoundingSphereInfoList = BundingSphereInfoList };
            JobHandle visibleHandle = boundingSphereVisibleJob.ScheduleAppend(VisibleCellIndexList, length, 100, handle);

            return visibleHandle;
        }
    }
}