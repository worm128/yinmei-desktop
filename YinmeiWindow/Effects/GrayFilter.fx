/// picture
sampler2D iChannel0 : register(S0);

//s ±•∫Õ∂»
//l ¡¡∂»
float4 ColorProcessing(float4 c, float h, float s, float l)
{
    float3x3 matrixH =
    {
        0.8164966f, 0, 0.5352037f,
         -0.4082483f, 0.70710677f, 1.0548190f,
         -0.4082483f, -0.70710677f, 0.1420281f
    };

    float3x3 matrixH2 =
    {
        0.84630f, -0.37844f, -0.37844f,
         -0.37265f, 0.33446f, -1.07975f,
          0.57735f, 0.57735f, 0.57735f
    };

    float a = c.a;
    float3x3 rotateZ =
    {
        cos(radians(h)), sin(radians(h)), 0,
		-sin(radians(h)), cos(radians(h)), 0,
		0, 0, 1
    };
    matrixH = mul(matrixH, rotateZ);
    matrixH = mul(matrixH, matrixH2);

    float i = 1 - s;
    float3x3 matrixS =
    {
        i * 0.3086f + s, i * 0.3086f, i * 0.3086f,
		i * 0.6094f, i * 0.6094f + s, i * 0.6094f,
		i * 0.0820f, i * 0.0820f, i * 0.0820f + s
    };
    matrixH = mul(matrixH, matrixS);

    float3 c1 = mul(c, matrixH);
    c1 += l;
    return saturate(float4(c1, c.a));
}

float4 PSMain(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(iChannel0, uv);
    return ColorProcessing(color, 0, 0.03, 0.1);
}

