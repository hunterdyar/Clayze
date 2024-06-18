using System;
using System.Collections.Generic;
using Clayze;
using Clayze.Connection;
using Connection;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.Assertions;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "SyncedCollection", menuName = "Clayze/Property Synchronizer Collection", order = 0)]
	public class SyncedPropertyCollection : ScriptableObject
	{
		public ConnectionStatus ConnectionStatus = ConnectionStatus.Idle;
		[SerializeField] private SocketSettings _socketSettings;
		private WebSocket _websocket;
		
		[SerializeField, SerializeReference]
		//what should the base class here be?
		private List<SyncableBase> _values = new List<SyncableBase>();
		//todo: cache id lookup with dictionary.
		public int ChangesSent => _changesSent;
		private int _changesSent;
		public void InitAndConnect()
		{
			_changesSent = 0;
			//todo: check that all ID's are unique.
			if (_socketSettings.connectionURL == "")
			{
				Debug.Log($"Connection URL for {this} is empty. Aborting.");
				return;
			}
			SetConnectionStatus(ConnectionStatus = ConnectionStatus.Idle);
			_websocket = new WebSocket(_socketSettings.connectionURL);
			_socketSettings.AddRecent(_socketSettings.connectionURL);

			_websocket.OnOpen += () =>
			{
				Debug.Log($"Connection open for {name}!");
				SetConnectionStatus(ConnectionStatus = ConnectionStatus.Connected);
				//connected! Let's clear what we have and update from the server.
				_websocket.Send(new byte[] { (byte)MessageType.GetAll });
			};

			_websocket.OnError += (e) => { Debug.Log("Error! " + e); };

			_websocket.OnClose += (e) =>
			{
				Debug.Log($"Connection closed for {name}! Code: {e}. URL: {_socketSettings.connectionURL}");
				SetConnectionStatus(ConnectionStatus.Disconnected);
			};
			

			_websocket.OnMessage += OnReceiveFromServer;

			SetConnectionStatus(ConnectionStatus.AttemptingToConnect);
			_websocket.Connect();
		}

		[ContextMenu("Ping Hello")]
		public void SendHello()
		{
			byte[] hello = new byte[] { (byte)MessageType.Echo, 0, 0, 0, 0};
			_websocket.Send(hello);
		}
		public void SendChangesIfNeeded()
		{
			//todo: check if we are waiting for a response on any previous changes before sending updates.
			foreach (var x in _values)
			{
				if (x.IsDirty)
				{
					//todo: check if bounds are out of bounds of the volume. 
					//we should do that elsewhere and it should never hit this list.
					var data = x.ToBytes();
					var packet = new byte[data.Length + 5];
					packet[0] = (byte)MessageType.Changed;
					BitConverter.GetBytes(x.ID).CopyTo(packet, 1);
					data.CopyTo(packet, 5);
					if (ConnectionStatus == ConnectionStatus.Connected)
					{
						//must be local only.... but we should check that
						_websocket.Send(packet);
						_changesSent++;
					}
					else
					{
						Debug.LogWarning("Local only add!");
					}

					x.IsDirty = false;
				}
			}
		}

		private void OnReceiveFromServer(byte[] data)
		{
			var messageType = (MessageType)data[0];
			switch (messageType)
			{
				case MessageType.Add:
					break;
				case MessageType.Remove:
					break;
				case MessageType.IDReply:
					break;
				case MessageType.Echo:
					Debug.Log("Echo Received.");
					break;
				case MessageType.GetAll:
					break;
				case MessageType.Clear:
					break;
				case MessageType.Changed:
					PropertyChanged(data);
					break;
				case MessageType.ChangeConfirm:
					_changesSent--;
					break;
				default:
					Debug.LogError($"{messageType} not handled by client.");
					break;
			}
		}

		private void PropertyChanged(byte[] data)
		{
			
			uint id = BitConverter.ToUInt32(new ArraySegment<byte>(data, 1, 4));
			var prop = _values.Find(x => x.ID == id);
			if (prop != null)
			{
				prop.SetFromBytes(new ArraySegment<byte>(data, 5, data.Length - 5));
				if (prop.IsOwner)
				{
					Debug.LogWarning("Got value. We are owner, so ignoring. Is someone else owner too?");
					prop.IsDirty = false;
				}
			}
		}

		private void SetConnectionStatus(ConnectionStatus newStatus)
		{
			ConnectionStatus = newStatus;
		}

		public void DispatchMessageQueue()
		{
			if (_websocket != null)
			{
				_websocket.DispatchMessageQueue();
			}
		}
		public void Stop()
		{
			if (_websocket != null)
			{
				_websocket.Close();
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Test Serialization")]
		public void TestSerialization()
		{
			foreach (var val in _values)
			{
				Assert.IsTrue(val.TestSerialization(), $"Serialization broken for {val}. {val.GetType()}");
			}
		}
#endif
	}
}