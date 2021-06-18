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
            BlockInfoService_ = blockInfoService_;
        }

        bool viewMode = true;//颜色视图

        public BlockInfoService BlockInfoService_ { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewMode)
            {
                ChangeLayoutToClassMode();
                (sender as Button).Content = "颜色视图";
                viewMode = false;
            }
            else
            {
                ChangeLayoutToColorMode();
                (sender as Button).Content = "方块视图";
                viewMode = true;
            }
        }

        private void ChangeLayoutToColorMode()
        {
            ClearLayout();

        }
        private void ChangeLayoutToClassMode()
        {
            ClearLayout();

        }

        private void ClearLayout()
        {
            block_list.Items.Clear();
        }
    }
}
