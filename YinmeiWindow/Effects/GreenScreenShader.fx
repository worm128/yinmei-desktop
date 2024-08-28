/// picture
sampler2D Input : register(S0);


float4 PSMain(float2 uv : TEXCOORD0) : COLOR
{
    float4 g_ChromaKeyColor = float4(0.0, 1.0, 0.0, 1.0); // �̲���ɫ���� (0, 1, 0, 1) ��ʾ��ɫ (R, G, B, A)
    float g_Tolerance = 0.8; // �ݲΧ
    
    float4 color = tex2D(Input, uv);
    
    // ���㵱ǰ������Ŀ����ɫ�Ĳ���
    float difference = length(color.rgb - g_ChromaKeyColor.rgb);
    
    // ����������ݲΧ�ڣ���������ɫ��Ϊ͸��
    if (difference < g_Tolerance)
    {
        color.a = 0;
        //͸��ͨ���������rgbȫ������Ϊ��ɫ
        color.rgb = float3(0.0, 0.0, 0.0);
    }
    
    return color;
}