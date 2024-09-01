#ifndef UNIVERSAL_FORWARD_LIT_PASS_INCLUDED
#define UNIVERSAL_FORWARD_LIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

// GLES2 has limited amount of interpolators
#if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#define _NORMALMAP

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

// keep this file in sync with LitGBufferPass.hlsl

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    #ifdef _NORMALMAP
        float4 tangentOS    : TANGENT;
    #endif
    float2 uv                 : TEXCOORD0;
    float2 staticLightmapUV   : TEXCOORD1;
    float2 dynamicLightmapUV  : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS               : SV_POSITION;
    float2 uv                       : TEXCOORD0;

    float3 positionWS               : TEXCOORD1;

    #ifdef _NORMALMAP
        half4 normalWS                 : TEXCOORD2;
        half4 tangentWS                : TEXCOORD3; //w is sign
        half4 bitangentWS              : TEXCOORD4;
    #else
        half3 normalWS                 : TEXCOORD2;
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half4 fogFactorAndVertexLight   : TEXCOORD5; // x: fogFactor, yzw: vertex light
    #else
        half  fogFactor                 : TEXCOORD5;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD6;
    #endif
    
    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 7);
    #ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV : TEXCOORD8; // Dynamic lightmap UVs
    #endif

    half3 viewDirTS                      : TEXCOORD9;
    half3 viewDirWS                      : TEXCOORD10;
    half4 positionSS                     : TEXCOORD11;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

#include "Assets/Shaders/ShaderLibrary/CourierInput.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierSurface.hlsl"

TEXTURE2D(_RefractionMap);          SAMPLER(sampler_RefractionMap);
TEXTURE2D(_FoamMap);                SAMPLER(sampler_FoamMap);
TEXTURE2D(_IntersectionFoamMap);    SAMPLER(sampler_IntersectionFoamMap);

VertexPositionInputs WaterVertexPositionInputs(float3 positionOS, out half3 normalOffset)
{
    VertexPositionInputs input;
    input.positionWS = TransformObjectToWorld(positionOS);
    half3 offset = half3(0.0, 0.0, 0.0);
    GerstnerWaves_float(input.positionWS, _WaveSteepness, _WaveLength, _WaveSpeed, _WaveDirections,
        offset, normalOffset);
    input.positionWS += offset;
    input.positionVS = TransformWorldToView(input.positionWS);
    input.positionCS = TransformWorldToHClip(input.positionWS);

    float4 ndc = input.positionCS * 0.5f;
    input.positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
    input.positionNDC.zw = input.positionCS.zw;

    return input;
}

// Used in Standard (Physically Based) shader
Varyings WaterVert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //vertex displacement (waves)

    half3 normalOffset = half3(0.0, 0.0, 0.0);
    VertexPositionInputs vertexInput = WaterVertexPositionInputs(input.positionOS.xyz, normalOffset);

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    #ifdef _NORMALMAP
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS.xyz, input.tangentOS);
    #else
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS.xyz);
    #endif

    normalInput.normalWS = normalOffset;
    
    output.positionCS = vertexInput.positionCS;
    output.positionSS = ComputeScreenPos(output.positionCS);
    output.positionWS = vertexInput.positionWS;
    
    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    output.viewDirWS = viewDirWS;
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
    
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif
    
    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
        half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
        output.viewDirTS = viewDirTS;
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


half2 FoamMovement(half2 uv, half foamDir, half foamSpd, half foamSize)
{
    float angle = foamDir * PI * 2;
    float2 dir = normalize(float2(cos(angle), sin(angle)));
    dir *= foamSpd * _Time.y;
    half2 newuv = uv * foamSize;
    newuv += dir;
    return newuv;
}

half2 FoamDistortion(float2 UV, half foamDistSpd, half foamDistort)
{
    float time = _Time.y * foamDistSpd;
    
    UV.y += foamDistort * 0.01 * (sin(UV.x * 3.5 + time * 0.35) + sin(UV.x * 4.8 + time * 1.05) + sin(UV.x * 7.3 + time * 0.45)) / 3.0;
    UV.x += foamDistort * 0.12 * (sin(UV.y * 4.0 + time * 0.50) + sin(UV.y * 6.8 + time * 0.75) + sin(UV.y * 11.3 + time * 0.2)) / 3.0;
    UV.y += foamDistort * 0.12 * (sin(UV.x * 4.2 + time * 0.64) + sin(UV.x * 6.3 + time * 1.65) + sin(UV.x * 8.2 + time * 0.45)) / 3.0;

    return UV;
}

