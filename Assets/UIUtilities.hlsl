#ifndef CUSTOM_TEMPLATE_INCLUDED
#define CUSTOM_TEMPLATE_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

// TEXTURE2D(_MainTex);
// SAMPLER(sampler_MainTex);
//
// UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
//    UNITY_DEFINE_INSTANCED_PROP(float, _MainTex_ST)
//    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
// UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
//
// struct Attributes{
//    float3 positionOS : POSITION;
//    float2 baseUV : TEXCOORD0;
//    UNITY_VERTEX_INPUT_INSTANCE_ID
// };
// struct Varyings {
//    float4 positionCS : SV_POSITION;
//    float2 baseUV : VAR_BASE_UV;
//    UNITY_VERTEX_INPUT_INSTANCE_ID
// };
// Varyings TemplateVertex(Attributes input)
// {   Varyings output;
//    UNITY_SETUP_INSTANCE_ID(input);
//    UNITY_TRANSFER_INSTANCE_ID(input, output);
//    float3 positionWS = TransformObjectToWorld(input.positionOS);
//    output.positionCS = TransformWorldToHClip(positionWS);
//    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MainTex_ST);
//    output.baseUV = input.baseUV * baseST.xy + baseST.zw;
//    return output;
// }
// float4 TemplateFragment(Varyings input) : SV_TARGET
// {
//    UNITY_SETUP_INSTANCE_ID(input);
//    float4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.baseUV);
//    float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
//    return baseMap * baseColor;
// }
#define PI 3.14159265359

half SimpleSmoothStep(half x)
{
    return x * x * (3.0h - 2.0h * x);
}
half SimpleCos(half x)
{
    half a = SimpleSmoothStep(frac(x));
    half b = round(frac(x/2));
    half y = 2 * a * b - a - b + 1;
    return (y * 0.900173h + SimpleSmoothStep(y) * 0.099827h - 0.5) * 2;
}
half SimpleSin(half x)
{
    // x += 0.5;
    // half a = pow(((frac(x) * 0.998h - 0.499h) * 2), 2) / 2;
    // half b = round(frac(x / 2));
    // half c = a * (1 - b) + (1 - a) * b;
    return SimpleCos(x - 0.5h);
}
half SimpleSinRadius(half x)
{
    return SimpleSin(x / PI);
}
half SimpleCosRadius(half x)
{
    return SimpleCos(x / PI);
}
float RemapSingle(float In, float2 InMinMax, float2 OutMinMax)
{
    return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

float4 MFRotation(float angle, float2 anchor, float2 uv)
{
    float2 dir = uv - anchor;
    float radian = angle / 180 * PI;
    float sinR;
    float cosR;
    // sincos(radian, sinR, cosR);
    cosR = SimpleCosRadius(radian);
    sinR = SimpleSinRadius(radian);
    return float4(dir.x * cosR - dir.y * sinR, dir.x * sinR + dir.y * cosR, 0, 0);
}

half MFMakePadding(float angle, float tilling, float padding)
{
    float4 rotated = MFRotation(angle, tilling.xx * 0.5, tilling.xx);
    float paded = rotated.x * 2;
    paded += RemapSingle((0.5 - 1 / tilling * padding), float2(0, 0.5), float2(0, tilling));
    return 1 - saturate(ceil(paded));
}

half MFRatio(in half ImageHeight, in half ImageWidth, out half AspectRatio, out half bls_Height_MoreeThan_Width)
{
    half HmW = saturate(ceil(ImageHeight - ImageWidth));
    half WmH = 1 - HmW;
    half t2 = ImageWidth / ImageHeight * WmH;
    half u1 = (ImageHeight / ImageWidth * HmW + t2 ) * HmW;
    bls_Height_MoreeThan_Width = (1-u1) * WmH;
    AspectRatio = max(u1, t2 * WmH);
}

half2 MFImageSlice(half2 uvEdgeScale, half2 imageWidthHeight, half4 lrtb, half2 uv)
{
    half2 aspectBool;
    MFRatio(imageWidthHeight.y, imageWidthHeight.x, aspectBool.x, aspectBool.y);
    half WmoreH = 1 - aspectBool.y;
    half horizontalTillingR =  uvEdgeScale.x * max(WmoreH, aspectBool.x * aspectBool.y);
    half verticalTillingR = uvEdgeScale.y * max(WmoreH * aspectBool.x, aspectBool.y);
    
    half leftPadding = MFMakePadding(0, 1/horizontalTillingR, lrtb.x);
    half righePadding = MFMakePadding(180, 1/horizontalTillingR, lrtb.y);
    half topPadding = MFMakePadding(90, 1/verticalTillingR, lrtb.z);
    half bottomPadding = MFMakePadding(270, 1/verticalTillingR, lrtb.w);
    
    half horizontalGradient = RemapSingle(uv.x, half2( 1 - horizontalTillingR * lrtb.y, horizontalTillingR * lrtb.x), half2( 1 - lrtb.y, lrtb.x));
    half horizontalGradientM = RemapSingle(uv.x, half2(0, horizontalTillingR * lrtb.x), half2(0, lrtb.x));
    half horizontalGradientL = RemapSingle(uv.x, half2(1 - horizontalTillingR * lrtb.y, 1), half2(1 - lrtb.y, 1));

    half verticalGradient = RemapSingle(uv.y, half2( 1 - verticalTillingR * lrtb.z, verticalTillingR * lrtb.w), half2( 1 - lrtb.z, lrtb.w));
    half verticalGradientM = RemapSingle(uv.y, half2(0, verticalTillingR * lrtb.w), half2(0, lrtb.w));
    half verticalGradientL = RemapSingle(uv.y, half2( 1 - verticalTillingR * lrtb.z, 1), half2( 1 - lrtb.z, 1));

    half finalu = max(max(saturate(leftPadding * horizontalGradientM), saturate((1 - leftPadding) * (1 - righePadding) * horizontalGradient)),saturate(righePadding * horizontalGradientL));
    half finalv = max(max(saturate(topPadding * verticalGradientL), saturate((1 - bottomPadding) * (1 - topPadding) * verticalGradient)), saturate(bottomPadding * verticalGradientM));
    
    return half2(finalu, finalv);
}


#endif