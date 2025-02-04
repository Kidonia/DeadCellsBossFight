sampler uImage0 : register(s0);

float strength;
float t;

float4 PixelShaderFunction(float2 u : TEXCOORD0) : COLOR0
{
    u += float2(sin(t * 0.4), sin(t * 0.6)) * 0.03;

    u.x = (u.x - 0.5) * (1.0 + sin(t * 0.4) * 0.15) + 0.5;
    u.y = (u.y - 0.5) * (1.0 + sin(t * 0.1) * 0.15) + 0.5;

    u.x += sin(u.y * 3.0 + t) * strength;
    u.y += cos(u.x * 4.5 + t) * strength;

    return tex2D(uImage0, u);
}

// ���ö�����ɫ����������ɫ����ڵ�
technique Technique1
{
    pass Drug
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}