Shader "Hidden/Raymarch"
{
	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4x4 _FrustumCornersES;
			uniform float4 _MainTex_TexelSize;
			uniform float4x4 _CameraInvViewMatrix;
			uniform float3 _CameraWS;
			uniform float3 _LightDir;
			uniform sampler2D _MainTex;
			uniform sampler2D _CameraDepthTexture;

			uniform int _MaxStep;
			uniform float _MaxDist;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;

				half index = v.vertex.z;
				v.vertex.z = 0.1;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;				

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					 o.uv.y = 1 - o.uv.y;
				#endif

				// Get the eyespace view ray (normalized)
				o.ray = _FrustumCornersES[(int)index].xyz;

				// Dividing by z "normalizes" it in the z axis
				// Therefore multiplying the ray by some number i gives the viewspace position
				// of the point on the ray with [viewspace z]=i
				o.ray /= abs(o.ray.z);

				// Transform the ray from eyespace to worldspace
				// Note: _CameraInvViewMatrix was provided by the script
				o.ray = mul(_CameraInvViewMatrix, o.ray);
				return o;
			}

			// Sphere
			// s: radius
			float sdSphere( float3 p, float s )
			{
			  return length(p)-s;
			}

			float sdEllipsoid( float3 p, float3 r )
			{
				return (length( p/r ) - 1.0) * min(min(r.x,r.y),r.z);
			}

			// Torus
			// t.x: diameter
			// t.y: thickness
			float sdTorus(float3 p, float2 t)
			{
				float2 q = float2(length(p.xz) - t.x, p.y);
				return length(q) - t.y;
			}

			// Box
			// b: half size of box in x/y/z
			float sdBox(float3 p, float3 b)
			{
				float3 d = abs(p) - b;
				return min(max(d.x, max(d.y, d.z)), 0.0) +
					length(max(d, 0.0));
			}

			// CappedCylinder
			// h.x: radius
			// h.y: half height
			float sdCappedCylinder( float3 p, float2 h )
			{
			  float2 d = abs(float2(length(p.xz),p.y)) - h;
			  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
			}

			float3 trans(float3 p, float4x4 matr)
			{
				return mul(matr,float4(p,1));
			}

			float sampleFigure(int number, float3 p, float4x4 params, float4x4 matr)
			{
				float3 pos = trans(p,matr)/params[0].w;
				if (number == 0)
					return sdBox(pos, params[0].xyz)*params[0].w;
				if (number == 1)
					return sdEllipsoid(pos,params[0].xyz)*params[0].w;
				if (number == 2)
					return sdCappedCylinder(pos,params[0].xy)*params[0].w;
				if (number == 3)
					return sdTorus(pos,params[0].xy)*params[0].w;
				return 0;
			}


			// Union
			float opU( float d1, float d2 )
			{
				return min(d1,d2);
			}

			// Blend
			float opB( float d1, float d2, float k)
			{
				float h = clamp( 0.5+0.5*(d2-d1)/k, 0.0, 1.0 );
				return lerp( d2, d1, h ) - k*h*(1.0-h);
			}

			// Subtraction
			float opS( float d1, float d2 )
			{
				return max(d1,-d2);
			}

			// Intersection
			float opI( float d1, float d2 )
			{
				return max(d1,d2);
			}

			// Repetion
			float3 opRep( float3 p, float3 c )
			{
				return fmod(p,c)-0.5*c;
			}


			float sampleOperation(int number, float op1, float op2, float4x4 params)
			{
				if (number == -1)
					return opU(op1,op2);
				if (number == -2)
					return opI(op1,op2);
				if (number == -3)
					return opS(op1,op2);
				if (number == -4)
					return opB(op1,op2, params[0].x);
				return 0;
			}

			uniform float _Numbers[64];
			uniform float4x4 _Transforms[64];
			uniform float4x4 _Params[64];
			uniform int _Size;

			float map(float3 p) 
			{
				float stack[32];
				int pointer = 0;
				for (int i=0; i<_Size; i++)
				{
					if (_Numbers[i] < 0)
					{
						stack[pointer-2] = sampleOperation((int)_Numbers[i],stack[pointer-2],stack[pointer-1], _Params[i]);
						pointer--;
					}
					else
					{
						stack[pointer] = sampleFigure((int)_Numbers[i], p, _Params[i], _Transforms[i]);
						pointer++;
					}
				}
				return stack[0];
			}

			float3 calcNormalCheap(float3 pos)
			{
				float2 e = float2(1.0,-1.0)*0.5773*0.0005;
				return normalize( e.xyy*map( pos + e.xyy ).x + 
								  e.yyx*map( pos + e.yyx ).x + 
								  e.yxy*map( pos + e.yxy ).x + 
								  e.xxx*map( pos + e.xxx ).x );
			}

			float3 calcNormal(float3 pos)
			{
				// epsilon - used to approximate dx when taking the derivative
				const float2 eps = float2(0.001, 0.0);

				// The idea here is to find the "gradient" of the distance field at pos
				// Remember, the distance field is not boolean - even if you are inside an object
				// the number is negative, so this calculation still works.
				// Essentially you are approximating the derivative of the distance field at this point.
				float3 nor = float3(
					map(pos + eps.xyy).x - map(pos - eps.xyy).x,
					map(pos + eps.yxy).x - map(pos - eps.yxy).x,
					map(pos + eps.yyx).x - map(pos - eps.yyx).x);
				return normalize(nor);
			}

			// Raymarch along given ray
			// ro: ray origin
			// rd: ray direction
			fixed4 raymarch(float3 ro, float3 rd, float s) 
			{
				fixed4 ret = fixed4(0,0,0,0);

				float t = 0; // current distance traveled along ray
				for (int i = 0; i < _MaxStep; ++i) 
				{
					// If we run past the depth buffer, stop and return nothing (transparent pixel)
					// this way raymarched objects and traditional meshes can coexist.
					if (t >= s || t > _MaxDist) 
					{
						break;
					}

					float3 p = ro + rd * t; // World space position of sample
					float d = map(p);       // Sample of distance field (see map())

					// If the sample <= 0, we have hit something (see map()).
					if (d < 0.001) 
					{
						// Lambertian Lighting
						float3 n = calcNormalCheap(p-rd*0.001);
						ret = fixed4(dot(-_LightDir.xyz, n).rrr, 1);
						break;
					}

					// If the sample > 0, we haven't hit anything yet so we should march forward
					// We step forward by distance d, because d is the minimum distance possible to intersect
					// an object (see map()).
					t += d;
				}
				return ret;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// ray direction
				float3 rd = normalize(i.ray.xyz);
				// ray origin (camera position)
				float3 ro = _CameraWS;

				float2 duv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					duv.y = 1 - duv.y;
				#endif

				// Convert from depth buffer (eye space) to true distance from camera
				// This is done by multiplying the eyespace depth by the length of the "z-normalized"
				// ray (see vert()).  Think of similar triangles: the view-space z-distance between a point
				// and the camera is proportional to the absolute distance.
				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, duv).r);
				depth *= length(i.ray.xyz);

				fixed3 col = tex2D(_MainTex,i.uv); // Color of the scene before this shader was run
				fixed4 add = raymarch(ro, rd, depth);

				// Returns final color using alpha blending
				return fixed4(col*(1.0 - add.w) + add.xyz * add.w,1.0);
			}

			ENDCG
		}
	}
}
