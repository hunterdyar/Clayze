using System;
using UnityEngine;

namespace SyncedProperty
{
	public class SyncPropertyInitializer : MonoBehaviour
	{
		public SyncedPropertyCollection[] Collections;
		[SerializeField] private float updateFrequency = 0.2f;
		private float updateTimer;
		private void Awake()
		{
			foreach (var collection in Collections)
			{
				collection.InitAndConnect();
			}
		}

		private void Update()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			foreach (var collection in Collections)
			{
				collection.DispatchMessageQueue();
			}
#endif
			updateTimer -= Time.deltaTime;
		}

		private void LateUpdate()
		{
			if (updateTimer > 0)
			{
				return;
			}

			updateTimer = updateFrequency;
			foreach (var col in Collections)
			{
				col.SendChangesIfNeeded();
			}
		}

		private void OnApplicationQuit()
		{
			foreach (var collection in Collections)
			{
				collection.Stop();
			}
		}
		private void OnDestroy()
		{
			foreach (var collection in Collections)
			{
				collection.Stop();
			}
		}
	}
}