using System;
using Clayze.Extensions;
using SyncedProperty;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clayze.Ink.Viewer
{
	//todo: rename this
	public class CanvasesViewer : MonoBehaviour
	{
		[FormerlySerializedAs("_inkManager")] [SerializeField] private InkManager2D inkManager2D;
		[SerializeField] private InkCanvasViewer _inkCanvasViewerPrefab;

		private void OnEnable()
		{
			inkManager2D.OnNewCanvas += OnNewCanvas;
		}

		private void OnDisable()
		{
			inkManager2D.OnNewCanvas -= OnNewCanvas;
		}

		private void OnNewCanvas(InkCanvas canvas)
		{
			var viewer = Instantiate(_inkCanvasViewerPrefab, transform);
			viewer.SetCanvas(canvas);
		}
	}
}