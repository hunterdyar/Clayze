using System;
using UnityEngine;

namespace SyncedProperty
{
	public class SyncPropertyInitializer : MonoBehaviour
	{
		public SyncedPropertyCollection PropertyCollection;
		[SerializeField] private float updateFrequency = 0.2f;
		private float updateTimer;
		private void Awake()
		{
			PropertyCollection.InitAndConnect();
		}

		private void Update()
		{
#if !UNITY_WEBGL || UNITY_EDITOR

			PropertyCollection.DispatchMessageQueue();
#endif
			updateTimer -= Time.deltaTime;
		}

		private void LateUpdate()
		{
			if (updateTimer > 0)
			{
				return;
			}

			if (PropertyCollection.ChangesSent == 0)
			{
				updateTimer = updateFrequency;
				PropertyCollection.SendChangesIfNeeded();
			}
		}

		private void OnApplicationQuit()
		{
			PropertyCollection.Stop();
		}
		private void OnDestroy()
		{
			PropertyCollection.Stop();
		}
	}
}