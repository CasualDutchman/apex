Shader "Unlit/UnlitTrees"
{
	Properties
	{
		_MainTex ("Spring Texture", 2D) = "white" {}
		_MainTex2 ("Summer Texture", 2D) = "white" {}
		_MainTex3 ("Fall Texture", 2D) = "white" {}
		_MainTex4 ("Winter Texture", 2D) = "white" {}
		_Blend("Texture Blend", Range(0,4)) = 0.0
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _MainTex2;
			sampler2D _MainTex3;
			sampler2D _MainTex4;
			half _Blend;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				if (_Blend >= 0 && _Blend < 1) {
					col = lerp(tex2D(_MainTex, i.uv), tex2D(_MainTex2, i.uv), _Blend);//tex0 -> tex1
				}
				else if (_Blend >= 1 && _Blend < 2) {
					col = lerp(tex2D(_MainTex2, i.uv), tex2D(_MainTex3, i.uv), _Blend - 1);//tex1 -> tex2
				}
				else if (_Blend >= 2 && _Blend < 3) {
					col = lerp(tex2D(_MainTex3, i.uv), tex2D(_MainTex4, i.uv), _Blend - 2);//tex2 -> tex3
				}
				else if (_Blend >= 3 && _Blend <= 4) {
					col = lerp(tex2D(_MainTex4, i.uv), tex2D(_MainTex, i.uv), _Blend - 3);//tex3 -> tex0
				}

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
