sampler uImage0 : register(s0);

float4 Input1RGBA;
float4 Input2RGBA;

//  ������һ����ɫ�ģ�R����shader������ֻ�Ե��������Ʒ��
float4 UI1Color(float2 texCoord : TEXCOORD0) : COLOR0
{
    float R = tex2D(uImage0, texCoord).r;
    return float4(Input1RGBA * R);
}
float4 UI2Color(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 RG = tex2D(uImage0, texCoord).rg;
    return float4(Input1RGBA * RG.r  + Input2RGBA * RG.g);
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