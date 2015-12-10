using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Threading;
using System.ComponentModel;

namespace WpfApplication1
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Controls.Image mainImage;
        BackgroundWorker bw = new BackgroundWorker();
        private Final.DataGrid dg;
        public MainWindow()
        {
            dg = new Final.DataGrid();
            InitializeComponent();
            mainImage = this.MainImage;
        }
        
        
        private void run_Click(object sender, RoutedEventArgs e)
        {
            this.MainImage.Opacity = 1;

            System.Windows.Threading.Dispatcher mainImageDispatcher = MainImage.Dispatcher;
            bw = new BackgroundWorker();
            bw.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                for (int i = 0; i < 1000; i++)
                {
                    BitmapImage bi = Update(); // dg);
                    updateImage update = new updateImage(updateMainImage);
                    mainImageDispatcher.BeginInvoke(update, bi);
                    Thread.Sleep(50);
                }
            };
            bw.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
              
            };

            bw.RunWorkerAsync();

            
        }
        
        public BitmapImage Update()
        {
            dg.runTimeStep();
            long[] array = dg.getArray();

            int width = 1180;
            int height = 500;
            //250 y
            //590 x
            int smallWidth = 590;
            int smallHeight = 250;

            double xscale = (double)width / (double)smallWidth;
            double yscale = (double)height / (double)smallHeight;
            Random r = new Random();

            uint[] graph = new uint[smallWidth * smallHeight];
            int red, green, blue, alpha;
            for (int x = 0; x < smallWidth; ++x)
            {
                for (int y = 0; y < smallHeight; ++y)
                {
                    
                    int i = (smallWidth * (smallHeight - 1 - y)) + x;
                    /*
                    red = (int)(r.NextDouble() * 255);
                    green = (int)(r.NextDouble() * 255);
                    blue = (int)(r.NextDouble() * 255);
                    alpha = 255;

                    if (green < 200)
                    {
                        alpha = 0;
                    }
                    */

                    if (array[i] > 1)
                    {
                        green = 255;
                        red = (int)(r.NextDouble() * 255);
                        blue = (int)(r.NextDouble() * 255);
                        alpha = 255;
                    }
                    else
                    {
                        red = 255;
                        green = (int)(r.NextDouble() * 255);
                        blue = (int)(r.NextDouble() * 255);
                        alpha = 0;
                    }

                    i = smallWidth * y + x;
                    graph[i] = (uint)((alpha << 24) + (green << 16) + (red << 8) + blue);
                }
            }

            uint[] pixels = new uint[width * height];
           
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    red = (int)(r.NextDouble() * 255);
                    green = (int)(r.NextDouble() * 255);
                    blue = (int)(r.NextDouble() * 255);
                    alpha = 255;

                    if (green < 200)
                    {
                        alpha = 0;
                    }

                    pixels[i] = (graph[(int)((int)(y / yscale) * smallWidth + (int)(x / xscale))]);
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0, 0);



            BitmapImage backgroundThreadImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                backgroundThreadImage.BeginInit();
                backgroundThreadImage.CacheOption = BitmapCacheOption.OnLoad;
                backgroundThreadImage.StreamSource = stream;
                backgroundThreadImage.EndInit();
                backgroundThreadImage.Freeze();
            }

            return backgroundThreadImage;
        }

        public void updateMainImage(BitmapImage bitmap)
        {
            this.MainImage.Source = bitmap;
        }
        public delegate void updateImage(BitmapImage bitmap);
    }
}
