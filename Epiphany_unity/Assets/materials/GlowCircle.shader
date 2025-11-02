Shader "Custom/GlowCircle"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0, 0.7, 1, 1)
        _Intensity ("Glow Intensity", Range(0, 8)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _GlowColor;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                // alpha da sprite
                float alpha = tex.a;
                // smoothstep cria decaimento suave do centro para as bordas
                float glow = smoothstep(0.15, 0.85, tex.a) * _Intensity;
                // cor final: glow vezes a cor escolhida, alpha usa o alpha da texture
                return fixed4(_GlowColor.rgb * glow, alpha * 1.0);
            }
            ENDCG
        }
    }
}
