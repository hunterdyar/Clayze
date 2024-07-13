using System;
using Clayze.Extensions;
using SyncedProperty;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	//todo: rename this
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