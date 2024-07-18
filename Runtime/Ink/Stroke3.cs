using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class Stroke3
	{
		//events
		public Action<InkPoint3> OnPointAdded;
		public Action OnFinish;
		
		//injected references. local to this unity scene or such.
		public InkManager3D Manager;
		public byte MyPenID;
		
		//unchanging settings
		public Color Color;
		public float Thickness;//base thickness

		public bool Local;
		
		public float TotalLength;
		//modified settings
		public List<InkPoint3> Points;
		
		/// <summary>
		/// Widths are an evenly spaced set of widths, unrelated to a respective point.
		/// This is because they are implemented as an animation curve with the keys generated. MOre sophisticated methods with better width->point connection is possible as a future goal.
		/// </summary>
		public List<byte> Widths;
		
		public Stroke3(InkManager3D manager, byte penID, bool local, float thickness, Color color)
		{
			Color = color;
			this.Thickness = thickness;
			Manager = manager;
			Points = new List<InkPoint3>();
			Widths = new List<byte>();
			Local = local;
			MyPenID = penID;
			
		}

		public void AddPoint(InkPoint3 point3)
		{
			Points.Add(point3);

			if (Points.Count > 1)
			{
				float d = InkPoint3.Distance(Points[^1], Points[^2]);
				if (d == 0)
				{
					Debug.LogWarning("Point added on top of previous point!");
				}
				Points[^1].SetDistance(d);//does this work on real data or iterator variable?
				point3.SetDistance(d);
				TotalLength += d;
			}

			OnPointAdded?.Invoke(point3); //update view
			if (Local)
			{
				//server needs to update
				Manager.OnPointAddLocal(this,point3);//update server
			}
		}

		
		public void UpdateLastPoint(InkPoint3 point2)
		{
			//this makes some assumptions and fucks with syncing, but it will make local feel snappier on higher fps.
			//we will just update the last x points every time on the net, bvecaseu smoothing, so inaccuracy should be removed by that.
			if (Points.Count > 0)
			{
				Points[^1] = point2;
			}
		}

		public void AddPoints(InkPoint3[] points)
		{
			Points.AddRange(points);
			if (Local)
			{
				foreach (var point in points)
				{
					OnPointAdded?.Invoke(point);
					Manager.OnPointAddLocal(this,point);
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
			if (Local)
			{
				Manager.SendStokeEndFromLocal(this);
			}
			OnFinish?.Invoke();
		}
	}
}