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
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        public InfoPanel()
        {
            InitializeComponent();
        }

        public string Position
        {
            get { return Position_label.Content.ToString(); }
            set { Position_label.Content = value; }
        }

        public string CanvasSize
        {
            get { return Canvas_size_label.Content.ToString(); }
            set { Canvas_size_label.Content = value; }
        }
    }
}
