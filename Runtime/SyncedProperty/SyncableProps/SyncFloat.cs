using System;
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
			return BitConverter.GetBytes(_value);
		}

		public override void SetFromBytes(byte[] data)
		{
			_value = BitConverter.ToSingle(data);
		}
	}
}