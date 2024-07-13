using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class Stroke
	{
		//injected references. local to this unity scene or such.
		public InkCanvas MyCanvas;
		
		//unchanging settings
		public Color Color;
		public float Thickness;

		//modified settings
		public List<Vector2> Points;
		
		/// <summary>
		/// Widths are an evenly spaced set of widths, unrelated to a respective point.
		/// This is because they are implemented as an animation curve with the keys generated. MOre sophisticated methods with better width->point connection is possible as a future goal.
		/// </summary>
		public List<float> Widths;
		
		public Stroke(InkCanvas canvas, float thickness, Color color)
		{
			Color = color;
			this.Thickness = thickness;
			MyCanvas = canvas;
			Points = new List<Vector2>();
			Widths = new List<float>();
		}

		public void AddPoint(Vector3 point, float width = 1)
		{
			Points.Add(point);
			Widths.Add(width);
		}

		public void AddPoint(float x, float y, float width = 1)
		{
			Points.Add(new Vector2(x,y));
			Widths.Add(width);
		}

		public void UpdateLastPoint(float xValue, float yValue, float pressureValue)
		{
			//this makes some assumptions and fucks with syncing, but it will make local feel snappier on higher fps.
			//we will just update the last x points every time on the net, bvecaseu smoothing, so inaccuracy should be removed by that.
			if (Points.Count > 0)
			{
				Points[^1] = new Vector2(xValue,yValue);
				Widths[^1] = pressureValue;
			}
		}

		public void AddPoints(Vector2[] points)
		{
			Points.AddRange(points);
			//update
		}
	}
}