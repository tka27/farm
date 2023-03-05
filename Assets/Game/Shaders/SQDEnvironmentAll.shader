Shader "SQD/EnvAll"
{
    Properties
    {
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _MainColor("Color", Color) = (1,1,1,1)
        [HideInInspector] _HColor("Highlight Color", Color) = (1,1,1,1)
        [HideInInspector] _ShadowColor("Shadow Color", Color) = (0.35,0.4,0.45,1.0)

        [HideInInspector] _RampThreshold("Ramp Threshold", Range(0, 1)) = 0.8
        [HideInInspector] _RampSmooth("Ramp Smoothing", Range(0, 1)) = 1

        [HideInInspector] [Toggle(_EmissionColor_ON)] _EmissionColorOff("Emission", Float) = 0
        [HideInInspector] [HDR] _EmissionColor("Color", Color) = (0,0,0)

        [HideInInspector] [Toggle(_RIM_ON)] _RIMoff ("RIM", Float) = 0
        [HideInInspector] _RimColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _RimPower("Power", Range(0.1, 10)) = 1
        [HideInInspector] _RimExponent("Exponent", Range(0.1, 10)) = 1
        [HideInInspector] _RimSaturation("Saturation", Range(0, 5)) = 1
        [HideInInspector] _RimSwitcher("Switcher", Range(-1, 1 )) = -1

        [HideInInspector] [Toggle(_RECEIVE_SHADOWS_ON)] _ReceiveShadowsOff("Receive Shadows", Float) = 1

    }

    CustomEditor "SQDEnvironmentAllShaderGUI"

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"

            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma shader_feature_local _ _RECEIVE_SHADOWS_ON
            #pragma shader_feature_local _ _EmissionColor_ON
            #pragma shader_feature_local _ _RIM_ON
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ShadowColor;
            float4 _HColor, _MainColor;
            float _RampThreshold, _RampSmooth;
            float4 _EmissionColor;
            float4 _RimColor;
            float _RimPower;
            float _RimExponent;
            float _RimSaturation;
            float _RimSwitcher;

            CBUFFER_END

            struct Attributes
            {
                float3 normalOS : NORMAL;
                float4 positionOS : POSITION;
                float4 positionCS : POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD3;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            //v2f
            struct Varyings
            {
                float3 normalWS : NORMAL;
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 oPosN : TEXCOORD4;
                float3 positionObj : TEXCOORD5;
                float3 positionOS : TEXCOORD6;
                #if _RECEIVE_SHADOWS_ON
                float4 shadowCoord              : TEXCOORD8; // compute shadow coord per-vertex for the main light
                #endif


                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };


            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionOS = input.positionOS.xyz;
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                //
                float3 camPOS = _WorldSpaceCameraPos.xyz;
                output.viewDir = normalize(camPOS - output.positionWS);


                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 color = half4(1, 1, 1, 1);
                half4 diffuseLight = half4(1, 1, 1, 1);

                half4 MainTex = tex2D(_MainTex, input.uv) * (_MainColor * 1.5);

                VertexPositionInputs vertexInput = (VertexPositionInputs)0;
                vertexInput.positionWS = input.positionWS;

                float3 normal = normalize(input.normalWS);
                #if _RECEIVE_SHADOWS_ON
                float4 shadowCoord = GetShadowCoord(vertexInput);
                half shadowAttenutation = MainLightRealtimeShadow(shadowCoord);
                #endif
                float3 L = _MainLightPosition;
                half nl = dot(normal, L) * 0.5 + 0.5;
                float ramp = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, nl);
                #if _RECEIVE_SHADOWS_ON
                ramp *= shadowAttenutation;
                #endif
                diffuseLight = lerp(_ShadowColor, _HColor, ramp);

                color = MainTex * diffuseLight;


                // rim
                #if _RIM_ON
                float3 NormalDotViewDir = 0.5 * (1 - _RimSwitcher) + _RimSwitcher * saturate(dot(normal, input.viewDir));

                float3 ColorRim = saturate(pow(NormalDotViewDir, _RimPower)*_RimExponent);
                ColorRim = ColorRim * _RimColor.rgb * _RimSaturation;
                #endif

                //---------------------Fog--------------------//

                color.rgb = MixFogColor(color.rgb, unity_FogColor, input.fogCoord);

                #if _EmissionColor_ON
                color += _EmissionColor;
                #endif

                // rim 
                #if _RIM_ON
                color += float4 (ColorRim, 0);
                #endif

                return color;
            }
            ENDHLSL
        }

        Pass
        {


            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #pragma multi_compile_fog
            #pragma enable_cbuffer


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;

                #if _ALPHATEST_ON
                float2 uv     : TEXCOORD0;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                #if _ALPHATEST_ON
                float2 uv     : TEXCOORD0;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput ShadowPassVertex(VertexInput v)
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normal.xyz);

                float4 positionCS =
                    TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));

                o.vertex = positionCS;


                return o;
            }

            half4 ShadowPassFragment(VertexOutput i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                #if _ALPHATEST_ON
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);;
                clip(col.a - _Alpha);
                #endif

                return 0;
            }
            ENDHLSL

        }

    }
}