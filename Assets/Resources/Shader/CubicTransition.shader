Shader "Custom/CubicTransition"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Progress("Progress", Range(0,1)) = 0.0
        _GridSize("Grid Size", Float) = 16.0
        _FadeColor("Fade Color", Color) = (0,0,0,1)
        _StartPos("StartPos", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Progress;
            float _GridSize;
            float4 _FadeColor;
            float _StartPos;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // グリッド番号 (0〜GridSize-1)
                float2 grid = floor(uv * _GridSize);

                float gridX = grid.x;
                float gridY = grid.y;

                // 進行方向による距離
                float gridDistance;
                if (_StartPos < 0.25)         // 下
                    gridDistance = (_GridSize - 1.0) - gridY;
                else if (_StartPos < 0.5)     // 左
                    gridDistance = gridX;
                else if (_StartPos < 0.75)    // 上
                    gridDistance = gridY;
                else                          // 右
                    gridDistance = (_GridSize - 1.0) - gridX;

                // グリッドごとにランダム値を足す（0〜1）
                float rand = hash21(grid);
                float randomDistance = gridDistance + rand;

                // 現在の進行位置
                float threshold = _Progress * (_GridSize + 1);

                // このブロックは透明化済み
                if (randomDistance < threshold - 1.0)
                    return float4(0,0,0,0);

                // このブロックはまだ残る（黒）
                if (randomDistance > threshold)
                    return float4(_FadeColor.rgb,1);

                // ★このグリッドの推移中（0→1の間）
                float t = frac(threshold);

                // ★ ブロックは丸ごとフェード。(ピクセル位置は見ない)
                float fade = smoothstep(1, 0, t);

                return lerp(float4(0,0,0,0), float4(_FadeColor.rgb,1), fade);
            }

            ENDCG
        }
    }
}
