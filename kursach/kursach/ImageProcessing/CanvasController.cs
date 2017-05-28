using kursach.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
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
		private Canvas Canvas { get; set; }
		private List<CanvasStateWithPrimitives> States { get; set; }
		private int currentStateIndex = 0;

		public CanvasController(Canvas canvas)
		{
			Canvas = canvas;
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

			States.Add(new CanvasStateWithPrimitives(Canvas.Clone(), new List<UIElement>()));
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
			
			States.Add(new CanvasStateWithPrimitives (Canvas.Clone(), States.Count> 0? States.Last().Primitives.CloneCollection() : new List<UIElement>()));
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
			return Canvas;
		}

		private void SetupMainCanvas(CanvasStateWithPrimitives canvasState)
		{
			Canvas.Width = canvasState.Canvas.Width;
			Canvas.Height = canvasState.Canvas.Height;
			Canvas.Background = canvasState.Canvas.Background;
			Canvas.Children.Clear();
			for(int i = 0; i < canvasState.Primitives.Count; i++)
			{
				//var childCloned = canvas.Children[i].Clone();
				//Canvas.Children.Add(childCloned);

				Canvas.Children.Add(canvasState.Primitives[i]);
			}
		}

		internal void UpdateCanvas(object toBitmap)
		{
			throw new NotImplementedException();
		}
	}
}
