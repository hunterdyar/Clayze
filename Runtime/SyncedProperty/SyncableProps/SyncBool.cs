using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	[CreateAssetMenu(fileName = "Bool", menuName = "Clayze/Sync Properties/Bool", order = 0)]

	public class SyncBool : Syncable<bool>
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
			_value = BitConverter.ToBoolean(data);
			OnChange?.Invoke(_value);
		}
	}
}