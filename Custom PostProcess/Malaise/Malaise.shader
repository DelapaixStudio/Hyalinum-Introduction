Shader "Hidden/Custom/Malaise"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Blend;
        float _Distortion;
		sampler2D _TextureNoise;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
		  float2 uv = i.texcoord;
		  float2 offset = float2(0, tex2D(_TextureNoise, float2( _Time[_Distortion] , uv.y)).r);
		  uv += offset / 2;

		  float4 color = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) );
		  




            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);

            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}