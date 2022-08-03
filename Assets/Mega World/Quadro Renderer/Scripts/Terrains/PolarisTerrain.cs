#if GRIFFIN_2020
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Pinwheel.Griffin;
using VladislavTsurikov;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public partial class PolarisTerrain : MonoBehaviour, ITerrain
    {
        public string TerrainType => "Polaris terrain";
        public GStylizedTerrain Terrain;
        public NativeArray<float> Heights;
        public Vector3 TerrainPosition = Vector3.zero;
        public Bounds TerrainBounds
        {
            get
            {
                if (Terrain)
                {
                    return new Bounds(Terrain.Bounds.center + transform.position, Terrain.Bounds.size);
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

            if (Terrain != null)
            {
                SetDataFromTerrain();
            }
        }

        void Reset()
        {
            FindTerrain();

            TerrainPosition = transform.position;
        }

        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            DisposeData();
        }

        void FindTerrain()
        {
            if (Terrain == null)
            {
                Terrain = gameObject.GetComponent<GStylizedTerrain>();
            }
        }

        void SetDataFromTerrain()
        {
            if(Terrain == null)
            {
                FindTerrain();
            }

            var terrainData = Terrain.TerrainData;
            Vector2 _heightmapTexelSize = terrainData.Geometry.HeightMap.texelSize;
            _heightmapScale = new Vector3(_heightmapTexelSize.x * terrainData.Geometry.Width, terrainData.Geometry.Height, _heightmapTexelSize.y * terrainData.Geometry.Length);
            _heightmapHeight = terrainData.Geometry.HeightMapResolution;
            _heightmapWidth = terrainData.Geometry.HeightMapResolution;

            _size = terrainData.Geometry.Size;
            _scale.x = _size.x / (_heightmapWidth - 1);
            _scale.y = _size.y;
            _scale.z = _size.z / (_heightmapHeight - 1);

            Vector2 terrainCenter = new Vector2(Terrain.transform.position.x, Terrain.transform.position.z);
            Vector2 terrainSize = new Vector2(_size.x, _size.z);
            _terrainRect = new Rect(terrainCenter, terrainSize);

            float[,] hs = Terrain.TerrainData.Geometry.GetHeights();

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
#endif