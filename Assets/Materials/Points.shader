Shader "Point Cloud/Point"
{
    Properties
    {
        _PointSize("Point Size", Float) = 0.05
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
        }
        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                half4 color : COLOR;
            };

            struct Point
            {
                float4 position : SV_Position;
                half4 color : COLOR;
                half size : PSIZE;
            };

            half4 _Tint;
            float4x4 _Transform;
            half _PointSize;

            Point Vertex(Attributes input)
            {
                Point o;
                o.position = UnityObjectToClipPos(input.position);
                o.color = input.color;
                o.size = _PointSize;
                return o;
            }

            half4 Fragment(Point input) : SV_Target
            {
                return input.color;
            }

            ENDCG
        }
    }
    CustomEditor "Pcx.PointMaterialInspector"
}