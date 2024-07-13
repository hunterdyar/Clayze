using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Ink
{
	public class InkManager : MonoBehaviour
	{
		//current canvas
		//current input
		private Dictionary<byte,InkCanvas> _canvases;

		public bool createCanvasZeroOnStart = true;
		private void Awake()
		{
			_canvases = new Dictionary<byte, InkCanvas>();
		}

		private void Start()
		{
			if (createCanvasZeroOnStart)
			{
				GetOrCreateCanvas(0);
			}
		}

		public InkCanvas GetOrCreateCanvas(byte canvasID)
		{
			if (_canvases.TryGetValue(canvasID, out var canvas))
			{
				return canvas;
			}
			else
			{
				canvas = new InkCanvas();
				_canvases.Add(canvasID,canvas);
				return canvas;
			}
		
		}
	}
}