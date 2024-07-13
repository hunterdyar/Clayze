using System;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class CanvasesViewer : MonoBehaviour
	{
		[SerializeField] private InkManager _inkManager;
		[SerializeField] private InkCanvasViewer _inkCanvasViewerPrefab;
		private void OnEnable()
		{
			_inkManager.OnNewCanvas += OnNewCanvas;
		}

		private void OnDisable()
		{
			_inkManager.OnNewCanvas -= OnNewCanvas;
		}

		private void OnNewCanvas(InkCanvas canvas)
		{
			var viewer = Instantiate(_inkCanvasViewerPrefab, transform);
			viewer.SetCanvas(canvas);
		}
	}
}