#ifndef COURIER_LIGHTING_INCLUDED
#define COURIER_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/Shaders/ShaderLibrary/CourierFunctions.hlsl"

// Must match Universal ShaderGraph master node

struct RimLighting
{
    half rimLightingToggle;
    half rimLightPower;
    half rimLightMultiplicative;
    half rimLightCutoff;
};

float CourierSpecular(float3 L, float3 N, float3 V, float smoothness)
{
    float3 halfVec = SafeNormalize(float3(L) + float3(V));
    float NdotH =  saturate(dot(N, halfVec));
    return pow(NdotH, smoothness);
}

half3 CalculateWaterLight(InputData inputData, half smoothness, half hardness, half4 specularColor)
{
    smoothness = exp2(10 * smoothness + 1);
    
    

    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.normalizedScreenSpaceUV, 0);
    
    //Light light = GetMainLight(TransformWorldToShadowCoord(inputData.positionWS));
    Light light = GetMainLight(inputData, shadowMask, aoFactor);
    
    MixRealtimeAndBakedGI(light, inputData.normalWS, inputData.bakedGI);
    half mainSpecular = 0.0;
    
    LightingData lightingData = (LightingData)0;
    lightingData.giColor = inputData.bakedGI;

    half3 normalWS = normalize(inputData.normalWS);
    half3 viewWS = SafeNormalize(inputData.viewDirectionWS);

    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    {
        mainSpecular = CourierSpecular(light.direction, normalWS, viewWS, smoothness);
        half stepped = step(0.5, mainSpecular);
        half3 mainColor = lerp(mainSpecular, stepped, hardness) * specularColor.rgb * specularColor.aaa;
        lightingData.mainLightColor = mainColor;
    }

    uint pixelLightCount = GetAdditionalLightsCount();
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light addLight = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
        if (IsMatchingLightLayer(addLight.layerMask, meshRenderingLayers))
        {
            float3 attenuatedLight = addLight.color * addLight.distanceAttenuation * addLight.shadowAttenuation;
            attenuatedLight *= specularColor.rgb;       
            float specular_soft = CourierSpecular(light.direction, normalWS, viewWS, smoothness);
            float specular_hard = smoothstep(0.005,0.01,specular_soft);
            float specular_term = lerp(specular_soft, specular_hard, hardness);
            specular_term *= specularColor.aaa;

            lightingData.additionalLightsColor += specular_term * attenuatedLight;
        }
    LIGHT_LOOP_END

    //return lightingData.additionalLightsColor;
    return lightingData.additionalLightsColor + lightingData.mainLightColor + lightingData.giColor;
    //return CalculateFinalColor(lightingData, surfaceData.alpha);
}
half CalculateHighestRGB(half3 lightColor)
{
    return max(lightColor.r, max(lightColor.g, lightColor.b));
}

half CutoffRGB(half3 lightColor, half cutoff)
{
    half colorVal = CalculateHighestRGB(lightColor);
    return smoothstep(cutoff, cutoff + 0.1, colorVal);
}

half3 CourierRimLight(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half3 rimLightPower, half rimLightCutoff, half rimLightMultiplicative)
{
    float NdotV = saturate(dot(normal, viewDir));
    float rimLightIntensity = max(1.0 - NdotV, 0.0);
    rimLightIntensity = pow(rimLightIntensity, rimLightPower);
    rimLightIntensity = smoothstep(0.4, 0.56, rimLightIntensity);
    //rimLightIntensity = when_gt()
    
    float NdotL = pow(smoothstep(0.4, 0.6, saturate(dot(normal, lightDir))), 2);
    float3 color = NdotL * lightColor;

    half cutoffDecision = CutoffRGB(lightColor, rimLightCutoff);
    
    return rimLightIntensity * rimLightMultiplicative * color * cutoffDecision;
    //return half3(0.0, 0.0, 0.0);
}

half3 HalfLambert(half3 lightColor, half3 lightDir, half3 normal, half wrapValue)
{
    
    float NdotL = saturate(dot(lightDir, normal));
    half halfLambertDiffuse = (NdotL * wrapValue) + (1-wrapValue);
    half biasedValue = 2 * (wrapValue - 0.5);
    halfLambertDiffuse = pow(halfLambertDiffuse, 2 - biasedValue);
    
    #ifdef _USEPOSTERIZE 
        float myStep = 1/_PosterizeSteps;
        //float soft_value = halfLambertDiffuse;
        float remainder = fmod(halfLambertDiffuse, myStep);
        float reg_multiple = floor(halfLambertDiffuse/ myStep);
        float lower_multiple = max(reg_multiple - 1, 0.0);
        float higher_multiple = reg_multiple + 1;
        float hard_value = reg_multiple * myStep;
        float lower = when_ge(_PosterizeAllowance, remainder);
        float higher = when_le(myStep - _PosterizeAllowance, remainder) *
            when_lt(reg_multiple, _PosterizeSteps - 1);  
        //float closer = xIsCloser(0, myStep, remainder);
        float smooth_param = (remainder/_PosterizeAllowance) * lower
            + (remainder - (myStep - _PosterizeAllowance))/_PosterizeAllowance * higher;
        float higher_smoothed = lerp(reg_multiple * myStep, (higher_multiple - 0.5) * myStep, smooth_param);
        float lower_smoothed = lerp((lower_multiple + 0.5) * myStep, reg_multiple * myStep, smooth_param);
        //higher_smoothed = reg_multiple * myStep;
        //lower_smoothed = lower_multiple * myStep;
        //higher_smoothed = half3(1, 1, 1);
        //lower_smoothed = half3(0, 0, 0);
        //higher_smoothed = lerp(reg_multiple * myStep, (higher_multiple - 0.5) * myStep, smooth_param) ; //(reg_multiple + higher_multiple) * 0.5 * myStep;
            //higher_smoothed = (reg_multiple + higher_multiple) * 0.5 * myStep;
        //lower_smoothed = (reg_multiple + lower_multiple) * 0.5 * myStep;
        float smooth_value = lower_smoothed * lower + higher_smoothed * higher;
        float use_smooth = min(lower + higher, 1);
        halfLambertDiffuse = smooth_value * use_smooth + hard_value * (1-use_smooth);
    #endif

    return halfLambertDiffuse * lightColor;
    // return hard_value * lightColor * (1-use_smooth) + use_smooth * half3(1, 1, 1);
}



