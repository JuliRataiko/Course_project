using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace kursach.Core
{
	public class CanvasStateWithPrimitives
	{
		public Canvas Canvas{ get; set; }
		public List<UIElement> Primitives{ get; set; }

		public CanvasStateWithPrimitives()
		{
			Primitives = new List<UIElement>();
		}

		public CanvasStateWithPrimitives(Canvas canvas, List<UIElement> primitives)
		{
			this.Canvas = canvas;
			this.Primitives = primitives;
		}
	}
}
