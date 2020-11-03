﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "BeatSaber/Metallic"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		[MaterialToggle] _CustomColors("Custom Colors", Float) = 0
		_MainTex("Reflection Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 normal : NORMAL;
			};

			float4 _Color;
			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
				o.normal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color * (pow(tex2D(_MainTex, float2(dot(i.normal.xyz,i.viewDir.xyz),dot(i.normal.xzy,i.viewDir.zyx))).r,10.0)*2.0);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col * float4(1.0,1.0,1.0,0);
			}
			ENDCG
		}
	}
}
