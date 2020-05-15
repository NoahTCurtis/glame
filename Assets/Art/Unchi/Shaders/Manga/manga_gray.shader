// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unchi/Manga/Gray" {

  // version 1.0.0

  Properties {
    _InkSize ("Ink Size", Float) = 0.1
    _InkColor ("Ink Color", Color) = (0,0,0,1)
    _ColorBase ("Diffuse", 2D) = "white" {}
    _ToneSize ("Tone Size", Float) = 3.0
    _ToneBlend ("Tone Blend", Range(3, 80)) = 30
    _Tone0Sampler ("Tone0", 2D) = "black" {}
    _Tone1Sampler ("Tone1", 2D) = "gray" {}
    _Tone2Sampler ("Tone2", 2D) = "gray" {}
    _Tone3Sampler ("Tone3", 2D) = "white" {}
  }

  SubShader {

    Tags {
      "Queue"="Geometry"
      "LightMode" = "ForwardBase"
    }

    // Outline pass
    Pass {
      Cull Front
      ZTest Less

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

#include "_edge_color.cginc"

ENDCG
    }

    // Main pass
    Pass {
      Cull Back
      ZTest LEqual

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers flash
#include "UnityCG.cginc"

      // Editable parameters
      
      uniform fixed _ToneSize;
      uniform fixed _ToneBlend;
      uniform fixed4 _ColorBase_ST;
      uniform sampler2D _ColorBase;
      uniform sampler2D _Tone0Sampler;
      uniform sampler2D _Tone1Sampler;
      uniform sampler2D _Tone2Sampler;
      uniform sampler2D _Tone3Sampler;
      
      // Vertex shader
      struct VS {
        half4 Pos: POSITION;
        half2 ScreenPos: TEXCOORD1;
        fixed2 UV: TEXCOORD2;
        fixed Shadow: TEXCOORD3;
      };

      // Light vector
      inline fixed3 getLightVector (fixed4 pos) {
        return normalize(_WorldSpaceLightPos0.xyz);
      }

      // Vertex shader
      VS vert (appdata_base i) {

        fixed3 world_pos = mul (unity_ObjectToWorld, i.vertex).xyz;
        fixed3 world_norml = normalize (mul (unity_ObjectToWorld, fixed4(i.normal, 0)).xyz);
        fixed3 world_light = getLightVector (i.vertex);
        fixed3 world_eye = normalize (WorldSpaceViewDir (i.vertex));

        fixed shadow = (dot (world_norml, world_light) + 1.0f) * 0.45f + 0.05f;
        // (0 -> 1) * 0.9 + 0.5

        VS o;
        o.Pos = UnityObjectToClipPos(i.vertex);

        fixed x = o.Pos.x / o.Pos.w;
        fixed y = o.Pos.y / o.Pos.w;
        
        o.ScreenPos.x = _ToneSize * x * _ScreenParams.x / _ScreenParams.y;
        o.ScreenPos.y = _ToneSize * y;
        o.UV = TRANSFORM_TEX(i.texcoord, _ColorBase);
        o.Shadow = shadow;

        return o;
      }

      // Fragment shader
      fixed4 frag (VS i): COLOR {
 
        fixed3 c = tex2D (_ColorBase, i.UV).rgb;
        fixed m = (0.299 * c.r + 0.587 * c.g + 0.114 * c.b) * i.Shadow;
        fixed2 pos = i.ScreenPos;

        fixed o = 0;

        if (m > 0.85) {
          fixed f = clamp((m - 0.85) * _ToneBlend, 0, 1);
          o = f + tex2D (_Tone3Sampler, pos).r * (1 - f);
        } else
        if (m > 0.60) {
          fixed f = clamp((m - 0.60) * _ToneBlend, 0, 1);
          o = tex2D (_Tone3Sampler, pos).r * f + tex2D (_Tone2Sampler, pos).r * (1 - f);
        } else
        if (m > 0.35) {
          fixed f = clamp((m - 0.35) * _ToneBlend, 0, 1);
          o = tex2D (_Tone2Sampler, pos).r * f + tex2D (_Tone1Sampler, pos).r * (1 - f);
        } else
        if (m > 0.10) {
          fixed f = clamp((m - 0.10) * _ToneBlend, 0, 1);
          o = tex2D (_Tone1Sampler, pos).r * f + tex2D (_Tone0Sampler, pos).r * (1 - f);
        } else {
          o = tex2D (_Tone0Sampler, pos).r;
        }

        return fixed4 (o.xxx, 1);
      }
ENDCG
    }
  }

  FallBack "VertexLit"
}
