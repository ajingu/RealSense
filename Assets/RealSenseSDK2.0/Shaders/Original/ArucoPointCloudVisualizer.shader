Shader "Custom/ArucoPointCloudVisualizer"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_DepthThreshold("Depth Threshold", Range(0.0, 10.0)) = 0.5
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass{
		CGPROGRAM

		#pragma target 5.0

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float _DepthThreshold;
		float4x4 _transformationM;
		float4x4 _trs;

		StructuredBuffer<float3> _PointCloudData;
		StructuredBuffer<uint3> _ColorData;

		float4 tmp;


		struct v2f
		{
			float4 pos : SV_POSITION;
			uint id : TEXCOORD4;
			fixed4 col : COLOR;
		};


		v2f vert(uint id : SV_VertexID)
		{
			v2f o = (v2f)0;

			o.id = id;

			float3 p = _PointCloudData[id];

			if (p.z < _DepthThreshold)
			{
				tmp = mul(_transformationM, float4(p.x, p.y * -1, p.z, 1.0));
				o.pos = UnityObjectToClipPos(mul(_trs, tmp / tmp.w).xyz);
				o.col = fixed4(_ColorData[id] * 0.0039215, 1); // 1/255 = 0.03921568...
			}
			else
			{
				o.pos = float4(0, 0, 0, 0);
				o.col = fixed4(0, 0, 0, 0);
			}



			return o;
		}


		fixed4 frag(v2f i) : SV_TARGET
		{
			return  i.col;
		}

			ENDCG
		}	
	}
}