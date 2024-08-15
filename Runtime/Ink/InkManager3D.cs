using System;
using System.Collections.Generic;
using Connection.Models;
using SyncedProperty;
using UnityEngine;

namespace Clayze.Ink
{
	public class InkManager3D : MonoBehaviour, IInkMessageHandler
	{
		public Action<Stroke3> OnNewStroke;

		[SerializeField] private SyncedPropertyCollection _propertyCollection;

		
		private Dictionary<Stroke3, List<InkPoint3>> _addBuffer = new Dictionary<Stroke3, List<InkPoint3>>();
		private bool _canAdd = true;
		private byte _lastPenID = 0;
		private Stroke3[] _activeStrokes = new Stroke3[255];

		private void Start()
		{
			_propertyCollection.SetInkMessageHandler(this);
		}

		public byte GetUniquePenID()
		{
			_lastPenID++;
			return _lastPenID;
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
						var data = new byte[2 + (points.Count * 13)];
						data[0] = (byte)MessageType.InkAdd;
						data[1] = kvp.Key.MyPenID;
						for (int i = 0; i < points.Count; i++)
						{
							var x = BitConverter.GetBytes(points[i].x);
							var y = BitConverter.GetBytes(points[i].y);
							var z = BitConverter.GetBytes(points[i].z);

							data[2 + (i * 9)] = x[0];
							data[3 + (i * 9)] = x[1];
							data[4 + (i * 9)] = x[2];
							data[5 + (i * 9)] = x[3];
							data[6 + (i * 9)] = y[0];
							data[7 + (i * 9)] = y[1];
							data[8 + (i * 9)] = y[2];
							data[9 + (i * 9)] = y[3];
							data[10 + (i * 9)] = z[0];
							data[11 + (i * 9)] = z[1];
							data[12 + (i * 9)] = z[2];
							data[13 + (i * 9)] = z[3];
							data[14 + (i * 9)] = points[i].Width;
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

			if (_activeStrokes[penID] != null)
			{
				_activeStrokes[penID].Finish();
			}

			var s = new Stroke3(this, penID, false, thickness, color);
			_activeStrokes[penID] = s;
			OnNewStroke?.Invoke(s);
		}

		public void OnInkEndFromServer(byte[] data)
		{
			var penID = data[1];
			_activeStrokes[penID].Finish();
			_activeStrokes[penID] = null;
		}

		public void OnInkAddFromServer(byte[] data)
		{
			//0 is messageType.InkAdd
			var penID = data[1];
			int pointCount = (data.Length - 2) / 13;
			InkPoint3[] points = new InkPoint3[pointCount];
			
			for (int i = 0; i < pointCount; i++)
			{
				int j = 3 + (i * 13);
				var x = BitConverter.ToSingle(data,j);
				var y = BitConverter.ToSingle(data, j + 4);
				var z = BitConverter.ToSingle(data, j + 8);
				var w = data[j + 13];
				points[i] = new InkPoint3(x, y,z,w);
			}

			if (pointCount == 1)
			{
				_activeStrokes[penID].AddPoint(points[0]);

			}else if (pointCount > 1)
			{
				_activeStrokes[penID].AddPoints(points);
			}
			
		}

		public void OnInkNewCanvasFromServer(byte[] data)
		{
			//blop
		}

		public void OnInkAddConfirm()
		{
			_canAdd = true;
		}

		public Stroke3 StartStroke(byte pen, bool local, Color color, float thickness = 1)
		{
			var s = new Stroke3(this, pen, local, thickness, color);
			_activeStrokes[pen] = s;

			if (local && _propertyCollection != null)
			{
				//send to server
				var data = new byte[18];
				data[0] = (byte)MessageType.InkStart;
				data[1] = s.MyPenID;
				Array.Copy(BitConverter.GetBytes(s.Color.r), 0, data, 2, 4);
				Array.Copy(BitConverter.GetBytes(s.Color.g), 0, data, 6, 4);
				Array.Copy(BitConverter.GetBytes(s.Color.b), 0, data, 10, 4);
				Array.Copy(BitConverter.GetBytes(s.Thickness), 0, data, 14, 4);
				_propertyCollection.SendMessageRaw(data);
			}

			OnNewStroke?.Invoke(s);
			return s;
		}

		public void SendStokeEndFromLocal(Stroke3 s)
		{
			var data = new byte[2];
			data[0] = (byte)MessageType.InkEnd;
			data[1] = s.MyPenID;
			_propertyCollection.SendMessageRaw(data);
			if (_addBuffer.ContainsKey(s))
			{
				_addBuffer.Remove(s);
			}
		}

		public void OnPointAddLocal(Stroke3 s, InkPoint3 point3)
		{
			if (_addBuffer.ContainsKey(s))
			{
				_addBuffer[s].Add(point3);
			}
			else
			{
				_addBuffer.Add(s,new List<InkPoint3>(){point3});
			}
			//add to a buffer and send once previous message is handshaked.... per stroke
		}
	}
}