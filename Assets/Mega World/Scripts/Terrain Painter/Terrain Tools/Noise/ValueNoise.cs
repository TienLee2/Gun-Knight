using System.Collections.Generic;

namespace MegaWorld
{
    /// <summary>
    /// A NoiseType implementation for Value noise
    /// </summary>
    [System.Serializable]
    public class ValueNoise : NoiseType<ValueNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Value",
            outputDir = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/",
            sourcePath = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/Implementation/ValueImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}