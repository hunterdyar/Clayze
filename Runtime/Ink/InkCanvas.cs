using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class InkCanvas
	{
		public List<Stroke> Strokes;
		
		public InkCanvas()
		{
			Strokes = new List<Stroke>();
		}
		public Stroke StartStroke(Color color, float width = 1)
		{
			var s = new Stroke(this, width,color);
			Strokes.Add(s);
			return s;
		}
	}
}