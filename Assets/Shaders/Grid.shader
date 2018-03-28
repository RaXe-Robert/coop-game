// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Grid" {
	Properties{
		_GridThickness("Grid Thickness", Float) = 0.02
		_GridSpacing("Grid Spacing", Float) = 10.0
		_GridColor("Grid Color", Color) = (0.5, 0.5, 1.0, 1.0)

		_Point("Point In World Space", Vector) = (0., 0., 0., 1.0)
		_DistanceNear("Threshold Distance", Float) = 5.0
	}

	SubShader{
		Tags{
			"Queue" = "Transparent"
	}

	Pass{
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		ZTest Always

		CGPROGRAM

		#pragma vertex vert  
		#pragma fragment frag 

		uniform float _GridThickness;
		uniform float _GridSpacing;
		uniform float4 _GridColor;
		uniform float4 _Point;
		uniform float _DistanceNear;

		struct vertexInput {
			float4 vertex : POSITION;
		};
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 worldPos : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input) {
			vertexOutput output;

			output.pos = UnityObjectToClipPos(input.vertex);
			output.worldPos = mul(unity_ObjectToWorld, input.vertex);
			return output;
		}

		float4 frag(vertexOutput input) : COLOR{
			float dist = distance(input.worldPos, _Point);

			if ((frac(input.worldPos.x / _GridSpacing) < _GridThickness || frac(input.worldPos.z / _GridSpacing) < _GridThickness) 
			&& dist < _DistanceNear)
			{
				return _GridColor;
			}
			else
			{
				return Vector(0.0, 0.0, 0.0, 0.0);
			}
		}

		ENDCG
		}
	}
}
