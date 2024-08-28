using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LightjamsSpoutLib;
using System.Drawing;
using System.IO;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace YinmeiWindow
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer; // 用于定时更新图像
        private byte[] m_buffer; // 假设这是你从视频帧获取的字节数组
        LightjamsSpoutReceiver m_receiver = new LightjamsSpoutReceiver();
        // spout.net
        //SpoutReceiver receiver = new SpoutReceiver();

        int m_width, m_height;
        int stride;
        int num = 0;

        /* private RenderTarget _renderTarget;
         private MediaFoundationRenderEngine _mediaFoundationRenderEngine;*/

        public unsafe MainWindow()
        {
            InitializeComponent();
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;

            this.MouseDown += new MouseButtonEventHandler(Window_MouseDown);

            // 添加 Loaded 事件处理器
            //this.Loaded += MainWindow_Loaded;

            //this.WindowStyle = WindowStyle.None;//设置窗口无边框
            //this.AllowsTransparency = true;//窗口工作区域支持透明
            //this.Opacity = 0;//设置透明度(值可以自己改动)

            #region Spout.NET发送器
            /*using (DeviceContext deviceContext = DeviceContext.Create()) // Create the DeviceContext
            {
                IntPtr glContext = IntPtr.Zero;
                glContext = deviceContext.CreateContext(IntPtr.Zero);
                deviceContext.MakeCurrent(glContext); // Make this become the primary context
                SpoutSender sender = new SpoutSender();
                sender.CreateSender("CsSender", 640, 360, 0); // Create the sender

                byte[] data = new byte[640 * 360 * 4];
                int i = 0;
                fixed (byte* pData = data) // Get the pointer of the byte array
                    while (true)
                    {
                        for (int j = 0; j < 640 * 360 * 4; j += 4)
                        {
                            data[j] = i == 0 ? byte.MaxValue : byte.MinValue;
                            data[j + 1] = i == 1 ? byte.MaxValue : byte.MinValue;
                            data[j + 2] = i == 2 ? byte.MaxValue : byte.MinValue;
                            data[j + 3] = byte.MaxValue;
                        }
                        Console.WriteLine($"Sending (i = {i})");
                        sender.SendImage(
                            pData, // Pixels
                            640, // Width
                            360, // Height
                            Gl.RGBA, // GL_RGBA
                            true, // B Invert
                            0 // Host FBO
                            );
                        Thread.Sleep(1000); // Delay
                        if (i < 2) i++;
                        else i = 0;
                    }
            }*/
            #endregion

            #region Lightjams接收器
            var m_glContext = new GLContext();
            m_glContext.Create();

            // the senderName as retrieved when enumerating senders or "" to use the active sender
            m_receiver.Connect("VTubeStudioSpout", out m_width, out m_height);
            imageControl.Width = 800;
            imageControl.Height = 650;
            imageControl.Stretch = Stretch.Uniform;

            const int bytesPerPixel = 3;    // RGB
            stride = 4 * ((m_width * bytesPerPixel + 3) / 4);
            m_buffer = new byte[stride * m_height];
            #endregion

            #region Spout.NET接收器
            /*uint width= 800, height= 650;

            string str = "VTubeStudioSpout";
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            // 分配内存并获取指针
            IntPtr ptr = Marshal.AllocHGlobal(byteArray.Length);
            // 将字节数组复制到非托管内存
            Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
            // 将IntPtr转换为sbyte*
            sbyte* p = (sbyte*)ptr.ToPointer();

            receiver.CreateReceiver(p, ref width, ref height, true);*/
            #endregion
            //=================Lightjams接收器
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) }; // 大约每33ms更新一次，接近30FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private async Task all()
        {
            // 开始定时更新图像
            //_timer.Start();

            // 这里应替换为实际的视频帧捕获逻辑
            await CaptureVideoFramesAsync();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await UpdateImage2();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task CaptureVideoFramesAsync()
        {
            var m_glContext = new GLContext();
            m_glContext.Create();

            // the senderName as retrieved when enumerating senders or "" to use the active sender
            m_receiver.Connect("VTubeStudioSpout", out m_width, out m_height);
            imageControl.Width = 800;
            imageControl.Height = 650;
            imageControl.Stretch = Stretch.Uniform;

            const int bytesPerPixel = 3;    // RGB
            stride = 4 * ((m_width * bytesPerPixel + 3) / 4);
            m_buffer = new byte[stride * m_height];


            // 示例代码，实际应用中使用相应库捕获视频帧
            while (true)
            {
                try
                {
                    // 通知UI线程更新图像
                    await UpdateImage2();
                    await Task.Delay(33); // 每隔33毫秒=30fps更新一次
                }catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private async Task UpdateUITextPeriodicallyAsync()

        {
            int counter = 0;
            while (true)
            {
                // 更新UI必须在UI线程执行，这里已经自动管理
                //txtStatus.Content = $"Counter: {counter}";

                // 暂停一段时间，避免过度占用CPU
                await Task.Delay(1000); // 每隔1秒更新一次

                counter++;


                // 根据需要添加退出循环的条件，例如：
                // if (counter >= 10) break;
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        private async Task UpdateImage()
        {
            System.Drawing.Bitmap m_bitmap = new System.Drawing.Bitmap(m_width, m_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            // 假设这是从视频流中获取的帧数据
            m_buffer = new byte[stride * m_height];
            m_receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.BGR);
            var data = m_bitmap.LockBits(new System.Drawing.Rectangle(0, 0, m_width, m_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(m_buffer, 0, data.Scan0, m_buffer.Length);
            m_bitmap.UnlockBits(data);


            /*m_bitmap.Save($"{num}.png");
            num++;*/

            /*var bitmap = BitmapSource.Create(

                m_width, // 视频帧的宽度
                m_height, // 视频帧的高度
                96, // 水平分辨率（DPI）
                96, // 垂直分辨率（DPI）
                PixelFormats.Bgr32, // 或根据实际情况选择其他PixelFormat
                null,
                m_buffer,
                stride); // 每行字节数，根据帧宽度和像素格式计算得出*/

            // 假设_image是XAML中定义的Image控件
            IntPtr hBitmap = m_bitmap.GetHbitmap();
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                            hBitmap,
                                            IntPtr.Zero,
                                            Int32Rect.Empty,
                                            BitmapSizeOptions.FromEmptyOptions());
            imageControl.Source = source;

            // 清空字节数组准备下一次填充
            DeleteObject(hBitmap);
        }

        private async Task UpdateImage2()
        {
            try
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(m_width, m_height, 96, 96, PixelFormats.Rgb24, null);
                m_buffer = new byte[stride * m_height];
                m_receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.RGB);
                //Debug.WriteLine(m_buffer.Length);

                writeableBitmap.Lock();
                Marshal.Copy(m_buffer, 0, writeableBitmap.BackBuffer, m_buffer.Length);
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, m_width, m_height));
                writeableBitmap.Unlock();

                imageControl.Source = writeableBitmap;
                //GC.Collect();
            }catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task UpdateImage3()
        {
            try
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(m_width, m_height, 96, 96, PixelFormats.Rgb24, null);
                m_buffer = new byte[stride * m_height];
                /*receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.RGB);
                //Debug.WriteLine(m_buffer.Length);

                writeableBitmap.Lock();
                Marshal.Copy(m_buffer, 0, writeableBitmap.BackBuffer, m_buffer.Length);
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, m_width, m_height));
                writeableBitmap.Unlock();

                imageControl.Source = writeableBitmap;*/
                //GC.Collect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ProcessGreenScreen(WriteableBitmap writeableBitmap)
        {
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int[] pixels = new int[width * height]; // 用于临时存储像素数据

            // 读取原始像素数据
            writeableBitmap.CopyPixels(pixels, width * sizeof(int), 0);

            // 遍历并处理像素
            for (int i = 0; i < pixels.Length; i++)
            {
                // 提取ARGB值
                int argb = pixels[i];
                byte a = (byte)(argb >> 24);
                byte r = (byte)(argb >> 16);
                byte g = (byte)(argb >> 8);
                byte b = (byte)argb;

                // 绿幕检测逻辑，可根据需要调整阈值
                if (IsGreenPixel(r, g, b))
                {
                    // 设置为透明
                    pixels[i] = (a << 24) | (0 << 16) | (0 << 8) | 0;
                }
            }

            // 写入处理后的像素数据
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * sizeof(int), 0);
        }

        private bool IsGreenPixel(byte r, byte g, byte b)
        {
            // 绿色范围判断，这里使用简单的阈值比较，实际应用中可能需要更精细的算法
            const int greenTolerance = 70; // 可调整
            return Math.Abs(g - 255) <= greenTolerance && Math.Abs(r - g) >= greenTolerance && Math.Abs(b - g) >= greenTolerance;
        }

        private WriteableBitmap ConvertRawBytesToWriteableBitmap(byte[] imageData, int width, int height)
        {
            if (imageData.Length != width * height * 4) // 检查数据长度是否匹配预期的像素格式（Bgra32）
                throw new ArgumentException("Image data size does not match the specified dimensions.");

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            // 锁定WriteableBitmap以便写入
            bitmap.Lock();

            // 直接复制字节数组到BackBuffer
            IntPtr ptr = bitmap.BackBuffer;
            Marshal.Copy(imageData, 0, ptr, imageData.Length);

            // 标记整个图像区域为已更改
            bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

            // 解锁WriteableBitmap
            bitmap.Unlock();

            return bitmap;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
            /*_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) }; // 大约每33ms更新一次，接近30FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();*/

            await CaptureVideoFramesAsync();

            /*System.Drawing.Bitmap m_bitmap = new System.Drawing.Bitmap(m_width, m_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //WriteableBitmap writeableBitmap = new WriteableBitmap(m_width, m_height, 96, 96, PixelFormats.Pbgra32, null);
            int i = 0;
            //Thread.Sleep(3000);
            while (i <= 100)
            {
                m_receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.BGR);
                var data = m_bitmap.LockBits(new System.Drawing.Rectangle(0, 0, m_width, m_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(m_buffer, 0, data.Scan0, m_buffer.Length);
                //writeableBitmap.WritePixels(new Int32Rect(0, 0, 573, 435), data.Scan0, data.Height * data.Stride, data.Stride, 0, 0);
                m_bitmap.UnlockBits(data);
                m_bitmap.Save($"{i}.png");

                // 将BitmapImage设置到Image控件上
                imageControl.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                        m_bitmap.GetHbitmap(),
                                        IntPtr.Zero,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());
                 //imageControl.Invalidate();


                 i++;
            }*/
            Console.WriteLine("退出");
        }

        public void ReceiveImg()
        {
            var m_glContext = new GLContext();

            m_glContext.Create();

            LightjamsSpoutReceiver m_receiver = new LightjamsSpoutReceiver();
            int m_width, m_height;
            // the senderName as retrieved when enumerating senders or "" to use the active sender
            m_receiver.Connect("VTubeStudioSpout", out m_width, out m_height);


            const int bytesPerPixel = 3;    // RGB
            int stride = 4 * ((m_width * bytesPerPixel + 3) / 4);
            byte[] m_buffer = new byte[stride * m_height];



            Bitmap m_bitmap = new Bitmap(m_width, m_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int i = 0;
            //Thread.Sleep(3000);
            while (i <= 100)
            {
                m_receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.BGR);
                var data = m_bitmap.LockBits(new System.Drawing.Rectangle(0, 0, m_width, m_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(m_buffer, 0, data.Scan0, m_buffer.Length);
                m_bitmap.UnlockBits(data);
                m_bitmap.Save(i + ".png");
                // 假设这是你的图片字节流
                //byte[] imageBytes = GetImageBytes();

                /*// 将字节流转换为BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                using (var memoryStream = new System.IO.MemoryStream(m_buffer))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
                Console.WriteLine(m_buffer.Length);*/
                // 将BitmapImage设置到Image控件上
                imageControl.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                        m_bitmap.GetHbitmap(),
                                        IntPtr.Zero,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());

                /* ThreadTest tt = new ThreadTest(m_receiver, m_buffer);
                 Thread thread = new Thread(new ThreadStart(tt.ThreadProc));
                 thread.Start();*/

                i++;
            }

        }

        // 这是一个示例方法，用于获取图片字节流，实际中你需要替换为你的字节流来源
        private byte[] GetImageBytes()
        {
            // 这里应该是你的图片字节流获取代码
            // 例如：return File.ReadAllBytes("path_to_image");
            return File.ReadAllBytes("J:\\ai\\ai-code\\YinmeiWindow\\YinmeiWindow\\bin\\Debug\\net8.0-windows\\2.png");
            { /* 图片字节数据 */ };
        }

        public class ThreadTest
        {
            private LightjamsSpoutReceiver m_receiver;
            private byte[] m_buffer;
            public ThreadTest(LightjamsSpoutReceiver a, byte[] b)
            {
                m_receiver = a;
                m_buffer = b;
            }
            public void ThreadProc()
            {
                m_receiver.ReceiveImage(m_buffer, LightjamsSpoutLib.EPixelFormat.BGR);
            }
        }

        /*public class MediaFoundationRenderEngine
        {
            private RenderTarget _renderTarget;
            private SharpDX.Direct2D1.Bitmap _bitmap;

            public MediaFoundationRenderEngine(RenderTarget renderTarget)
            {
                _renderTarget = renderTarget;
            }

            public void SetFrame(*//* AVFrame *//* object frame)
            {
                // 这里应该是将AVFrame转换为SharpDX.Direct2D1.Bitmap
                // _bitmap = ...
            }

            public void RenderFrame()
            {
                if (_bitmap != null)
                {
                    _renderTarget.DrawBitmap(_bitmap, 1, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                }
            }
        }*/
    }


}