sampler uImage0 : register(s0);
texture2D noiseTex;
sampler2D noiseTexture = sampler_state
{
    Texture = <noiseTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float uTime;
float Strength;
// 定义噪声采样函数
float Noise(in float2 uv, float time)
{
    return tex2D(noiseTexture, uv + float2(0.0, time * 0.1)).r;
}

// PS入口
float4 PSFunction(float2 uv : TEXCOORD) : COLOR0
{
    float2 distortedUV = uv + Noise(uv, uTime) * Strength; // 这里乘以一个小的系数以减小扰动
    float4 color = tex2D(uImage0, distortedUV);
    return color;
}
technique Technique1
{
    pass WaveEffect
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}