Shader "Custom/ReflectiveMetalNo-Gloss-Reflect" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionColor ("Emission Color",Color) = (1,1,1,1)
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
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		sampler2D _SpecularMap;
		fixed4  _Color;
		float4 _SpecularColor;
		float4 _EmissionColor;
		half _Roughness;
 
		struct Input {
			float2 uv_MainTex;
			INTERNAL_DATA
		};
 
		void surf (Input IN, inout SurfaceOutput o) {
			float roughness = _Roughness;
			float4 specularColor = _SpecColor * tex2D(_SpecularMap,IN.uv_MainTex);
			float4 emissionColor = _EmissionColor * tex2D(_EmissionMap,IN.uv_MainTex);
 
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			o.Gloss = c.a;
			o.Specular = _Roughness;
			o.Normal = UnpackNormal( tex2D(_BumpMap, IN.uv_MainTex) );
			o.Emission = emissionColor;
		
		}
		ENDCG
	}
	FallBack "Specular"
}