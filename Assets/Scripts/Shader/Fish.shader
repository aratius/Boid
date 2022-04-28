Shader "Unlit/Fish"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                const fixed2 CENTER = fixed2(0.5, 0.5);
                uv -= CENTER;

                float angle = atan2(uv.x, uv.y);
                float dist = length(uv);
                const float RANGE = 0.1;
                float far_bias = pow(dist / 0.5, 2.);
                float add_angle = sin(_Time.y * 3.) * far_bias * RANGE;
                uv = fixed2(sin(angle + add_angle) * dist, cos(angle + add_angle) * dist);

                uv += CENTER;
                fixed4 col = fixed4(step(uv.x, 0.5), 0., 0., 1.);
                return col;
            }
            ENDCG
        }
    }
}
