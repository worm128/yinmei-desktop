using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace YinmeiWindow
{
	
	public class GrayFilter : ShaderEffect {
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayFilter), 0);
		public GrayFilter() {
			init();
		}

		public void init()
        {
			PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"/YinmeiWindow;Component/Effects/GrayFilter.ps", UriKind.Relative);
			this.PixelShader = pixelShader;
			  
			this.UpdateShaderValue(InputProperty); 
		}  

		public Brush Input {
			get {
				return ((Brush)(this.GetValue(InputProperty)));
			}
			set {
				this.SetValue(InputProperty, value);
			}
		}
	}
}
