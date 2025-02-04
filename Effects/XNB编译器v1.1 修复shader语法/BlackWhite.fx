sampler2D uImage : register(s0);
float InputR;
float InputG;
float InputB;
float3 InputColor;


int MatchGlowKey(float4 inputcolor)
{
    return (inputcolor.rgb == float3(0, 0, 1));
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // 读取像素颜色
    float4 color = tex2D(uImage, coords);

    if (any(color)) {
        // 判断像素颜色是否为蓝色
        if (MatchGlowKey(color))
        {
            // 将蓝色替换为目标颜色
            color = float4(InputR, InputG, InputB, 1.0);

            // 计算当前像素到中心像素的距离
            float distance = length(coords - 0.5);

            // 如果距离小于等于24个像素
            if (distance <= 24.0 / 512.0)
            {
                // 计算当前像素的 alpha 值
                float alpha = 1.0 - (distance / (24.0 / 512.0));

                // 设置新的颜色
                color = float4(InputR, InputG, InputB, alpha);
            }
            return color;
        }
    }
    return color;
}

technique Technique1 {
    pass Test {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
