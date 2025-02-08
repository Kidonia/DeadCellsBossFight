sampler uImage0 : register(s0);

float3 Input1RGB;
float3 Input2RGB;

//  仅包含一种颜色的（R）的shader，处理只吃单卷轴的物品。
float4 UI1Color(float2 texCoord : TEXCOORD0) : COLOR0
{
    float R = tex2D(uImage0, texCoord).r;
    return float4(Input1RGB * R, 1);
}

float4 UI2Color(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 RG = tex2D(uImage0, texCoord).rg;
    return float4(Input1RGB * RG.r  + Input2RGB * RG.g, 1);
}
technique Technique1
{
    pass UI1Color
    {
        PixelShader = compile ps_2_0 UI1Color();
    }
    
    pass UI2Color
    {
        PixelShader = compile ps_2_0 UI2Color();
    }
}