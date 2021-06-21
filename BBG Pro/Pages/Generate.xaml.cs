using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace BBG_Pro.Pages
{
    /// <summary>
    /// Generate.xaml 的交互逻辑
    /// </summary>
    public partial class Generate : Page
    {
        public Generate()
        {
            InitializeComponent();
            myGenerate = Generate2D;
            blockInfoService.Init();
            blockInfoService.EnableAllHeight();

        }
        Service.BlockInfoService blockInfoService = new Service.BlockInfoService();
        bool BlockInfoServiceInited = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BlockInfoServiceInited)
            {
                PageTo(1);
                GoBack.Visibility = Visibility.Visible;
            }
            else
            {
                ChooseBlock chooseBlock = new ChooseBlock(blockInfoService);
                frame1.Content = chooseBlock;
                PageTo(1);
                GoBack.Visibility = Visibility.Visible;
                BlockInfoServiceInited = true;
            }
        }

        private void PageTo(int c) => trans.SelectedIndex = c;

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            PageTo(0);
            colors_choosen.Text = $"颜色已选择：{pv.colors_cnt}";
            GoBack.Visibility = Visibility.Collapsed;
        }

        string filePath = string.Empty;
        string myfileName = string.Empty;
        int height = 0;
        int width = 0;
        //Image current_img;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "图像文件(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|所有文件(*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    myfileName = openFileDialog.SafeFileName;
                    try
                    {
                        showImg.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                            width = image.Width;
                            height = image.Height;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"ERROR 读取图片的异常：{ex.Message}");
                        throw;
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        public delegate void Gen();
        Gen myGenerate;
        bool Is3dEnabled = false;
        /// <summary>
        /// 生成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (myGenerate != null && filePath != string.Empty)
            {
                if (ditherEnabled)
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(filePath);
                    Service.DitherService ditherService = new Service.DitherService();
                    switch (ditherType)
                    {
                        case -1: ditheredBmp = (bitmap); break;
                        case 0: ditheredBmp = ditherService.DoBurkesDithering(bitmap); break;
                        case 1: ditheredBmp = ditherService.FakeDithering(bitmap); break;
                        case 2: ditheredBmp = ditherService.FloydSteinbergDithering(bitmap); break;
                        case 3: ditheredBmp = ditherService.JarvisJudiceNinkeDithering(bitmap); break;
                        case 4: ditheredBmp = ditherService.SierraDithering(bitmap); break;
                        case 5: ditheredBmp = ditherService.SierraLiteDithering(bitmap); break;
                        case 6: ditheredBmp = ditherService.SierraTwoRowDithering(bitmap); break;
                        case 7: ditheredBmp = ditherService.StuckiDithering(bitmap); break;

                        default:
                            ditheredBmp = ditherService.DoAtkinsonDithering(bitmap);
                            break;
                    }
                }
                myGenerate();
            }

        }
        Service.MapGenerator MapGenerator = new Service.MapGenerator();

        private void Generate2D()
        {
            Service.ImageProcessService imageProcessService = new Service.ImageProcessService();
            if (!ditherEnabled)
            {
                if (filePath == string.Empty)
                {
                    Snackbar.MessageQueue.Enqueue("请先添加图片！在此之前，生成将不会进行。");
                    return;
                }
                imageProcessService.LoadImage(filePath);//载入图片
            }
            else
            {
                if (ditheredBmp == null)
                {
                    Snackbar.MessageQueue.Enqueue("请先添加图片！在此之前，生成将不会进行。");
                    return;
                }
                imageProcessService.LoadImage(ditheredBmp);
            }
            MapGenerator.Generate(blockInfoService.GetDatas(), imageProcessService.GetArrayImg());

            Console.WriteLine("2D Test Generated.");
            Generate_finished();

        }

        private void Generate3D()
        {
            Service.ImageProcessService imageProcessService = new Service.ImageProcessService();
            if (!ditherEnabled)
            {
                if (filePath == string.Empty)
                {
                    Snackbar.MessageQueue.Enqueue("请先添加图片！在此之前，生成将不会进行。");
                    return;
                }
                imageProcessService.LoadImage(filePath);//载入图片
            }
            else
            {
                if (ditheredBmp == null)
                {
                    Snackbar.MessageQueue.Enqueue("请先添加图片！在此之前，生成将不会进行。");
                    return;
                }
                imageProcessService.LoadImage(ditheredBmp);
            }

            blockInfoService.EnableAllHeight();
            MapGenerator.Generate3D(blockInfoService.GetDatas(), imageProcessService.GetArrayImg());

            Console.WriteLine("2D Test Generated.");
            Generate_finished();
        }

        private void Generate_finished()
        {
            PageTo(2);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (toggle.IsChecked ?? true)
            {
                myGenerate = new Gen(Generate3D);
                Is3dEnabled = true;
                tb_3d.Text = "立体地图画：开启";
            }
            else
            {
                myGenerate = new Gen(Generate2D);
                Is3dEnabled = false;
                tb_3d.Text = "立体地图画：关闭";
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string saveToPath = string.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = ".bmp";
                saveFileDialog.Title = "为地图画保存预览图片";
                saveFileDialog.Filter = "BMP图片(*.bmp)|*.bmp|所有文件(*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = myfileName.Split('.')[0]; //默认文件名

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    saveToPath = saveFileDialog.FileName;
                    Service.PreviewImageService previewImageService = new Service.PreviewImageService();
                    previewImageService.Init(blockInfoService, height, width, MapGenerator.result);
                    if (File.Exists(saveToPath))
                    {
                        File.Delete(saveToPath);
                    }
                    previewImageService.GetBitamp().Save(saveToPath);
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Service.SchematicsService schematicsService = new Service.SchematicsService();
            schematicsService.ReadBlockDatas(blockInfoService.GetChoosenBlocks());
            if (!Is3dEnabled)
            {
                schematicsService.Read2D(MapGenerator.result);
            }
            else
            {
                schematicsService.Read3D(MapGenerator.result, MapGenerator.height);
            }
            schematicsService.Generate();
            string saveToPath = string.Empty;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = ".schem";
                saveFileDialog.Title = "为地图画保存原理图(schematic文件)";
                saveFileDialog.Filter = "schematic文件(*.schem)|*.schem|BKS通用文件(*.bks)|*.bks|所有文件(*.*)|*.* ";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = myfileName.Split('.')[0]; //默认文件名

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    saveToPath = saveFileDialog.FileName;
                    schematicsService.Save(saveToPath);
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }

        bool ditherEnabled = true;
        int ditherType = -1;

        System.Drawing.Bitmap ditheredBmp;
        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectIndex = cb.SelectedIndex;
            //当前选中的文本
            string str = cb.SelectedItem.ToString();
            if (selectIndex==0)
            {
                ditherEnabled = false;
            }
            else
            {
                ditherEnabled = true;
            }
            if (selectIndex!=-1)
            {
                ditherType = selectIndex;
            }
            else
            {
                ditherType = -1;
            }
        }
    }
}
