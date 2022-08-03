using System.Collections.Generic;

namespace MegaWorld
{
    /// <summary>
    /// A NoiseType implementation for Ridge noise
    /// </summary>
    [System.Serializable]
    public class RidgeNoise : NoiseType<RidgeNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Ridge",
            outputDir = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/",
            sourcePath = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/Implementation/RidgeImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}
