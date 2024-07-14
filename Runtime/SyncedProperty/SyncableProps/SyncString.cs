using System;
using System.Linq;
using UnityEngine;

namespace SyncedProperty
{
	[Serializable]
	[CreateAssetMenu(fileName = "String", menuName = "Clayze/Sync Properties/String", order = 0)]

	public class SyncString : Syncable<string>
	{
		public override byte[] ToBytes()
		{
			return System.Text.Encoding.UTF8.GetBytes(_value);
		}

		public override void SetFromBytes(byte[] data)
		{
			_value = System.Text.Encoding.UTF8.GetString(data);
			OnChange?.Invoke(_value);
		}
	}
}