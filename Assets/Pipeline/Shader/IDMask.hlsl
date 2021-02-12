// #ifndef CUSTOM_TEMPLATE_INCLUDED
// #define CUSTOM_TEMPLATE_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

float4 _Id;
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

half4 fragID (v2f i) : SV_Target
{
   return _Id;
}
// #endif