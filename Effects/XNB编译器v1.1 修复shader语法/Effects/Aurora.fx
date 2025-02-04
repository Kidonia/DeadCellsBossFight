sampler uImage0 : register(s0);
float uTime;
float _SurAuroraColFactor = 0.5;

float2x2 RotateMatrix(float a)
{
    float c = cos(a);
    float s = sin(a);
    return float2x2(c, s, -s, c);
}
float tri(float x)
{
    return clamp(abs(frac(x) - 0.5), 0.01, 0.49);
}
float2 tri2(float2 p)
{
    return float2(tri(p.x) + tri(p.y), tri(p.y + tri(p.x)));
} 
// 极光噪声
float SurAuroraNoise(float2 pos)
{
    float intensity = 1.8;
    float size = 2.5;
    float rz = 0;
    pos = mul(RotateMatrix(pos.x * 0.06), pos);
    float2 bp = pos;
    for (int i = 0; i < 5; i++)
    {
        float2 dg = tri2(bp * 1.85) * .75;
        dg = mul(RotateMatrix(uTime), dg);
        pos -= dg / size;

        bp *= 1.3;
        size *= .45;
        intensity *= .42;
        pos *= 1.21 + (rz - 1.0) * .02;

        rz += tri(pos.x + tri(pos.y)) * intensity;
        pos = mul(-float2x2(0.95534, 0.29552, -0.29552, 0.95534), pos);
    }
    return clamp(1.0 / pow(rz * 29., 1.3), 0, 0.55);
}


float SurHash(float2 n)
{
    return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
}
float4 SurAurora(float2 pos, float2 ro)
{
    float4 col = float4(0, 0, 0, 0);
    float4 avgCol = float4(0, 0, 0, 0);

        // 逐层
    for (int i = 0; i < 30; i++)
    {
            // 坐标
        float of = 0.006 * SurHash(pos.xy) * smoothstep(0, 15, i);
        float pt = ((0.8 + pow(i, 1.4) * 0.002) - ro.y) / (pos.y * 2.0 + 0.8);
        pt -= of;
        float2 bpos = ro + pt * pos;
        float2 p = bpos.yx;

            // 颜色
        float noise = SurAuroraNoise(p);
        float4 col2 = float4(0, 0, 0, noise);
        col2.rgb = (sin(1.0 - float3(2.15, -.5, 1.2) + i * _SurAuroraColFactor * 0.1) * 0.8 + 0.5) * noise;
        avgCol = lerp(avgCol, col2, 0.5);
        col += avgCol * exp2(-i * 0.065 - 2.5) * smoothstep(0., 5., i);
        col *= clamp(pos.y * 15. + .4, 0., 1.);

        return col * 1.8;
    }
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 surAuroraCol = smoothstep(0.0, 1.5, SurAurora(
                                                    float2(coords.x, coords.y),
                                                    float2(0, -6.7)
                                                    ));
   // float4 color = tex2D(uImage0, coords);
    return surAuroraCol;
}
technique Technique1
{
	pass Aurora
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
   
}