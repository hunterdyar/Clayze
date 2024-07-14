using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class Stroke
	{
		//events
		public Action<InkPoint> OnPointAdded;
		//injected references. local to this unity scene or such.
		public InkCanvas MyCanvas;
		public byte MyPenID;
		
		//unchanging settings
		public Color Color;
		public float Thickness;//base thickness
		public float PressureControlPercentage = 1;

		public bool Local;
		
		public float TotalLength;
		//modified settings
		public List<InkPoint> Points;
		
		/// <summary>
		/// Widths are an evenly spaced set of widths, unrelated to a respective point.
		/// This is because they are implemented as an animation curve with the keys generated. MOre sophisticated methods with better width->point connection is possible as a future goal.
		/// </summary>
		public List<byte> Widths;
		
		public Stroke(InkCanvas canvas, byte penID, bool local, float thickness, Color color, float pressureControlPercentage)
		{
			Color = color;
			this.Thickness = thickness;
			this.PressureControlPercentage = Mathf.Clamp01(pressureControlPercentage);
			MyCanvas = canvas;
			Points = new List<InkPoint>();
			Widths = new List<byte>();
			Local = local;
			MyPenID = penID;
			
		}

		public void AddPoint(InkPoint point)
		{
			Points.Add(point);

			if (Points.Count > 1)
			{
				float d = InkPoint.Distance(Points[^1], Points[^2]);
				Points[^1].SetDistance(d);//does this work on real data or iterator variable?
				TotalLength += d;
			}

			OnPointAdded?.Invoke(point); //update view
			if (Local)
			{
				//server needs to update
				MyCanvas.Manager.OnPointAddLocal(this,point);//update server
			}
		}

		
		public void UpdateLastPoint(InkPoint point)
		{
			//this makes some assumptions and fucks with syncing, but it will make local feel snappier on higher fps.
			//we will just update the last x points every time on the net, bvecaseu smoothing, so inaccuracy should be removed by that.
			if (Points.Count > 0)
			{
				Points[^1] = point;
			}
		}

		public void AddPoints(InkPoint[] points)
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

		public static byte WidthByteFromFloat(float f)
		{
			return (byte)(Mathf.Clamp01(f) * 255);
		}

		public static float WidthFloatFromByte(byte w)
		{
			return (float)w / 255f;
		}
	}
}