half3 GIRimLight(half3 giColor, half3 normal, half3 viewDir, half3 rimLightPower, half rimLightCutoff, half rimLightMultiplicative)
{
    float NdotV = saturate(dot(normal, viewDir));
    float rimLightIntensity = max(1.0 - NdotV, 0.0);
    rimLightIntensity = pow(rimLightIntensity, rimLightPower);
    rimLightIntensity = smoothstep(0.25, 0.35, rimLightIntensity);

    half cutOffDecision = CutoffRGB(giColor, rimLightCutoff);

    return rimLightIntensity * rimLightMultiplicative * giColor * cutOffDecision;
}

half3 CalculateLighting(Light light, InputData inputData, SurfaceData surfaceData, RimLighting rimData, half lambertWrap)
{
    half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    half3 lightColor = HalfLambert(attenuatedLightColor, light.direction, inputData.normalWS, lambertWrap);
    //half3 lightColor = LightingLambert(attenuatedLightColor, light.direction, inputData.normalWS);

    lightColor *= surfaceData.albedo;
    
    half3 rimLight = rimData.rimLightingToggle * CourierRimLight(attenuatedLightColor, light.direction, inputData.normalWS, inputData.viewDirectionWS,  rimData.rimLightPower, rimData.rimLightCutoff, rimData.rimLightMultiplicative);
    lightColor += rimLight;
    
    return lightColor;
}

half3 CalculateGI(InputData inputData, SurfaceData surfaceData, RimLighting rimData)
{
    half3 giColor = inputData.bakedGI * surfaceData.albedo;
    //half3 rimLight = rimData.rimLightingToggle * GIRimLight(giColor, inputData.normalWS, inputData.viewDirectionWS, rimData.rimLightPower, rimData.rimLightCutoff, rimData.rimLightMultiplicative);
    
    return giColor; //+ rimLight;
}

// Put Together Lighting
half4 CourierShade(InputData inputData, SurfaceData surfaceData, RimLighting rimData, float4 shadowCol)
{
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);

    #ifdef _OCCLUSIONMAP
        half occludedValue = 1 - (surfaceData.occlusion * _OcclusionStrength);
        surfaceData.albedo.rgb *= half3(occludedValue, occludedValue, occludedValue);
    #endif
    
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    
    inputData.bakedGI = CalculateGI(inputData, surfaceData, rimData);

    LightingData lightingData = CreateLightingData(inputData, surfaceData);
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        lightingData.mainLightColor += CalculateLighting(mainLight, inputData, surfaceData, rimData, _HalfLambertWrap);
        //lightingData.mainLightColor += CalculateBlinnPhong(mainLight, inputData, surfaceData);
    }
    
    #if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            lightingData.additionalLightsColor += CalculateLighting(light, inputData, surfaceData, rimData, _HalfLambertWrap);
            //lightingData.additionalLightsColor += CalculateBlinnPhong(light, inputData, surfaceData);
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    {
        lightingData.additionalLightsColor += CalculateLighting(light, inputData, surfaceData, rimData, _HalfLambertWrap);
        //lightingData.additionalLightsColor += CalculateBlinnPhong(light, inputData, surfaceData);
    }
    LIGHT_LOOP_END
    #endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting;
    #endif

    half3 lightColor = lightingData.mainLightColor + lightingData.additionalLightsColor + lightingData.giColor;

    half lum = GetAccurateLuminance(lightColor);
    half decval = 1.0 - when_lt(lum, _ShadowThreshold);
    //return half4(decval.xxx, 1.0);
    
    //lightColor = lerp(lightColor.rgb, shadowCol.rgb, shadowCol.a * 0.5f * decval);
        //lightColor = half4(0, 0, 0, 1.0);
    //float myStep = 1/_PosterizeSteps;
    //lightColor = floor(lightColor/myStep) * myStep;
    
    //return half4(lightingData.mainLightColor, 1.0);
    return half4(lightColor, 1.0);
    //return half4(lightColor + lightingData.vertexLightingColor + lightingData.giColor, 1.0); 
    //return CalculateFinalColor(lightingData, surfaceData.alpha);
}

#endif