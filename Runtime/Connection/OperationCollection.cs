using System;
using System.Collections.Generic;
using System.Linq;
using Clayze.Connection;
using Clayze.Marching.Operations;
using Connection;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.Assertions;

namespace Clayze
{
	//this class will also handle "compacting" the base of operations down to a sampleOp.
	[CreateAssetMenu(fileName = "Op Collection",menuName = "Clayze/OperationCollection",order = 1)]
	public class OperationCollection : ScriptableObject
	{
		public Action<ConnectionStatus> OnConnectionStatusChanged;
		//Network
		public ConnectionStatus ConnectionStatus = ConnectionStatus.Idle;
		public SocketSettings _socketSettings;
		public string localStatus = "";
		private WebSocket _websocket;
		private Queue<int> _operationsWaitingForIndex = new Queue<int>();
		[Tooltip("Time in seconds after connection failure to attempt again.")]
		public int reconnectionDelay = 30;

		//List Things
		public Action<IOperation> OperationChanged;//changed or added.
		public Action ForceRefresh;
		
		public List<IOperation> Operations => _operations;
		[SerializeReference, SubclassSelector]
		private List<IOperation> _operations = new List<IOperation>();
		
		public void InitAndConnect()
		{
			if (_socketSettings.connectionURL == "")
			{
				Debug.Log($"Connection URL for {this} is empty. Not bothering to try to connect.");
				return;
			}
			
			SetConnectionStatus(ConnectionStatus = ConnectionStatus.Idle);
			localStatus = "Initialized";
			_websocket = new WebSocket(_socketSettings.connectionURL);
			_socketSettings.AddRecent(_socketSettings.connectionURL);

			_websocket.OnOpen += () =>
			{
				Debug.Log("Connection open!");
				SetConnectionStatus(ConnectionStatus = ConnectionStatus.Connected);
				//connected! Let's clear what we have and update from the server.
				_websocket.Send(new byte[] { (byte)MessageType.GetAll });
			};

			_websocket.OnError += (e) => { Debug.Log("Error! " + e); };

			_websocket.OnClose += (e) =>
			{
				Debug.Log($"Connection closed! Code: {e}");
				SetConnectionStatus(ConnectionStatus.Disconnected);
			};

			_websocket.OnMessage += OnReceiveFromServer;

			SetConnectionStatus(ConnectionStatus.AttemptingToConnect);
			_websocket.Connect();
		}

		private void SetConnectionStatus(ConnectionStatus status)
		{
			ConnectionStatus = status;
			OnConnectionStatusChanged?.Invoke(ConnectionStatus);
		}
		private void OnReceiveFromServer(byte[] data)
		{
			var messageType = (MessageType)data[0];
			switch (messageType)
			{
				case MessageType.Add:
					AddOperationFromServer(data,1);
					break;
				case MessageType.Remove:
					RemoveOperationFromServer(data);
					break;
				case MessageType.IDReply:
					OnIDReplyFromServer(data);
					break;
				case MessageType.Echo:
					Debug.Log("Echo Received.");
					break;
				case MessageType.GetAll:
					_operations.Clear();
					AddOperationsFromServer(data);
					//fire off event.
					break;
				case MessageType.Clear:
					_operations.Clear();
					ForceRefresh?.Invoke();
					break;
				case MessageType.Changed:
				case MessageType.ChangeConfirm:
					//ignore, these are for sync properties.
					break;
				default:
					Debug.LogError($"{messageType} not handled by client.");
					break;
			}
		}

		private void RemoveOperationFromServer(byte[] data)
		{
			var intbytes = new byte[4];
			if (!BitConverter.IsLittleEndian) //uh, shouldn't we just know what we get from the server?
			{
				intbytes = new[] { data[4], data[3], data[2], data[1] };
			}
			else
			{
				intbytes = new[] { data[1], data[2], data[3], data[4] };
			}

			uint removeID = BitConverter.ToUInt32(intbytes);
			var removeIndex =_operations.FindIndex(x => x.UniqueID == removeID);
			if (removeIndex != -1)
			{
				var op = _operations[removeIndex];
				_operations.RemoveAt(removeIndex);
				//even though the operation is removed, the changed is just a fresh within the worldbounds of the op.
				OperationChanged?.Invoke(op);
			}
			else
			{
				Debug.LogWarning("Unable to Remove item despite removal of it from net collection. We have probably optimized out list. Uh oh!");
			}
		}

		private void AddOperationsFromServer(byte[] data)
		{
			var offset = 1;//the first byte is the type of message, here it's "get all"
			int consumed = 0;
			while (offset < data.Length)
			{
				consumed = AddOperationFromServer(data,offset,false);
				offset += consumed;
				if (consumed == 0)
				{
					break;//uh oh!
				}
			}
			//todo: send hard refresh.
			ForceRefresh?.Invoke();
		}

