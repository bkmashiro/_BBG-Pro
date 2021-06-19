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
        }
        Service.BlockInfoService blockInfoService = new Service.BlockInfoService();
        bool BlockInfoServiceInited = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BlockInfoServiceInited)
            {
                PageTo(1);
            }
            else
            {
                blockInfoService.Init();
                blockInfoService.EnableAllHeight();
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
            GoBack.Visibility = Visibility.Collapsed;
        }
    }
}
