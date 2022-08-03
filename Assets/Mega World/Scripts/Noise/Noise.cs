﻿using System;
using System.Collections;
using UnityEngine;

namespace MegaWorld
{
    /// <summary>
    /// Abstract class for generating noise.
    /// </summary>
	public abstract class NoiseCPU : INoiseCPU
	{

        /// <summary>
        /// The frequency of the fractal.
        /// </summary>
        public float Frequency { get; set; }

        /// <summary>
        /// The amplitude of the fractal.
        /// </summary>
        public float Amplitude { get; set; }

        /// <summary>
        /// The offset applied to each dimension.
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Create a noise object.
        /// </summary>
		public NoiseCPU()
		{
            
		}

        /// <summary>
        /// Sample the noise in 1 dimension.
        /// </summary>
		public abstract float Sample1D(float x);

        /// <summary>
        /// Sample the noise in 2 dimensions.
        /// </summary>
		public abstract float Sample2D(float x, float y);

        /// <summary>
        /// Sample the noise in 3 dimensions.
        /// </summary>
		public abstract float Sample3D(float x, float y, float z);

        /// <summary>
        /// Update the seed.
        /// </summary>
        public abstract void UpdateSeed(int seed);
		
	}

}











