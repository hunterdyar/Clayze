using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	/// <summary>
	/// A Collection of PenStrokes, and data about the origin/transform.
	/// Overlapping canvases can be used as layers.
	/// Or they can be manipulated through space, as the renderer should respect associated origin data. (wip)
	/// </summary>
	public class InkCanvas
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int MatrixID;
		
		public List<Stroke> Strokes;
		
		private Stroke[] _activeStrokes = new Stroke[255];
		public InkCanvas()
		{
			Width = Screen.width;
			Height = Screen.height;
			MatrixID = 0;
			Strokes = new List<Stroke>();
		}

		public InkCanvas(int width, int height, int matrixID)
		{
			Strokes = new List<Stroke>();
			this.Width = width;
			this.Height = height;
			this.MatrixID = matrixID;
		}

		public Stroke StartStroke(byte pen, Color color, float width = 1)
		{
			var s = new Stroke(this, width,color);
			Strokes.Add(s);
			_activeStrokes[pen] = s;
			return s;
		}

		public void AddPoint(byte pen, Vector2 point)
		{
			_activeStrokes[pen].AddPoint(point);
		}

		public void AddPoints(byte pen, Vector2[] points)
		{
			_activeStrokes[pen].AddPoints(points);
		}
		public void EndStroke(byte pen)
		{
			_activeStrokes[pen] = null;
		}
	}
}