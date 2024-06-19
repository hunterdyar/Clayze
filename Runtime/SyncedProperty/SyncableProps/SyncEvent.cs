using System;
using UnityEngine;
using UnityEngine.Events;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "Event", menuName = "Clayze/Sync Properties/Event ()", order = 0)]
	public class SyncEvent : SyncableBase
	{
		public UnityEvent _event;
		public override byte[] ToBytes()
		{
			return Array.Empty<byte>();
		}

		public override void SetFromBytes(byte[] data)
		{
		}

		public override bool TestSerialization()
		{
			return true;
		}

		public void Invoke()
		{
			if (_event != null)
			{
				_event.Invoke();
			}
		}
	}
}