using BBG_Pro.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BBG_Pro
{
    /// <summary>
    /// ChooseBlock.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseBlock : Page
    {
        public ChooseBlock(Service.BlockInfoService blockInfoService_)
        {
            InitializeComponent();
            BlockInfoService = blockInfoService_;
            checked_cnt = blockInfoService_.classEnabled.Length;
            //Console.WriteLine(checked_cnt);
            choose_stas.Text = $"{checked_cnt}/{BlockInfoService.classEnabled.Length}";
            ChangeLayoutToColorMode();
            btn_sw.Content = "切换到：总览与分析";
            btn_sw.ToolTip = "目前在：颜色视图";
            viewMode = true;
        }

        bool viewMode = false;//颜色视图

        public BlockInfoService BlockInfoService { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewMode)
            {
                ChangeLayoutToBlockMode();
                (sender as Button).Content = "切换到：颜色视图";
                (sender as Button).ToolTip = "目前在：总览与分析";
                viewMode = false;
            }
            else
            {
                ChangeLayoutToColorMode();
                (sender as Button).Content = "切换到：总览与分析";
                (sender as Button).ToolTip = "目前在：颜色视图";
                viewMode = true;
            }
        }
        bool isColorModeLoaded = false;
        UIElement colorMode;
        private void ChangeLayoutToColorMode()
        {
            if (!isColorModeLoaded)
            {
                ClearLayout();
                holder.Children.Add(GetColorItem(BlockInfoService.GetBlockDatas()));
                colorMode = holder.Children[0];
                isColorModeLoaded = true;
            }
            else
            {
                holder.Children.Clear();
                holder.Children.Add(colorMode);
            }

        }
        private void ChangeLayoutToBlockMode()
        {
            //ClearLayout();
            //TextBlock tb = new TextBlock();
            //tb.Text = "ClassView";
            //tb.FontSize = 50;
            //holder.Children.Add(tb);
            Pages.diagnosis diagnosis = new Pages.diagnosis(BlockInfoService);
            Frame f = new Frame();
            f.Content = diagnosis;
            holder.Children.Clear();
            holder.Children.Add(f);
        }

        private void ClearLayout()
        {
            holder.Children.Clear();
        }

        private Grid GetBlockItem(CommonStructure.BlockData blockData, int index)
        {
            Grid grid = new Grid();

            return grid;
        }

        List<CheckBox> checkBoxes = new List<CheckBox>();
        private WrapPanel GetColorItem(CommonStructure.BlockData[] blockData)
        {

            //w.ItemWidth = 16;
            //w.ItemHeight = 16;
            //w.Background = System.Windows.Media.Brushes.Red;
            if (BlockInfoService.ThreeDEnabled)
            {
                CommonStructure.BlockData[] blockDatas = BlockInfoService.GetChoosenBlocks();
                List<CommonStructure.BlockData> tmp = new List<CommonStructure.BlockData>();

                for (int i = 0; i < blockDatas.Length / 3; i++)
                {
                    tmp.Add(blockDatas[i]);
                }
               return Getwp(tmp);
            }
            else
            {
                return Getwp(blockData);
            }
        }


        private WrapPanel Getwp(IEnumerable<CommonStructure.BlockData> ts)
        {
            WrapPanel w = new WrapPanel();
            w.FlowDirection = FlowDirection.LeftToRight;
            foreach (var item in ts)
            {
                StackPanel sp = new StackPanel();
                sp.FlowDirection = FlowDirection.LeftToRight;

                sp.Children.Add(GetSingeBlocklImage2(item.image[0], item.classid, "显示方块：导出的图片上的该颜色将用该方块表征。"));

                CheckBox cb = new CheckBox();
                System.Windows.Media.Color color = new System.Windows.Media.Color();
                var org = item.RGBColor.ToColor();
                color.A = 255;
                color.R = org.R;
                color.G = org.G;
                color.B = org.B;
                cb.Background = new System.Windows.Media.SolidColorBrush(color);
                cb.IsChecked = true;
                cb.Click += Cb_Checked;
                cb.MouseEnter += Cb_MouseEnter;
                cb.Width = 32;
                cb.Height = 32;
                cb.Margin = new Thickness(0, 20, 0, 0);


                cb.RenderTransformOrigin = new Point(0, 0.5);
                ScaleTransform sctr = new ScaleTransform();
                sctr.ScaleY = 2;
                sctr.ScaleX = 2;
                TransformGroup trfg = new TransformGroup();
                trfg.Children.Add(sctr);
                cb.RenderTransform = trfg;

                cb.Tag = item.classid;
                cb.ToolTip = $"点击来关闭颜色({ToHTMLColorCode(item.RGBColor.ToColor())})";
                checkBoxes.Add(cb);
                sp.Children.Add(cb);

                foreach (var u in item.image)
                {
                    //TextBlock tb = new TextBlock();
                    //tb.Text = "ClassView";
                    //tb.FontSize = 50;
                    //w.Children.Add(tb);
                    sp.Children.Add(GetSingeBlocklImage(u, item.classid, u.Split('.')[0]));
                }
                w.Children.Add(sp);
            }

            return w;
        }
        private void Cb_MouseEnter(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("enter");

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var c = sender as CheckBox;
                if (c.IsChecked ?? true)
                {
                    (sender as CheckBox).IsChecked = false;
                }
                else
                {
                    (sender as CheckBox).IsChecked = true;
                }
                Cb_Checked(sender, e);
            }
        }

        int checked_cnt = 0;
        private void Cb_Checked(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("click");
            bool isChecked = (sender as CheckBox).IsChecked ?? false;
            var checkbox = sender as CheckBox;
            if (isChecked)
            {
                checkbox.ToolTip = (sender as CheckBox).ToolTip.ToString().Replace("开启", "关闭");
                BlockInfoService.classEnabled[int.Parse(checkbox.Tag.ToString()) - 1] = true;
                if (checkbox.Parent != null)
                {
                    (checkbox.Parent as StackPanel).Opacity = 1.0f;
                    ++checked_cnt;
                }
            }
            else
            {
                --checked_cnt;
                if (checked_cnt == 0)
                {
                    Snackbar.MessageQueue.Clear();
                    Snackbar.MessageQueue.Enqueue("警告：至少应该选用一种颜色。否则生成将无法正确进行。");
                    checked_cnt = 1;
                    checkbox.IsChecked = true;
                    return;
                }
                checkbox.ToolTip = (sender as CheckBox).ToolTip.ToString().Replace("关闭", "开启");
                BlockInfoService.classEnabled[int.Parse(checkbox.Tag.ToString()) - 1] = false;
                if (checkbox.Parent != null)
                {
                    (checkbox.Parent as StackPanel).Opacity = 0.5f;
                }
            }
            choose_stas.Text = $"{checked_cnt}/{BlockInfoService.classEnabled.Length}";

        }

        private Image GetSingeBlocklImage(string uri, CommonStructure.BlockData blockData)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(@"/blockdata/" + uri, UriKind.Relative));
            image.MouseDown += Image_MouseDown;
            image.Tag = blockData.enabled;

            return image;
        }
        private Image GetSingeBlocklImage(string uri, int classid, string help = "没有备注.")
        {
            Image image = new Image();
            image.Width = 32;
            image.Height = 32;
            image.MouseDown += Image_MouseDown;
            image.Tag = classid;
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            //Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata\" + uri);
            string dir = System.AppDomain.CurrentDomain.BaseDirectory;
            image.Source = new BitmapImage(new Uri(dir + @"blockdata/" + uri, UriKind.Absolute));
            //Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //Console.WriteLine(@"blockdata\" + uri);
            //image.Source = new BitmapImage(new Uri(@"C:\Users\Administrator\source\repos\BBG Pro\BBG Pro\bin\Debug\blockdata\TNT.bmp"));
            image.ToolTip = help;
            return image;
        }

        List<Image> demo_images = new List<Image>();
        private Image GetSingeBlocklImage2(string uri, int classid, string help = "没有备注.")
        {
            Image image = new Image();
            image.Width = 32;
            image.Height = 32;
            image.MouseDown += Image_MouseDown1;
            image.MouseEnter += Image_MouseEnter; ;
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            image.Tag = classid;
            //Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata\" + uri);
            string dir = System.AppDomain.CurrentDomain.BaseDirectory;
            image.Source = new BitmapImage(new Uri(dir + @"blockdata/" + uri, UriKind.Absolute));
            //Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //Console.WriteLine(@"blockdata\" + uri);
            //image.Source = new BitmapImage(new Uri(@"C:\Users\Administrator\source\repos\BBG Pro\BBG Pro\bin\Debug\blockdata\TNT.bmp"));
            image.ToolTip = help;
            demo_images.Add(image);
            return image;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var c = sender as Image;
                var d = checkBoxes[int.Parse(c.Tag.ToString()) - 1];
                if (d.IsChecked ?? true)
                {
                    d.IsChecked = false;
                }
                else
                {
                    d.IsChecked = true;
                }
                Cb_Checked(d, e);
            }
        }

        private void Image_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Image;
            var d = checkBoxes[int.Parse(c.Tag.ToString()) - 1];
            if (d.IsChecked ?? true)
            {
                d.IsChecked = false;
            }
            else
            {
                d.IsChecked = true;
            }
            Cb_Checked(d, e);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = int.Parse((sender as Image).Tag.ToString()) - 1;
            Snackbar.MessageQueue.Clear();
            if (BlockInfoService.classEnabled[index])
            {
                BlockInfoService.demoBlock[index] = (sender as Image).Source.ToString();
                demo_images[index].Source = (sender as Image).Source;
            }
            else
            {
                Snackbar.MessageQueue.Enqueue("注意：您需要先启用该颜色再选择该颜色下的显示方块！");
            }
        }

        private static string ToHTMLColorCode(System.Drawing.Color color) => System.Drawing.ColorTranslator.ToHtml(color);

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
