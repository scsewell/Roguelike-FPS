Shader "Custom/ReflectiveMetalNo-Gloss-Em" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse", 2D) = "white" {}
		_Reflections ("Reflection Cubemap", CUBE) = "black" {}
		_ReflectionColor ("Reflection Color",Color) = (1,1,1,1)
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_SpecularMap ("Specular Map", 2D) = "white" {}
		_SpecColor  ("Specular Color", Color) = (1,1,1,1)
		_Roughness ("Gloss",Range(0.01,1)) = 0.01
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
 
		CGPROGRAM
		#pragma target 3.0
		#pragma glsl
		#pragma surface surf BlinnPhong
 
		sampler2D _MainTex;
		samplerCUBE _Reflections;
		sampler2D _BumpMap;
		sampler2D _SpecularMap;
		fixed4  _Color;
		float4 _ReflectionColor;
		float4 _SpecularColor;
		half _Roughness;
 
		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
			INTERNAL_DATA
		};
 
		void surf (Input IN, inout SurfaceOutput o) {
			float roughness = _Roughness;
			float4 specularColor = _SpecColor * tex2D(_SpecularMap,IN.uv_MainTex);
 
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			o.Gloss = c.a;
			o.Specular = _Roughness;
			o.Normal = UnpackNormal( tex2D(_BumpMap, IN.uv_MainTex) );
			o.Emission = (texCUBElod(_Reflections,float4(WorldReflectionVector(IN,o.Normal),(1-(roughness))*8)) * specularColor * _ReflectionColor);
		}
		ENDCG
	}
	FallBack "Specular"
}