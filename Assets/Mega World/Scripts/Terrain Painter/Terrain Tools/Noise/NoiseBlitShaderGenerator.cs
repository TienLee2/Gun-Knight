using UnityEngine;

namespace MegaWorld
{
    internal class NoiseBlitShaderGenerator : NoiseShaderGenerator<NoiseBlitShaderGenerator>
    {
        private static ShaderGeneratorDescriptor m_desc = new ShaderGeneratorDescriptor()
        {
            name = "NoiseBlit",
            shaderCategory = "Hidden/TerrainTools/Noise/NoiseBlit",
            outputDir = "Assets/Mega World/Shaders/Terrain Tools/Generated/",
            templatePath = "Assets/Mega World/Shaders/Terrain Tools/NoiseLib/Templates/Blit.noisehlsltemplate"
        };

        public override ShaderGeneratorDescriptor GetDescription() => m_desc;
    }
}