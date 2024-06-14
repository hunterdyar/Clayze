using System;
using UnityEngine;

namespace Clayze.Connection
{
	public class OperationInitializer : MonoBehaviour
	{
		public OperationCollection OpCollection;
		private float reconnectionTimer = 0;
		private void Awake()
		{
			OpCollection.InitAndConnect();
		}

		void Update()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			OpCollection.DispatchMessageQueue();
#endif
			if (OpCollection.ConnectionStatus == ConnectionStatus.Disconnected)
			{
				reconnectionTimer += Time.deltaTime;
				if (reconnectionTimer > OpCollection.reconnectionDelay)
				{
					OpCollection.InitAndConnect();
					reconnectionTimer = 0;
				}
			}
		}


		private void OnDestroy()
		{
			OpCollection.Stop();
		}

		private void OnApplicationQuit()
		{
			OpCollection.Stop();
		}
	}
}