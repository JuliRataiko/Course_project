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

namespace Maestro.UI
{
    /// <summary>
    /// Interaction logic for Tools.xaml
    /// </summary>
    public partial class Tools : UserControl
    {
        public Tools()
        {
            InitializeComponent();
        }

        private void pencil_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Pencil;
        }

        private void eraser_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Eraser;
        }

        private void color_picker_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.ColorPicker;
        }

        private void magnifier_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Magnifier;
            MessageBox.Show("Press 'W' for zoom in and 'Q' for zoom out.",
                "instruction");
        }

        private void fill_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Fill;
        }

        private void text_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Text;
        }

    }
}
