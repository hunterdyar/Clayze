using System;
using System.Reflection;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	/// <summary>
	/// I didn't feel like cleaning this up into a generic and 2D/3D children. so it's basically two classes in one. its.... fine, whatever. reimplement it yourself.
	/// </summary>
	public class StrokeView : MonoBehaviour
	{
		private Stroke2 _stroke2;
		private Stroke3 _stroke3;

		private LineRenderer _lineRenderer;
		private float _pressureControlAmount = 1;
		private AnimationCurve _widthsCurve = new AnimationCurve();
		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = 0;
			_lineRenderer.useWorldSpace = false;
		}

		public void SetStroke(Stroke2 s)
		{
			_stroke2 = s;
			s.OnPointAdded += OnPointAdded;
			_lineRenderer.startColor = s.Color;
			_lineRenderer.endColor = s.Color;
			_lineRenderer.widthMultiplier = s.Thickness;
		}

		public void SetStroke3(Stroke3 s)
		{
			_stroke3 = s;
			s.OnPointAdded += OnPointAdded3D;
			_lineRenderer.startColor = s.Color;
			_lineRenderer.endColor = s.Color;
			_lineRenderer.widthMultiplier = s.Thickness;
		}

		private void OnPointAdded(InkPoint2 p)
		{
			_lineRenderer.positionCount++;
			var world = Camera.main.ScreenToWorldPoint(new Vector3(p.x,p.y,10));//todo: this is controlled by the canvas, should send canvas local world coordinates.
			_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, world);
			
			RecreateWidths2D();
		}

		private void OnPointAdded3D(InkPoint3 p)
		{
			_lineRenderer.positionCount++;
			_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, p.GetVector3());
			RecreateWidths3D();
		}

		private void RecreateWidths2D()
		{
			if (_pressureControlAmount == 0)
			{
				return;
			}
			//Assuming evenly distributed points, which we CANNOT assume! it's wrong! it only works good-enough because I don't want to track stroke distances
			//(we would track stroke speed, as that affects the drawing, and the math would be annoying.
			_widthsCurve.ClearKeys();
			var c = _stroke2.Points.Count;
			float d = 0;
			for (int i = 0; i < c; i++)
			{
				d += _stroke2.Points[i].DistanceFromPrevious;
				_widthsCurve.AddKey(d/_stroke2.TotalLength, _stroke2.Points[i].Width / (float)255);
			}

			_lineRenderer.widthCurve = _widthsCurve;
		}

		private void RecreateWidths3D()
		{
			if (_pressureControlAmount == 0)
			{
				return;
			}
			
			//Assuming evenly distributed points, which we CANNOT assume! it's wrong! it only works good-enough because I don't want to track stroke distances
			//(we would track stroke speed, as that affects the drawing, and the math would be annoying.
			_widthsCurve.ClearKeys();
			var c = _stroke3.Points.Count;
			float d = 0;
			for (int i = 0; i < c; i++)
			{
				d += _stroke3.Points[i].DistanceFromPrevious;
				_widthsCurve.AddKey(d / _stroke3.TotalLength, _stroke3.Points[i].Width / (float)255);
			}

			_lineRenderer.widthCurve = _widthsCurve;
		}

		private void OnDestroy()
		{
			if (_stroke2 != null)
			{
				_stroke2.OnPointAdded -= OnPointAdded;
			}

			if (_stroke3 != null)
			{
				_stroke3.OnPointAdded -= OnPointAdded3D;
			}
		}
	}
}