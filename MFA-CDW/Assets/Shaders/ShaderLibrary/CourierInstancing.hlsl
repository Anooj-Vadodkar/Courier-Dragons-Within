#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTNACING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DOTS_INSTANCED_PROP(float4, _Cutoff)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _BaseColor      UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4, Metadata_BaseColor)
#define _Cutoff         UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float , Metadata_Cutoff)

#endif