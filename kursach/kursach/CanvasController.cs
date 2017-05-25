using kursach.Core;
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
		private List<CanvasStateWithPrimitives> States { get; set; }
		private int currentStateIndex = 0;

		public CanvasController(NewWindow controlledWindow)
		{
			ControlledWindow = controlledWindow;
			States = new List<CanvasStateWithPrimitives>();
		}

		public void UndoAllChanges(BitmapSource originalImage)
		{
			currentStateIndex = 0;
			States.Clear();
			UpdateCanvas(originalImage);
		}

		public int GetNumberOfChanges()
		{
			return States.Count;
		}

		public void UpdateCanvas(BitmapSource newImage)
		{
			if (States.Count != 0)
				if (currentStateIndex != States.Count - 1)
				{
					States.RemoveRange(currentStateIndex, States.Count - currentStateIndex - 1);
				} 

			States.Add(new CanvasStateWithPrimitives(ControlledWindow.Canvas.Clone(), new List<UIElement>()));
			currentStateIndex = States.Count - 1;
			States[currentStateIndex].Canvas.Background = new ImageBrush { ImageSource = newImage };
			SetupMainCanvas(States[currentStateIndex]);
		}

		public void UpdateCanvas<T>(T newChild) where T : UIElement
		{
			if (States.Count != 0)
				if (currentStateIndex != States.Count - 1)
				{
					States.RemoveRange(currentStateIndex + 1, States.Count - currentStateIndex - 1);
				}
			
			States.Add(new CanvasStateWithPrimitives (ControlledWindow.Canvas.Clone(), States.Count> 0? States.Last().Primitives.CloneCollection() : new List<UIElement>()));
			currentStateIndex = States.Count - 1;
			States[currentStateIndex].Primitives.Add(newChild);
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

		public Canvas GetActualCanvas()
		{
			return ControlledWindow.Canvas;
		}

		private void SetupMainCanvas(CanvasStateWithPrimitives canvasState)
		{
			ControlledWindow.Canvas.Width = canvasState.Canvas.Width;
			ControlledWindow.Canvas.Height = canvasState.Canvas.Height;
			ControlledWindow.Canvas.Background = canvasState.Canvas.Background;
			ControlledWindow.Canvas.Children.Clear();
			for(int i = 0; i < canvasState.Primitives.Count; i++)
			{
				//var childCloned = canvas.Children[i].Clone();
				//ControlledWindow.Canvas.Children.Add(childCloned);

				ControlledWindow.Canvas.Children.Add(canvasState.Primitives[i]);
			}
		}
	}
}
