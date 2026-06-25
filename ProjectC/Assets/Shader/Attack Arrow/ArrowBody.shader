Shader "Hearthstone/Arrow/ArrowBody"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        _Speed                   ("Speed", Float) = 1
        _Scale                   ("Scale", Vector) = (1, 1, 0, 0)
        [Toggle] _ScrollX        ("Scroll X axis (off = Y axis)", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent"
            "RenderType"     = "Transparent"
        }

        Cull   Off
        ZWrite Off
        ZTest  LEqual
        Blend  SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ArrowBody"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float  _Speed;
                float2 _Scale;
                float  _ScrollX;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 scaledUV   : TEXCOORD1;
                float4 color      : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color      = IN.color;

                float2 scaledUV = IN.uv * _Scale;
                OUT.scaledUV    = scaledUV;

                // X / Y 축 선택해서 스크롤
                float scroll    = _Time.y * _Speed;
                scaledUV.x     += scroll * _ScrollX;
                scaledUV.y     += scroll * (1.0 - _ScrollX);
                OUT.uv          = scaledUV;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // fade: 선의 시작점(UV.x=0)에서 투명하게
                // _ScrollX=1 이면 X축 fade, 0이면 Y축 fade
                float fadeU     = IN.scaledUV.x * _ScrollX
                                + IN.scaledUV.y * (1.0 - _ScrollX);
                float fadeAlpha = fadeU * fadeU;

                col.a = min(col.a, fadeAlpha);
                col  *= IN.color;

                return col;
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}