Shader"Unlit/URPFurShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _FurTexture ("Fur Texture", 2D) = "white" {}
        _FurLength ("Fur Length", Range(0.01, 1.0)) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    float3 normalOS : NORMAL;
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionCS : SV_POSITION;
    float3 normalWS : NORMAL;
};

            TEXTURE2D(_FurTexture); SAMPLER(sampler_FurTexture);
float4 _Color;
float _FurLength;

Varyings vert(Attributes IN)
{
    Varyings OUT;
    VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
    VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);
                
    OUT.positionCS = posInputs.positionCS;
    OUT.uv = IN.uv;
    OUT.normalWS = normInputs.normalWS;
    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    half furIntensity = saturate(dot(IN.normalWS, float3(0, 1, 0)));
    half4 furColor = SAMPLE_TEXTURE2D(_FurTexture, sampler_FurTexture, IN.uv) * _Color;
    return lerp(furColor, half4(0, 0, 0, 1), _FurLength * (1 - furIntensity));
}

            ENDHLSL
        }
    }
}
