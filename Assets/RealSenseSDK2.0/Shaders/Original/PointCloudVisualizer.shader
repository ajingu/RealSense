Shader "Custom/PointCloudVisualizer" 
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"RenderType" = "Opaque"}
		LOD 100

		Pass{
			CGPROGRAM

			#pragma target 5.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4x4 _trs;

			StructuredBuffer<float3> _PointCloudData;
			StructuredBuffer<uint3> _ColorData;
			

			struct v2f
			{
				float4 pos : SV_POSITION;
				uint id : TEXCOORD4;
				fixed4 col: COLOR;
			};


			v2f getVertexOut(uint id) 
			{
				v2f o = (v2f)0;
				float3 p = _PointCloudData[id];
				
				o.pos = UnityObjectToClipPos(mul(_trs, float4(p.x, p.y * -1, p.z, 1.0)).xyz);
				o.id = id;
			    o.col = fixed4(_ColorData[id] * 0.0039215, 1); // 1/255 = 0.03921568...
				
				return o;
			}

			

			v2f vert(uint id : SV_VertexID)
			{	
				return getVertexOut(id);
			}


			fixed4 frag(v2f i) : SV_TARGET
			{
				return i.col;
			}

			ENDCG
		}
	}
}
