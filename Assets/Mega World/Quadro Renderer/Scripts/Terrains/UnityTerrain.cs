using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class UnityTerrain : MonoBehaviour, ITerrain
    {
        public NativeArray<float> Heights;
        public Terrain terrain;
        public string TerrainType => "Unity terrain";
        public Vector3 TerrainPosition = Vector3.zero;

        public Bounds TerrainBounds
        {
            get
            {
                if (terrain)
                {
                    return new Bounds(terrain.terrainData.bounds.center + transform.position, terrain.terrainData.bounds.size);
                }

                return new Bounds(Vector3.zero, Vector3.zero);
            }
        }

        private int _heightmapHeight;
        private int _heightmapWidth;
        private Vector3 _size;
        private Vector3 _scale;
        private Vector3 _heightmapScale;
        private Rect _terrainRect;

        void Awake()
        {
            FindTerrain();

            if (terrain != null)
            {
                SetDataFromTerrain();
            }
        }

        void Reset()
        {
            FindTerrain();

            TerrainPosition = transform.position;
        }

        void OnDisable()
        {
            DisposeData();
        }

        void FindTerrain()
        {
            if (terrain == null)
            {
                terrain = gameObject.GetComponent<Terrain>();
            }
        }

        void SetDataFromTerrain()
        {
            FindTerrain();

            var terrainData = terrain.terrainData;
            _heightmapScale = terrainData.heightmapScale;
            _heightmapHeight = terrainData.heightmapResolution;
            _heightmapWidth = terrainData.heightmapResolution;

            _size = terrainData.size;
            _scale.x = _size.x / (_heightmapWidth - 1);
            _scale.y = _size.y;
            _scale.z = _size.z / (_heightmapHeight - 1);

            Vector2 terrainCenter = new Vector2(TerrainPosition.x, TerrainPosition.z);
            Vector2 terrainSize = new Vector2(_size.x, _size.z);
            _terrainRect = new Rect(terrainCenter, terrainSize);

            float[,] hs = terrain.terrainData.GetHeights(0, 0, _heightmapWidth, _heightmapHeight);

            if (Heights.IsCreated) Heights.Dispose();

            Heights = new NativeArray<float>(_heightmapWidth * _heightmapHeight, Allocator.Persistent);
            Heights.CopyFromFast(hs);
        }

        public void DisposeData()
        {
            if (Heights.IsCreated)
            {
                Heights.Dispose();
            }
        }

        public JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default(JobHandle))
        {
            SetDataFromTerrain();

            if (cellBoundsRect.Overlaps(_terrainRect))
            {
                UnityTerranCellSampleJob unityTerranCellSampleJob = new UnityTerranCellSampleJob
                {
                    InputHeights = Heights,
                    CellBoundsList = cellBoundsList,
                    HeightMapScale = _heightmapScale,
                    HeightmapHeight = _heightmapHeight,
                    HeightmapWidth = _heightmapWidth,
                    TerrainPosition = TerrainPosition,
                    MinHeightCells = minHeightCells,
                    TerrainRect = RectExtension.CreateRectFromBounds(TerrainBounds)
                };

                JobHandle handle = unityTerranCellSampleJob.Schedule(cellBoundsList.Length, 32, dependsOn);
                return handle;
            }

            return dependsOn;
        }

        public void Refresh()
        {
            SetDataFromTerrain();
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