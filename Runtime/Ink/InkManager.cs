using System;
using System.Collections.Generic;
using System.Data;
using Connection.Models;
using SyncedProperty;
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
		public Action<InkCanvas> OnNewCanvas;
		private Dictionary<byte,InkCanvas> _canvases;
		[SerializeField] private SyncedPropertyCollection _propertyCollection;
		private Dictionary<Stroke, List<InkPoint>> _addBuffer = new Dictionary<Stroke, List<InkPoint>>();
		private bool _canAdd = true;
		private byte _lastPenID = 0;

		public SyncTransformMatrix[] CanvasTransforms;
		private void Awake()
		{
			_canvases = new Dictionary<byte, InkCanvas>();
		}

		private void Start()
		{
			_propertyCollection.SetInkMessageHandler(this);
		}

		//need some matrix for origin and scale control
		public bool TryGetCanvas(byte canvasID, out InkCanvas inkCanvas)
		{
			return _canvases.TryGetValue(canvasID, out inkCanvas);
		}

		public InkCanvas CreateCanvas(byte id, int width, int height, int mid)
		{
			var ic = new InkCanvas(this,id,width, height, mid);	
			_canvases.Add(id, ic);
			OnNewCanvas?.Invoke(ic);
			return ic;
		}

		private void Update()
		{
			if (_canAdd)
			{
				foreach (var kvp in _addBuffer)
				{
					if (kvp.Value.Count > 0)
					{
						//send add of points array to server.
						var points = kvp.Value;
						var data = new byte[3 + (points.Count * 9)];
						data[0] = (byte)MessageType.InkAdd;
						data[1] = kvp.Key.MyCanvas.ID;
						data[2] = kvp.Key.MyPenID;
						for (int i = 0; i < points.Count; i++)
						{
							var x = BitConverter.GetBytes(points[i].x);
							var y = BitConverter.GetBytes(points[i].y);
							data[3 + (i * 9)] = x[0];
							data[4 + (i * 9)] = x[1];
							data[5 + (i * 9)] = x[2];
							data[6 + (i * 9)] = x[3];
							data[7 + (i * 9)] = y[0];
							data[8 + (i * 9)] = y[1];
							data[9 + (i * 9)] = y[2];
							data[10 + (i * 9)] = y[3];
							data[11 + (i * 9)] = points[i].Width;

						}
						_propertyCollection.SendMessageRaw(data);
						//is this an iterator or real reference?
						kvp.Value.Clear();
					}
				}
			}
		}

		public void OnInkStartFromServer(byte[] data)
		{
			//data[0] is assumed to by 'messageType.InkAdd'
			var canvasID = data[1];
			var penID = data[2];//something unique to separate users, so we can receive multiple users drawing on the same canvas at the same time.
			if (penID > _lastPenID)
			{
				_lastPenID = penID;
			}
			float red = BitConverter.ToSingle(data, 3);
			float green = BitConverter.ToSingle(data, 7);
			float blue = BitConverter.ToSingle(data, 11);
			float thickness = BitConverter.ToSingle(data, 15);
			var color = new Color(red, green, blue, 1);
			if (TryGetCanvas(canvasID, out var ic))
			{
				ic.StartStroke(penID, false, color, thickness);
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
			int pointCount = (data.Length - 2) / 9;
			InkPoint[] points = new InkPoint[pointCount];
			if (TryGetCanvas(canvasID, out var c))
			{
				for (int i = 0; i < pointCount; i++)
				{
					int j = 3 + (i * 9);
					var x = BitConverter.ToSingle(data,j);
					var y = BitConverter.ToSingle(data, j + 4);
					var w = data[j + 8];
					points[i] = new InkPoint(x, y,w);
				}

				if (pointCount == 1)
				{
					c.AddPointFromServer(penID,points[0]);
				}else if (pointCount > 1)
				{
					c.AddPointsFromServer(penID,points);
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
				_canvases.Add(newCanvasID, new InkCanvas(this,newCanvasID,width, height, matrixID));
			}
		}

		public void OnInkAddConfirm()
		{
			_canAdd = true;
		}

		public void OnNewStrokeLocal(Stroke s)
		{
			var data = new byte[19];
			data[0] = (byte)MessageType.InkStart;
			data[1] = (byte)s.MyCanvas.ID;
			data[2] = s.MyPenID;
			Array.Copy(BitConverter.GetBytes(s.Color.r),0,data,3,4);
			Array.Copy(BitConverter.GetBytes(s.Color.g), 0, data, 7, 4);
			Array.Copy(BitConverter.GetBytes(s.Color.b), 0, data, 11, 4);
			Array.Copy(BitConverter.GetBytes(s.Thickness),0,data,15,4);
			_propertyCollection.SendMessageRaw(data);
		}

		public void OnStrokeEndLocal(Stroke s)
		{
			var data = new byte[3];
			data[0] = (byte)MessageType.InkEnd;
			data[1] = s.MyCanvas.ID;
			data[2] = s.MyPenID;
			_propertyCollection.SendMessageRaw(data);
			if (_addBuffer.ContainsKey(s))
			{
				_addBuffer.Remove(s);
			}
		}

		public void OnPointAddLocal(Stroke s, InkPoint point)
		{
			if (_addBuffer.ContainsKey(s))
			{
				_addBuffer[s].Add(point);
			}
			else
			{
				_addBuffer.Add(s,new List<InkPoint>(){point});
			}
			//add to a buffer and send once previous message is handshaked.... per stroke
		}

		public SyncTransformMatrix GetCanvasMatrix(byte matrixID)
		{
			if (matrixID < CanvasTransforms.Length)
			{
				return CanvasTransforms[matrixID];
			}
			else
			{
				Debug.LogWarning("Runtime creation of matrices not supported. Suggestion is to pre-populate arrays.");
			}

			return null;
		}
		
		public byte GetUniquePenID()
		{
			_lastPenID++;
			return _lastPenID;
		}
	}
}