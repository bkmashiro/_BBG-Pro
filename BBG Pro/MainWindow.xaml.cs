using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BBG_Pro
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Main_frame_Loaded(object sender, RoutedEventArgs e)
        {
            //加载welcome
            //LoadPageTo(@"Pages\Welcome.xaml", Main_frame);
            LoadPageTo(@"Pages\Generate.xaml", Main_frame);
            //LoadChooseBlockPage();
        }
     
        private void LoadPageTo(string uri, Frame frame)
        {
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frame.Navigate(new Uri(uri, UriKind.Relative));

        }
        private void LoadPageTo(Page page, Frame frame)
        {
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frame.Navigate(page);
        }

        private void LoadChooseBlockPage()
        {
            Service.BlockInfoService blockInfoService = new Service.BlockInfoService();
            blockInfoService.Init();//初始化
            blockInfoService.EnableHeight(Service.BlockInfoService.HeightState.flat);//仅载入平面的
            ChooseBlock chooseBlock = new ChooseBlock(blockInfoService);
            LoadPageTo(chooseBlock, Main_frame);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Service.BlockInfoService blockInfoService = new Service.BlockInfoService();
            blockInfoService.Init();//初始化
            blockInfoService.EnableHeight(Service.BlockInfoService.HeightState.flat);//仅载入平面的

            Service.MapGenerator mapGenerator = new Service.MapGenerator();
            Service.ImageProcessService imageProcessService = new Service.ImageProcessService();

            imageProcessService.LoadImage(@"C:\Users\Administrator\Desktop\test.gif");//载入图片

            mapGenerator.Generate(blockInfoService.GetChoosenBlocks(), imageProcessService.GetArrayImg());

            Service.SchematicsService schematicsService = new Service.SchematicsService();

            schematicsService.Read2D(mapGenerator.result);
            schematicsService.ReadBlockDatas(blockInfoService.GetChoosenBlocks());

            schematicsService.Generate();
            schematicsService.Save();


            Console.WriteLine("2D Test Generated.");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Service.BlockInfoService blockInfoService = new Service.BlockInfoService();
            blockInfoService.Init();//初始化
            blockInfoService.EnableAllHeight();//载入三维数据
            //blockInfoService.EnableHeight(Service.BlockInfoService.HeightState.higher);//仅更高
            Service.MapGenerator mapGenerator = new Service.MapGenerator();
            Service.ImageProcessService imageProcessService = new Service.ImageProcessService();

            imageProcessService.LoadImage(@"C:\Users\Administrator\Desktop\test.gif");//载入图片

            mapGenerator.Generate3D(blockInfoService.GetChoosenBlocks(), imageProcessService.GetArrayImg());

            Service.SchematicsService schematicsService = new Service.SchematicsService();

            schematicsService.Read3D(mapGenerator.result, mapGenerator.height);
            schematicsService.ReadBlockDatas(blockInfoService.GetChoosenBlocks());

            schematicsService.Generate();
            schematicsService.Save();


            Console.WriteLine("3D Test Generated.");
        }
    }
}
