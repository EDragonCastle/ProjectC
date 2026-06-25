Shader "Hearthstone/Arrow/ArrowEnd"
{
    Properties
    {
        _MainTex ("Arrow Head Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,0,0,1)

        _StencilComp      ("Stencil Comparison", Float) = 8
        _Stencil          ("Stencil ID", Float) = 0
        _StencilOp        ("Stencil Operation", Float) = 0
        _StencilReadMask  ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _ColorMask        ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True"
               "RenderType"="Transparent" "PreviewType"="Plane" }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _TintColor;
            float4    _ClipRect;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 color    : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                col *= _TintColor * i.color;

                #ifdef UNITY_UI_CLIP_RECT
                    col.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                    clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}