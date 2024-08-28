/// picture
sampler2D Input : register(S0);


float4 PSMain(float2 uv : TEXCOORD0) : COLOR
{
    float4 g_ChromaKeyColor = float4(0.0, 1.0, 0.0, 1.0); // 绿布颜色，如 (0, 1, 0, 1) 表示绿色 (R, G, B, A)
    float g_Tolerance = 0.8; // 容差范围
    
    float4 color = tex2D(Input, uv);
    
    // 计算当前像素与目标颜色的差异
    float difference = length(color.rgb - g_ChromaKeyColor.rgb);
    
    // 如果差异在容差范围内，则将像素颜色设为透明
    if (difference < g_Tolerance)
    {
        color.a = 0;
        //透明通道，必须把rgb全部设置为白色
        color.rgb = float3(0.0, 0.0, 0.0);
    }
    
    return color;
}