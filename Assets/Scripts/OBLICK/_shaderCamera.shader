Shader "Custom/GlassScanner"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BlinkColor ("Blink Color", Color) = (1,0,0,1) // Добавляем отдельный цвет для мигания
        _IRReflection ("Reflection Intensity", Range(0, 5)) = 1
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _BlinkStrength ("Blink Strength", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _BlinkColor;
            float _IRReflection;
            float _Transparency;
            float _BlinkStrength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Плавное мигание между базовым и blink цветом
                float blinkFactor = sin(_Time.y * 5) * 0.5 + 0.5;
                fixed4 finalColor = lerp(_BaseColor, _BlinkColor, blinkFactor * _BlinkStrength);
                
                // Добавляем отражение
                finalColor.rgb *= lerp(1.0, _IRReflection, _BlinkStrength);
                finalColor.a = _Transparency;

                return finalColor;
            }
            ENDCG
        }
    }
}