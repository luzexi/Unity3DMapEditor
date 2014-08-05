Shader "Unlit/OccluderEffect" {

	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white"
		_OccluderTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Occluder("Occluder", Range(0,1)) = 0	
	}

	SubShader {
			Tags {"RenderType"="Opaque"}
			LOD 200
		     	Cull Off
		     	//Blend SrcAlpha OneMinusSrcAlpha 
			  CGPROGRAM
		      #pragma surface surf Lambert 
		      struct Input {
		          float2 uv_MainTex;
		          float2 uv_OccluderTex;
		      };
		      sampler2D _MainTex;
		      sampler2D _OccluderTex;
		      float     _Occluder;
		      void surf (Input IN, inout SurfaceOutput o) 
		      {
		          float4 texColor = tex2D (_MainTex, IN.uv_MainTex);
		          float4 occluderColor = tex2D (_OccluderTex, IN.uv_OccluderTex);
		          o.Albedo = 0;
		          if(_Occluder > 0.5)
		          {
		          	o.Emission = (1-occluderColor.a)*texColor.rgb + occluderColor.rgb*occluderColor.a;
		          }
		          else
		          {
		        	o.Emission = texColor.rgb;
		          }
		          o.Alpha = 1;
		      }
		      ENDCG
	   }
	   Fallback "Unlit/TransparentColorBlend"
}