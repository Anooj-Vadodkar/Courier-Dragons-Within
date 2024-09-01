#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#if defined(_NORMALMAP)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

// -----------------------------------------------------------------------------------------------------
//Functions to include

#if SHADER_LIBRARY_VERSION_MAJOR < 9
// Computes the world space view direction (pointing towards the viewer).
float3 GetWorldSpaceViewDir(float3 positionWS) {
    if (unity_OrthoParams.w == 0) {
        // Perspective
        return _WorldSpaceCameraPos - positionWS;
    } else {
        // Orthographic
        float4x4 viewMat = GetWorldToViewMatrix();
        return viewMat[2].xyz;
    }
}

half3 GetWorldSpaceNormalizeViewDir(float3 positionWS) {
    float3 viewDir = GetWorldSpaceViewDir(positionWS);
    if (unity_OrthoParams.w == 0) {
        // Perspective
        return half3(normalize(viewDir));
    } else {
        // Orthographic
        return half3(viewDir);
    }
}
#endif

//pretty much just copied from URP initializer for inputData
//puts out inputdata positionWS, normalWS, viewDirWs, shadows, fog, vertex lighting
//bakedGI, normalizedScreenSpaceUV, shadowMask, dynamic and static Lightmaps UVS
void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;
	
    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
		half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w); // viewDir has been stored in w components of these in vertex shader
		inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
	#else
		half3 viewDirWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
		inputData.normalWS = input.normalWS;
	#endif

	inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

	viewDirWS = SafeNormalize(viewDirWS);
	inputData.viewDirectionWS = viewDirWS;

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
		inputData.shadowCoord = input.shadowCoord;
	#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
		inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
	#else
		inputData.shadowCoord = float4(0, 0, 0, 0);
	#endif

	// Fog
	#ifdef _ADDITIONAL_LIGHTS_VERTEX
		inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
		inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
	#else
		inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactor);
		inputData.vertexLighting = half3(0, 0, 0);
	#endif


	#if defined(DYNAMICLIGHTMAP_ON)
		inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
	#else
		inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
	#endif

	inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
	inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

	#if defined(DEBUG_DISPLAY)
	#if defined(DYNAMICLIGHTMAP_ON)
		inputData.dynamicLightmapUV = input.dynamicLightmapUV;
	#endif
	#if defined(LIGHTMAP_ON)
		inputData.staticLightmapUV = input.staticLightmapUV;
	#else
		inputData.vertexSH = input.vertexSH;
	#endif
	#endif
}

