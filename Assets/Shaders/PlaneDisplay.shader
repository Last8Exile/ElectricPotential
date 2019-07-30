// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My/PlaneDisplay"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "" {}
		_MaxValue ("Value",float) = 0
		_ModValue ("ModValue", float) = 0.1
		_Transparency("Transparency", Range(0,1)) = 1
		_Width("Iso Width", Range(0,1)) = 0.95
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SMOOTH
			
			#include "UnityCG.cginc"

			struct vs_input {
				float4 vertex : POSITION;
			};
		 
			struct ps_input {
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
			};

			sampler3D _MainTex;
			uniform float _MaxValue;
			uniform float _ModValue;
			uniform float _Smooth;
			uniform float _Transparency;
			uniform float _Width;

			uniform float _PI;
			uniform float _PI_Inv;
			
			ps_input vert (vs_input v)
			{
				ps_input o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
		 
			float4 frag (ps_input i) : COLOR
			{
				float val = tex3D(_MainTex, i.uv).x;
				

				#if SMOOTH
					return float4(val/_MaxValue,1-val/_MaxValue,0, pow( sin(val*_MaxValue/_ModValue*2*_PI_Inv-_PI*0.5)*0.5+0.5 , _Width) * _Transparency );
				#else 
					if (fmod(val,_ModValue) > (1-_Width)*_ModValue)
						return float4(0,0,0,1);
					return float4(val/_MaxValue,1-val/_MaxValue,0,_Transparency);
				#endif
			}
			ENDCG
		}
	}
}
