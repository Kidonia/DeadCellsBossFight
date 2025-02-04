sampler uImage0 : register(s0);

float InputR;
float InputG;
float InputB;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, texCoord);
    if (any(color))
    {
        color = float4(InputR, InputG, InputB, color.a);
    }
    return color;
}
technique Technique1
{
    pass Glow
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
    
    pass fx
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}