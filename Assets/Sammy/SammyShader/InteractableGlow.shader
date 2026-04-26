Shader "Custom/InteractableAuraSphere"
{
    Properties
    {
        _AuraColor ("Aura Color", Color) = (0.2, 0.85, 1.0, 1)
        _Strength ("Strength", Range(0, 10)) = 2.5
        _Alpha ("Alpha", Range(0, 1)) = 0.35
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.2
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 1.5
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent+80"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Back

        Pass
        {
            Name "InteractableAura"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _AuraColor;
            float _Strength;
            float _Alpha;
            float _RimPower;
            float _PulseSpeed;
            float _PulseAmount;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normal = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = normalize(normal.normalWS);

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                float3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));

                float rim = 1.0 - saturate(dot(normalWS, viewDir));
                rim = pow(rim, _RimPower);

                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;

                float alpha = rim * _Alpha * pulse;
                float3 color = _AuraColor.rgb * _Strength * pulse;

                return half4(color, alpha);
            }

            ENDHLSL
        }
    }
}