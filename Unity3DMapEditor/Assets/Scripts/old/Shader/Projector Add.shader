Shader "Projector/Add" {
   Properties {
   	  _Color ("Main Color", Color) = (1,1,1,1)  
      _ProjTex ("Texture", 2D) = "gray" { TexGen ObjectLinear }
   }

   Subshader {
      Tags { "RenderType"="Transparent-1" }
      Pass {
         ZWrite Off
         Fog { Color (0, 0, 0) }
         Color [_Color]
         ColorMask RGB
         Blend One One
		 Offset -1, -1
         SetTexture [_ProjTex] {
            combine texture * primary
            Matrix [_Projector]
         }
      }
   }
}