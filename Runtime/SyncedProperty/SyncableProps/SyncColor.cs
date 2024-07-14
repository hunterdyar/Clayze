using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	[CreateAssetMenu(fileName = "Color", menuName = "Clayze/Sync Properties/Color", order = 0)]

	public class SyncColor : Syncable<Color>
	{
		public override byte[] ToBytes()
		{
			//todo: endian
			byte[] data = new byte[16];

			if (BitConverter.IsLittleEndian)
			{
				BitConverter.GetBytes(_value.r).CopyTo(data, 0);
				BitConverter.GetBytes(_value.g).CopyTo(data, 4);
				BitConverter.GetBytes(_value.b).CopyTo(data, 8);
				BitConverter.GetBytes(_value.a).CopyTo(data, 12);
			}
			else
			{
				BitConverter.GetBytes(_value.r).Reverse().ToArray().CopyTo(data, 0);
				BitConverter.GetBytes(_value.g).Reverse().ToArray().CopyTo(data, 4);
				BitConverter.GetBytes(_value.b).Reverse().ToArray().CopyTo(data, 8);
				BitConverter.GetBytes(_value.a).Reverse().ToArray().CopyTo(data, 12);
			}
			return data;
		}

		public override void SetFromBytes(byte[] data)
		{
			float r = BitConverter.ToSingle(data, 0);
			float g = BitConverter.ToSingle(data, 4);
			float b = BitConverter.ToSingle(data, 8);
			float a = BitConverter.ToSingle(data, 12);
			_value = new Color(r, g, b, a);
			OnChange?.Invoke(_value);
		}
	}
}