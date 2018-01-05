Shader "Custom/RimLight" {
	Properties {
	  _MainTex ("Texture", 2D) = "white" {}
	  _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _RimCutoff ("Rim cutoff threshold", Range(0.0, 1.0)) = 0.0
	}
	SubShader {
	  Tags { "RenderType" = "Opaque" }
	  CGPROGRAM
	  #pragma surface surf Lambert
	  struct Input {
	      float2 uv_MainTex;
	      float2 uv_BumpMap;
          float3 viewDir;
	  };

	  sampler2D _MainTex;
	  float4 _RimColor;
      float _RimPower;
      float _RimCutoff;
	  void surf (Input IN, inout SurfaceOutput o) {
	      o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
	      half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
	      if(rim > _RimCutoff) {
	      	o.Emission = _RimColor.rgb * pow (rim, _RimPower);
	      } else {
			o.Emission = _RimColor.rgb * 0.0;
	      }
	  }
	  ENDCG
	} 
	Fallback "Diffuse"
}
