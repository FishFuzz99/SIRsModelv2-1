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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }




        public void Update()
        {
            this.MainImage.Opacity = 1;
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
                    int i = smallWidth * y + x;
                    red = (int)(r.NextDouble() * 255);
                    green = (int)(r.NextDouble() * 255);
                    blue = (int)(r.NextDouble() * 255);
                    alpha = 255;

                    if (green < 200)
                    {
                        alpha = 0;
                    }

                    graph[i] = (uint)((alpha << 24) + (green << 16) + (red << 8) + blue);
                }
            }

            uint[] pixels = new uint[width * height];
            /*
            int red, green, blue, alpha;
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

                    pixels[i] = (uint)((alpha << 24) + (green << 16) + (red << 8) + blue);
                }
            }
            */
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

                    pixels[i] = (graph[(int)((y / yscale) * smallWidth + (x / xscale))]);
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0, 0);

            this.MainImage.Source = bitmap;
            //Do things
        }




        private void run_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            while (count < 10)
            {
                runLoop();
                Task.Delay(250);
                count++;
                count = 10;
            }
        }

        private void runLoop()
        {
            /*while (count < 10)
            {
                Update();
                System.Threading.Thread.Sleep(1000);
                count++;
            }*/
            Update();
        }
    }
}
