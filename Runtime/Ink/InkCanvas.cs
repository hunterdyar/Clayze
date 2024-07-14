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
		public List<Stroke> Strokes;

		public Action<Stroke> OnNewStroke;
		public InkManager Manager => _manager;
		private InkManager _manager;
		
		private Stroke[] _activeStrokes = new Stroke[255];
		
		public InkCanvas(InkManager manager)
		{
			_manager = manager;
			Width = Screen.width;
			Height = Screen.height;
			MatrixID = 0;
			Strokes = new List<Stroke>();
			ID = 0;
		}

		public InkCanvas(InkManager manager, byte id, int width, int height, int matrixID)
		{
			_manager = manager;
			Strokes = new List<Stroke>();
			this.Width = width;
			this.Height = height;
			this.MatrixID = matrixID;
			this.ID = id;
		}
		
		public Stroke StartStroke(byte pen, bool local, Color color, float thickness = 1, float pressureControl = 0)
		{
			var s = new Stroke(this, pen, local, thickness, color,pressureControl);
			Strokes.Add(s);
			_activeStrokes[pen] = s;
			if (local)
			{
				_manager.OnNewStrokeLocal(s);
			}
			OnNewStroke?.Invoke(s);
			return s;
		}
		
		public void AddPointFromServer(byte pen, InkPoint point)
		{
			_activeStrokes[pen].AddPoint(point);
		}

		public void AddPointsFromServer(byte pen, InkPoint[] points)
		{
			_activeStrokes[pen].AddPoints(points);
		}
		
		public void EndStroke(byte pen)
		{
			
			if (_activeStrokes[pen].Local)
			{
				_manager.OnStrokeEndLocal(_activeStrokes[pen]);
			}

			_activeStrokes[pen] = null;
		}
	}
}