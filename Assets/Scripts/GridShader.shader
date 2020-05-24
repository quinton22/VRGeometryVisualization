Shader "Custom/GridShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        [NoScaleOffset] 
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        [NoScaleOffset]
        _GridTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Color", Color) = (0,0,0,0)
        [NoScaleOffset]
        _EmissionMap ("Emission", 2D) = "white" {}
        _GridScale ("Grid Scale", float) = 1
        _GridLineThickness ("Grid Line Thickness", float) = 0.01
        [KeywordEnum(Line, Area, Volume, Mesh)]
        _ShapeType ("Shape Type", int) = 0
        [HideInInspector]
        _Scale ("Scale", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _GridTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 objectPos;
            //float3 localPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _MainColor;
        fixed4 _GridColor;
        bool _EmissionEnabled;
        fixed4 _EmissionColor;
        sampler2D _EmissionMap;
        float _GridScale;
        float _GridLineThickness;
        int _ShapeType;
        float3 _Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void computeGrid (float objPos, float scale, float scaleAdjustment, inout SurfaceOutputStandard o, bool isVolume=false)
        {
            // TODO: there is likely a better way of doing this
            float adjustedGridLineThickness = _GridLineThickness / 2;
            int total = floor(scale * scaleAdjustment * _GridScale) + 1;
            if (isVolume && scale * scaleAdjustment * _GridScale == total - 1) total -= 1; 
            float start;
            for (int i = isVolume ? 1 : 0; i < total; i++)
            {
                start = (objPos + 0.5 * scaleAdjustment) * scale - 1.0 / _GridScale * i;
                if (start <= adjustedGridLineThickness && start >= -adjustedGridLineThickness)
                {
                    o.Albedo = _GridColor.rgb;
                    return;
                }
            }
        }

        void lineGrid (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _MainColor.rgb;

            computeGrid(IN.objectPos.y, _Scale.y, 2, o);
        }

        void areaGrid (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _MainColor.rgb;

            computeGrid(IN.objectPos.y, _Scale.y, 1, o);
            computeGrid(IN.objectPos.x, _Scale.x, 1, o);
        }

        void volumeGrid (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _MainColor.rgb;

            computeGrid(IN.objectPos.y, _Scale.y, 1, o, true);
            computeGrid(IN.objectPos.x, _Scale.x, 1, o, true);
            computeGrid(IN.objectPos.z, _Scale.z, 1, o, true);
        }

        void meshGrid (Input IN, inout SurfaceOutputStandard o)
        {

        }

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.objectPos = v.vertex; // mul(unity_WorldToObject, v.vertex).xyz;
            // o.localPos = mul(unity_WorldToObject, v.vertex).xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _MainColor;
            //o.Albedo = c.rgb;
            if (_EmissionEnabled)
            {
                o.Emission = (tex2D (_EmissionMap, IN.uv_MainTex) * _EmissionColor).rgb;
            }
            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0,0,0,1));
            
            if (_ShapeType == 0)
                lineGrid(IN, o);
            else if (_ShapeType == 1)
                areaGrid(IN, o);
            else if (_ShapeType == 2)
                volumeGrid(IN, o);
            else if (_ShapeType == 3)
                meshGrid(IN, o);

            
            // // Metallic and smoothness come from slider variables
            // o.Metallic = _Metallic;
            // o.Smoothness = _Glossiness;
            // o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
