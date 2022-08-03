using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    public enum RandomisationType 
    { 
        None, 
        Square,
        Sphere,
    }
    
    [Serializable]
    public class Grid 
    {
        public Vector3 VisualOrigin;
        public RandomisationType RandomisationType = RandomisationType.Square;
        [Range (0, 1)]
        public float Vastness = 1;
        public bool UniformGrid = true;
        public Vector2 GridStep = new Vector2(3, 3);
        public float GridAngle = 0;

        public void SetPosition(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            List<Vector3> spawnPointList = new List<Vector3>();

            UpdateSpawnPointList(ref spawnPointList, painterVariables.RaycastInfo.hitInfo.point, painterVariables);
    
            foreach (Vector3 spawnPoint in spawnPointList)
            {    
                func.Invoke(GetCurrentRandomPosition(spawnPoint));
            }
        }

        public void UpdateGrid(Vector3 dragPoint, float size)
        {
            Vector3 gridOrigin = Vector3.zero;
            Vector3 localGridStep = new Vector3(GridStep.x, GridStep.y, 1);
            Vector3 gridNormal = Vector3.up;

            float halfSpawnRange = size / 2;  
            
            Vector3 point = new Vector3(dragPoint.x - halfSpawnRange, 0, dragPoint.z - halfSpawnRange);

            Matrix4x4 gridMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(GridAngle, gridNormal) * Quaternion.LookRotation(gridNormal), Vector3.one)
                                 * Matrix4x4.TRS(gridOrigin, Quaternion.identity, localGridStep);

            Vector3 gridSpacePoint = gridMatrix.inverse.MultiplyPoint(point);

            gridSpacePoint = new Vector3(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y), gridSpacePoint.z);

            Vector3 snappedHitPoint = gridMatrix.MultiplyPoint(gridSpacePoint);
            VisualOrigin = snappedHitPoint;
        }

        public void UpdateSpawnPointList(ref List<Vector3> distanceSpawnInfoList, Vector3 dragPoint, PainterVariables painterVariables)
        {
            distanceSpawnInfoList.Clear();

            UpdateGrid(dragPoint, painterVariables.Size);

            Vector3 gridOrigin = Vector3.zero;

            Vector3 position = Vector3.zero;
            float halfSpawnRange = painterVariables.Radius;

            Vector3 maxPosition = gridOrigin + (Vector3.one * (halfSpawnRange * 2));

            for (position.x = gridOrigin.x; position.x < maxPosition.x; position.x += GridStep.x)
            {
                for (position.z = gridOrigin.z; position.z < maxPosition.y; position.z += GridStep.y)
                {
                    Vector3 newLocalPosition = Utility.RotatePointAroundPivot(position, new Vector3(maxPosition.x / 2, 0, maxPosition.z / 2), Quaternion.AngleAxis(GridAngle, Vector3.up));
                    Vector3 offsetLocalPosition = new Vector3(VisualOrigin.x + newLocalPosition.x, dragPoint.y, VisualOrigin.z + newLocalPosition.z);
                    distanceSpawnInfoList.Add(offsetLocalPosition);
                }
            }
        }

        public Vector3 GetRandomSquarePoint(Vector3 distancePositionPoint)
        {
            float halfDistanceX = GridStep.x / 2;
            float halfDistanceY = GridStep.y / 2;
            Vector3 distanceOffset = new Vector3(UnityEngine.Random.Range(-halfDistanceX, halfDistanceX), 0, UnityEngine.Random.Range(-halfDistanceY, halfDistanceY));
            return distancePositionPoint + distanceOffset;
        }

        public Vector3 GetRandomSpherePoint(Vector3 spawnPoint)
        {
            float halfDistance = Mathf.Lerp(0, GridStep.x / 2, Vastness);
            Vector3 distanceOffset = new Vector3(UnityEngine.Random.Range(-halfDistance, halfDistance), 0, UnityEngine.Random.Range(-halfDistance, halfDistance));
            return spawnPoint + distanceOffset;
        }

        public Vector3 GetCurrentRandomPosition(Vector3 spawnPoint)
        {
            switch (RandomisationType)
            {
                case RandomisationType.Square:
                {
                    return GetRandomSquarePoint(spawnPoint);
                }
                case RandomisationType.Sphere:
                {
                    return GetRandomSpherePoint(spawnPoint);
                }
            }

            return spawnPoint;
        }
    }
}