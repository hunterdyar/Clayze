using System;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "Quaternion", menuName = "Clayze/Sync Properties/Quaternion", order = 0)]
	public class SyncQuaternion : Syncable<Quaternion>
	{
		public override byte[] ToBytes()
		{
			//todo: endian
			var data = new byte[4 * 4];
			BitConverter.GetBytes(_value.x).CopyTo(data, 0);
			BitConverter.GetBytes(_value.y).CopyTo(data, 4);
			BitConverter.GetBytes(_value.z).CopyTo(data, 8);
			BitConverter.GetBytes(_value.w).CopyTo(data, 8);

			return data;
		}

		public override void SetFromBytes(byte[] data)
		{
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			float w = BitConverter.ToSingle(data, 12);
			_value = new Quaternion(x, y, z,w);
		}
	}

}