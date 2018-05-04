﻿Shader "Custom/Terrain" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4;

		int layerCount;
		float baseUseBiomeTint[maxLayerCount];
		float3 baseColors[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColorStrength[maxLayerCount];

		float minHeight;
		float maxHeight;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
			float2 uv_MainTex;
		};

		sampler2D _MainTex;

		float inverseLerp(float a, float b, float value) {
			return saturate((value - a) / (b - a));
		}

		void surf(Input IN, inout SurfaceOutput o) {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			for (int i = 0; i < layerCount; i++) {
				float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);
				
				float3 baseColor;

				if (baseUseBiomeTint[i] == 0)
					baseColor = baseColors[i] * baseColorStrength[i];
				else
					baseColor = tex2D(_MainTex, IN.uv_MainTex).rgb;

				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor) * drawStrength;
			}
		}

		ENDCG
		}
	FallBack "Diffuse"
}
