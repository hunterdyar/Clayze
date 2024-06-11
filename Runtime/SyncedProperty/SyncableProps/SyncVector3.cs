using System;
using UnityEngine;

namespace SyncedProperty
{
	public class SyncVector3 : ISyncable
	{
		public Action<Vector3> OnChange;
		public string Title => $"Synced Vector 3 ({Value.x:N3},{Value.y:N3},{Value.z:N3})";
		public bool IsOwner => _isOwner;
		[SerializeField] private bool _isOwner;
		public Vector3 Value => _value;
		public uint ID => (uint)id;
		[SerializeField] private int id;
		
		[SerializeField]
		private Vector3 _value;

		public SyncVector3(int id, bool isOwner = false)
		{
			this.id = id;
			_isOwner = isOwner;

			OnChange = null;
			_value = default;
		}
		public byte[] ToBytes()
		{
			//todo: endian
			var data = new byte[4 * 3]; 
			BitConverter.GetBytes(_value.x).CopyTo(data,0);
			BitConverter.GetBytes(_value.y).CopyTo(data, 4);
			BitConverter.GetBytes(_value.z).CopyTo(data, 8);

			return data;
		}

		public void SetFromBytes(byte[] data)
		{
			float x = BitConverter.ToSingle(data,0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			_value = new Vector3(x, y, z);
		}

		public void SetValue(Vector3 newValue)
		{
			_value = newValue;
			OnChange?.Invoke(newValue);
		}
		
		public override string ToString()
		{
			return $"Vector 3 ({Value.x:N3},{Value.y:N3},{Value.z:N3})";
		}
	}
}