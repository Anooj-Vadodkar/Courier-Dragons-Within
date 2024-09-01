#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

// -----------------------------------------------------------------------------------------------------
// Textures, Samples, Global Properties
TEXTURE2D(_MetallicSpecGlossMap);   SAMPLER(sampler_MetallicSpecGlossMap);
TEXTURE2D(_AlphaMap);               SAMPLER(sampler_AlphaMap);

// -----------------------------------------------------------------------------------------------------
//Functions to include
//no specular for right now, add later if needed
//template from cyanilux's URP shader tutorial

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;
    #ifdef _METALLICPSECGLOSSMAP
        specGloss = SAMPLE_TEXTURE2D(_MetallicSpecGlossMap, sampler_MetallicSpecGlossMap, uv);
        #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            specGloss.a = albedoAlpha * _Smoothness;
        #else
            specGloss.a *= _Smoothness;
        #endif
    #else
        //specGloss.rgb = _Metallic.rrr;
        specGloss.rgb = half3(0.0, 0.0, 0.0);
        #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            specGloss.a = albedoAlpha * _Smoothness;
        #else
            specGloss.a = 0.0;
            //specGloss.a = _Smoothness;
        #endif
    #endif
    return specGloss;
}

half SampleOcclusion(float2 uv)
{
    #ifdef _OCCLUSIONMAP
    #if defined(SHADER_API_GLES)
        return SAMPLE_TEXTURE2D(_MetallicSpecGlossMap, sampler_MetallicSpecGlossMap, uv).g;
    #else
        half occ = SAMPLE_TEXTURE2D(_MetallicSpecGlossMap, sampler_MetallicSpecGlossMap, uv).g;
        return LerpWhiteTo(occ, _OcclusionStrength);
    #endif
    #else
        return 1.0;
    #endif
}

half4 SampleAlphaFromMap(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    #ifdef _ALPHAMAP_ON
        half4 color = half4(SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv));
        return half4(color.rgb, SAMPLE_TEXTURE2D(_AlphaMap, sampler_AlphaMap, uv).r);
    #else
        return half4(1.0, 1.0, 1.0, 1.0);
    #endif
}

// -----------------------------------------------------------------------------------------------------
// SurfaceData

void InitializeSurfaceData(float2 inputuv, out SurfaceData surfaceData)
{
    surfaceData = (SurfaceData)0;
    ZERO_INITIALIZE(SurfaceData, surfaceData);
    #ifdef _ALPHAMAP_ON
        half4 albedoAlpha = SampleAlphaFromMap(inputuv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    #else
        half4 albedoAlpha = SampleAlbedoAlpha(inputuv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    #endif
    surfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

    surfaceData.normalTS = SampleNormal(inputuv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    surfaceData.emission = half3(0.0, 0.0, 0.0);
    //surfaceData.emission = SampleEmission(inputuv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
    surfaceData.occlusion = SampleOcclusion(inputuv);
    
    //change code here in case we do specular setup
    half4 specGloss = SampleMetallicSpecGloss(inputuv, albedoAlpha.a);
    surfaceData.metallic = specGloss.r;
    surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
    surfaceData.smoothness = specGloss.a;

    surfaceData.clearCoatMask = 0.0;
    surfaceData.clearCoatSmoothness = 0.0;
}


