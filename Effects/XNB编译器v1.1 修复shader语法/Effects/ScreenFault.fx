sampler uImage0 : register(s0);

//_Params.x
float Speed;

//_Params.y
float BlockSize;
float iTime;


float randomNoise(float2 seed)
{
    return frac(sin(dot(seed * floor(iTime * Speed), float2(17.13, 3.71))) * 43758.5453123);
}

float4 Frag(float2 coords : TEXCOORD0) : COLOR0
{

    //float2 block = randomNoise(floor(i.texcoord * BlockSize));
    //float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);

    //float ColorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).r;
    //float ColorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(displaceNoise * 0.05 * randomNoise(7.0), 0.0)).g;
    //float ColorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - float2(displaceNoise * 0.05 * randomNoise(13.0), 0.0)).b;
    
    float2 block = randomNoise(floor(coords * BlockSize));
    float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
    
    float ColorR = tex2D(uImage0, coords).r;
    
    float ColorG = tex2D(uImage0, coords + float2(displaceNoise * 0.05 * randomNoise(7.0), 0.0)).g;
    float ColorB = tex2D(uImage0, coords - float2(displaceNoise * 0.05 * randomNoise(13.0), 0.0)).b;

    return float4(ColorR, ColorG, ColorB, 1.0);
}
technique Technique1
{
    pass ScreenFault
    {
        PixelShader = compile ps_2_0 Frag();
    }
}