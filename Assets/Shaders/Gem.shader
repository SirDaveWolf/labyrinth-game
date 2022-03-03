Shader "Custom/Gem" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _RefPow("Refraction Distortion", Range(0,128)) = 15.0
    }
        SubShader{
            GrabPass {"_Refraction"}
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Gem fullforwardshadows noambient

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Refraction;
        float4 _Refraction_TexelSize;

        struct Input {
            float4 screenPos;
            float3 worldRefl;
        };

        half _Glossiness;
        half _RefPow;
        fixed4 _Color;

        half4 LightingGem(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 h = normalize(viewDir + lightDir);
            half nh = max(0, dot(normalize(s.Normal), h));
            half spec = pow(nh, (pow(s.Specular, 2) * 256.0) + 0.1) * s.Specular * 4;

            half4 c;
            c.rgb = spec * _LightColor0 * atten;
            c.a = s.Alpha;
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            half2 offset = o.Normal * _RefPow * _Refraction_TexelSize.xy;
            IN.screenPos.xy /= IN.screenPos.w;
            IN.screenPos.xy = IN.screenPos.z * offset + IN.screenPos.xy;
            o.Emission = (tex2D(_Refraction, IN.screenPos.xy) + DecodeHDR(UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldRefl), unity_SpecCube0_HDR)) * 0.5 * _Color;

            o.Specular = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
        FallBack "Diffuse"
}