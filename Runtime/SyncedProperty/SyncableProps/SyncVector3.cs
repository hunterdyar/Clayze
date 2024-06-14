using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "Vec3", menuName = "Clayze/Sync Properties/Vector 3", order = 0)]
	public class SyncVector3 : Syncable<Vector3>
	{
		public override byte[] ToBytes()
		{
			//todo: endian
			var data = new byte[4 * 3];
			if (BitConverter.IsLittleEndian)
			{
				BitConverter.GetBytes(_value.x).CopyTo(data, 0);
				BitConverter.GetBytes(_value.y).CopyTo(data, 4);
				BitConverter.GetBytes(_value.z).CopyTo(data, 8);
			}
			else
			{
				BitConverter.GetBytes(_value.x).Reverse().ToArray().CopyTo(data, 0);
				BitConverter.GetBytes(_value.y).Reverse().ToArray().CopyTo(data, 4);
				BitConverter.GetBytes(_value.z).Reverse().ToArray().CopyTo(data, 8);
			}

			return data;
		}

		public override void SetFromBytes(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				float x = BitConverter.ToSingle(data, 0);
				float y = BitConverter.ToSingle(data, 4);
				float z = BitConverter.ToSingle(data, 8);
				_value = new Vector3(x, y, z);
			}
			else
			{
				float x = BitConverter.ToSingle(data.Reverse().ToArray(), 0);
				float y = BitConverter.ToSingle(data.Reverse().ToArray(), 4);
				float z = BitConverter.ToSingle(data.Reverse().ToArray(), 8);
				_value = new Vector3(x, y, z);

			}

			OnChange?.Invoke(_value);
		}
	}
}