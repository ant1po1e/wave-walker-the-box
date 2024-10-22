Shader "Unlit/Force Field"
{
    Properties
    {
        _MainTex ("Distortion Normal Map", 2D) = "gray" {}
		_DistortionStrength("Distortion Strength", Range(0, 1)) = 0.1
		_DistortionScrollSpeed("Distortion Scroll Speed", float) = 10.0
		_DistortionDirectionX("Distortion Scroll Direction X", float) = 0
		_DistortionDirectionY("Distortion Scroll Direction Y", float) = 1

		_EdgeGlow("Edge Glow", Color) = (1,1,1,1) // Rim Glow
		_EdgeColour("Edge Colour", Color) = (1,1,1,1) // Rim Edge

		_FresnelPower("Fresnel Power", Range(0,5)) = 0.5
		_EdgeLength("Edge Length", Range(0,5)) = 0.5
		_EdgeFade("Edge Fade", Range(0,5)) = 0.5
    }

    SubShader
    {
		//Tags { "RenderQueue" = "Overlay" }
		Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" } // "RenderType" = "Transparent" 

		/* 1st Pass Start, Frasnel & Intersection */
        Pass
        {
			Lighting Off 
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 fresnel : COLOR0;
				float4 screenPos : TEXCOORD1;
            };

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture; // Scene Depth

			float4 _MainTex_ST;
			float4 _EdgeGlow;
			float4 _EdgeColour;

			float _FresnelPower;
			float _EdgeLength; // Length of the edge detection
			float _EdgeFade;

			// VERTEX MAIN
            v2f vert (appdata v)
            {
                v2f o;

				// Fresnel
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float fresnelDot = 1.0 - saturate(dot(v.normal, viewDir));
				o.fresnel = smoothstep(1.0 - _FresnelPower, 1.0, fresnelDot);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				COMPUTE_EYEDEPTH(o.screenPos.z);

                return o;
            }

			// FRAGMENT MAIN
            fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
				// Intersection
				float depth = saturate(LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.screenPos)) - (i.screenPos.z + _EdgeLength));
				depth = 1.0 - pow(depth, _EdgeFade);

				// Fresnel
				i.fresnel.xyz *= step(0.5, facing);

				float alpha = saturate(depth + i.fresnel.r);

				return float4(lerp(_EdgeGlow.rgb, _EdgeColour.rgb, alpha), alpha);
            }
            ENDCG
        }
		/* 1st Pass End */



		//Tags { "RenderQueue" = "Overlay" }

		// Grab Scene Texture
		GrabPass { } // _BackgroundTexture

		/* 2nd Pass Start, Back Face Distortion */
		Pass
		{
			Lighting Off
			ZWrite On// Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float4 normal: NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _GrabTexture; // Scene Color

			float4 _MainTex_ST;

			float _DistortionStrength;
			float _DistortionScrollSpeed;
			float _DistortionDirectionX;
			float _DistortionDirectionY;

			// VERTEX MAIN
			v2f vert(appdata v) {
				v2f o;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				return o;
			}

			// FRAGMENT MAIN
			fixed4 frag(v2f i) : SV_Target
			{
				i.uv.x += (_DistortionScrollSpeed * _Time) * _DistortionDirectionX;
				i.uv.y += (_DistortionScrollSpeed * _Time) * _DistortionDirectionY;

				i.screenPos.xy += ((tex2D(_MainTex, i.uv) - 0.5) * _DistortionStrength).xy;

				return tex2Dproj(_GrabTexture, i.screenPos);
			}
			ENDCG
		}
		/* 2nd Pass End */

		/* 3nd Pass Start, Front Face Distortion */
		Pass
		{
			Lighting Off
			ZWrite On//Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
	
			CGPROGRAM
	
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			struct appdata {
				float4 vertex : POSITION;
				float4 normal: NORMAL;
				float2 uv : TEXCOORD0;
			};
	
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};
	
			sampler2D _MainTex;
			sampler2D _GrabTexture; // Scene Color
	
			float4 _MainTex_ST;
	
			float _DistortionStrength;		
			float _DistortionScrollSpeed;
			float _DistortionDirectionX;
			float _DistortionDirectionY;
	
			// VERTEX MAIN
			v2f vert(appdata v) {
				v2f o;
	
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
	
				return o;
			}
	
			// FRAGMENT MAIN
			fixed4 frag(v2f i) : SV_Target
			{
				i.uv.x += (_DistortionScrollSpeed * _Time) * _DistortionDirectionX;
				i.uv.y += (_DistortionScrollSpeed * _Time) * _DistortionDirectionY;
				
				i.screenPos.xy += ((tex2D(_MainTex, i.uv) - 0.5) * _DistortionStrength).xy;
	
				return tex2Dproj(_GrabTexture , i.screenPos);
			}
			ENDCG
		}
		/* 3nd Pass End */
    }
}
