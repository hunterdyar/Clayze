using System;
using UnityEngine;

namespace SyncedProperty
{
	public class SyncVector3 : ISyncable
	{
		public string Title => $"Synced Vector 3 ({Value.x:N3},{Value.y:N3},{Value.z:N3})";
		public bool IsOwner => _isOwner;
		[SerializeField] private bool _isOwner;
		public Vector3 Value;

		public byte[] ToBytes()
		{
			//todo: endian
			var data = new byte[4 * 3]; 
			BitConverter.GetBytes(Value.x).CopyTo(data,0);
			BitConverter.GetBytes(Value.y).CopyTo(data, 4);
			BitConverter.GetBytes(Value.z).CopyTo(data, 8);

			return data;
		}

		public void SetFromBytes(byte[] data)
		{
			float x = BitConverter.ToSingle(data,0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			Value = new Vector3(x, y, z);
		}

		public override string ToString()
		{
			return $"Vector 3 ({Value.x:N3},{Value.y:N3},{Value.z:N3})";
		}
	}
}