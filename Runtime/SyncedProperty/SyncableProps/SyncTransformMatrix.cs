using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "Matrix4x4", menuName = "Clayze/Sync Properties/Transform Matrix", order = 0)]
	public class SyncTransformMatrix : Syncable<Matrix4x4>
	{
		public override byte[] ToBytes()
		{
			var data = new byte[16*4];
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 16; i++)
				{
					BitConverter.GetBytes(_value[i]).CopyTo(data, i*4);
				}
			}
			else
			{
				for (int i = 0; i < 16; i++)
				{
					BitConverter.GetBytes(_value[i]).Reverse().ToArray().CopyTo(data, i*4);
				}
			}

			return data;
		}

		public override void SetFromBytes(byte[] data)
		{
			for (int i = 0; i < 16; i++)
			{
				_value[i] = BitConverter.ToSingle(data, i*4);
			}
			OnChange?.Invoke(_value);
		}
	}
}