		private void OnIDReplyFromServer(byte[] data)
		{
			if (_operationsWaitingForIndex.Count < 0)
			{
				Debug.LogError("Got IDReply but no message waiting! Uh oh, bad state!");
				return;
			}
			else
			{
				var intbytes = new byte[4];
				if (!BitConverter.IsLittleEndian) //uh, shouldn't we just know what we get from the server?
				{
					intbytes = new[] { data[4], data[3], data[2], data[1] };
				}
				else
				{
					intbytes = new[] { data[1], data[2], data[3], data[4] };
				}

				var waiting = _operationsWaitingForIndex.Dequeue();
				_operations[waiting].SetID(BitConverter.ToUInt32(intbytes));
				waiting = -1;
			}

			localStatus = "Idle";
		}

		
		/// <summary>
		/// Called when we receive an "add" from the server with the byte data.
		/// </summary>
		public int AddOperationFromServer(byte[] data, int startOffset, bool sendUpdate = true)
		{
			var op = OperationSerializer.FromBytes(data, startOffset, out var bytesConsumed);//skip the first byte used to determine the type of message.
			if (op == null)
			{
				//some operation isn't handled yet. e.g. "pass".
				return 0;
			}
			_operations.Add(op);
			if (sendUpdate)
			{
				OperationChanged?.Invoke(op);
			}

			return bytesConsumed;
		}

		//Close the connection. todo: rename
		public void Stop()
		{
			localStatus = "Stopped";
			if (_websocket != null)
			{
				_websocket.DispatchMessageQueue();
				_websocket.Close();
			}
			else
			{
				SetConnectionStatus(ConnectionStatus.Disconnected);
			}
		}

		public void DispatchMessageQueue()
		{
			if (_websocket != null)
			{
				_websocket.DispatchMessageQueue();
			}
		}
		
		//Local add!
		public void Add(IOperation op, bool local = true)
		{
			
			//todo: check if bounds are out of bounds of the volume. 
			//we should do that elsewhere and it should never hit this list.
			localStatus = "Add Local, Need ID from server...";
			_operations.Add(op);
			int opindex = _operations.Count - 1;
		#if UNITY_EDITOR
			//weird bug chasing. i fixed it but just testing in case it reappears
			Assert.AreEqual(_operations.IndexOf(op),opindex);
		#endif
			_operationsWaitingForIndex.Enqueue(opindex);
			var m = OperationSerializer.ToBytes(op);
			var packet = new byte[m.Length + 1];
			m.CopyTo(packet, 1);
			packet[0] = (byte)MessageType.Add;
			if (ConnectionStatus == ConnectionStatus.Connected)
			{
				//must be local only.... but we should check that.
				_websocket.Send(packet);
			}
			else
			{
				Debug.LogWarning("Local only add!");
			}

			OperationChanged?.Invoke(op);//instantly update the local client, while we wait for an ID.
		}

		public IEnumerator<IOperation> GetEnumerator()
		{
			return _operations.GetEnumerator();
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			_operations.Clear();
			if (_websocket != null)
			{
				_websocket.Send(new byte[] { (byte)MessageType.Clear });
			}

			ForceRefresh?.Invoke();
			//UH gotta tell volume to hard-refresh
		}

		public void Remove(IOperation op, bool local = true)
		{
			_operations.Remove(op);
			//I guess this would get us to resample this region so....
			OperationChanged?.Invoke(op);

		}

		public void RemoveAt(int index, bool local = true)
		{
			_operations.RemoveAt(index);
			//UH gotta tell volume to hardrefresh
		}

		public void UpdateValue(IOperation oldVal, IOperation newVal, bool local = true)
		{
			//todo: test that this preserves order correctly.
			var i = _operations.IndexOf(oldVal);
			_operations.RemoveAt(i);
			_operations.Insert(i,newVal);
			
			if (oldVal.OperationWorldBounds() != newVal.OperationWorldBounds())
			{
				OperationChanged?.Invoke(oldVal);
			}

			OperationChanged?.Invoke(newVal);
		}

		[ContextMenu("Clear Recent Server URLs")]
		private void ClearRecentConnectionURLs()
		{
			_socketSettings.recentConnectionURLs.Clear();
		}

		public void BakeVolumeToCache(Volume volume)
		{
			var cache = new VolumeCacheOp(volume);
			_operations.Clear();//local only!
			_operations.Add(cache);//local only!
			
			//First, loop for operations whose bounding areas are the entire area, like ClearOp. Remove everything before ClearOp.

			//Remove Operations that are entirely contained inside of later operations.
			//Start at the end of the list, and keep a boundingArea (?? list of points? boundingBoxes?)
			//loop towards the start, and if an operation has all of it's points inside of this area, remove it. Else, add it's points to this set.

			ForceRefresh?.Invoke();
		}
	}
}