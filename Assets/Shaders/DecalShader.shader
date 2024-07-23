Shader "Custom/DecalShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DecalTex ("Decal (RGB)", 2D) = "white" {}
        _DecalPos ("Decal Position", Vector) = (0,0,0,0)
        _DecalSize ("Decal Size", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DecalTex;
            float4 _DecalPos;
            float4 _DecalSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Calculate decal UVs
                float2 decalUV = (i.uv - _DecalPos.xy) / _DecalSize.xy;
                if(decalUV.x >= 0 && decalUV.x <= 1 && decalUV.y >= 0 && decalUV.y <= 1)
                {
                    half4 decalCol = tex2D(_DecalTex, decalUV);
                    col = lerp(col, decalCol, decalCol.a);
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
