using System;
using System.Collections.Generic;
using SyncedProperty;
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
		public readonly int MatrixID;//
		public readonly byte ID;
		public List<Stroke2> Strokes;

		public Action<Stroke2> OnNewStroke;
		public InkManager2D Manager2D => _manager2D;
		private InkManager2D _manager2D;
		
		private Stroke2[] _activeStrokes = new Stroke2[255];
		
		public InkCanvas(InkManager2D manager2D)
		{
			_manager2D = manager2D;
			Width = Screen.width;
			Height = Screen.height;
			MatrixID = 0;
			Strokes = new List<Stroke2>();
			ID = 0;
		}

		public InkCanvas(InkManager2D manager2D, byte id, int width, int height, int matrixID)
		{
			_manager2D = manager2D;
			Strokes = new List<Stroke2>();
			this.Width = width;
			this.Height = height;
			this.MatrixID = matrixID;
			this.ID = id;
		}
		
		public Stroke2 StartStroke(byte pen, bool local, Color color, float thickness = 1)
		{
			var s = new Stroke2(this, pen, local, thickness, color);
			Strokes.Add(s);
			_activeStrokes[pen] = s;
			if (local)
			{
				_manager2D.OnNewStrokeLocal(s);
			}
			OnNewStroke?.Invoke(s);
			return s;
		}
		
		public void AddPointFromServer(byte pen, InkPoint2 point2)
		{
			_activeStrokes[pen].AddPoint(point2);
		}

		public void AddPointsFromServer(byte pen, InkPoint2[] points)
		{
			_activeStrokes[pen].AddPoints(points);
		}
		
		public void EndStroke(byte pen)
		{
			
			if (_activeStrokes[pen].Local)
			{
				_manager2D.OnStrokeEndLocal(_activeStrokes[pen]);
			}

			_activeStrokes[pen] = null;
		}
	}
}