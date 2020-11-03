﻿// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "BeatSaber/CellShading_Wnormals"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		[MaterialToggle] _CustomColors("Custom Colors", Float) = 0
		_MainTex("Texture", 2D) = "white" {}
		_Glow ("Glow", Range (0, 1)) = 0
		_Ambient ("Ambient Lighting", Range (0, 1)) = 0
		_LightDir ("Light Direction", Vector) = (-1,-1,0)
		_NormalMap ("Normals map", 2D) = "white" {}
		_Strength ("Normal Strength", Float) = 1
			_Cutout("Cutout", Range(0, 1)) = 0.5
		
		[Header(Culling Mode)]
		[Enum(UnityEngine.Rendering.CullMode)] _Culling("Cull Mode", Int) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull [_Culling]

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
				float4 vertex : SV_POSITION;
				fixed3 normalDir : TEXCOORD1;
				fixed3 viewDir : TEXCOORD3;
				float3 posWorld : TEXCOORD4;
				float2 nor : TEXCOORD5;
			};

			float4 _Color;
			float _Glow;
			float _Ambient;
			float3 _LightDir;
			float _Strength;
			float _Cutout;

			sampler2D _MainTex;
			uniform sampler2D _NormalMap;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize(mul( float4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.viewDir = normalize(UnityWorldSpaceViewDir(o.posWorld));
				o.nor = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 lightDir = normalize(_LightDir.xyz) *-1.0;
				
				// sample the texture
				fixed4 col = _Color * tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				float3 norTex = UnpackNormal(tex2D(_NormalMap, i.uv));
				float3 N = normalize(norTex.xyz);
				float shadow = round(dot(lightDir, i.nor) / 2.0 + 0.5);

				if (col.a < _Cutout) discard;

				float3 t = ((dot(lightDir, N) * 1) + 1);

				t = pow(t, _Strength);

				shadow = clamp((shadow * t),0.001, 1.2);

				col = col * lerp(_Ambient,1.0,shadow);

				return col * float4(1.0,1.0,1.0,_Glow);
			}
			ENDCG
		}
	}
}
