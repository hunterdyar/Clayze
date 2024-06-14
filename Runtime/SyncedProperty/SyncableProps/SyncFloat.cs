using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	[CreateAssetMenu(fileName = "Float", menuName = "Clayze/Sync Properties/Float", order = 0)]

	public class SyncFloat : Syncable<float>
	{
		public override byte[] ToBytes()
		{
			//todo: endian
			if (BitConverter.IsLittleEndian)
			{
				return BitConverter.GetBytes(_value);
			}
			else
			{
				return BitConverter.GetBytes(_value).Reverse().ToArray();
			}
		}

		public override void SetFromBytes(byte[] data)
		{
			_value = BitConverter.ToSingle(data);
			OnChange?.Invoke(_value);
		}
	}
}