using System.Windows;
using System.Windows.Controls;
namespace Maestro.UI
{
    /// <summary>
    /// Interaction logic for Shapes.xaml
    /// </summary>
    public partial class Shapes : UserControl
    {
        public Shapes()
        {
            InitializeComponent();
        }

        private void line_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Line;
        }

        private void ellipse_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Ellipse;
        }

        private void rectangle_button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Tool = Utils.Tools.Rectangle;
        }
    }
}
