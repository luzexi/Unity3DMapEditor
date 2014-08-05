Shader "Character/TwoSideVertexLit" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("Main Color", Color) = (0.6,0.6,0.6,1)
	_Selected("Selected", Range(0,1)) = 0
	_Emission ("Emissive Color", Color) = (0.7,0.7,0.7,0)
	_SelectColor("Select Color", Color) = (0,0,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"RenderType"="Opaque"}
	LOD 200
     Cull Off
	 Alphatest Greater [_Cutoff]
	  CGPROGRAM
      #pragma surface surf Lambert 
      struct Input {
          float2 uv_MainTex;
          float3 worldNormal; 
          float3 viewDir;
      };
      sampler2D _MainTex;
      float4    _Color;
      float     _Selected;
      float4   _Emission;
      float4 _SelectColor;
      void surf (Input IN, inout SurfaceOutput o) 
      {
          float4 texColor = tex2D (_MainTex, IN.uv_MainTex);
          o.Albedo = texColor.rgb*_Color.rgb;
          float3 viewDir = normalize(IN.viewDir);
   		  float3 worldNormal = normalize(IN.worldNormal);
   		  float edge = 1-abs(dot(viewDir, worldNormal));
   		  edge = edge*edge;
          o.Emission = lerp( _Emission.rgb*texColor.rgb , _SelectColor.rgb ,(_Selected>0.5?edge:0)  );
          o.Alpha = texColor.a;
      }
      ENDCG
    }
	Fallback "Transparent/Cutout/VertexLit"

}