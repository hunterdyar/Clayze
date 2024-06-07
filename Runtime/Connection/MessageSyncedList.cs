// using System;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using Models;
// using Models.Shared;
// using NativeWebSocket;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace Connection
// {
// 	[CreateAssetMenu(fileName = "Synced List", menuName = "Synced List", order = 0)]
// 	public class MessageSyncedList : ScriptableObject
// 	{
// 		public ConnectionStatus ConnectionStatus = ConnectionStatus.Idle;
// 		[FormerlySerializedAs("status")] public string localStatus = "";//todo: status enum
// 		public string url;
// 		public ObservableCollection<Message> Messages => _messages;
// 		private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
// 		[Tooltip("Time in seconds after connection failure to attempt again.")]
// 		public int reconnectionDelay = 30;
// 		private WebSocket _websocket;
// 		private int messageWaitingForIDIndex = -1;
//
// 		public void InitAndConnect()
// 		{
// 			ConnectionStatus = ConnectionStatus.Idle;
// 			localStatus = "Initialized";
// 			messageWaitingForIDIndex = -1;
// 			_websocket = new WebSocket(url);
// 			
// 			_websocket.OnOpen += () => { Debug.Log("Connection open!");
// 				ConnectionStatus = ConnectionStatus.Connected;
// 				EchoGreeting();
// 			};
//
// 			_websocket.OnError += (e) => { Debug.Log("Error! " + e); };
//
// 			_websocket.OnClose += (e) => { Debug.Log($"Connection closed! {e}");
// 				this.ConnectionStatus = ConnectionStatus.Disconnected;
// 			};
//
// 			_websocket.OnMessage += WebsocketOnOnMessage;
//
// 			ConnectionStatus = ConnectionStatus.AttemptingToConnect;
// 			_websocket.Connect();
// 		}
//
// 		private void EchoGreeting()
// 		{
// 			//todo: not sure what the deal is with a build user messaging first?
// 			byte[] greeting = new byte[] { (byte)MessageType.Echo, (byte)1 };
// 			_websocket.Send(greeting);
// 		}
//
// 		private void WebsocketOnOnMessage(byte[] data)
// 		{
// 			var messageType = (MessageType)data[0];
// 			switch (messageType)
// 			{
// 				case MessageType.Add:
// 					AddMessageFromServer(data);
// 					break;
// 				case MessageType.IDReply:
// 					OnIDReplyFromServer(data);
// 					break;
// 				case MessageType.Echo:
// 					Debug.Log("Echo Received.");
// 					break;
// 				default:
// 					Debug.LogError($"{messageType} not handled by client.");
// 					break;
// 			}
// 		}
//
// 		private void OnIDReplyFromServer(byte[] data)
// 		{
// 			if (messageWaitingForIDIndex < 0)
// 			{
// 				Debug.LogError("Got IDReply but no message waiting! Uh oh, bad state!");
// 				return;
// 			}
// 			else
// 			{
// 				var intbytes = new byte[4];
// 				if (!BitConverter.IsLittleEndian)//uh, shouldn't we just know what we get from the server?
// 				{
// 					intbytes = new[] { data[4], data[3], data[2], data[1] };
// 				}
// 				else
// 				{
// 					intbytes = new[] { data[1], data[2], data[3], data[4] };
// 				}
// 				_messages[messageWaitingForIDIndex].SetID(BitConverter.ToUInt32(intbytes));
// 				messageWaitingForIDIndex = -1;
// 			}
//
// 			localStatus = "Idle";
// 		}
//
// 		public void AddMessageFromLocal(Message message)
// 		{
// 			localStatus = "Add Local, Need ID from server...";
// 			_messages.Add(message);
// 			//count.
// 			messageWaitingForIDIndex = _messages.IndexOf(message);
// 			var m =  message.GetBytes();
// 			var packet = new byte[m.Length + 1];
// 			m.CopyTo(packet,1);
// 			packet[0] = (byte)MessageType.Add;
// 			_websocket.Send(packet);
// 		}
//
// 		public void AddMessageFromServer(byte[] data)
// 		{
// 			var message = new Models.Message(data, 1);
// 			_messages.Add(message);
// 			// _messages.Sort(); sort by id.
// 		}
//
// 		public void Stop()
// 		{
// 			localStatus = "Stopped";
// 			_websocket.DispatchMessageQueue();
// 			_websocket.Close();
// 		}
//
// 		public void DispatchMessageQueue()
// 		{
// 			if (_websocket != null)
// 			{
// 				_websocket.DispatchMessageQueue();
// 			}
// 		}
// 	}
//
// }