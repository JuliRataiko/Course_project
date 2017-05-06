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
using System.Windows.Shapes;

namespace kursach
{
    /// <summary>
    /// Логика взаимодействия для NewWindow.xaml
    /// </summary>
    public partial class NewWindow : Window
    {
        public NewWindow()
        {
            InitializeComponent();
        }
        private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Just making sure the popup has opened.");
        }

        private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Just making sure the popup has closed.");
        }
    }
}
