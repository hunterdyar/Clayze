using System;
using Clayze.Extensions;
using SyncedProperty;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class InkCanvasViewer : MonoBehaviour
	{
		private InkCanvas _canvas;
		[SerializeField] public StrokeView _viewPrefab;
		private SyncTransformMatrix _matrix;

		public void SetCanvas(InkCanvas canvas)
		{
			_canvas = canvas;
			_canvas.OnNewStroke += OnNewStroke;
			gameObject.name = "Canvas " + _canvas.ID;
			//todo: move this into a utility function in canvas.
			_matrix = canvas.Manager.GetCanvasMatrix(canvas.ID);
			if (_matrix != null)
			{
				_matrix.OnChange += OnMatrixChange;
			}
		}

		private void OnMatrixChange(Matrix4x4 matrix)
		{
			transform.SetFromMatrix(matrix);
		}
		

		private void OnNewStroke(Stroke s)
		{
			var sv = Instantiate(_viewPrefab, transform);
			sv.SetStroke(s);
		}

		private void Update()
		{
			OutgoingSyncTick();
		}

		private void OnDestroy()
		{
			_canvas.OnNewStroke -= OnNewStroke;
		}
		
		

		private void OutgoingSyncTick()
		{
			if (_matrix != null && _matrix.IsOwner)
			{
				if (_matrix.Value != transform.localToWorldMatrix)
				{
					_matrix.SetValue(transform.localToWorldMatrix);
				}
			}
		}
	}
}