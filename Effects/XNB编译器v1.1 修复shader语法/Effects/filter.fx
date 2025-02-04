sampler uImage0 : register(s0);
float3 filterRGB;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
        // »Ò¶È = r*0.3 + g*0.59 + b*0.11
    float gs = dot(float3(0.3, 0.59, 0.11), color.rgb);
    
    return float4(gs + filterRGB.r, gs + filterRGB.g, gs + filterRGB.b, color.a);
}
technique Technique1
{
    pass Filter
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}