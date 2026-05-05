Shader "Custom/GridFloor"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.05, 0.03, 0.07, 1)
        _LineColor ("Line Color", Color) = (0.25, 0.15, 0.35, 1)
        _GridSize ("Grid Size", Float) = 5
        _LineWidth ("Line Width", Range(0.01, 0.1)) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _LineColor;
                float _GridSize;
                float _LineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half2 worldUV = input.positionWS.xz / _GridSize;
                half2 grid = abs(frac(worldUV - 0.5) - 0.5);
                half gridLine = min(grid.x, grid.y);
                half mask = gridLine < _LineWidth ? (half)1.0 : (half)0.0;
                return lerp(_BaseColor, _LineColor, mask);
            }
            ENDHLSL
        }
    }
}