Texture2D g_Texture : register(t0); // ��������
SamplerState g_Sampler : register(s0); // ������״̬

float4 main(float2 uv : TEXCOORD) : SV_Target
{
    const float4 greenColor = float4(0.0, 1.0, 0.0, 1.0); // RGB + Alpha
    float4 color = g_Texture.Sample(g_Sampler, uv);

    // ������ɫ����ɫ֮��ľ���
    float distance = length(color.rgb - greenColor.rgb);
    float threshold = 0.1; // ��ֵ���ɸ�����Ҫ����

    // �������С����ֵ����������Ϊ͸��
    if (distance < threshold)
    {
        return float4(0.0, 0.0, 0.0, 0.0); // ����Ϊ��ȫ͸��
    }
    else
    {
        return color; // ����ԭʼ��ɫ
    }
}