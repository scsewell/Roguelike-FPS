﻿/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

Shader "Hidden/OutlineEffect" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LineColor ("Line Color", Color) = (1,1,1,0.5)
	}

	SubShader 
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off

			CGPROGRAM
            
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

            sampler2D _MainTex;
            sampler2D _OutlineSource;

			float _LineThickness;
			float4 _MainTex_TexelSize;

			half4 frag(v2f i) : COLOR
			{
				half4 outlineSource = tex2D(_OutlineSource, i.uv);

				float2 texelSize = _MainTex_TexelSize * 1000;

				half4 sample1 = tex2D(_OutlineSource, i.uv + float2(_LineThickness, 0) * texelSize);
				half4 sample2 = tex2D(_OutlineSource, i.uv + float2(-_LineThickness, 0) * texelSize);
				half4 sample3 = tex2D(_OutlineSource, i.uv + float2(0, _LineThickness) * texelSize);
				half4 sample4 = tex2D(_OutlineSource, i.uv + float2(0, -_LineThickness) * texelSize);

				const float h = 0.95f;
				bool red = sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h;
				bool green = sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h;
				bool blue = sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h;
				 
				if ((red && blue) || (green && blue) || (red && green))
				{
					return 0;
				}
				else
				{
					return outlineSource;
				}
			}
			ENDCG
		}

		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			
			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ CORNER_OUTLINES
			#include "UnityCG.cginc"

			struct v2f
			{
			   float4 position : SV_POSITION;
			   float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_img v)
			{
			   	v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
			   	return o;
			}

            sampler2D _MainTex;
            sampler2D _OutlineSource;
            float4 _MainTex_TexelSize;
			float _LineThickness;
			float _LineIntensity;
			half4 _LineColor1;
			half4 _LineColor2;
			half4 _LineColor3;
			float _FillAmount;
			int _CornerOutlines;

			half4 frag(v2f i) : COLOR
			{	
				half4 originalPixel = tex2D(_MainTex, i.uv);
				half4 outlineSource = tex2D(_OutlineSource, i.uv);
		
				half4 outline = 0;
				float2 texelSize = _MainTex_TexelSize * 1000.0f;

				half4 sample1 = tex2D(_OutlineSource, i.uv + float2(_LineThickness,0.0) * texelSize);
				half4 sample2 = tex2D(_OutlineSource, i.uv + float2(-_LineThickness,0.0) * texelSize);
				half4 sample3 = tex2D(_OutlineSource, i.uv + float2(0.0, _LineThickness) * texelSize);
				half4 sample4 = tex2D(_OutlineSource, i.uv + float2(0.0,-_LineThickness) * texelSize);

				const float h = 0.95f;

#if CORNER_OUTLINES
				half4 sample5 = tex2D(_OutlineSource, i.uv + float2(_LineThickness, _LineThickness) * texelSize);
				half4 sample6 = tex2D(_OutlineSource, i.uv + float2(-_LineThickness, -_LineThickness) * texelSize);
				half4 sample7 = tex2D(_OutlineSource, i.uv + float2(_LineThickness, -_LineThickness) * texelSize);
				half4 sample8 = tex2D(_OutlineSource, i.uv + float2(-_LineThickness, _LineThickness) * texelSize);
                
				if (sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h ||
					sample5.r > h || sample6.r > h || sample7.r > h || sample8.r > h)
				{
					outline = _LineColor1 * _LineIntensity * _LineColor1.a;
				}
				else if (
					sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h ||
					sample5.g > h || sample6.g > h || sample7.g > h || sample8.g > h)
				{
					outline = _LineColor2 * _LineIntensity * _LineColor2.a;
				}
				else if (
					sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h ||
					sample5.b > h || sample6.b > h || sample7.b > h || sample8.b > h)
				{
					outline = _LineColor3 * _LineIntensity * _LineColor3.a;
				}
#else
				if (sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h)
				{
					outline = _LineColor1 * _LineIntensity * _LineColor1.a;
				}
				else if (sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h)
				{
					outline = _LineColor2 * _LineIntensity * _LineColor2.a;
				}
				else if (sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h)
				{
					outline = _LineColor3 * _LineIntensity * _LineColor3.a;
				}
#endif

				if (outlineSource.a > h)
				{
					outline *= max(max(outlineSource.r, outlineSource.g), outlineSource.b) * _FillAmount * outlineSource.a;
				}

                return lerp(originalPixel + outline, outline, _FillAmount);
			}
			ENDCG
		}
	}
}