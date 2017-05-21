using Maestro.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace kursach
{
	public class CanvasController
	{
		private NewWindow ControlledWindow { get; set; }
		private List<Canvas> States { get; set; }
		private int currentStateIndex = 0;

		public CanvasController(NewWindow controlledWindow)
		{
			ControlledWindow = controlledWindow;
			States = new List<Canvas>();
		}

		public void UpdateCanvas(BitmapSource newImage)
		{
			if (States.Count != 0)
				if (currentStateIndex != States.Count - 1)
				{
					States.RemoveRange(currentStateIndex, States.Count - currentStateIndex - 1);
				}

			States.Add(ControlledWindow.Canvas.Clone());
			currentStateIndex = States.Count - 1;
			States[currentStateIndex].Background = new ImageBrush { ImageSource = newImage };
			SetupMainCanvas(States[currentStateIndex]);
		}

		public void UndoChanges()
		{
			if (currentStateIndex != 0)
			{
				currentStateIndex--;
				//States.Remove(States.Last());
				SetupMainCanvas(States[currentStateIndex]);
			}
		}

		public void RedoChanges()
		{
			if (currentStateIndex + 1 < States.Count)
			{
				currentStateIndex++;
				//States.Remove(States.Last());
				SetupMainCanvas(States[currentStateIndex]);
			}
		}

		private void SetupMainCanvas(Canvas canvas)
		{
			ControlledWindow.Canvas.Width = canvas.Width;
			ControlledWindow.Canvas.Height = canvas.Height;
			ControlledWindow.Canvas.Background = canvas.Background;
			ControlledWindow.Canvas.Children.Clear();
			foreach (var child in canvas.Children)
			{
				ControlledWindow.Canvas.Children.Add((UIElement)child);
			}
		}
	}
}
