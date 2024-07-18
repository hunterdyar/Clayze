using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink.Viewer
{
	public class Stroke3DViewer : MonoBehaviour
	{
		[SerializeField] private InkManager3D _manager;
		public Transform strokeViewParent;
		public StrokeView strokeViewPrefab;

		public List<Stroke3> Strokes = new List<Stroke3>();

		private void OnEnable()
		{
			_manager.OnNewStroke+= OnNewStroke;
		}

		private void OnDisable()
		{
			_manager.OnNewStroke -= OnNewStroke;
		}

		private void OnNewStroke(Stroke3 s)
		{
			Strokes.Add(s);
			var sv = Instantiate(strokeViewPrefab, strokeViewParent);
			sv.SetStroke3(s);
		}
	}
}