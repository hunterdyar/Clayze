using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	[CreateAssetMenu(fileName = "Int", menuName = "Clayze/Sync Properties/Int", order = 0)]

	public class SyncInt : Syncable<int>
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
			_value = BitConverter.ToInt32(data);
			OnChange?.Invoke(_value);
		}
	}
}