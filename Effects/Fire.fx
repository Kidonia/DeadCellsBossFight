// 定义常量缓冲区，用于传递参数到着色器

float aspectRatio; // 屏幕分辨率比例
float iTime; // 时间
float fireWidth;
float fireStrength; // 火势大小
float fireStrength2; // 火势大小2
float fireLight; // 火焰亮度
float sunken;
float broken;
float twist;
float disappear;

float noise(float3 p)
{
    float3 i = floor(p);
    float4 a = dot(i, float3(1.0, 57.0, 21.0)) + float4(0.0, 57.0, 21.0, 78.0);
    float3 f = cos((p - i) * 3.14159265358979323846) * (-0.5) + 0.5;
    a = lerp(sin(cos(a) * a), sin(cos(1.0 + a) * (1.0 + a)), f.x);
    a.xy = lerp(a.xz, a.yw, f.y);
    return lerp(a.x, a.y, f.z);
}

float sphere(float3 p, float4 spr)
{
    return length(spr.xyz - p) - spr.w;
}

// 定义函数 flame
float flame(float3 p)
{
    //float3(
    //火焰宽度，越大越细；
    //火势大小，建议2到0.5，由圆润（宽）到正好旺起来（窄）；
    //火焰亮度，越大越暗，建议1.2到1
    //)
    
    //float4(
    //位置水平偏移，中间为0，越大越右；
    //表现火焰的哪一部分，越小越上，建议熄灭时-0.9到-4快速过
    //火焰大小，越大越大，建议0
    //还是火势大小，越大看起来越像火焰，建议搭配上面的，从0.7到1
    //)
    float d = sphere(p * float3(fireWidth, fireStrength, fireLight), float4(0.0, disappear, 0.0, fireStrength2));
    
    //iTime*火焰动频，越大越快
    //p*k1)*k2)*k3
    //k1：火焰中的凹纹效果，越大纹越多，建议2.9到3.2间变换
    //k2：火焰碎裂效果，有点差强人意，建议0.3到0.5间变换
    //k3：火焰扭曲效果，同样差强人意，建议0.24到0.32间变换
    return d + (noise(p + float3(0.0, iTime * 3.6, 0.0)) + noise(p * 3.0) * 0.3) * 0.25 * (p.y);

}
// 定义场景函数
float scene(float3 p)
{
    return min(100.0 - length(p), abs(flame(p)));
}

// 定义函数 raymarch
float4 raymarch(float3 org, float3 dir)
{
    float d = 0.0;
    float glow = 0.0;
    float eps = 0.02;
    float3 p = org;
    bool glowed = false;
    
    for (int i = 0; i < 64; i++)
    {
        d = scene(p) + eps;
        p += d * dir;
        if (d > eps)
        {
            if (flame(p) < 0.0)
                glowed = true;
            if (glowed)
                glow = float(i) / 64.0;
        }
    }
    return float4(p, glow);
}

// 定义像素着色器

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    if (coords.x < 0.25 || coords.x > 0.75)
    {
        return float4(0.0, 0.0, 0.0, 1.0);
    }
    
    float2 v = -1.0 + 2.0 * coords;
    // v.x *= iResolution.x / iResolution.y;
    v.x *= aspectRatio;
    
    float3 org = float3(0.0, -2.0, 3.0);
    float3 dir = normalize(float3(v.x * 1.6, -v.y * 1.4, -1.6));
    
    float4 p = raymarch(org, dir);
    float glow = p.w;
    float4 col = lerp(float4(1.0, 0.2, 0.2, 1.0), float4(0.9, 0.3, 1.0, 1.0), p.y * 0.02 + 0.4);
    float4 finalColor = lerp(float4(0.0, 0.0, 0.0, 1.0), col, pow(glow * 2.0, 4.0));
    
    return finalColor;
}

// 设置顶点着色器和像素着色器入口点
technique Technique1
{
    pass Fire
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
