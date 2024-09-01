Shader "PostProcess/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("_Scale", Float) = 1.0
        _DepthThreshold("Depth Threshold", Float) = 0.6
        _NormalThreshold("Normal Threshold", Float) = 0.6
        _DepthNormalThreshold("Depth Normal Threshold", Float) = 0.6
        _DepthNormalThresholdScale("Depth Normal Threshold Scale", Float) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Assets/Shaders/ShaderLibrary/CourierFunctions.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewSpaceDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                o.viewSpaceDir = -normalize(GetViewForwardDir());
                o.viewSpaceDir = TransformWorldToViewDir(o.viewSpaceDir);
                return o;
            }

            float3 DecodeNormal(float4 enc)
            {
                float kScale = 1.7777;
                float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
                float g = 2.0 / dot(nn.xyz,nn.xyz);
                float3 n;
                n.xy = g*nn.xy;
                n.z = g-1;
                return n;
            }

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            
            float _Scale;
            float _DepthThreshold;
            float _NormalThreshold;
            float _DepthNormalThreshold;
            float _DepthNormalThresholdScale;

            TEXTURE2D(_CameraDepthNormalsTexture);
            SAMPLER(sampler_CameraDepthNormalsTexture);

            half4 frag (v2f i) : SV_Target
            {
                float4 origColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                float halfScaleFloor = floor(_Scale * 0.5);
                float halfScaleCeil = ceil(_Scale * 0.5);

                //robert's cross
                float2 bottomLeftUV = i.uv - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 topRightUV = i.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
                float2 bottomRightUV = i.uv + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 topLeftUV = i.uv + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

                float3 normal0 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, bottomLeftUV));
                float3 normal1 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, bottomRightUV));
                float3 normal2 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, topRightUV));
                float3 normal3 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, topLeftUV));

                float3 normalFiniteDifference0 = normal2 - normal0;
                float3 normalFiniteDifference1 = normal3 - normal1;

                float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
                //edgeNormal = when_gt(edgeNormal, _NormalThreshold);
                edgeNormal = smoothstep(_NormalThreshold - 0.015, _NormalThreshold + 0.015, edgeNormal);
                
                float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
                float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
                float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
                float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;

                float depthFiniteDifference0 = depth2 - depth0;
                float depthFiniteDifference1 = depth3 - depth1;

                float edgeDepth = depthFiniteDifference0 * depthFiniteDifference0 + depthFiniteDifference1 * depthFiniteDifference1;
                edgeDepth = sqrt(edgeDepth) * 100;

                //modulation and other stuff here
                float3 viewNormal = normal0;
                float NdotV = 1.0 - dot(viewNormal, normalize(i.viewSpaceDir));
                NdotV = pow(NdotV, 1.5);
                float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
                float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;
                
                float depthThreshold = _DepthThreshold * depth0 * normalThreshold;
                //edgeDepth = smoothstep(depthThreshold - 0.001, depthThreshold + 0.001, edgeDepth); //(ed, _DepthThreshold);
                edgeDepth = when_gt(edgeDepth, depthThreshold);
                
                float edge = max(edgeDepth, edgeNormal);
                //edge = edgeDepth;

                //color sampling
                float3 color0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, bottomLeftUV).rgb;
                float3 color1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, bottomRightUV).rgb;
                float3 color2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, topRightUV).rgb;
                float3 color3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, topLeftUV).rgb;

                float3 weightedColor = color0 * depth0 + color1 * depth1 + color2 * depth2 + color3 * depth3;
                float depthTotal = depth0 + depth1 + depth2 + depth3;
                weightedColor = saturate(weightedColor/depthTotal);

                // add new variable here for multiplier for weighted - maybe could change depending on something?
                float4 edgeColor = float4(weightedColor* 0.5, edgeDepth * depthTotal * 500);

                //float val = when_gt(edgeDepth, 0.0);
                //return half4(val.rrr, 1.0);
                // TODO: UNCOMMENT LATER
                return half4(lerp(origColor.rgb, edgeColor.rgb, saturate(edgeColor.a)), 1.0);
                
                
                // just invert the colors
                /*
                float3 center = tex2D(_MainTex, i.uv).rgb;
                float3 north = tex2D(_MainTex, i.uv + half2(0, _MainTex_TexelSize.y)).rgb;
                float3 south = tex2D(_MainTex, i.uv - half2(0, _MainTex_TexelSize.y)).rgb;
                float3 east = tex2D(_MainTex, i.uv - half2(_MainTex_TexelSize.x, 0)).rgb;
                float3 west = tex2D(_MainTex, i.uv + half2(_MainTex_TexelSize.x, 0)).rgb;
                */

                /*
                float clum = GetAccurateLuminance(center);
                float nlum = GetAccurateLuminance(north);
                float slum = GetAccurateLuminance(south);
                float elum = GetAccurateLuminance(east);
                float wlum = GetAccurateLuminance(west);

                float finalValue = nlum + slum + elum + wlum - 4 * clum;
                */
            }
            ENDHLSL
        }
    }
}
