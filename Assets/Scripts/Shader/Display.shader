Shader "Hidden/Display"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _MyTex ("Texture", 2D) = "white" {}
        _MaxIter ("MaxIter", int) = 0
        _Refrection ("Refrection", Range(0, 3)) = 0
        _Brightness ("Brightness", Range(0, 3)) = 0
        _Fineness ("Fitness", Range(0, 30)) = 0
        _Speed ("Speed", Range(0, 10)) = 0
        _Speed2 ("Speed2", Range(0, 10)) = 0
        _Power ("Power", Range(0, 3)) = 0
        _DirectionX ("DirectionX", Range(0, 3)) = 0
        _DirectionY ("DirectionY", Range(0, 3)) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _Resolution ("Resolution", Vector) = (1024, 500, 0, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MyTex;
            int _MaxIter;
            float _Refrection;
            float _Brightness;
            float _Fineness;
            float _Speed;
            float _Speed2;
            float _Power;
            float _DirectionX;
            float _DirectionY;
            fixed4 _Color;
            fixed4 _Resolution;

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time * 30;

                half2 sp = i.uv * normalize(_Resolution.xy); // surfacePosition は i.uv で置き換える
                half2 p = sp * _Fineness - half2(_Fineness * 1.3, _Fineness * 1.3);
                half2 _i = p;
                float c = _Refrection;
                float inten = .025 * _Brightness;
                float speed = _Speed;
                float speed2 = _Speed2;
                float freq = _Power;
                float xflow = _DirectionX;
                float yflow = _DirectionY;

                for (int n = 0; n < _MaxIter; n++)
                {
                    float t = time * (1.0 - (3.0 / (n + speed)));
                    _i = p + half2(cos(t - _i.x * freq) + sin(t + _i.y * freq) + (time * xflow), sin(t - _i.y * freq) + cos(t + _i.x * freq) + (time * yflow));
                    c += 1.0 / length(half2(p.x / (sin(_i.x + t * speed2) / inten), p.y / (cos(_i.y + t * speed2) / inten)));
                }

                c /= _MaxIter;
                c = 1.5 - sqrt(c);

                // _MyTex にエフェクトを適用させるために uv に変形を加えつつ波のエフェクトを tex に加算する
                // _Color プロパティを使用してエディタから色味を変更できるようにする
                float cccc = pow(c, 6.);
                half3 _col = half3(cccc, cccc, cccc) + half3(_Color.r, _Color.g, _Color.b);
                half4 tex = tex2D(_MyTex, i.uv + c * .025);
                tex.rgb += _col * 0.1;
                return tex;

                // 上の三行をコメントアウトして下記を有効にすると移植元の GLSL と同じような感じになる
                //  half3 _col = half3(cccc, cccc, cccc) + half3(0.0, 0.4, 0.55);
                //  return half4(_col, 1);
            }
            ENDCG
        }
    }
}
