Shader "Custom/DiceEdgeEnergyFill"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.12, 0.12, 0.12, 1)   // темний метал/пластик
        _EnergyColor ("Energy Color", Color) = (1, 0.35, 0.1, 1)   // колір лави/енергії

        _Charge ("Charge", Range(0,1)) = 0

        _EdgeFresnelPower ("Edge Fresnel Power", Range(0.5, 10)) = 2.5
        _EnergyIntensity ("Energy Intensity", Range(0, 50)) = 18

        // Flow look
        _FlowScale ("Flow Scale", Range(0.1, 20)) = 6
        _FlowSpeed ("Flow Speed", Range(0, 10)) = 2.2
        _FlowSharpness ("Flow Sharpness", Range(0.5, 10)) = 2.2

        // Fill behaviour
        _FillWidth ("Fill Width", Range(0.001, 0.3)) = 0.08       // м’якість “заповнення”
        _MinVisible ("Min Visible", Range(0, 1)) = 0.02           // щоб на 0 не було зовсім порожньо

        // Ready pulse
        _ReadyThreshold ("Ready Threshold", Range(0,1)) = 0.98
        _PulseFreq ("Pulse Freq", Range(0, 20)) = 6
        _PulseAmp ("Pulse Amp", Range(0, 1)) = 0.25
        _PulseBase ("Pulse Base", Range(0, 2)) = 0.9
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "EdgeEnergyFill"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EnergyColor;
                float _Charge;

                float _EdgeFresnelPower;
                float _EnergyIntensity;

                float _FlowScale;
                float _FlowSpeed;
                float _FlowSharpness;

                float _FillWidth;
                float _MinVisible;

                float _ReadyThreshold;
                float _PulseFreq;
                float _PulseAmp;
                float _PulseBase;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS  : TEXCOORD0; // object space for stable pattern
                float3 normalWS    : TEXCOORD1;
                float3 positionWS  : TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs nrm   = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = pos.positionCS;
                OUT.positionWS  = pos.positionWS;
                OUT.normalWS    = nrm.normalWS;
                OUT.positionOS  = IN.positionOS.xyz;
                return OUT;
            }

            // Cheap hash noise
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // 0..1 animated flow field
            float flowField(float3 posOS, float t)
            {
                float2 p = posOS.xz * _FlowScale;

                float w1 = sin(p.x + t * _FlowSpeed);
                float w2 = sin(p.y * 1.37 - t * (_FlowSpeed * 0.85));
                float n  = (hash21(p + t * 0.25) * 2 - 1);

                float f = (w1 * 0.6 + w2 * 0.4 + n * 0.35);
                f = f * 0.5 + 0.5;                 // 0..1
                f = pow(saturate(f), _FlowSharpness);
                return f;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float charge = saturate(_Charge);
                float t = _Time.y;

                // Edge mask (so energy stays on rim nicely)
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(GetWorldSpaceViewDir(IN.positionWS));
                float fresnel = pow(1.0 - saturate(dot(N, V)), _EdgeFresnelPower);

                // Animated energy pattern 0..1
                float flow = flowField(IN.positionOS, t);

                // Progress fill:
                // We "reveal" energy as charge increases by comparing flow to charge.
                // When charge is small -> only small parts appear; later -> more.
                float fill = smoothstep(charge - _FillWidth, charge + _FillWidth, flow);

                // Keep tiny visibility to avoid “dead” look at 0
                fill = max(fill, _MinVisible * charge);

                // Ready pulse (only at the end)
                float ready = step(_ReadyThreshold, charge);
                float pulse = sin(t * _PulseFreq) * _PulseAmp + _PulseBase;
                float pulseMul = lerp(1.0, pulse, ready);

                // Compose
                float energy = fresnel * fill * _EnergyIntensity * pulseMul;

                float3 rgb = _BaseColor.rgb + _EnergyColor.rgb * energy;
                float a = _BaseColor.a; // кайомка завжди видима
                return half4(rgb, a);
            }
            ENDHLSL
        }
    }
}
