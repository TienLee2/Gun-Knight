Shader "QuadroRenderer/Billboard/2DRendererTreeCreator" 
{
	Properties 
    {
        _Color ("Color", Color) = (0.6,0.6,0.6,1)
		_AlbedoAtlas ("Albedo Atlas", 2D) = "white" {}
		_NormalAtlas("Normal Atlas", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.3
		_FrameCount("FrameCount", Float) = 8

        _HealthyColor("Healthy Color", Color) = (1,0.9735294,0.9338235,1)
		_DryColor("Dry Color", Color) = (0.8676471,0.818369,0.6124567,1)
		_ColorNoiseSpread("Color Noise Spread", Float) = 50

		_TranslucencyColor ("Translucency Color", Color) = (0,0,0,1)
		_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8

		[Toggle(NormalInvert)]
		_NormalInvert ("Normal Invert", Float) = 0

		[Toggle(SPDTREE_HUE_VARIATION)]
		_UseHueVariation ("Use Hue Variation", Float) = 0
		
		[Toggle(_BILLBOARDFACECAMPOS_ON)] _BillboardFaceCamPos("BillboardFaceCamPos", Float) = 0
	}
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" "DisableBatching"="True" } //"ForceNoShadowCasting" = "True" }
		LOD 400
		CGPROGRAM

		sampler2D _AlbedoAtlas;
		sampler2D _NormalAtlas;
		float _Cutoff;
		float _FrameCount;
        half4 _Color;

        uniform float4 _DryColor;
		uniform float4 _HealthyColor;
		uniform float _ColorNoiseSpread;

		float _UseHueVariation;
		float _NormalInvert;

		fixed3 _TranslucencyColor;
		half _ShadowStrength;

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
        #include "../Include/QuadroRendererGPUIBillboardInclude.cginc"
		#include "../Include/QuadroRendererInclude.cginc"

		#pragma multi_compile __ _BILLBOARDFACECAMPOS_ON
        #pragma instancing_options procedural:setupQuadroRenderer
		#pragma surface surf TreeLeaf vertex:vert nolightmap noforwardadd addshadow //exclude_path:deferred
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma target 4.5

		struct Input {
            float3 worldPos;
			float2 atlasUV;
			float3 tangentWorld;
            float3 bitangentWorld;
            float3 normalWorld;
		};

		struct LeafSurfaceOutput {
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Translucency;
			fixed Alpha;
			float Depth;
		};

        float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}

		inline half4 LightingTreeLeaf(LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);

			half nl = dot (s.Normal, lightDir);

			// view dependent back contribution for translucency
			fixed backContrib = saturate(dot(viewDir, -lightDir));

			// normally translucency is more like -nl, but looks better when it's view dependent
			backContrib = lerp(saturate(-nl), backContrib, 1);

			fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;

			// wrap-around diffuse
			nl = max(0, nl * 0.6 + 0.4);

			fixed4 c;
			c.rgb = s.Albedo * (translucencyColor * 2 + nl);
			c.rgb = c.rgb * _LightColor0.rgb;// + spec;
			c.rgb = lerp (c.rgb, float3(0,0,0), s.Depth * 1.75);

			// For directional lights, apply less shadow attenuation
			// based on shadow strength parameter.
			#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
			c.rgb *= lerp(1, atten, _ShadowStrength);
			#else
			c.rgb *= atten;
			#endif

			c.a = s.Alpha;

			return c;
		}

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		
			GPUIBillboardVertex(v.vertex, v.normal, v.tangent, o.tangentWorld, o.bitangentWorld, o.normalWorld, v.texcoord, o.atlasUV, _FrameCount);
		}

        void surf (Input IN, inout LeafSurfaceOutput o) 
        {
			half4 c = tex2D (_AlbedoAtlas, IN.atlasUV);
			clip(c.a - _Cutoff);

			c.rgb = c.rgb * _Color;

			half4 normalDepth = GPUIBillboardNormals(_NormalAtlas, IN.atlasUV, _FrameCount, IN.tangentWorld, IN.bitangentWorld, IN.normalWorld);

			if(_NormalInvert == 1)
			{
				o.Normal = -normalDepth.xyz;
			}
			else
			{
				o.Normal = normalDepth.xyz;
			}
			
			if(_UseHueVariation == 1)
			{
				float3 ase_worldPos = IN.worldPos;
				float2 appendResult357 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simplePerlin2D347 = snoise( ( appendResult357 / _ColorNoiseSpread ) );
				float4 lerpResult363 = lerp(_DryColor, _HealthyColor , simplePerlin2D347);
				float4 temp_output_35_0 = (c * lerpResult363);
				o.Albedo = temp_output_35_0.rgb;
			}
			else
			{
				o.Albedo = lerp(c.rgb, float3(0,0,0), normalDepth.w);
			}
			
			o.Translucency = 1; //TO-DO: use as property
			o.Alpha = c.a;
		}
		ENDCG
	}
}