using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "Vec2", menuName = "Clayze/Sync Properties/Vector 2", order = 0)]
	public class SyncVector2 : Syncable<Vector2>
	{
		public override byte[] ToBytes()
		{
			//todo: endian
			var data = new byte[8];
			if (BitConverter.IsLittleEndian)
			{
				BitConverter.GetBytes(_value.x).CopyTo(data, 0);
				BitConverter.GetBytes(_value.y).CopyTo(data, 4);
			}
			else
			{
				BitConverter.GetBytes(_value.x).Reverse().ToArray().CopyTo(data, 0);
				BitConverter.GetBytes(_value.y).Reverse().ToArray().CopyTo(data, 4);
			}

			return data;
		}

		public override void SetFromBytes(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				float x = BitConverter.ToSingle(data, 0);
				float y = BitConverter.ToSingle(data, 4);
				_value = new Vector2(x, y);
			}
			else
			{
				float x = BitConverter.ToSingle(data.Reverse().ToArray(), 0);
				float y = BitConverter.ToSingle(data.Reverse().ToArray(), 4);
				_value = new Vector2(x, y);
			}

			OnChange?.Invoke(_value);
		}
	}
}