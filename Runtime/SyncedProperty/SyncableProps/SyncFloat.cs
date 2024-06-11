using System;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	public struct SyncFloat : ISyncable
	{
		public Action<float> OnChange;
		public string Title => Value.ToString();
		public bool IsOwner => _isOwner;
		[SerializeField]
		private bool _isOwner;

		[SerializeField] private int id;
		public uint ID => (uint)id;
		
		public float Value => _value;
		[SerializeField]
		private float _value;

		public SyncFloat(int id, bool isOwner = false)
		{
			this.id = id;
			_isOwner = isOwner;

			OnChange = null;
			_value = 0;
		}
		public byte[] ToBytes()
		{
			//todo: endian
			return BitConverter.GetBytes(_value);
		}

		public void SetValue(float newValue)
		{
			_value = newValue;
			OnChange?.Invoke(_value);
		}

		public void SetFromBytes(byte[] data)
		{
			_value = BitConverter.ToSingle(data);
		}
	}
}