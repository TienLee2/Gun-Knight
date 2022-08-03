using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [BurstCompile(CompileSynchronously = true)]
    public struct MeshCellSampleJob : IJobParallelFor
    {
        public NativeArray<Bounds> CellBoundsList;
        public Rect TerrainRect;
        public float TerrainMinHeight;
        public float TerrainMaxHeight;

        public void Execute(int index)
        {
            Bounds cellBounds = CellBoundsList[index];
            Rect cellRect = RectExtension.CreateRectFromBounds(cellBounds);
            if (!TerrainRect.Overlaps(cellRect)) return;

            float minHeight;
            float maxHeight = cellBounds.center.y + cellBounds.extents.y;

            if (cellBounds.center.y < 99999)
            {
                minHeight = TerrainMinHeight;
            }
            else
            {
                minHeight = cellBounds.center.y - cellBounds.extents.y;
            }

            if (TerrainMinHeight < minHeight) minHeight = TerrainMinHeight;
            if (TerrainMaxHeight > maxHeight) maxHeight = TerrainMaxHeight;

            float centerY = (maxHeight + minHeight) / 2f;
            float height = maxHeight - minHeight;
            cellBounds =
                new Bounds(new Vector3(cellBounds.center.x, centerY, cellBounds.center.z),
                    new Vector3(cellBounds.size.x, height, cellBounds.size.z));
            CellBoundsList[index] = cellBounds;
        }
    }

    [ExecuteInEditMode]
    public partial class MeshTerrain : MonoBehaviour, ITerrain
    {
        public string TerrainType => "Mesh Terrain";

        public Bounds RaycastTerrainBounds = new Bounds(Vector3.zero, new Vector3(100, 20, 100));

        public Bounds TerrainBounds =>
            new Bounds(RaycastTerrainBounds.center + TerrainPosition, RaycastTerrainBounds.size);

        public Vector3 TerrainPosition = Vector3.zero;

        public JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default(JobHandle))
        {
            Rect terrainRect = RectExtension.CreateRectFromBounds(TerrainBounds);
            if (!cellBoundsRect.Overlaps(terrainRect)) return dependsOn;

            MeshCellSampleJob raycastTerranCellSampleJob = new MeshCellSampleJob
            {
                CellBoundsList = cellBoundsList,
                TerrainMinHeight = TerrainBounds.center.y - TerrainBounds.extents.y,
                TerrainMaxHeight = TerrainBounds.center.y + TerrainBounds.extents.y,
                TerrainRect = terrainRect
            };

            JobHandle handle = raycastTerranCellSampleJob.Schedule(cellBoundsList.Length, 32, dependsOn);
            return handle;
        }

        public void Refresh()
        {

        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                TerrainPosition = transform.position;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(TerrainBounds.center, TerrainBounds.size);
        }
    }
}