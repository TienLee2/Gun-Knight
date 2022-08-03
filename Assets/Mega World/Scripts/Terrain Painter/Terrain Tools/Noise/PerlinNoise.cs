using System.Collections.Generic;

namespace MegaWorld
{
    /// <summary>
    /// A NoiseType implementation for Perlin noise
    /// </summary>
    [System.Serializable]
    public class PerlinNoise : NoiseType<PerlinNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {            
            name = "Perlin",
            outputDir = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/",
            sourcePath = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/Implementation/PerlinImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}
