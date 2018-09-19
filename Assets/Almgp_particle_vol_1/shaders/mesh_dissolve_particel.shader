// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:34555,y:32458,varname:node_4013,prsc:2|emission-7275-OUT,clip-114-OUT,voffset-3938-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:33006,y:32734,ptovrint:False,ptlb:Freshnel color,ptin:_Freshnelcolor,varname:_Freshnelcolor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9485294,c2:0.7453555,c3:0.6137543,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7751,x:32165,y:33061,varname:_Noisetexture,prsc:2,tex:597854b766d306b4e8742ebf0af48649,ntxv:0,isnm:False|UVIN-6189-UVOUT,TEX-6886-TEX;n:type:ShaderForge.SFN_Tex2d,id:3482,x:32791,y:32491,ptovrint:False,ptlb:gradient,ptin:_gradient,varname:_gradient,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d7807d7e688002044bdc25924014c7d0,ntxv:0,isnm:False|UVIN-4503-OUT;n:type:ShaderForge.SFN_Append,id:4659,x:32496,y:32746,varname:node_4659,prsc:2|A-2420-OUT,B-2420-OUT;n:type:ShaderForge.SFN_Slider,id:3310,x:32751,y:33272,ptovrint:False,ptlb:power,ptin:_power,varname:_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6633624,max:1;n:type:ShaderForge.SFN_Vector1,id:9544,x:32597,y:33042,varname:node_9544,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:5621,x:32612,y:33213,varname:node_5621,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6379,x:33186,y:32380,varname:node_6379,prsc:2|A-1304-RGB,B-6268-OUT,C-2420-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6886,x:31688,y:33175,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:597854b766d306b4e8742ebf0af48649,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:8218,x:31595,y:33451,varname:node_8218,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8776,x:31919,y:33397,varname:node_8776,prsc:2|A-8218-UVOUT,B-7403-OUT;n:type:ShaderForge.SFN_Vector1,id:7403,x:31805,y:33604,varname:node_7403,prsc:2,v1:0.33;n:type:ShaderForge.SFN_Tex2d,id:2236,x:32165,y:33240,varname:_node_2236,prsc:2,tex:597854b766d306b4e8742ebf0af48649,ntxv:0,isnm:False|UVIN-663-UVOUT,TEX-6886-TEX;n:type:ShaderForge.SFN_Blend,id:2420,x:32393,y:33133,varname:node_2420,prsc:2,blmd:10,clmp:True|SRC-7751-R,DST-2236-R;n:type:ShaderForge.SFN_Panner,id:6189,x:31923,y:32926,varname:node_6189,prsc:2,spu:0.1,spv:0.15|UVIN-8218-UVOUT;n:type:ShaderForge.SFN_Panner,id:663,x:32208,y:33445,varname:node_663,prsc:2,spu:-0.05,spv:0.03|UVIN-8776-OUT;n:type:ShaderForge.SFN_RemapRange,id:4503,x:32603,y:32552,varname:node_4503,prsc:2,frmn:0.2,frmx:0.9,tomn:0,tomx:1|IN-4659-OUT;n:type:ShaderForge.SFN_VertexColor,id:1726,x:32885,y:33488,varname:node_1726,prsc:2;n:type:ShaderForge.SFN_Blend,id:114,x:33422,y:32940,varname:node_114,prsc:2,blmd:16,clmp:True|SRC-2420-OUT,DST-30-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:30,x:33186,y:33081,ptovrint:False,ptlb:UseVertexAlpha,ptin:_UseVertexAlpha,varname:_UseVertexAlpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-3310-OUT,B-1726-A;n:type:ShaderForge.SFN_Fresnel,id:6268,x:33525,y:32182,varname:node_6268,prsc:2|NRM-2142-OUT,EXP-2369-OUT;n:type:ShaderForge.SFN_NormalVector,id:2142,x:33085,y:32109,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2369,x:33059,y:32180,ptovrint:False,ptlb:exponent,ptin:_exponent,varname:_exponent,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:0.9618391,max:3;n:type:ShaderForge.SFN_Add,id:3918,x:33430,y:32540,varname:node_3918,prsc:2|A-6379-OUT,B-3482-RGB;n:type:ShaderForge.SFN_Multiply,id:7275,x:33818,y:32629,varname:node_7275,prsc:2|A-3918-OUT,B-114-OUT,C-3828-OUT;n:type:ShaderForge.SFN_Slider,id:3828,x:33799,y:32485,ptovrint:False,ptlb:emmis_power,ptin:_emmis_power,varname:_emmis_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:50;n:type:ShaderForge.SFN_NormalVector,id:8762,x:33909,y:32903,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:3938,x:34173,y:32707,varname:node_3938,prsc:2|A-8762-OUT,B-2420-OUT,C-252-OUT;n:type:ShaderForge.SFN_Vector1,id:252,x:34138,y:33035,varname:node_252,prsc:2,v1:0.3;proporder:1304-3482-3310-6886-30-2369-3828;pass:END;sub:END;*/

