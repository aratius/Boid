// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Fish"
{
    Properties
    {
        [NoScaleOffset] _MyTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", float) = 0
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
                const float RANGE = 0.1;
                float far_bias = pow(dist / 0.25, 2.);
                float add_angle = sin(_Progress * 50.) * far_bias * RANGE;
                if(uv.y < 0.) add_angle *= -1.;
                uv = fixed2(sin(angle + add_angle) * dist, cos(angle + add_angle) * dist);

                uv += CENTER;

                if(uv.x > 1. || uv.x < 0. || uv.y > 1. || uv.y < 0.) discard;

                fixed4 col = tex2D(_MyTex, uv);
                if(col.r == 0.) col.a = 0.;
                return col;
            }
            ENDCG
        }
    }
}
