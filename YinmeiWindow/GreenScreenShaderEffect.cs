using System.Windows.Media.Effects;
using System.Windows.Media;
using System;
using System.Windows;


namespace YinmeiWindow
{
    public class GreenScreenShaderEffect : ShaderEffect
    {
        public GreenScreenShaderEffect()
        {
            //======== 这里是SharpDX.Direct3D11的PixelShader
            /* var device = new Device(DriverType.Hardware, DeviceCreationFlags.None);
             var deviceContext = device.ImmediateContext;

             // 加载着色器二进制数据
             byte[] shaderCode = File.ReadAllBytes("GreenScreenShader.ps");

             // 创建PixelShader对象
             var pixelShader = new SharpDX.Direct3D11.PixelShader(device, shaderCode);

             // 将像素着色器对象设置到设备上下文中
             deviceContext.PixelShader.Set(pixelShader);*/

            //======== 这里是System.Windows.Media.Effects的PixelShader
            PixelShader = new System.Windows.Media.Effects.PixelShader { UriSource = new Uri(@"pack://application:,,,/YinmeiWindow;Component/Effects/GreenScreenShader.ps", UriKind.RelativeOrAbsolute) };
            UpdateShaderValue(InputProperty);
            /*PixelShader.InvalidPixelShaderEncountered += (s, e) =>
            {
                Debug.WriteLine($"Pixel Shader Error: {e}");
            };*/
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(GreenScreenShaderEffect), 0, SamplingMode.Bilinear);

    }
}