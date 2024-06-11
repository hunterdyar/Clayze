using System;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	public struct SyncFloat : ISyncable
	{
		public string Title => Value.ToString();
		public bool IsOwner => _isOwner;
		[SerializeField]
		private bool _isOwner;
		public float Value;
		public byte[] ToBytes()
		{
			//todo: endian
			return BitConverter.GetBytes(Value);
		}

		public void SetFromBytes(byte[] data)
		{
			Value = BitConverter.ToSingle(data);
		}
	}
}