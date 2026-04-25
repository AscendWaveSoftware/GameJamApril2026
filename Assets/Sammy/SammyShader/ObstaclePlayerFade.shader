Shader "Custom/ObstaclePlayerFade"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        _PlayerFade ("Player Fade", Range(0,1)) = 0
        _FadedAlpha ("Faded Alpha", Range(0,1)) = 0.35

        _FadeTint ("Fade Tint", Color) = (1,1,1,1)
        _FadeTintStrength ("Fade Tint Strength", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ObstaclePlayerFade"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float4 _BaseColor;

            float _PlayerFade;
            float _FadedAlpha;

            float4 _FadeTint;
            float _FadeTintStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float4 baseCol = tex * _BaseColor;

                float fade = saturate(_PlayerFade);

                float3 fadedColor = lerp(
                    baseCol.rgb,
                    baseCol.rgb * _FadeTint.rgb,
                    fade * _FadeTintStrength
                );

                float alpha = lerp(baseCol.a, _FadedAlpha, fade);

                return half4(fadedColor, alpha);
            }

            ENDHLSL
        }
    }
}