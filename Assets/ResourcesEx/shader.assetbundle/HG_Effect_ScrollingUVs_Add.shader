Shader "HG/Effect/ScrollingUVs_Add" {
Properties {
		_MainColor("Diffuse Color", Color) = (0.6985294,0.6985294,0.6985294,1)
        _WaveBias("Wave(X),Distorted(Y), Strengh(Z), Color Mix(W)",Vector)=(0.8,1,0.03,0.86)
		_MainTex ("Main Texture", 2D)= "white" {}
		_Dudvmap ("Dudv Map", 2D) = "white" {}

}
Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }	
//	Blend One One
		Blend SrcAlpha One
		Cull Off

		ZWrite Off
	SubShader {
		Pass {		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			
			fixed4 _MainColor;
			sampler2D _Dudvmap;
			fixed4 _Dudvmap_ST;
	
			half4 _WaveBias;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				float4 color : COLOR;
			} ;
			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 noruv : TEXCOORD3;
				float4 screenPos : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 normal : TEXCOORD4;
				float2 uv_dudv : TEXCOORD5;
				float4 color : COLOR;
			} ;
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord.y += _Time.y * _WaveBias.x;

				
				o.uv_dudv = TRANSFORM_TEX(v.texcoord,_Dudvmap);
				o.uv_dudv.y += _Time.x * _WaveBias.z;
				o.color = v.color;

				o.normal = v.normal;
				return o;
			}
			fixed4 frag (v2f i) : COLOR
			{
				
				fixed4 tex_dudv = tex2D(_Dudvmap, i.uv_dudv);


				i.texcoord.xy += tex_dudv.xy * 0.3f;
				fixed4 tex = tex2D(_MainTex, i.texcoord);

				tex.rgb *=_MainColor ;
				tex.a *= tex_dudv.a * _MainColor.a;
				return tex * i.color;
			}
			ENDCG 
		}
	} 	
}
}
