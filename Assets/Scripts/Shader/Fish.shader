// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Fish"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _MyTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", float) = 0
        _IllProgress ("IllProgress", float) = 0  // 病気 0-1 1で死亡
        _PainProgress ("PainProgress", float) = 0  // 身体的な傷 0-1 1で死亡
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MyTex;
            fixed _Progress;
            fixed _IllProgress = 0.;
            fixed _PainProgress = 0.;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                const fixed2 CENTER = fixed2(0.5, 0.5);
                uv -= CENTER;

                float angle = atan2(uv.x, uv.y);
                float dist = length(uv);
                const float RANGE = 0.15;
                float far_bias = pow(dist / 0.25, 2.);
                float add_angle = sin(_Progress * 5.) * far_bias * RANGE;
                if(uv.y < 0.) add_angle *= -1.;
                uv = fixed2(sin(angle + add_angle) * dist, cos(angle + add_angle) * dist);

                uv += CENTER;

                if(uv.x > 1. || uv.x < 0. || uv.y > 1. || uv.y < 0.) discard;

                fixed4 col = tex2D(_MyTex, uv);
                const fixed4 NORMAL = fixed4(0.65, 0.65, 0.65, 1.);
                const fixed4 ILL = fixed4(88./255., 158./255., 166./255., 0.);

                if(col.r == 0.) discard;
                else col = lerp(NORMAL, ILL, _IllProgress);
                return col;
            }
            ENDCG
        }
    }
}
