Shader "Hidden/DepthToColor"
{
    Properties
    {
		[NoScaleOffset] _MainTex ("Texture", 2D) = "black" {}
		_MinDepth ("Min Depth (mm)", Int) = 0
		_MaxDepth ("Max Depth (mm)", Int) = 5000
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			int _MinDepth;
			int _MaxDepth;

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_MainTex, i.uv).r;
				float2 depthRange = float2(_MinDepth, _MaxDepth) / 65535.;
				// Linear grayscale coloring along specified depth range, from white (near) to black (far)
				depth = clamp(depth, depthRange.x, depthRange.y);
				fixed c = (depthRange.y - depth) / (depthRange.y - depthRange.x);
				// Pixels of unknown depth are rendered transparent
				fixed a = depth == 0 ? 0 : 1;
				return fixed4(c, c, c, a);
            }
            ENDCG
        }
    }
}
