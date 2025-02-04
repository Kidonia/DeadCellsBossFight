sampler uImage0 : register(s0);//color
sampler uImage1 : register(s1);//shape(Laser)
//sampler uImage2 : register(s2);//mask
//sampler uImage3 : register(s3);
//float3 uColor;
//float3 uSecondaryColor;
//float2 uScreenResolution;
//float2 uScreenPosition;
//float2 uTargetPosition;
//float2 uDirection;
//float uOpacity;
//float uTime;
//float uIntensity;
//float uProgress;
//float2 uImageSize1;
//float2 uImageSize2;
//float2 uImageSize3;
//float2 uImageOffset;
//float uSaturation;
//float4 uSourceRect;
//float2 uZoom;

//float4x4 uTransform;

//var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
//var model = Matrix.CreateTranslation(new Vector3(-Main. screenPosition.X, -Main. screenPosition.Y, 0));
//Ni.TestEffect.Parameters["uTransform"].SetValue(model * projection);//免除减去屏幕坐标的烦恼

float4 main(float2 texCoord : TEXCOORD) : COLOR
{
    float grayValue = tex2D(uImage0, texCoord).r;
    float4 colorValue = tex2D(uImage1, float2(grayValue, 0));
    return colorValue;
}

technique Technique1
{
    pass MyPass
    {
        PixelShader = compile ps_2_0 main();
    }

}