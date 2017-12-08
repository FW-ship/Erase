Shader "Sprites/Custom"
 {
Properties {
    _MainTex ("Multiply Leak", 2D) = "white" {}
}

Category {
    Tags { "Queue" = "Transparent+1" }
    Blend Zero SrcColor
    Lighting Off ZWrite Off Fog { Color (1,1,1,1) }
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }

    SubShader {
        Pass {
            SetTexture [_MainTex] {
                constantColor (1,1,1,1)
                combine texture lerp(texture) constant
            }
        }
    }
}
}