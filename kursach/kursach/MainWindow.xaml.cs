using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

using Path = System.IO.Path;

namespace kursach
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryInfo currentDirectory;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewWindow newW = new NewWindow();

            newW.ShowDialog();
        }

        private void Button_left_enter(object sender, MouseEventArgs e)
        {
            this.left.Visibility = System.Windows.Visibility.Collapsed;


        }

        private void Button_left_leave(object sender, MouseEventArgs e)
        {
            this.left.Visibility = System.Windows.Visibility.Visible;
        }

        private void Open_item_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ViewedPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }

            var directory = new FileInfo(openFileDialog.FileName).Directory;
            string[] supportedExtensions = new[] { ".bmp", ".jpeg", ".jpg", ".png", ".tiff" };
            //var files = Directory.GetFiles(System.IO.Path.Combine(root), "*.*").Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
            var files = Directory.GetFiles(directory.FullName).Select(f => new FileInfo(f)).Where(f => supportedExtensions.Contains(f.Extension)).Select(f => f.FullName);

            List<ImageDetails> images = new List<ImageDetails>();

            foreach (var file in files)
            {
                ImageDetails id = new ImageDetails()
                {
                    Path = file,
                    FileName = Path.GetFileName(file),
                    Extension = Path.GetExtension(file)
                };

                BitmapImage img = new BitmapImage(new Uri(file));
                //img.BeginInit();
                //img.CacheOption = BitmapCacheOption.OnLoad;
                //img.UriSource = new Uri(file, UriKind.Absolute);
                //img.EndInit();
                id.Width = img.PixelWidth;
                id.Height = img.PixelHeight;

                FileInfo fi = new FileInfo(file);
                id.Size = fi.Length;
                images.Add(id);
            }

            ImageList.ItemsSource = images;
        }

        public void SelectedAnotherImage(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            var img = (sender as ListBox).SelectedItems.Cast<ImageDetails>().First();

            ViewedPhoto.Source = new BitmapImage(new Uri(img.Path));
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
