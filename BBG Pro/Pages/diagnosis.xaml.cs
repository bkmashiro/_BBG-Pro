using BBG_Pro.Service;
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
using Colourful;
using Colourful.Conversion;
using System.Drawing;

namespace BBG_Pro.Pages
{
    /// <summary>
    /// diagnosis.xaml 的交互逻辑
    /// </summary>
    public partial class diagnosis : Page
    {
        public diagnosis(Service.BlockInfoService blockInfoService)
        {
            InitializeComponent();
            BlockInfoService = blockInfoService;
        }

        private BlockInfoService BlockInfoService { get; }

        List<LuvColor> luvColors = new List<LuvColor>();

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ColourfulConverter colourfulConverter = new ColourfulConverter();
            for (int i = 0; i < BlockInfoService.classEnabled.Length; i++)
            {
                if (BlockInfoService.classEnabled[i])
                {
                    luvColors.Add(colourfulConverter.ToLuv(BlockInfoService.choosen_blockDatas[i].RGBColor));
                }
            }
            Bitmap bmp = new Bitmap("rgb.bmp");
            Graphics g = Graphics.FromImage(bmp);

            double radius = 2;
            foreach (var item in luvColors)
            {
                g.FillEllipse(System.Drawing.Brushes.Red, (float)(item.u - radius)+100, (float)(item.v - radius)+100, (float)(radius * 2), (float)(radius * 2));
            }

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Stretch = Stretch.Fill;
            img.Width = 512;
            img.Height = 512;

            img.Source = BitmapToBitmapImage(bmp);
            cv.Children.Add(img);
            //LuvColor luvColor;
            //Bitmap bmp = new Bitmap(200, 200);
            //Graphics g = Graphics.FromImage(bmp);
            //System.Drawing.Pen pen;
            //for (int u = -100; u < 101; u++)
            //{
            //    for (int v = -100; v < 101; v++)
            //    {
            //        luvColor = new LuvColor(100, u, v);
            //        pen = new System.Drawing.Pen(colourfulConverter.ToRGB(luvColor).ToColor());

            //        g.DrawEllipse(pen, u + 100, v + 100, u + 100, v + 100);
            //    }
            //}
            //bmp.Save("rgb.bmp");
            //Console.WriteLine();
        }

        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            g.Visibility = Visibility.Visible;
            btn.Visibility = Visibility.Collapsed;
        }
    }
}
