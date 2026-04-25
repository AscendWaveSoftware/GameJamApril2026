Shader "Custom/StylizedOverall"
{
    Properties
    {
        _Saturation ("Saturation", Range(0, 2)) = 1.15
        _Contrast ("Contrast", Range(0, 2)) = 1.15
        _Brightness ("Brightness", Range(-1, 1)) = 0.02

        _ShadowTint ("Shadow Tint", Color) = (0.55, 0.65, 1.0, 1)
        _HighlightTint ("Highlight Tint", Color) = (1.0, 0.86, 0.55, 1)

        _PosterizeSteps ("Posterize Steps", Range(2, 32)) = 10
        _PosterizeStrength ("Posterize Strength", Range(0, 1)) = 0.25

        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.18
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "StylizedOverall"

            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Saturation;
            float _Contrast;
            float _Brightness;

            float4 _ShadowTint;
            float4 _HighlightTint;

            float _PosterizeSteps;
            float _PosterizeStrength;

            float _VignetteStrength;

            float3 ApplySaturation(float3 color, float saturation)
            {
                float luminance = dot(color, float3(0.299, 0.587, 0.114));
                return lerp(luminance.xxx, color, saturation);
            }

            float3 ApplyContrast(float3 color, float contrast)
            {
                return (color - 0.5) * contrast + 0.5;
            }

            float3 ApplyPosterize(float3 color, float steps, float strength)
            {
                steps = max(steps, 2.0);
                float3 posterized = floor(color * steps) / steps;
                return lerp(color, posterized, strength);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb;

                float luminance = dot(color, float3(0.299, 0.587, 0.114));

                float shadowMask = smoothstep(0.55, 0.0, luminance);
                float highlightMask = smoothstep(0.45, 1.0, luminance);

                color = lerp(color, color * _ShadowTint.rgb, shadowMask * 0.35);
                color = lerp(color, color * _HighlightTint.rgb, highlightMask * 0.25);

                color = ApplySaturation(color, _Saturation);
                color = ApplyContrast(color, _Contrast);
                color += _Brightness;

                color = ApplyPosterize(color, _PosterizeSteps, _PosterizeStrength);

                float2 centeredUV = uv * 2.0 - 1.0;
                float vignette = 1.0 - dot(centeredUV, centeredUV) * _VignetteStrength;
                color *= saturate(vignette);

                return half4(saturate(color), 1.0);
            }

            ENDHLSL
        }
    }
}