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
    // ��ȡ������ɫ
    float4 color = tex2D(uImage, coords);

    if (any(color)) {
        // �ж�������ɫ�Ƿ�Ϊ��ɫ
        if (MatchGlowKey(color))
        {
            // ����ɫ�滻ΪĿ����ɫ
            color = float4(InputR, InputG, InputB, 1.0);

            // ���㵱ǰ���ص��������صľ���
            float distance = length(coords - 0.5);

            // �������С�ڵ���24������
            if (distance <= 24.0 / 512.0)
            {
                // ���㵱ǰ���ص� alpha ֵ
                float alpha = 1.0 - (distance / (24.0 / 512.0));

                // �����µ���ɫ
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
