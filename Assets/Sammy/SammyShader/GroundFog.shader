Shader "Custom/GroundFog"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0.65, 0.85, 1.0, 0.45)

        _MainTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 4
        _NoiseStrength ("Noise Strength", Range(0, 2)) = 1

        _SpeedX ("Speed X", Range(-0.2, 0.2)) = 0.05
        _SpeedY ("Speed Y", Range(-0.2, 0.2)) = 0.02
        _GlobalSpeed ("Global Speed", Range(0, 0.2)) = 0.03

        _Alpha ("Alpha", Range(0, 1)) = 0.35
        _Softness ("Softness", Range(0.01, 1)) = 0.25
        _Cutoff ("Cutoff", Range(0, 1)) = 0.35

        _Distortion ("Distortion", Range(0, 1)) = 0.15
        _EdgeFade ("Edge Fade", Range(0, 1)) = 0.35

        _WorldInfluence ("World Influence", Range(0, 0.05)) = 0.005
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "GroundFog"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;

            float4 _FogColor;
            float _NoiseScale;
            float _NoiseStrength;

            float _SpeedX;
            float _SpeedY;

            float _Alpha;
            float _Softness;
            float _Cutoff;

            float _Distortion;
            float _EdgeFade;

            float _GlobalSpeed;

            float _WorldInfluence;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionHCS = posInputs.positionCS;
                output.uv = input.uv;
                output.positionOS = input.positionOS.xyz;
                output.positionWS = posInputs.positionWS;

                return output;
            }

            float SampleNoise(float2 uv)
            {
                float t = _Time.y * _GlobalSpeed;

                float2 uv1 = uv * _NoiseScale + float2(_SpeedX, _SpeedY) * t;
                float2 uv2 = uv * (_NoiseScale * 1.8) - float2(_SpeedY, _SpeedX) * t * 0.6;

                float n1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv1).r;
                float n2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv2).r;

                return saturate((n1 * 0.65 + n2 * 0.35) * _NoiseStrength);
            }

            half4 Frag(Varyings input) : SV_Target
            {
       
                float2 uv = input.uv + input.positionWS.xz * _WorldInfluence;

                float distortionNoise = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    uv * _NoiseScale * 0.7 + float2(_SpeedX, _SpeedY) * _Time.y * _GlobalSpeed * 0.5
                ).r;

                uv += (distortionNoise - 0.5) * _Distortion;

                float noise = SampleNoise(uv);

                float fogMask = smoothstep(_Cutoff, _Cutoff + _Softness, noise);

                float2 centeredUV = input.uv * 2.0 - 1.0;
                float edge = 1.0 - saturate(length(centeredUV));
                edge = smoothstep(0.0, _EdgeFade, edge);

                float alpha = fogMask * edge * _Alpha * _FogColor.a;

                return half4(_FogColor.rgb, alpha);
            }

            ENDHLSL
        }
    }
}