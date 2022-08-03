using UnityEngine;
using System;

namespace QuadroRendererSystem
{
    [Serializable]
    public class ShaderInstance
    {
        public string Name;
        public Shader InstancedShader;
        public bool IsOriginalInstanced;

        public ShaderInstance(string name, Shader instancedShader, bool isOriginalInstanced, string extensionCode = null)
        {
            Name = name;
            InstancedShader = instancedShader;
            IsOriginalInstanced = isOriginalInstanced;
        }

#if UNITY_EDITOR
        public virtual void Regenerate()
        {
            if (IsOriginalInstanced)
            {
                InstancedShader = GPUInstancedIndirectShaderUtility.CreateGPUInstancedIndirectShader(InstancedShader, true);
                return;
            }

            Shader originalShader = Shader.Find(Name);
            if (originalShader != null)
            {
                InstancedShader = GPUInstancedIndirectShaderUtility.CreateGPUInstancedIndirectShader(originalShader);
            }
        }
#endif
    }
}