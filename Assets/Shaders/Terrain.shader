Shader "Custom/Terrain" {
	Properties{

	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4;

		int layerCount;
		float3 baseColors[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColorStrength[maxLayerCount];

		float minHeight;
		float maxHeight;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value - a) / (b - a));
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			for (int i = 0; i < layerCount; i++) {
				float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

				float3 baseColor = baseColors[i] * baseColorStrength[i];

				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor) * drawStrength;
			}
		}
		ENDCG
		}
	FallBack "Diffuse"
}
