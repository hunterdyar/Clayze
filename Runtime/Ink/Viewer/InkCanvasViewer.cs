using System;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class InkCanvasViewer : MonoBehaviour
	{
		private InkCanvas _canvas;
		[SerializeField] public StrokeView _viewPrefab;
		public void SetCanvas(InkCanvas canvas)
		{
			_canvas = canvas;
			_canvas.OnNewStroke += OnNewStroke;
			gameObject.name = "Canvas " + _canvas.ID;
		}

		private void OnNewStroke(Stroke s)
		{
			var sv = Instantiate(_viewPrefab, transform);
			sv.SetStroke(s);
		}
		
		private void OnDestroy()
		{
			_canvas.OnNewStroke -= OnNewStroke;
		}
	}
}