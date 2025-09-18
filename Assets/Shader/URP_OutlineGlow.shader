Shader "Custom/URP_OutlineGlow"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 2.0
        _RimPower ("Rim Power", Range(1, 10)) = 4.0
        _Pulse ("Pulse", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Front // 关键：只渲染背面（外翻面），实现轮廓

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDirWS : TEXCOORD0;
            };

            float3 _OutlineColor;
            float _GlowIntensity;
            float _RimPower;
            float _Pulse;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;

                // 外扩法线方向（制造轮廓厚度）
                float3 positionWS = vertexInput.positionWS;
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                positionWS += normalWS * 0.02; // 微微外扩
                output.positionCS = TransformWorldToHClip(positionWS);

                output.viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 viewDir = normalize(input.viewDirWS);
                float3 normal = float3(0, 0, -1); // 近似外表面法线（面向摄像机）
                float rim = 1.0 - saturate(dot(viewDir, normal));
                rim = pow(rim, _RimPower);

                half3 glow = _OutlineColor.rgb * rim * _GlowIntensity * _Pulse;
                return half4(glow, rim * _OutlineColor.a * _Pulse);
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Unlit"
}