Shader "Almgp/vfx1/mesh_dissolve_particel" {
    Properties {
        _Freshnelcolor ("Freshnel color", Color) = (0.9485294,0.7453555,0.6137543,1)
        _gradient ("gradient", 2D) = "white" {}
        _power ("power", Range(0, 1)) = 0.6633624
        _Noise ("Noise", 2D) = "white" {}
        [MaterialToggle] _UseVertexAlpha ("UseVertexAlpha", Float ) = 0
        _exponent ("exponent", Range(0.5, 3)) = 0.9618391
        _emmis_power ("emmis_power", Range(1, 50)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Freshnelcolor;
            uniform sampler2D _gradient; uniform float4 _gradient_ST;
            uniform float _power;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform fixed _UseVertexAlpha;
            uniform float _exponent;
            uniform float _emmis_power;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_150 = _Time + _TimeEditor;
                float2 node_6189 = (o.uv0+node_150.g*float2(0.1,0.15));
                float4 _Noisetexture = tex2Dlod(_Noise,float4(TRANSFORM_TEX(node_6189, _Noise),0.0,0));
                float2 node_663 = ((o.uv0*0.33)+node_150.g*float2(-0.05,0.03));
                float4 _node_2236 = tex2Dlod(_Noise,float4(TRANSFORM_TEX(node_663, _Noise),0.0,0));
                float node_2420 = saturate(( _node_2236.r > 0.5 ? (1.0-(1.0-2.0*(_node_2236.r-0.5))*(1.0-_Noisetexture.r)) : (2.0*_node_2236.r*_Noisetexture.r) ));
                v.vertex.xyz += (v.normal*node_2420*0.3);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_150 = _Time + _TimeEditor;
                float2 node_6189 = (i.uv0+node_150.g*float2(0.1,0.15));
                float4 _Noisetexture = tex2D(_Noise,TRANSFORM_TEX(node_6189, _Noise));
                float2 node_663 = ((i.uv0*0.33)+node_150.g*float2(-0.05,0.03));
                float4 _node_2236 = tex2D(_Noise,TRANSFORM_TEX(node_663, _Noise));
                float node_2420 = saturate(( _node_2236.r > 0.5 ? (1.0-(1.0-2.0*(_node_2236.r-0.5))*(1.0-_Noisetexture.r)) : (2.0*_node_2236.r*_Noisetexture.r) ));
                float node_114 = saturate(round( 0.5*(node_2420 + lerp( _power, i.vertexColor.a, _UseVertexAlpha ))));
                clip(node_114 - 0.5);
////// Lighting:
////// Emissive:
                float2 node_4503 = (float2(node_2420,node_2420)*1.428571+-0.2857143);
                float4 _gradient_var = tex2D(_gradient,TRANSFORM_TEX(node_4503, _gradient));
                float3 emissive = (((_Freshnelcolor.rgb*pow(1.0-max(0,dot(i.normalDir, viewDirection)),_exponent)*node_2420)+_gradient_var.rgb)*node_114*_emmis_power);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _power;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform fixed _UseVertexAlpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_3452 = _Time + _TimeEditor;
                float2 node_6189 = (o.uv0+node_3452.g*float2(0.1,0.15));
                float4 _Noisetexture = tex2Dlod(_Noise,float4(TRANSFORM_TEX(node_6189, _Noise),0.0,0));
                float2 node_663 = ((o.uv0*0.33)+node_3452.g*float2(-0.05,0.03));
                float4 _node_2236 = tex2Dlod(_Noise,float4(TRANSFORM_TEX(node_663, _Noise),0.0,0));
                float node_2420 = saturate(( _node_2236.r > 0.5 ? (1.0-(1.0-2.0*(_node_2236.r-0.5))*(1.0-_Noisetexture.r)) : (2.0*_node_2236.r*_Noisetexture.r) ));
                v.vertex.xyz += (v.normal*node_2420*0.3);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_3452 = _Time + _TimeEditor;
                float2 node_6189 = (i.uv0+node_3452.g*float2(0.1,0.15));
                float4 _Noisetexture = tex2D(_Noise,TRANSFORM_TEX(node_6189, _Noise));
                float2 node_663 = ((i.uv0*0.33)+node_3452.g*float2(-0.05,0.03));
                float4 _node_2236 = tex2D(_Noise,TRANSFORM_TEX(node_663, _Noise));
                float node_2420 = saturate(( _node_2236.r > 0.5 ? (1.0-(1.0-2.0*(_node_2236.r-0.5))*(1.0-_Noisetexture.r)) : (2.0*_node_2236.r*_Noisetexture.r) ));
                float node_114 = saturate(round( 0.5*(node_2420 + lerp( _power, i.vertexColor.a, _UseVertexAlpha ))));
                clip(node_114 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
