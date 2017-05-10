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
		private LinkedList<ImageDetails> images = new LinkedList<ImageDetails>();
		private int selectedImageIndex;

		public MainWindow()
		{
			InitializeComponent();
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
			var files = Directory.GetFiles(directory.FullName).Select(f => new FileInfo(f)).Where(f => supportedExtensions.Contains(f.Extension)).Select(f => f.FullName);

			images.Clear();

			foreach (var file in files)
			{
				ImageDetails id = new ImageDetails()
				{
					Path = file,
					FileName = Path.GetFileName(file),
					Extension = Path.GetExtension(file)
				};

				BitmapImage img = new BitmapImage(new Uri(file));
				id.Width = img.PixelWidth;
				id.Height = img.PixelHeight;

				images.AddLast(id);
			}

			ImageList.ItemsSource = null;
			ImageList.ItemsSource = images;
			selectedImageIndex = images.TakeWhile(i => !(i.Path == openFileDialog.FileName)).Count();
			ImageList.SelectedIndex = selectedImageIndex;
		}

		public void SelectedAnotherImage(object sender, SelectionChangedEventArgs args)
		{
			if ((sender as ListBox).SelectedItems.Count == 0)
			{
				return;
			}

			ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
			var img = (sender as ListBox).SelectedItems.Cast<ImageDetails>().First();

			ViewedPhoto.Source = new BitmapImage(new Uri(img.Path));
			selectedImageIndex = images.TakeWhile(n => !n.Equals(img)).Count();
			SetLeftRightButtonsState();
		}

		private void Left_Click(object sender, RoutedEventArgs e)
		{
			selectedImageIndex--;
			ViewedPhoto.Source = new BitmapImage(new Uri(images.ElementAt(selectedImageIndex).Path));
			SetLeftRightButtonsState();
			ImageList.SelectedIndex = selectedImageIndex;
		}

		private void Left_MouseEnter(object sender, RoutedEventArgs e)
		{
			left.Opacity = 1;
		}

		private void Left_MouseLeave(object sender, RoutedEventArgs e)
		{
			left.Opacity = 0.01;
		}

		private void Right_Click(object sender, RoutedEventArgs e)
		{
			selectedImageIndex++;
			ViewedPhoto.Source = new BitmapImage(new Uri(images.ElementAt(selectedImageIndex).Path));
			SetLeftRightButtonsState();
			ImageList.SelectedIndex = selectedImageIndex;
		}

		private void Right_MouseEnter(object sender, RoutedEventArgs e)
		{
			right.Opacity = 1;
		}

		private void Right_MouseLeave(object sender, RoutedEventArgs e)
		{
			right.Opacity = 0.01;
		}

		private void SetLeftRightButtonsState()
		{
			if (selectedImageIndex == 0)
			{
				left.IsEnabled = false;
				right.IsEnabled = true;
				return;
			}

			if (selectedImageIndex == images.Count - 1)
			{
				right.IsEnabled = false;
				left.IsEnabled = true;
				return;
			}

			right.IsEnabled = true;
			left.IsEnabled = true;
		}

		private void Edit_Click(object sender, RoutedEventArgs ar)
		{
			if (images.Count == 0) return;

			NewWindow newW = new NewWindow(new BitmapImage(new Uri(images.ElementAt(selectedImageIndex).Path)));
			newW.WindowState = WindowState.Maximized;
			newW.ShowDialog();
		}
	}
}
