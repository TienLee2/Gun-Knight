using System.Collections.Generic;
using UnityEngine;
using System;

namespace MegaWorld
{
    [Serializable]
    public class RandomClustered 
    {
        public int NumberOfCluster = 30;
        public bool MinMaxRange = true;

        public int MaxClusterСhecks = 300;
        public int MinClusterСhecks = 800;

        public float MaxSeedDistance = 10;
        public float MinSeedDistance = 30;

        public void SetPosition(Type type, PainterVariables painterVariables, Func<Vector3, bool> func)
        {
            Vector3 extents = new Vector3(painterVariables.Bounds.size.x / 2, painterVariables.Bounds.size.y / 2, painterVariables.Bounds.size.z / 2);
            Vector3 centerPosition = painterVariables.Bounds.center;

            for (int cluster = 0; cluster < NumberOfCluster; cluster++)
            {
                int spawnLocationsIdx = 0;

                float localMaxSeedDistance;
                int localMaxClusterСhecks;

                if(MinMaxRange == true)
                {
                    localMaxSeedDistance = UnityEngine.Random.Range(MinSeedDistance, MaxSeedDistance);
                    localMaxClusterСhecks = UnityEngine.Random.Range(MinClusterСhecks, MaxClusterСhecks);
                }
                else
                {
                    localMaxSeedDistance = MaxSeedDistance;
                    localMaxClusterСhecks = MaxClusterСhecks;
                }

                Vector3 spawnPosition = new Vector3();

                List<Vector3> spawnPositions = new List<Vector3>();

                for (int checks = 0; checks < localMaxClusterСhecks; checks++)
                {
                    if (spawnPositions.Count == 0 || painterVariables.Bounds.Contains(spawnPositions[spawnPositions.Count - 1]) == false)
                    {
                        spawnPosition = new Vector3(UnityEngine.Random.Range(-extents.x, extents.x), 0f, UnityEngine.Random.Range(-extents.z, extents.z));
                        spawnPosition = centerPosition + spawnPosition;

                        spawnLocationsIdx = 0;
                        spawnPositions.Clear();

                        spawnPositions.Add(spawnPosition);
                    }
                    else
                    {
                        spawnPosition = GetRandomRange(localMaxSeedDistance);
                        spawnPosition = spawnPositions[spawnLocationsIdx++] + spawnPosition;

                        spawnPositions.Add(spawnPosition);
                    }

                    if(painterVariables.Bounds.Contains(spawnPositions[spawnPositions.Count - 1]) == true)
                    {
                        func.Invoke(spawnPosition);
                    }
                }
            }
        }

        public Vector3 GetRandomRange(float range)
        {
            return new Vector3(UnityEngine.Random.Range(-range, range), 0f, UnityEngine.Random.Range(-range, range));
        }
    }
}