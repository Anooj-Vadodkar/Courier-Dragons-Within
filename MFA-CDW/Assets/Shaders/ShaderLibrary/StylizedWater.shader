Shader "Courier/Stylized Water"
{
    Properties
    {
        // Specular vs Metallic workflow
        //_WorkflowMode("WorkflowMode", Float) = 1.0

        //[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _ShallowColor("Shallow Color", Color) = (0,0.8,0.7,1)
        _DeepColor("Deep Color", Color) = (0.0, 0.3, 0.35, 1)
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _RefractionMap("Refraction Map", 2D) = "white" {}
        _FoamMap("Foam Map", 2D) = "white" {}
        _IntersectionFoamMap("Intersection Foam Map", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "white" {}
        
        [Toggle(_POSTERIZE)] _PosterizeToggle("Posterize", Float) = 1.0
        [Toggle(_USEOVERLAY)] _OverlayToggle("Overlay Toggle", Float) = 1.0
        [Toggle(_USELIGHTING)] _UseLighting("Use Lighting", Float) = 1.0
        [HideInInspector] _HalfLambertWrap("Float", Float) = 0.5
        [HideInInspector] _ShadowThreshold("Float", Float) = 0.3
        
        _LightHardness("Light Hardness", Float) = 0.5
        _LightSmoothness("Light Smoothness", Float) = 0.5
        
        _RefractionScale("Refraction Scale", Float) = 1.0
        _RefractionSpeed("Refraction Speed", Float) = 1.0
        _RefractionStrength("Refraction Strength", Float) = 1.0
        
        _FoamSize("Foam Size", Vector) = (1.0, 1.0, 0.0, 0.0)
        _FoamDistortion("Foam Distortion", Float) = 1.0
        _FoamCutoff("Foam Cutoff", Float) = 0.5
        _FoamColor("Foam Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _PanningDirection("Foam Direction", Range(0, 1)) = 0
        _FoamDistSpeed("Foam Distortion Spd", Float) = 1.0
        _PanningSpeed("Foam Pan Speed", Float) = 0.0
        
        _HorizonDistance("Horizon Distance", Float) = 2.0
        _HorizonColor("Horizon Color", Color) = (0.8, 0.4, 0.3, 1.0)
        _PosterizeSteps("Posterize Steps", Int) = 1
        _DepthFadeDistance("Depth Fade Distance", Float) = 1.0
        
        _IntersectionFoamColor("Intersect Foam Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _IntersectionPanSpeed("Intersect Pan Speed", Float) = 1.0
        _IntersectionDepth("Intersect Foam Depth", Float) = 1.0
        _IntersectionFade("Intersect Foam Fade", Float) = 1.0
        _IntersectionCutoff("Intersect Foam Cutoff", Float) = 0.5
        _IntersectionSize("Intersection Foam Size", Vector) = (1.0, 1.0, 0.0, 0.0)
        _IntersectionFoamAdd(" Intersection Foam Cutoff Add", Float) = 0.1
        _IntersectionSmoothing("Intersection Smoothing", Float) = 0.0
        _IntersectionDistSpeed("Intersection Distortion Spd", Float) = 1.0

        _NormalScale("Normals Scale", Float) = 1.0
        _NormalStrength("Normal Strength", Float) = 1.0
        _NormalSpeed("Normal Speed", Float) = 1.0
        
        _WaveSteepness("Wave Steepness", Float) = 1.0
        _WaveLength("Wave Length", Float) = 1.0
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveDirections("Wave Directions", Vector) = (1, -1, 0, 0.5)
        
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        [HideInInspector]_BumpScale("Bump Scale", Float) = 1.0
        
        // Blending state
        //[HideInInspector]_Surface("__surface", Float) = 0.0
        //[HideInInspector]_Blend("__blend", Float) = 0.0
        //[HideInInspector]_Cull("__cull", Float) = 2.0
        [ToggleUI] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0

        [ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline" 
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _BaseMap_ST;
        float4 _BaseColor;
        float4 _ShallowColor;
        float4 _DeepColor;
        float4 _HorizonColor;
        float4 _FoamColor;
        float4 _SpecularColor;
        float4 _IntersectionFoamColor;
        float4 _FoamSize;
        float4 _IntersectionSize;
        float4 _WaveDirections;
        half _ShadowThreshold;
        half _HalfLambertWrap;
        half _WaveSpeed;
        half _WaveLength;
        half _WaveSteepness;
        half _LightHardness;
        half _LightSmoothness;
        half _NormalScale;
        half _NormalStrength;
        half _NormalSpeed;
        half _IntersectionPanSpeed;
        half _IntersectionCutoff;
        half _IntersectionFoamAdd;
        half _IntersectionDistSpeed;
        half _IntersectionDepth;
        half _IntersectionFade;
        half _IntersectionSmoothing;
        half _FoamCutoff;
        half _FoamDistortion;
        half _FoamDistSpeed;
        half _RefractionSpeed;
        half _PanningDirection;
        half _PanningSpeed;
        half _RefractionScale;
        half _RefractionStrength;
        half _DepthFadeDistance;
        half _HorizonDistance;
        half _PosterizeSteps;
        half _Cutoff;
        half _BumpScale;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            //#pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _POSTERIZE
            #pragma shader_feature_local_fragment _USEOVERLAY
            #pragma shader_feature_local_fragment _USELIGHTING
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex WaterVert
            #pragma fragment WaterFrag

            #include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"
            #include "Assets/Shaders/ShaderLibrary/StylizedWater.hlsl"
            ENDHLSL 
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            // -------------------------------------
            // Universal Pipeline keywords

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta

            #pragma shader_feature EDITOR_VISUALIZATION
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Assets/Shaders/ShaderLibrary/CourierInstancing.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 normalOS     : NORMAL;
                float2 uv0          : TEXCOORD0;
                float2 uv1          : TEXCOORD1;
                float2 uv2          : TEXCOORD2;
                #ifdef _TANGENT_TO_WORLD
                    float4 tangentOS    : TANGENT;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   :   SV_POSITION;
                float2 uv           :   TEXCOORD0;
                #ifdef EDITOR_VISUALIZATION
                    float2 VizUV        :   TEXCOORD1;
                    float4 LightCoord   :   TEXCOORD2;
                #endif
            };

            
            #include "Assets/Shaders/ShaderLibrary/CourierSurface.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            Varyings UniversalVertexMeta(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                #ifdef EDITOR_VISUALIZATION
                    UnityEditorVizData(input.positionOS.xyz, input.uv0, input.uv1, input.uv2, output.VizUV, output.LightCoord);
                #endif
                return output;
            }

            half4 UniversalFragmentMeta(Varyings input) : SV_Target {
                SurfaceData surfaceData;
                InitializeSurfaceData(input.uv, surfaceData);

                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

                MetaInput metaInput;

                #ifdef EDITOR_VISUALIZATION
                    metaInput.VizUV = input.VizUV;
                    metaInput.LightCoord = input.LightCoord;
                #endif

                metaInput.Albedo = brdfData.diffuse + brdfData.specular * brdfData.specular * brdfData.roughness * 0.5;
                metaInput.Emission = surfaceData.emission;
                
                return UnityMetaFragment(metaInput);
            }
            
            ENDHLSL
        }

    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}
