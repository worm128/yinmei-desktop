Texture2D g_Texture : register(t0); // 输入纹理
SamplerState g_Sampler : register(s0); // 采样器状态

float4 main(float2 uv : TEXCOORD) : SV_Target
{
    const float4 greenColor = float4(0.0, 1.0, 0.0, 1.0); // RGB + Alpha
    float4 color = g_Texture.Sample(g_Sampler, uv);

    // 计算颜色与绿色之间的距离
    float distance = length(color.rgb - greenColor.rgb);
    float threshold = 0.1; // 阈值，可根据需要调整

    // 如果距离小于阈值，则将像素设为透明
    if (distance < threshold)
    {
        return float4(0.0, 0.0, 0.0, 0.0); // 设置为完全透明
    }
    else
    {
        return color; // 保留原始颜色
    }
}