half DepthFadeValue(Varyings input, half2 screenUVs, half refract)
{
    half shouldRefract = sign(refract);
    
    float refracScale = (1/_RefractionScale);
    float refracSpeed = (_Time.y * _RefractionSpeed);
    float2 gradientUV = input.uv * half2(refracScale, refracScale) + refracSpeed;
    
    float gradient = SAMPLE_TEXTURE2D(_RefractionMap, sampler_RefractionMap, gradientUV).r;
    gradient = (gradient - 0.5) * 2;
    float2 oldScreenUVs = screenUVs;

    screenUVs = shouldRefract * (screenUVs + (gradient * _RefractionStrength)) +
        (1 - shouldRefract) * screenUVs;
    
    float rawDepth = SampleSceneDepth(screenUVs);
    float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    float3 scenePosWS = _WorldSpaceCameraPos + ((-input.viewDirWS/input.positionSS.w) * sceneEyeDepth);
    
    //depth check // erase depoth check if you want less control
    half depthdec = when_ge(input.positionWS.y, scenePosWS.y);
    rawDepth = changeResult(rawDepth, SampleSceneDepth(oldScreenUVs), depthdec);
    sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    scenePosWS = _WorldSpaceCameraPos + ((-input.viewDirWS/input.positionSS.w) * sceneEyeDepth);

    float3 surfaceToSeabed = input.positionWS - scenePosWS;
    float depthFade = surfaceToSeabed.y;

    return depthFade;
}

half DepthFadeValue(Varyings input, half2 screenUVs, half refract, out half3 mySceneColor)
{
    half shouldRefract = sign(refract);
    
    float refracScale = (1/_RefractionScale);
    float refracSpeed = (_Time.y * _RefractionSpeed);
    float2 gradientUV = input.uv * half2(refracScale, refracScale) + refracSpeed;
    
    float gradient = SAMPLE_TEXTURE2D(_RefractionMap, sampler_RefractionMap, gradientUV).r;
    gradient = (gradient - 0.5) * 2;
    float2 oldScreenUVs = screenUVs;

    screenUVs = shouldRefract * (screenUVs + (gradient * _RefractionStrength)) +
        (1 - shouldRefract) * screenUVs;
    
    float rawDepth = SampleSceneDepth(screenUVs);
    mySceneColor = SampleSceneColor(screenUVs);
    float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    float3 scenePosWS = _WorldSpaceCameraPos + ((-input.viewDirWS/input.positionSS.w) * sceneEyeDepth);
    
    
    //depth check // erase depoth check if you want less control
    half depthdec = when_ge(input.positionWS.y, scenePosWS.y);
    rawDepth = changeResult(rawDepth, SampleSceneDepth(oldScreenUVs), depthdec);
    mySceneColor = changeResult(mySceneColor, SampleSceneColor(oldScreenUVs), depthdec);
    sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    scenePosWS = _WorldSpaceCameraPos + ((-input.viewDirWS/input.positionSS.w) * sceneEyeDepth);

    float3 surfaceToSeabed = input.positionWS - scenePosWS;
    float depthFade = surfaceToSeabed.y;

    return depthFade;
}

half3 GetSurfaceNormal(Varyings input)
{
    half aScale = 1/(_NormalScale * 0.5);
    half aSpeed = -0.5 * _NormalSpeed;
    half2 aUV = FoamMovement(input.uv, _PanningDirection, aSpeed, aScale);
    //half3 aText = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, aUV) * 2 - 1.0;
    half3 aText = SampleNormal(aUV, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
    
    half bScale = 1/(_NormalScale);
    half2 bUV = FoamMovement(input.uv, _PanningDirection, _NormalSpeed, bScale);
    //half3 bText = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, bUV) * 2 - 1.0;
    half3 bText = SampleNormal(bUV, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));

    half3 blend = NormalBlend(aText, bText);
    blend = half3(blend.rg * _NormalStrength, lerp(1, blend.b, saturate(_NormalStrength)));
    return blend;
}

