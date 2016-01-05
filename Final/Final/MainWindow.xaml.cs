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

        public IList<string> cityList = new List<string>
        {
            "Los Angeles",
            "New York",
            "Boston",
            "Seattle",
            "San Francisco",
            "San Diego",
            "Las Vegas",
            "Houston",
            "Chicago"
        };


        public long DaysPassed;

        public MainWindow()
        {
            //dg = new Final.DataGrid();
            DaysPassed = 0;
            InitializeComponent();
            this.daysPassed.Content = "Total Days Passed: " + DaysPassed;
            mainImage = this.MainImage;
        }
        private string iRate;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
	    // ... Get control that raised this event.
	    var textBox = sender as TextBox;
	    // ... Change Window Title.
	    this.Title = textBox.Text +
		"[Length = " + textBox.Text.Length.ToString() + "]";
	}

        private void run_Click(object sender, RoutedEventArgs e)
        {
            this.run.IsEnabled = false;
            this.MainImage.Opacity = .75;
            if (dg == null) {
                dg = new Final.DataGrid(Convert.ToInt16(this.numberOfDays.GetLineText(0)),
                    Convert.ToSingle(this.RRate.GetLineText(0)),
                    Convert.ToSingle(this.DRate.GetLineText(0)),
                    Convert.ToSingle(this.travelRate.GetLineText(0)),
                    Convert.ToSingle(this.airTravelRate.GetLineText(0)),
                    this.comboBox.SelectionBoxItem.ToString(),
                    Convert.ToSingle(this.IRate.GetLineText(0)));
            }
            else
            {
                dg.deathRate = Convert.ToSingle(this.DRate.GetLineText(0));
                dg.k = Convert.ToSingle(this.RRate.GetLineText(0));
                dg.airportTravelRate = Convert.ToSingle(this.airTravelRate.GetLineText(0));
                dg.borderTravelRate = Convert.ToSingle(this.travelRate.GetLineText(0));
                dg.b = Convert.ToSingle(this.IRate.GetLineText(0));

            }
            System.Windows.Threading.Dispatcher mainImageDispatcher = MainImage.Dispatcher;
            bw = new BackgroundWorker();
            bw.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                for (int i = 0; i < dg.totalT; i++)
                {
                    BitmapImage bi = Update(); // dg);
                    updateImage update = new updateImage(updateMainImage);
                    mainImageDispatcher.BeginInvoke(update, bi);
                    Thread.Sleep(50);
                }
            };
            bw.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
                this.totalDead.Content = "Total Dead: " + dg.getTotalDead();
                this.totalInfected.Content = "Total Infected: " + dg.getTotalInfected();
                this.totalRecovered.Content = "Total Recovered: " + dg.getTotalRecovered();
                this.run.IsEnabled = true;

            };

            bw.RunWorkerAsync();

        }
        
        public BitmapImage Update()
        {
            dg.runTimeStep();//dg);
            double[] array = dg.getArray();

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
                    green = 0;
                    red = 0;
                    blue = 0;
                    alpha = 255;
                    if (array[i] > 0.35)
                    {
                        red = 255;
                        blue = 0;
                        green = 15;
                    }
                    if (array[i] > 0.30)
                    {
                        red = 255;
                        blue = 0;
                        green = 35;
                    }
                    if (array[i] > 0.25)
                    {
                        red = 255;
                        blue = 0;
                        green = 55;
                    }
                    else if (array[i] > 0.2)
                    {
                        red = 255;
                        blue = 0;
                        green = 75;
                    }
                    else if (array[i] > 0.18)
                    {
                        red = 255;
                        blue = 0;
                        green = 95;
                    }
                    else if (array[i] > 0.15)
                    {
                        red = 255;
                        blue = 0;
                        green = 115;
                    }
                    else if (array[i] > 0.13)
                    {
                        red = 255;
                        blue = 0;
                        green = 135;
                    }
                    else if (array[i] > 0.11)
                    {
                        red = 255;
                        blue = 0;
                        green = 155;
                    }
                    else if (array[i] > 0.09)
                    {
                        red = 255;
                        blue = 0;
                        green = 175;
                    }
                    else if (array[i] > 0.07)
                    {
                        red = 255;
                        blue = 0;
                        green = 195;
                    }
                    else if (array[i] > 0.05)
                    {
                        red = 255;
                        blue = 0;
                        green = 215;
                    }
                    else if (array[i] > 0.03)
                    {
                        red = 255;
                        blue = 0;
                        green = 235;
                    }
                    else if (array[i] > 0)
                    {
                        red = 255;
                        blue = 0;
                        green = 255;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                        alpha = 0;
                    }

                    i = smallWidth * y + x;
                    graph[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            uint[] pixels = new uint[width * height];
           
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
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
            DaysPassed++;
            this.daysPassed.Content = "Total Days Passed: " + DaysPassed;
        }
        public delegate void updateImage(BitmapImage bitmap);
    }
}
