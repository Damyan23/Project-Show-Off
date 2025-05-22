Shader "Custom/HDRP/ObjectGlow"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        [Space(10)]
        
        [Header(Glow Effect)]
        _GlowColor ("Glow Color", Color) = (1,0.5,0,1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2.0
        _GlowRange ("Glow Range", Range(0, 5)) = 1.0
        _GlowFalloff ("Glow Falloff", Range(0.1, 5)) = 1.5
        
        // HDRP standard properties (hidden but necessary)
        [HideInInspector] _EmissionColor("Emission Color", Color) = (1, 1, 1)
        [HideInInspector] _RenderQueueType("Render Queue Type", Float) = 5
        [HideInInspector] [ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 0.0
        [HideInInspector] _StencilRef("Stencil Ref", Int) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Int) = 6
        [HideInInspector] _StencilRefDepth("Stencil Ref Depth", Int) = 8
        [HideInInspector] _StencilWriteMaskDepth("Stencil Write Mask Depth", Int) = 8
        [HideInInspector] _StencilRefMV("Stencil Ref MV", Int) = 40
        [HideInInspector] _StencilWriteMaskMV("Stencil Write Mask MV", Int) = 40
        [HideInInspector] _StencilRefDistortionVec("Stencil Ref Distortion Vec", Int) = 4
        [HideInInspector] _StencilWriteMaskDistortionVec("Stencil Write Mask Distortion Vec", Int) = 4
    }

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" "RenderType" = "HDLitShader" }
        
        // Base pass for normal object rendering
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "ForwardOnly" }
            
            Blend One OneMinusSrcAlpha
            ZWrite On
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float4 _Color;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRange;
            float _GlowFalloff;
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Transform position to world space
                float3 positionWS = mul(UNITY_MATRIX_M, float4(input.positionOS.xyz, 1.0)).xyz;
                output.positionWS = positionWS;
                
                // Transform to clip space
                output.positionCS = mul(UNITY_MATRIX_VP, float4(positionWS, 1.0));
                
                // Pass UVs
                output.uv = input.uv;
                
                // Transform normals to world space
                output.normalWS = normalize(mul(input.normalOS, (float3x3)UNITY_MATRIX_I_M));
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                // Sample base texture
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                
                // Calculate view direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                
                // Fresnel effect for glow along edges
                float fresnel = pow(1.0 - saturate(dot(input.normalWS, viewDir)), _GlowFalloff);
                
                // Create glow effect
                float3 glow = _GlowColor.rgb * fresnel * _GlowIntensity;
                
                // Mix base albedo with glow
                float3 finalColor = albedo.rgb + glow;
                
                // Extend glow range
                float glowAlpha = saturate(fresnel * _GlowRange);
                
                // Final alpha (base + glow)
                float finalAlpha = max(albedo.a, glowAlpha);
                
                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
        
        // Shadow casting pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = mul(UNITY_MATRIX_M, float4(input.positionOS.xyz, 1.0)).xyz;
                output.positionCS = mul(UNITY_MATRIX_VP, float4(positionWS, 1.0));
                
                return output;
            }
            
            void frag() {}
            ENDHLSL
        }
        
        // Depth pass for proper z-buffer
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = mul(UNITY_MATRIX_M, float4(input.positionOS.xyz, 1.0)).xyz;
                output.positionCS = mul(UNITY_MATRIX_VP, float4(positionWS, 1.0));
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.HighDefinition.HDLitGUI"
}