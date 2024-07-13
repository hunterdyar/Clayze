using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class Stroke
	{
		//events
		public Action<Vector2> OnPointAdded;
		//injected references. local to this unity scene or such.
		public InkCanvas MyCanvas;
		public byte MyPenID;
		
		//unchanging settings
		public Color Color;
		public float Thickness;

		public bool Local;
		//modified settings
		public List<Vector2> Points;
		
		
		/// <summary>
		/// Widths are an evenly spaced set of widths, unrelated to a respective point.
		/// This is because they are implemented as an animation curve with the keys generated. MOre sophisticated methods with better width->point connection is possible as a future goal.
		/// </summary>
		public List<float> Widths;
		
		public Stroke(InkCanvas canvas, byte penID, bool local, float thickness, Color color)
		{
			Color = color;
			this.Thickness = thickness;
			MyCanvas = canvas;
			Points = new List<Vector2>();
			Widths = new List<float>();
			Local = local;
			MyPenID = penID;
		}

		public void AddPoint(Vector3 point, float width = 1)
		{
			Points.Add(point);
			Widths.Add(width);
			OnPointAdded?.Invoke(point); //update view
			if (Local)
			{
				//server needs to update
				MyCanvas.Manager.OnPointAddLocal(this,point);//update server
			}
		}

		public void AddPoint(float x, float y, float width = 1)
		{
			var p = new Vector2(x, y);
			Points.Add(p);
			Widths.Add(width);
			OnPointAdded?.Invoke(p); //update view;
			if (Local)
			{
				MyCanvas.Manager.OnPointAddLocal(this,p);//update server
			}
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
			if (Local)
			{
				foreach (var point in points)
				{
					OnPointAdded?.Invoke(point);
					MyCanvas.Manager.OnPointAddLocal(this,point);
				}
			}
			else
			{
				foreach (var point in points)
				{
					OnPointAdded?.Invoke(point);
				}
			}
		}

		public void Finish()
		{
			MyCanvas.EndStroke(MyPenID);
		}
	}
}