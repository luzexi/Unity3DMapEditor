Shader "Projector/AlphaBlend" {
   Properties {
   	  _Color ("Main Color", Color) = (1,1,1,1)  
      _ProjTex ("Texture", 2D) = "gray" { TexGen ObjectLinear }
   }

   Subshader {
      Tags { "RenderType"="Transparent-1" }
      Pass {
         ZWrite Off
         Color [_Color]
         ColorMask RGB
         Blend srcalpha Oneminussrcalpha
		 Offset -1, -1
         SetTexture [_ProjTex] {
            combine texture * primary
            Matrix [_Projector]
         }
      }
   }
}