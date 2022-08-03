using System.Collections.Generic;

namespace MegaWorld
{
    /// <summary>
    /// A NoiseType implementation for Billow noise
    /// </summary>
    [System.Serializable]
    public class BillowNoise : NoiseType<BillowNoise>
    {
        private static NoiseTypeDescriptor desc =  new NoiseTypeDescriptor()
        {
            name = "Billow",
            outputDir = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib",
            sourcePath = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/Implementation/BillowImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
            // inputStructDefinition = new List<HlslInput>()
            // {
            //     new HlslInput() { name = "scale", valueType = HlslValueType.Float4, float4Value = new HlslFloat4(1,1,1,1) }
            // }
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}