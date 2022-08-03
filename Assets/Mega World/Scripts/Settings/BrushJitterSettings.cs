using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rand = System.Random;

namespace MegaWorld
{
    [Serializable]
    public class BrushJitterSettings 
    {
        [SerializeField]
        private float _brushSizeJitter;
        public float BrushSizeJitter
        {
            get
            {
                return _brushSizeJitter;
            }
            set
            {
                _brushSizeJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float _brushScatter;
        public float BrushScatter
        {
            get
            {
                return _brushScatter;
            }
            set
            {
                _brushScatter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushRotationJitter;
        public float BrushRotationJitter
        {
            get
            {
                return brushRotationJitter;
            }
            set
            {
                brushRotationJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float _brushScatterJitter;
        public float BrushScatterJitter
        {
            get
            {
                return _brushScatterJitter;
            }
            set
            {
                _brushScatterJitter = Mathf.Clamp01(value);
            }
        }

        public void SetRandomVariables(ref RaycastInfo raycastInfo, ref float size, ref float rotation, Vector3 point, BrushSettings brush, Type type, LayerMask layersMask)
        {
            Rand rand = new Rand(Time.frameCount);

            size = brush.BrushSize;
            rotation = brush.BrushRotation;

            size -= (brush.BrushRadius * BrushSizeJitter * (float)rand.NextDouble()) * 2;
            
            rotation += Mathf.Sign((float)rand.NextDouble() - 0.5f) * brush.BrushRotation * BrushRotationJitter * (float)rand.NextDouble();

            Vector3 scatterDir = new Vector3((float)(rand.NextDouble() * 2 - 1), 0, (float)(rand.NextDouble() * 2 - 1)).normalized;
            float scatterLengthMultiplier = BrushScatter - (float)rand.NextDouble() * BrushScatterJitter;
            float scatterLength = brush.BrushRadius * scatterLengthMultiplier;

            point += scatterDir * scatterLength;

            Utility.Raycast(Utility.GetCurrentRayForBrushTool(point, type), out raycastInfo, layersMask);
        }
    }
}