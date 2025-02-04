// ���峣�������������ڴ��ݲ�������ɫ��

float aspectRatio; // ��Ļ�ֱ��ʱ���
float iTime; // ʱ��
float fireWidth;
float fireStrength; // ���ƴ�С
float fireStrength2; // ���ƴ�С2
float fireLight; // ��������
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

// ���庯�� flame
float flame(float3 p)
{
    //float3(
    //�����ȣ�Խ��Խϸ��
    //���ƴ�С������2��0.5����Բ�󣨿���������������խ����
    //�������ȣ�Խ��Խ��������1.2��1
    //)
    
    //float4(
    //λ��ˮƽƫ�ƣ��м�Ϊ0��Խ��Խ�ң�
    //���ֻ������һ���֣�ԽСԽ�ϣ�����Ϩ��ʱ-0.9��-4���ٹ�
    //�����С��Խ��Խ�󣬽���0
    //���ǻ��ƴ�С��Խ������Խ����棬�����������ģ���0.7��1
    //)
    float d = sphere(p * float3(fireWidth, fireStrength, fireLight), float4(0.0, disappear, 0.0, fireStrength2));
    
    //iTime*���涯Ƶ��Խ��Խ��
    //p*k1)*k2)*k3
    //k1�������еİ���Ч����Խ����Խ�࣬����2.9��3.2��任
    //k2����������Ч�����е��ǿ���⣬����0.3��0.5��任
    //k3������Ť��Ч����ͬ����ǿ���⣬����0.24��0.32��任
    return d + (noise(p + float3(0.0, iTime * 3.6, 0.0)) + noise(p * 3.0) * 0.3) * 0.25 * (p.y);

}
// ���峡������
float scene(float3 p)
{
    return min(100.0 - length(p), abs(flame(p)));
}

// ���庯�� raymarch
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

// ����������ɫ��

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

// ���ö�����ɫ����������ɫ����ڵ�
technique Technique1
{
    pass Fire
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
