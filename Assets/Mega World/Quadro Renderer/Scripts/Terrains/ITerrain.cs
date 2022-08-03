using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace QuadroRendererSystem
{
    public interface ITerrain
    {
        string TerrainType { get; }
        Bounds TerrainBounds { get; }

        void Refresh();

        JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells, Rect cellBoundsRect,
            JobHandle dependsOn = default(JobHandle));
    }
}
