using System;
using System.Collections.Generic;
using Clayze;
using Clayze.Connection;
using Connection;
using NativeWebSocket;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "SyncedCollection", menuName = "Synced Properties", order = 0)]
	public class SyncedPropertyCollection : ScriptableObject
	{
		public ConnectionStatus ConnectionStatus = ConnectionStatus.Idle;
		[SerializeField] private SocketSettings _socketSettings;
		private WebSocket _websocket;
		
		[SerializeField, SerializeReference]
		private List<ISyncable> _values = new List<ISyncable>();

		[ContextMenu("Add Sync Float")]
		public void AddSyncFloat()
		{
			_values.Add(new SyncFloat(_values.Count));			
		}

		[ContextMenu("Add Sync V3")]
		public void AddSyncV3()
		{
			_values.Add(new SyncVector3(_values.Count));
		}

		public void InitAndConnect()
		{
			if (_socketSettings.connectionURL == "")
			{
				Debug.Log($"Connection URL for {this} is empty. Not bothering to try to connect.");
				return;
			}
			SetConnectionStatus(ConnectionStatus = ConnectionStatus.Idle);
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
				default:
					Debug.LogError($"{messageType} not handled by client.");
					break;
			}
		}

		private void SetConnectionStatus(ConnectionStatus newStatus)
		{
			ConnectionStatus = newStatus;
		}

		public void Stop()
		{
			
		}
	}
}