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

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MessageView messageview;
        public MainWindow()
        {
            InitializeComponent();

            messageview = new MessageView();

            DataContext = messageview;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            messageview.StopServer();
        }
    }
}
