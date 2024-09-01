#ifndef UNIVERSAL_FORWARD_LIT_PASS_INCLUDED
#define UNIVERSAL_FORWARD_LIT_PASS_INCLUDED

#include "Assets/Shaders/ShaderLibrary/CourierLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    #ifdef _NORMALMAP
        float4 tangentOS    : TANGENT;
    #endif
    float2 uv     : TEXCOORD0;
    float2 staticLightmapUV   : TEXCOORD1;
    float2 dynamicLightmapUV  : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS               : SV_POSITION;
    float2 uv                       : TEXCOORD0;

    float3 positionWS               : TEXCOORD2;

    #ifdef _NORMALMAP
        half4 normalWS                  : TEXCOORD3;
        half4 tangentWS                 : TEXCOORD4;
        half4 bitangentWS               : TEXCOORD5;
    #else
        half3 normalWS                  : TEXCOORD3;
    #endif
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light
    #else
        half fogFactor                  : TEXCOORD6;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD7;
    #endif

    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
    #ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
    #endif
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

#include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierInput.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierSurface.hlsl"

#ifndef SHADOW_MAP_INCLUDED
#define SHADOW_MAP_INCLUDED
TEXTURE2D(_ShadowMap);               SAMPLER(sampler_ShadowMap);
#endif


///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Physically Based) shader
Varyings HalfLambertPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    #ifdef _NORMALMAP
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS.xyz, input.tangentOS);
    #else
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS.xyz);
    #endif

    output.positionCS = vertexInput.positionCS;
    output.positionWS = vertexInput.positionWS;

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

    half fogFactor = 0;
    #if !defined(_FOG_FRAGMENT)
        fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    #endif

    #ifdef _NORMALMAP
        output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
        output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
        output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
    #else
        output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    #endif

    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
        output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    #else
        output.fogFactor = fogFactor;
    #endif
    
    // perform any weird uv changes here
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    return output;
}

// Used in Standard (Physically Based) shader
half4 HalfLambertPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeSurfaceData(input.uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);
    SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);

    RimLighting rimData = (RimLighting)0;
    rimData.rimLightCutoff = _RimLightCutoff;
    rimData.rimLightMultiplicative = _RimLightMultiplicative;
    rimData.rimLightPower = _RimLightPower;
    rimData.rimLightingToggle = _UseRimLighting;

#ifdef _DBUFFER
    ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
#endif

    //light stuff here

    half2 ssuv = GetNormalizedScreenSpaceUV(input.positionCS);
    ssuv = RotateByAngle(ssuv, 35);
    half4 shadowCol = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, ssuv * _ShadowScale);
    half4 color = CourierShade(inputData, surfaceData, rimData, shadowCol);

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    
    color.a = surfaceData.alpha;
    return color;
}

#endif
