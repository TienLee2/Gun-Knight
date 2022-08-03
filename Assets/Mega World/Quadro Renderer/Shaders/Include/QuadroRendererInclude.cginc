#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

	struct IndirectShaderDataQuadroRenderer
	{
		float4x4 positionMatrix;
		float4x4 inversePositionMatrix;
		float4 controlData;
	};

	#if defined(SHADER_API_GLCORE) || defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE)
		StructuredBuffer<IndirectShaderDataQuadroRenderer> VisibleShaderDataBufferQuadroRenderer;
	#endif	

#endif

void setupQuadroRenderer()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif

#ifdef unity_WorldToObject
#undef unity_WorldToObject
#endif
	unity_LODFade = VisibleShaderDataBufferQuadroRenderer[unity_InstanceID].controlData; 
	unity_ObjectToWorld = VisibleShaderDataBufferQuadroRenderer[unity_InstanceID].positionMatrix;
	unity_WorldToObject = VisibleShaderDataBufferQuadroRenderer[unity_InstanceID].inversePositionMatrix;
#endif
}