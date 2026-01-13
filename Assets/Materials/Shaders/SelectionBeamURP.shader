Shader "VFX/SelectionBeamURP"
{
    Properties
    {
        [HDR]_Color("Beam Color (HDR)", Color) = (0.25, 1.0, 0.9, 1)
        _Intensity("Intensity", Range(0, 10)) = 2.0

        _BottomBoost("Bottom Boost", Range(0, 5)) = 1.5
        _VerticalPower("Vertical Power", Range(0.5, 10)) = 3.0
        _RadialPower("Radial Power", Range(0.5, 10)) = 2.0

        _FresnelPower("Fresnel Power", Range(0.5, 10)) = 4.0
        _FresnelStrength("Fresnel Strength", Range(0, 3)) = 1.0

        _NoiseStrength("Noise Strength", Range(0, 2)) = 0.25
        _NoiseScale("Noise Scale", Range(0.1, 20)) = 6.0
        _NoiseSpeed("Noise Speed", Range(0, 5)) = 0.8

        _Alpha("Alpha", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            // Additive transparency
            Blend One One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Intensity;

                float _BottomBoost;
                float _VerticalPower;
                float _RadialPower;

                float _FresnelPower;
                float _FresnelStrength;

                float _NoiseStrength;
                float _NoiseScale;
                float _NoiseSpeed;

                float _Alpha;
            CBUFFER_END

            // tiny cheap noise (no texture)
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float noise2D(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   nrmInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = posInputs.positionCS;
                OUT.positionOS  = IN.positionOS.xyz;
                OUT.normalWS    = nrmInputs.normalWS;

                float3 camPosWS = GetCameraPositionWS();
                OUT.viewDirWS   = normalize(camPosWS - posInputs.positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // === Vertical mask (bottom bright -> top transparent)
                // Cylinder in Unity usually spans Y ~ [-0.5..0.5] in object space.
                // Remap y from [-0.5..0.5] -> [0..1]
                float y01 = saturate((IN.positionOS.y + 0.5) / 1.0);
                float vertical = pow(1.0 - y01, _VerticalPower);
                vertical *= (1.0 + _BottomBoost * (1.0 - y01)); // extra pop at bottom

                // === Radial mask (center bright -> edges weaker)
                float2 xz = IN.positionOS.xz;
                float r = length(xz); // 0 center
                // For default cylinder radius ~0.5
                float radial = saturate(1.0 - (r / 0.5));
                radial = pow(radial, _RadialPower);

                // === Fresnel (edge glow)
                float ndv = saturate(dot(normalize(IN.normalWS), normalize(IN.viewDirWS)));
                float fresnel = pow(1.0 - ndv, _FresnelPower) * _FresnelStrength;

                // === Animated noise to make it "alive"
                float t = _Time.y * _NoiseSpeed;
                float n = noise2D(float2(xz.x, IN.positionOS.y) * _NoiseScale + float2(0, t));
                // sharpen a bit
                n = smoothstep(0.35, 1.0, n);

                float mask = vertical * radial;
                float glow = mask * (1.0 + fresnel) + (n * _NoiseStrength * mask);

                float3 col = _Color.rgb * (_Intensity * glow);
                float a = saturate(glow * _Alpha);

                // Additive blend ignores alpha mostly, but keep it for compatibility / future changes
                return half4(col, a);
            }
            ENDHLSL
        }
    }
}
