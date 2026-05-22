Shader "Custom/FlashWhite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        [HDR] _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0, 0.05)) = 0.02
        _OutlineEnabled ("Outline Enabled", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "SpriteAtlasTextureParam" = "_MainTex"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        // 아웃라인 패스
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineEnabled;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_OutlineEnabled < 0.5) discard;

                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a > 0.01) discard;

                float2 offsets[4];
                offsets[0] = float2(_OutlineThickness, 0);
                offsets[1] = float2(-_OutlineThickness, 0);
                offsets[2] = float2(0, _OutlineThickness);
                offsets[3] = float2(0, -_OutlineThickness);

                float neighborAlpha = 0;
                for (int j = 0; j < 4; j++)
                    neighborAlpha += tex2D(_MainTex, i.uv + offsets[j]).a;

                if (neighborAlpha < 0.01) discard;

                return _OutlineColor;
            }
            ENDCG
        }

        // 플래시 패스 — 스프라이트 실루엣을 흰색으로 덮음, _MainTex 알파만 사용
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _FlashColor;
            float _FlashAmount;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.01) discard;
                // 알파만 유지하고 rgb는 FlashColor로 완전히 덮음
                return fixed4(_FlashColor.rgb, col.a * _FlashAmount);
            }
            ENDCG
        }
    }
}
