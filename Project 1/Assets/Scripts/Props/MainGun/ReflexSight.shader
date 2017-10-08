Shader "Custom/ReflexSight"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_MainCol("Main Color", Color) = (1,1,1,1)
		_AlphaMask("Alpha Mask", 2D) = "white" {}
		_Brightness("Brightness", Range(1,3)) = 1.1
		_ReticleSize("Reticle Size", Range(0.01,1)) = 0.2
	}

	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off ZWrite Off
       
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
                       
			#include "UnityCG.cginc"
 
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 sightcoord : TEXCOORD1;
			};
 
			sampler2D _MainTex;
			sampler2D _AlphaMask;
			fixed4 _MainCol;
			fixed _Brightness;
			float _ReticleSize;
                       
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.sightcoord = (v.vertex.xy / _ReticleSize) - mul((float3x3)unity_WorldToObject,
						_WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(0,0,0,1))
					).xy / _ReticleSize + 0.5;
				return o;
			}
            
			fixed4 frag (v2f i): COLOR
			{
				clip(0.5 - abs(i.sightcoord - 0.5));
				fixed4 col = tex2D(_MainTex, i.sightcoord) * _MainCol * _Brightness;
				col.a *= tex2D(_AlphaMask, i.texcoord).r;
				return col;
			}
			ENDCG
		}
	}
}