half4 WaterFrag(Varyings input) : SV_Target
{
    // Depth Fade // 0.1 units in worldspace
    float2 screenUVs = input.positionSS.xy / input.positionSS.w;
    input.uv = -input.positionWS.xz * 0.1;
    
    half3 sceneColor;
    
    //possibly add functionality to change direction/axis instead of just xz plane/y axis
    half yCoord = DepthFadeValue(input, screenUVs, 1.0, sceneColor);
    //try to make surface of water to seabed distance
    yCoord = saturate(exp(-yCoord/_DepthFadeDistance));
    //float4 outputColor = lerp(_DeepColor, _ShallowColor, yCoord);
    float4 outputColor = half4(HSVLerp(_DeepColor, _ShallowColor, yCoord));
    
    float2 FoamUV = FoamMovement(input.uv, _PanningDirection, _PanningSpeed, _FoamSize);
    FoamUV = FoamDistortion(FoamUV, _FoamDistSpeed, _FoamDistortion);
    float Foam = SAMPLE_TEXTURE2D(_FoamMap, sampler_FoamMap, FoamUV).r;

    //intersection foam
    half iFoamCoord = DepthFadeValue(input, screenUVs, 0.0);
    iFoamCoord = saturate(exp(-iFoamCoord/_IntersectionDepth)) + 0.1;
    float intersectionMask = smoothstep(saturate(_IntersectionFade), 1, iFoamCoord);

    float2 interFoamUV = FoamMovement(input.uv, _PanningDirection, _IntersectionPanSpeed, _IntersectionSize);
    //interFoamUV = FoamDistortion(interFoamUV, _IntersectionDistSpeed, _IntersectionDistortion);
    float interFoam = SAMPLE_TEXTURE2D(_IntersectionFoamMap, sampler_IntersectionFoamMap, interFoamUV).r;

    
    half adjustedCutoff = 1.0 - (_IntersectionCutoff* intersectionMask);
    interFoam = smoothstep(saturate(adjustedCutoff - _IntersectionSmoothing), adjustedCutoff, interFoam + _IntersectionFoamAdd) * intersectionMask;
    //interFoam = lerp(0, step(1.0 - (_IntersectionCutoff* intersectionMask), interFoam), intersectionMask);

    half3 surfaceNormal = input.normalWS;

    #ifdef _USELIGHTING
        half3 normalTS = GetSurfaceNormal(input);
        InputData inputData;
        InitializeInputData(input, normalTS, inputData);
        surfaceNormal = inputData.normalWS;
    #endif
    //inputData.normalWS = NormalBlend(inputData.normalWS, input.normalWS);
    
    //fresnel should maybe not use surfacenormal
    float fresnel = pow(1.0 - abs(dot(normalize(input.normalWS), normalize(input.viewDirWS))), _HorizonDistance);
    outputColor.xyz = HSVLerp(outputColor, _HorizonColor, fresnel);
    outputColor.xyz += half3(sceneColor * (1.0 - outputColor.a));
    
    #ifdef _POSTERIZE 
        float myStep = 1/_PosterizeSteps;
        outputColor = max(floor(outputColor/myStep) * myStep, _DeepColor);
    #endif

    #ifdef _USEOVERLAY
        outputColor.rgb = Overlay(outputColor.rgb, _FoamColor.rgb, smoothstep(_FoamCutoff, _FoamCutoff + 0.35, Foam * _FoamColor.a));
        outputColor.rgb = Overlay(outputColor.rgb, _IntersectionFoamColor.rgb, interFoam * _IntersectionFoamColor.a);
    #else
        outputColor.rgb = lerp(outputColor.rgb, _FoamColor.rgb, smoothstep(_FoamCutoff, _FoamCutoff + 0.35, Foam * _FoamColor.a));
        outputColor.rgb = lerp(outputColor.rgb, _IntersectionFoamColor.rgb, interFoam * _IntersectionFoamColor.a);
    #endif

    #ifdef _USELIGHTING
        half3 lighting = CalculateWaterLight(inputData, _LightSmoothness, _LightHardness, _SpecularColor);
        outputColor.rgb += lighting;
    #endif
    
    //return half4(GetSurfaceNormal(input), 1);
    return outputColor;
    
    //return half4(SampleNormal(olduv, TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap)), 1.0);
    //return half4(normalize((SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, olduv) * 2 - 1.0).rgb), 1.0);
    //return half4(interFoam, interFoam, interFoam, 1.0);
    //return half4(intersectionMask, intersectionMask, intersectionMask, 1.0);
}

#endif
