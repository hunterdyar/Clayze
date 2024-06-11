using System;
using UnityEngine;

namespace SyncedProperty
{
	public class SyncPropertyInitializer : MonoBehaviour
	{
		public SyncedPropertyCollection[] Collections;

		private void Awake()
		{
			foreach (var collection in Collections)
			{
				collection.InitAndConnect();
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