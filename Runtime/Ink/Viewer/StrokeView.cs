using System;
using System.Reflection;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class StrokeView : MonoBehaviour
	{
		private Stroke _stroke;
		private LineRenderer _lineRenderer;
		private float _pressureControlAmount = 1;
		private AnimationCurve _widthsCurve = new AnimationCurve();
		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = 0;
			_lineRenderer.useWorldSpace = false;
		}

		public void SetStroke(Stroke s)
		{
			_stroke = s;
			s.OnPointAdded += OnPointAdded;
			_lineRenderer.startColor = s.Color;
			_lineRenderer.endColor = s.Color;
			_lineRenderer.widthMultiplier = s.Thickness;
		}

		private void OnPointAdded(InkPoint p)
		{
			_lineRenderer.positionCount++;
			var world = Camera.main.ScreenToWorldPoint(new Vector3(p.x,p.y,10));//todo: this is controlled by the canvas, should send canvas local world coordinates.
			_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, world);
			
			RecreateWidths();
		}

		private void RecreateWidths()
		{
			if (_pressureControlAmount == 0)
			{
				return;
			}
			//Assuming evenly distributed points, which we CANNOT assume! it's wrong! it only works good-enough because I don't want to track stroke distances
			//(we would track stroke speed, as that affects the drawing, and the math would be annoying.
			_widthsCurve.ClearKeys();
			var c = _stroke.Points.Count;
			float d = 0;
			for (int i = 0; i < c; i++)
			{
				d += _stroke.Points[i].DistanceFromPrevious;
				_widthsCurve.AddKey(d/_stroke.TotalLength, _stroke.Points[i].Width / (float)255);
			}

			_lineRenderer.widthCurve = _widthsCurve;
		}

		private void OnDestroy()
		{
			_stroke.OnPointAdded -= OnPointAdded;
		}
	}
}