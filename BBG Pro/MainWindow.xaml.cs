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
            LoadPageTo(@"Pages\Welcome.xaml", Main_frame);
        }

        private void LoadPageTo(string uri ,Frame frame)
        {
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frame.Navigate(new Uri(uri,UriKind.Relative));

        }
    }
}
