using System;
using System.Collections.Generic;
using System.Data;
using Connection.Models;
using UnityEngine;

namespace Clayze.Ink
{
	/// <summary>
	/// InkManager collects all of the canvases - and their pen strokes - and manages sending to and from the server.
	/// </summary>
	public class InkManager : MonoBehaviour, IInkMessageHandler
	{
		//current canvas
		//current input
		private Dictionary<byte,InkCanvas> _canvases;
		private void Awake()
		{
			_canvases = new Dictionary<byte, InkCanvas>();
		}

		private void Start()
		{
		
		}

		//need some matrix for origin and scale control
		public bool TryGetCanvas(byte canvasID, out InkCanvas inkCanvas)
		{
			return _canvases.TryGetValue(canvasID, out inkCanvas);
		}

		public InkCanvas CreateCanvas(byte id, int width, int height, int mid)
		{
			var ic = new InkCanvas(width, height, mid);	
			_canvases.Add(id, ic);
			return ic;
		}

		public void OnInkStartFromServer(byte[] data)
		{
			//data[0] is assumed to by 'messageType.InkAdd'
			var canvasID = data[1];
			var penID = data[2];//something unique to separate users, so we can receive multiple users drawing on the same canvas at the same time.
			//float red = data[3..6];
			//float green = data[7..9];
			//fl.oat blue = ...
			var color = Color.black;
			if (TryGetCanvas(canvasID, out var ic))
			{
				ic.StartStroke(penID, color);
			}
		}

		public void OnInkEndFromServer(byte[] data)
		{
			var canvasID = data[1];
			var penID = data[2];
			if (TryGetCanvas(canvasID, out var ic))
			{
				ic.EndStroke(penID);
			}
		}

		public void OnInkAddFromServer(byte[] data)
		{
			//0 is messageType.InkAdd
			var canvasID = data[1];
			var penID = data[2];
			int pointCount = (data.Length - 2) / 8;
			Vector2[] points = new Vector2[pointCount];
			if (TryGetCanvas(canvasID, out var c))
			{
				for (int i = 0; i < pointCount; i++)
				{
					int j = 3 + (i * 8);
					var x = BitConverter.ToSingle(data,j);
					var y = BitConverter.ToSingle(data, j + 4);
					points[i] = new Vector2(x, y);
				}

				if (pointCount == 1)
				{
					c.AddPoint(penID,points[0]);
				}else if (pointCount > 1)
				{
					c.AddPoints(penID,points);
				}
			}
		}

		public void OnInkNewCanvasFromServer(byte[] data)
		{
			var newCanvasID = data[1];
			var width = BitConverter.ToInt32(data, 3);
			var height = BitConverter.ToInt32(data, 7);
			var matrixID = data[11];
			if (_canvases.ContainsKey(newCanvasID))
			{
				Debug.LogError("uhoh, that canvas (canvasID) already exists!");
			}
			else
			{
				_canvases.Add(newCanvasID, new InkCanvas(width, height, matrixID));
			}
		}


		public byte GetUniquePenID()
		{
			//bruh idfk we store an intproperty and count it up?
			return 0;
		}
	}
}