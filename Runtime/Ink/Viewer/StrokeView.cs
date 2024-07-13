using System;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class StrokeView : MonoBehaviour
	{
		private Stroke _stroke;
		private LineRenderer _lineRenderer;

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = 0;
			_lineRenderer.widthMultiplier = 0.04f;
		}

		public void SetStroke(Stroke s)
		{
			_stroke = s;
			s.OnPointAdded += OnPointAdded;
			_lineRenderer.startColor = s.Color;
			_lineRenderer.endColor = s.Color;
		}

		private void OnPointAdded(Vector2 p)
		{
			_lineRenderer.positionCount++;
			var world = Camera.main.ScreenToWorldPoint(new Vector3(p.x,p.y,10));//todo: this is controlled by the canvas, should send canvas local world coordinates.
			_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, world);
		}

		private void OnDestroy()
		{
			_stroke.OnPointAdded -= OnPointAdded;
		}
	}
}