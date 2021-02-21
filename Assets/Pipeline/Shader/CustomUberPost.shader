Shader "14/CustomUberPost"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScreenAspect("ScreenAspect", Float) = 1
        [Toggle(USE_ID_TEXTURE)]_IDTexture("ID Texture", Float) = 0
    	
    	[Header(Overlay)]
    	[Toggle(USE_POLARUV)]_PolarUV("PolarUV", Float) = 1
    	_SideTex("SideTex", 2D) = "Black"{}
    	_NormalTex("NormalTex", 2D) = "Bump"{}
    	_NormalStr("NormalStr", Range(0,3)) = 1
    	_GradingOffset("GradingOffset", Float) = 0
    	
    	[Header(CustomDOF)]
    	_Offset("Offset", Range(0,10)) = 1
    	[Min(0)]_FocusLength("Focus Length", Float) = 50
    	[Min(0)]_DOFRadius("DOF Radius", Float) = 25
    	
    	[Header(RadialBlur)]
    	_RadialBlurCenter("Center", Vector) = (0.5, 0.5, 0, 0)
        _RadialBlurMask ("MaskTex", 2D) = "white" {}
        _RadialBlurMaskStr("Mask Str", Range(0,2)) = 1
        _RadialBlurSampleTime("Sample Time", Int) = 3
        _RadialBlurOffsetStr("OffsetStr", Float) = 0.1
    	
    	[Header(RainDrop)]
    	_Rain("Rain", 2D) = "black" {}
		_Ripple("Ripple", 2D) = "black" {}
		_NoiseTex("Noise Tex", 2D) = "black"{}
		_RainForce("RainForce",Float) = 0
		[Space(10)]
		_RainDropSize("RainDropSize", Float) = 1
		_RainDropAngle("RainDropAngle", Range(-90,90)) = 0
		_RainDropTex("RainDropTex", 2D) = "black" {}
		_DropAspect("DropAspect", Range(0.1,10)) = 2
		_DropSpeed("DropSpeed", Float) = 0.2
		_DropMotionTurb("DropMotionTurb", Float) = 1
		_Distortion("Distortion", Float) = 0.5
		_Blur("Blur", Range(0,1)) = 0.5
    	
    	[Header(Eye Mask)]
    	_Handle("Handle", Range(0,1)) = 1
        _Gradient("Gradient", Range(0.01,0.99)) = 0.1
        _GradientTex("GradientTex", 2D) = "black"{}
    	
    	[Header(CustomVignette)]
    	[HDR]_VignetteColor("Color", Color) = (1,1,1,1)
        _VignetteCenter("Center", Vector) = (0.5,0.5,0,0)
        _VignetteIntensity("Intensity", Range(0,1)) = 0
        _VignetteSmoothness("Smoothness", Range(0.001,1)) = 0
    	
    	[Header(Glitch)]
    	_ScanLineJitter("ScanLineJitter", Float) = 0
        _VerticalJump("VerticalJump", Float) = 0
        _HorizontalShake("HorizontalShake", Float) = 0
        _RGBShiftAmp("RGBShiftAmp", Float) = 1
        _AnalogNoise("AnalogNoise", 2D) = "gray" {}
        _ManualShift("ManualShift", Vector) = (0, 0, 0, 0.01)
    	_ShiftSpeed("ShiftSpeed", Float) = 0.05
        _Lighten("Lighten", Float) = 0.01
        _Color1("Color1", Color) = (1, 0, 0, 1)
        _Color2("Color2", Color) = (0, 1, 0, 1)
        _Color3("Color3", Color) = (0, 0, 1, 1)
        [Range(0,1)]_Balance("Balance", Float) = 0.5
        [Toggle]_X("X", Float) = 1
        [Toggle]_Y("Y", Float) = 0
    	
    	[Header(Saturate)]
    	_PostCustomSaturate("Post Custom Saturate", Range(0,1)) = 1
    }
	
	HLSLINCLUDE
		#pragma shader_feature_local _ USE_ID_TEXTURE
		#pragma multi_compile_local _ USE_POLARUV
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
	
		Texture2D _MainTex;
		SamplerState sampler_MainTex;

		Texture2D _CustomDepthTexture;
		SamplerState sampler_CustomDepthTexture;

		// Texture2D _IDMask;
		// SamplerState sampler_IDMask;

	
	#ifdef USE_ID_TEXTURE
		Texture2D _IDMask;
		SamplerState sampler_IDMask;
        // uniform Texture2D _IdMaskTex;
        // uniform SamplerState sampler_IdMaskTex;
	#endif
		/*DOF*/
		Texture2D _DOFSource;
		SamplerState sampler_DOFSource;

		/*RainDrop*/
		Texture2D _NoiseTex, _Rain, _Ripple, _RainDropTex;
		SamplerState sampler_NoiseTex, sampler_Rain, sampler_Ripple, sampler_RainDropTex;

		/*EyeMask*/
        Texture2D _GradientTex;
        SamplerState sampler_GradientTex;

		/*Glitch*/
	    Texture2D _AnalogNoise;
        SamplerState sampler_AnalogNoise;

		/*RadialBlur*/
		Texture2D _RadialBlurMask;
        SamplerState sampler_RadialBlurMask;

		/*Overlay*/
		Texture2D _NormalTex;
		SamplerState sampler_NormalTex;
		Texture2D _SideTex;
		SamplerState sampler_SideTex;

	CBUFFER_START(UnityPerMaterial)
		//0.DownSample 1.UpSample
		half _Offset;
		half _ScreenAspect;

		//2.DOF
		half _FocusLength;
		half _DOFRadius;

		//3.RainDrop
		float4x4 _FrustumDir;
		half3 _CameraForward;
		half _RainForce;
		half _RainDropSize;
		half _RainDropAngle;
		half _DropAspect;
		half _DropSpeed;
		half _DropMotionTurb;
		half _Distortion;
		half _Blur;

		//4.Vignette
		float4 _MainTex_ST;
        half4 _VignetteColor;
        half2 _VignetteCenter;
        half _VignetteIntensity;
        half _VignetteSmoothness;

		//5.EyeMask
		half _Handle;
        half _Gradient;

		//6.Glitch
		half _ShiftSpeed;
        half4 _NoiseTex_ST;
        half _MaxDepth;
        half4 _Color1;
        half4 _Color2;
        half4 _Color3;
        half4 _ManualShift;
        half _Lighten;
        half _RGBShiftAmp;
        half _X;
        half _Y;
        half _Balance;
        
        half _ScanLineJitter; // (displacement, threshold)
        half _VerticalJump;   // (amount, time)
        half _HorizontalShake;
        half _ColorDrift;     // (amount, time)

		//7.RadialBlur
		half2 _RadialBlurCenter;
        half _RadialBlurMaskStr;
        int _RadialBlurSampleTime;
        half _RadialBlurOffsetStr;

		//8.CustomSaturate
        half _PostCustomSaturate;

		//9.Overlay
		half _NormalStr;
		half4 _NormalTex_ST;
	    half4 _SideTex_ST;
		half _GradingOffset;
    CBUFFER_END
	ENDHLSL

    SubShader
    {
//    	Name "ClusterForwardLit"
        Tags 
    	{ 
    		"RenderType" = "Opaque" 
//    		"LightMode" = "ClusterForwardLit"
    	}
        LOD 100


    	//0.DownSample	F
        Pass
		{
			HLSLPROGRAM
			
			#pragma vertex Vert_DownSample
			#pragma fragment Frag_DownSample
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			
			
			struct v2f_DownSample
			{
				float4 vertex: SV_POSITION;
				// float2 texcoord: TEXCOORD0;
				float2 uv: TEXCOORD0;
				float4 uv01: TEXCOORD1;
				float4 uv23: TEXCOORD2;
			};

			float2 TransformTriangleVertexToUV(float2 vertex)
			{
			    float2 uv = (vertex + 1.0) * 0.5;
			    return uv;
			}

			v2f_DownSample Vert_DownSample(appdata v)
			{
				v2f_DownSample o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				// o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				// o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
				//
				//
				// #if UNITY_UV_STARTS_AT_TOP
				// 	o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
				// #endif
				float2 uv = v.uv;//o.texcoord;
				float2 _MainTex_TexelSize = _ScreenParams.zw - 1;
				// _MainTex_TexelSize *= 0.5;
				o.uv = uv;
				o.uv01.xy = uv - _MainTex_TexelSize * float2(1 + _Offset, 1 + _Offset);//top right
				o.uv01.zw = uv + _MainTex_TexelSize * float2(1 + _Offset, 1 + _Offset);//bottom left
				o.uv23.xy = uv - float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//top left
				o.uv23.zw = uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//bottom right
				
				return o;
			}
			
			half4 Frag_DownSample(v2f_DownSample i): SV_Target
			{
				half4 sum = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * 4;
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.zw);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.zw);
				
				return sum * 0.125;
			}
		
			ENDHLSL
			
		}

    	//1. UpSample	F
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex Vert_UpSample
			#pragma fragment Frag_UpSample
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f_UpSample
			{
				float4 vertex: SV_POSITION;
				// float2 texcoord: TEXCOORD0;
				float4 uv01: TEXCOORD0;
				float4 uv23: TEXCOORD1;
				float4 uv45: TEXCOORD2;
				float4 uv67: TEXCOORD3;
			};
			
			float2 TransformTriangleVertexToUV(float2 vertex)
			{
			    float2 uv = (vertex + 1.0) * 0.5;
			    return uv;
			}

			v2f_UpSample Vert_UpSample(appdata v)
			{
				v2f_UpSample o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				// o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				// o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
				//
				// #if UNITY_UV_STARTS_AT_TOP
				// 	o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
				// #endif
				float2 uv = v.uv;//o.texcoord;

				float2 _MainTex_TexelSize = _ScreenParams.zw - 1;
				// _MainTex_TexelSize *= 0.5;
				half2 offset = float2(1 + _Offset, 1 + _Offset);
				
				o.uv01.xy = uv + float2(-_MainTex_TexelSize.x * 2, 0) * offset;
				o.uv01.zw = uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y) * offset;
				o.uv23.xy = uv + float2(0, _MainTex_TexelSize.y * 2) * offset;
				o.uv23.zw = uv + _MainTex_TexelSize * offset;
				o.uv45.xy = uv + float2(_MainTex_TexelSize.x * 2, 0) * offset;
				o.uv45.zw = uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * offset;
				o.uv67.xy = uv + float2(0, -_MainTex_TexelSize.y * 2) * offset;
				o.uv67.zw = uv - _MainTex_TexelSize * offset;
				
				return o;
			}
			
			half4 Frag_UpSample(v2f_UpSample i): SV_Target
			{
				half4 sum = 0;
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.zw) * 2;
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.zw) * 2;
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv45.zw) * 2;
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv67.xy);
				sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv67.zw) * 2;
				
				return sum * 0.0833;
			}
			
			ENDHLSL
			
		}
		
		//2. DOF	F
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f_UpSample
			{
				float4 vertex: SV_POSITION;
				float2 texcoord: TEXCOORD0;
			};
			

			v2f_UpSample vert(appdata v)
			{
				v2f_UpSample o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);//float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = v.uv;//v.vertex.xy;
				
				// #if UNITY_UV_STARTS_AT_TOP
				// 	o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
				// #endif
				
				return o;
			}
			
			half4 frag(v2f_UpSample i): SV_Target
			{
				half4 blurred =  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy);
				half4 original = SAMPLE_TEXTURE2D(_DOFSource, sampler_DOFSource, i.texcoord.xy);
				half depth = LinearEyeDepth(SAMPLE_TEXTURE2D(_CustomDepthTexture, sampler_CustomDepthTexture, i.texcoord.xy).r, _ZBufferParams);
				half str = saturate(abs(depth - _FocusLength)/_DOFRadius);
				// return str;
				return lerp(original, blurred, str * str);
			}
			ENDHLSL
		}
    	
    	//3. RainDrop	F
    	Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
			#include "UIUtilities.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 frustumDir : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				float2 uv = v.uv;

				int ix = (int)uv.x;
				int iy = (int)uv.y;
				// int index = 0;
				// if (v.uv.x < 0.5 && v.uv.y > 0.5)
		  //           index = 0;
		  //       else if (v.uv.x > 0.5 && v.uv.y > 0.5)
		  //           index = 1;
		  //       else if (v.uv.x < 0.5 && v.uv.y < 0.5)
		  //           index = 2;
		  //       else
		  //           index = 3;
				// o.frustumDir = _FrustumDir[index];
				o.frustumDir = _FrustumDir[ix + 2 * iy];

				o.uv = uv;

				return o;
			}

			half lum(half3 c)
			{
				return c.r * 0.2 + c.g * 0.7 + c.b * 0.1;
			}
			
			half N21(half2 p){
				p = frac(p*float2(123.34,345.45));
				p +=dot( p,p+34.345 ) ;
				return frac(p.x*p.y);
			}

			half3 Drop(half2 uv, half t)
			{
				float2 aspect = float2(_DropAspect, max(0.5,0.5/_DropSpeed));
				
				uv = MFRotation(_RainDropAngle, half2(_ScreenAspect,1) * 0.5, uv * half2(_ScreenAspect,1)).xy;
				uv = uv * _RainDropSize * aspect;
				uv.y += t * _DropSpeed;
				float2 gv = frac(uv) - 0.5;
				half2 id = floor(uv);

				float n =N21(id);
			//  用t+上现在每格水滴的系数，达到每格的时间都不一样
				t+= n*5.345;
				
				float w = uv.y *10;
				float x = (n-0.5)*0.8;
			//  0.4-abs( x )的值是控制幅度，然后乘以之前我们做的运动曲线 
				x += (0.4-abs(x))*SimpleSinRadius(3*w)*pow(SimpleSinRadius(w),4)*0.3 * _DropMotionTurb;
				float y =-SimpleSinRadius(t+SimpleSinRadius(t+SimpleSinRadius(t)*0.5))*0.45;
			//  改变了水滴的形状，目前是gv.x（-0.5，0.5）相乘得到一个对称的值
				//y -= (gv.x-x)*(gv.x-x);

				
				float2 dropPos =(gv - float2(x, y))/aspect;
				float drop =/* SAMPLE_TEXTURE2D(_RainDropTex, sampler_RainDropTex, (gv+0.5)/aspect * 2 - half2(0,0.5) );*/smoothstep(0.05,0.03,length(dropPos));
				
				float2 dropTrailPos = (gv - float2(x, 0)) / aspect;
				dropTrailPos.y = (frac(dropTrailPos.y * 8)/8)-0.03;
				float dropTrail = smoothstep(0.03, 0.02, length(dropTrailPos));
				float fogTrail = smoothstep(-0.05, 0.05, dropPos.y);
				fogTrail *= smoothstep(0.5, y, gv.y);
				// fogTrail *= fogTrail;
				dropTrail *= fogTrail;
				fogTrail *= smoothstep(0.05,0.04,abs(dropPos.x));
				// col += fogTrail * 0.5;
				// col += drop;
				// col += dropTrail;
				float2 offs = drop*dropPos+dropTrail*dropTrailPos;
				return half3(offs, fogTrail);
			}

			half4 frag (v2f i) : SV_Target
			{
				half4 col = 0;
				/************头盔雨*************/
				float t = fmod(_Time.y,7200) * _DropSpeed;
				float3 drops = Drop(i.uv,t);
				// drops += Drop(i.uv*1.35+7.51,t);
				// drops += Drop(i.uv*0.95+1.54,t);
				// drops += Drop(i.uv*1.57-6.54,t);
				// float blur = _Blur * 7 * (1-drops.z);
				/************场景雨*************/
				i.uv = i.uv + drops.xy * _Distortion;
				col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				float depth = SAMPLE_DEPTH_TEXTURE(_CustomDepthTexture, sampler_CustomDepthTexture, i.uv);
				float linear01Depth = Linear01Depth(depth, _ZBufferParams); 
				float linearEyeDepth = LinearEyeDepth(depth, _ZBufferParams);
				//世界坐标
				float3 worldPos = _WorldSpaceCameraPos + linear01Depth * i.frustumDir.xyz;
				
				//随机滚动混合噪声分布
				float2 fogUV = (worldPos.xz + worldPos.y * 0.5) * 0.0025;
				half fogNoiseR = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x + _Time.x * 0.15, fogUV.y)).r;
				half fogNoiseG = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x , fogUV.y + _Time.x * 0.1)).g;
				half fogNoiseB = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x - _Time.x * 0.05, fogUV.y - _Time.x * 0.3)).b;
		
				//涟漪噪声
				half3 rippleNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, worldPos.xz * 0.005 - _Time.y);
				//反向读取，step按01距离清除远处背景噪声
				half3 ripple = (1-SAMPLE_TEXTURE2D(_Ripple, sampler_Ripple, worldPos.xz * ((fogNoiseR + fogNoiseG + fogNoiseB + rippleNoise * 0.3) * 0.1 + 0.7)) )* step(linear01Depth, 0.99);

				//叠加更多噪声蒙版
				ripple *= step(ripple.r, col.r * 0.6 + 0.5);
				ripple *= step(col.r * 0.6 + 0.3, ripple.r);
				
				ripple *= (rippleNoise.r * rippleNoise.g * rippleNoise.b);
				ripple *= (fogNoiseR + fogNoiseG) * fogNoiseB + 0.5;

				//滚动雨uv制造下落感
				i.uv = MFRotation(_RainDropAngle, 0, i.uv);
				i.uv.y +=1;
				half2 rainUV = half2(i.uv.x , i.uv.y * 0.01 + _Time.x * 1.1);
				rainUV.y += i.uv.y * 0.001;
				//让uv随摄像机视角维持世界空间垂直
				rainUV.x += pow(i.uv.y + (_CameraForward.y + 0.5), _CameraForward.y + 1.15) * (rainUV.x - 0.5) * _CameraForward.y;
				half rainMask = SAMPLE_TEXTURE2D(_Rain, sampler_Rain, i.uv + half2(0, _Time.y*20));
				half3 rain = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, rainUV) * rainMask;
				// col.rgb += ripple * (1 - i.uv.y) * 0.8 * _RainForce * 2;
				col.rgb += saturate(rain.r - rain.g * (1 - _RainForce * 0.5) - rain.b * (1 - _RainForce * 0.5)) * 0.15 * (i.uv.y) * _RainForce * 2;
				return col;

			}
			ENDHLSL
		}
    	
    	//4. Vignette	F
    	Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }
            

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                half2 center = UnityStereoTransformScreenSpaceTex(_VignetteCenter);
                float2 dist = abs(i.uv - center) * _VignetteIntensity * 3;

            // #if defined(UNITY_SINGLE_PASS_STEREO)
            //     dist.x /= unity_StereoScaleOffset[unity_StereoEyeIndex].x;
            // #endif

                dist.x *= _ScreenAspect;
                float factor = pow(saturate(1.0 - dot(dist, dist)), _VignetteSmoothness * 5);
                return lerp(_VignetteColor, col, factor);
                // return col;
            }
            ENDHLSL
        }
    	
    	//5. Eye Mask	F
    	Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                _Handle = _Handle / (1-_Gradient);
                half threshold = -0.5 * _Handle * pow(i.uv.x - 0.5, 2) + 1.125 * _Handle;
                half gray = 1- saturate((i.uv.y+ _Gradient - threshold)/_Gradient) ;
                half4 filled = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, half2(1, 1-gray));
                // return filled;
                return lerp(col, filled , /*sqrt*/(1-gray) /** (1-gray)*/);//i.uv.y <threshold ?col : 0;
            }
            ENDHLSL
        }
    	
    	//6. Glitch	T
    	Pass
        {
            /*stencil
            {
                Ref 15
                Comp Equal
            }*/

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            struct appdata
            {
                half4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f i;
                i.uv = v.uv;
                i.vertex = TransformObjectToHClip(v.vertex.xyz);
                return i;
            }
            
            float nrand(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            }
    
            /*float4 RGBShift( float2 uv, float3 shift)
            {
                float2 rs = float2((shift.x - _RGBShiftAmp * 0.05) * _X, (-shift.y + _RGBShiftAmp * 0.05) * _Y) ;
                float2 gs = float2((shift.y - _RGBShiftAmp * 0.05) * _X, (-shift.z + _RGBShiftAmp * 0.05) * _Y) ;
                float2 bs = float2((shift.z - _RGBShiftAmp * 0.05) * _X, (-shift.x + _RGBShiftAmp * 0.05) * _Y) ;
                
                float r = tex2D(_MainTex, uv+rs).x;
                float g = tex2D(_MainTex, uv+gs).y;
                float b = tex2D(_MainTex, uv+bs).z;
                
                return r * _Color1 + g * _Color2 + b * _Color3;
                return float4(r, g, b, 1);
            }*/
            
            half4 frag (v2f i) : SV_Target
            {
                float u = i.uv.x;
                float v = i.uv.y;
            
                float ScaledTime = _Time.x * _ShiftSpeed;
            	
                half threshold = -0.5 * _Handle * pow(i.uv.x - 0.5, 2) + 1.125 * _Handle;
                half gray = 1- saturate((i.uv.y+ _Gradient - threshold)/_Gradient) ;
            	gray = 1- gray;
            	gray = gray * gray;
            	// return gray;
            	// gray = 1-gray;
            	
                // Scan line jitter
                float jitter = nrand(v, ScaledTime) * 2 - 1;
                half2 ScanLineJitterD = float2(0.002f + pow(_ScanLineJitter, 3) * 0.05f, saturate(1.0f - _ScanLineJitter * 1.2f));
                jitter *= step(ScanLineJitterD.y, abs(jitter)) * ScanLineJitterD.x;
        
                // Vertical jump
                half jump = lerp(v, frac(v + _VerticalJump * ScaledTime * 11.3f), _VerticalJump);
        
                // Horizontal shake
                half shake = (nrand(ScaledTime, 2) - 0.5) * _HorizontalShake * 0.2f;
        
            	
                // Color drift
                //float drift = sin(jump + _Time * 606.11) * _ColorDrift.x * 0.04;
                float2 shiftuv = frac(float2(u + (jitter + shake) , jump));
                //rgbshift
                half3 noise = SAMPLE_TEXTURE2D(_AnalogNoise, sampler_AnalogNoise, float2(ScaledTime, ScaledTime * 0.08)).xyz;
            	half depth = LinearEyeDepth(SAMPLE_TEXTURE2D(_CustomDepthTexture, sampler_CustomDepthTexture, i.uv).r, _ZBufferParams);
            	depth = 1-depth/100;
                half3 shift = (noise - 0.05) * _RGBShiftAmp + _ManualShift.xyz * _ManualShift.w * depth;
                half3 offset = _RGBShiftAmp * 0.05 * noise;
            	// return half4(offset*2000,1);
                half2 rs = float2((shift.x - offset.r) * _X, (-shift.y + offset.r) * _Y) ;
                half2 gs = float2((shift.y - offset.g) * _X, (-shift.z + offset.g) * _Y) ;
                half2 bs = float2((shift.z - offset.b) * _X, (-shift.x + offset.b) * _Y) ;

            #ifdef USE_ID_TEXTURE
                float idmask = SAMPLE_TEXTURE2D(_IDMask, sampler_IDMask, i.uv);
            	float id = idmask * 65536 % 4 == 0;
            	half maskshiftR = SAMPLE_TEXTURE2D(_IDMask, sampler_IDMask, shiftuv + rs);
            	float mR = maskshiftR * 65536 % 4 == 0;
            	half maskshiftG = SAMPLE_TEXTURE2D(_IDMask, sampler_IDMask, shiftuv + gs);
            	float mG = maskshiftG * 65536 % 4 == 0;
            	half maskshiftB = SAMPLE_TEXTURE2D(_IDMask, sampler_IDMask, shiftuv + bs);
            	float mB = maskshiftB * 65536 % 4 == 0;
            	half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + rs).x * mR;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + gs).y * mG;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + bs).z * mB;
                half3 rgbShift = (r * _Color1.rgb + g * _Color2.rgb + b * _Color3.rgb) * _Lighten;
            #else
            	half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + rs).x;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + gs).y;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv + bs).z;
                half3 rgbShift = (r * _Color1.rgb + g * _Color2.rgb + b * _Color3.rgb) * _Lighten;
            #endif
            	
                half4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 src1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftuv);


            	// gray = 1-gray;
                half4 col = lerp(src1 , half4(rgbShift,1) , _Balance);
            #ifdef USE_ID_TEXTURE
                float finalID = lerp(id * 1.0, (mR * 1.0 + mG * 1.0 + mB * 1.0) * _Lighten, _Balance );
            	// return finalID;
				col = lerp(src, col, finalID);
            #endif
            	return col;
            }
            ENDHLSL
        }
    	
    	//7. RadialBlur	F
    	Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = ComputeScreenPos(o.vertex).xy;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 Dir = uv - _RadialBlurCenter;
                float2 moveDir = uv;
                float2 piece = Dir * _RadialBlurOffsetStr / _RadialBlurSampleTime;
                float4 effect;
                half4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                for(int t = 0; t < _RadialBlurSampleTime; t++)
                {
                    effect += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, moveDir);
                    moveDir = moveDir + piece;
                }
                effect = effect / _RadialBlurSampleTime;
                half Mask = saturate(SAMPLE_TEXTURE2D(_RadialBlurMask, sampler_RadialBlurMask, i.uv).r * _RadialBlurMaskStr);
                //return Mask;
                return lerp(original, effect, Mask);
            }
            ENDHLSL
        }
    	
    	//8. Saturate	T
    	Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            #ifdef USE_ID_TEXTURE
                float idmask = SAMPLE_TEXTURE2D(_IDMask, sampler_IDMask, i.uv);
            	float id = idmask * 65536;
            	id = id % 2;
				return lerp(col, Luminance(col) * idmask + col * (1 - idmask), _PostCustomSaturate);
            #endif
                return lerp(col, Luminance(col), _PostCustomSaturate);
                
            }
            ENDHLSL
        }
    	
    	//9.Post Overlay
    	Pass
    	{
    		HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			// #include "../UI/UIUtilities.hlsl"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
             
				// half4 v = half4(0, 0, 1, 0);
				// Light mainLight = GetMainLight();
				// half3 h = normalize(TransformWorldToViewDir(mainLight.direction) + v);
				// half nh = saturate(dot(normalOffset, h));
				// half diffuse = min(20, mainLight.color) * distance * 5 * normal.b * nh; 
				// diffuse.a = (diffuse.r + diffuse.g + diffuse.b) * 0.3333;
				half2 originaluv = i.uv;
				half3 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				float Time = fmod(_Time.x, 2e5);
				half2 dir = i.uv - half2(0.5,0.5);
				half leng = dot(dir, dir)*2;
				leng *= leng;
			#ifdef USE_POLARUV
				i.uv.x = leng;
				i.uv.y =(atan2(dir.y,dir.x)/PI+0.5)*0.5+0.5;
			#endif
				half4 normal = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv * _NormalTex_ST.xy + _NormalTex_ST.zw * Time);
				half2 normalOffset = UnpackNormal(normal).rg * _NormalStr;
				originaluv += normalOffset.rg;
				half3 dissolved = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, originaluv);
				half grading = saturate(leng*5 + _GradingOffset);
				// return grading;
				original = lerp(original,dissolved, grading);
				half3 side = SAMPLE_TEXTURE2D(_SideTex, sampler_SideTex, i.uv * _SideTex_ST.xy + _SideTex_ST.zw * Time);
				half luminance = Luminance(side);
				return half4(lerp(original, side, luminance * grading), 1);
            }
            ENDHLSL
    	}
    }
}
