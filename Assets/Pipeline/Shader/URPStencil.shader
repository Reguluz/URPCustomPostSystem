Shader"Unlit/URPStencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ref("Ref", Int) = 1
        _Id("ID", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        
        Pass
        {
            Stencil
            {
                Ref [_Ref]
                Comp Always
                Pass Replace
            }
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
               float4 vertex : SV_POSITION;
               float2 uv : TEXCOORD0;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "IDMask"
            Tags
            {
                "LightMode" = "IDMask"
            }
//            ColorMask Off
            HLSLPROGRAM
            // #include "IDMask.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            #pragma vertex vertID
            #pragma fragment fragID
            struct appdata
            {
               float4 vertex : POSITION;
            };

            struct v2f
            {
               float4 vertex : SV_POSITION;
            };


            v2f vertID (appdata v)
            {
               v2f o;
               o.vertex = TransformObjectToHClip(v.vertex);
               return o;
            }
            float4 _Id;
            half4 fragID (v2f i) : SV_Target
            {
               return _Id;
            }
            ENDHLSL
        }
    }
}
