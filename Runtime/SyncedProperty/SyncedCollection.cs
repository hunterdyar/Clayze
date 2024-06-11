using System.Collections.Generic;
using UnityEngine;

namespace SyncedProperty
{
	[CreateAssetMenu(fileName = "SyncedCollection", menuName = "Synced Properties", order = 0)]
	public class SyncedCollection : ScriptableObject
	{
		[SerializeField, SerializeReference]
		private List<ISyncable> _values = new List<ISyncable>();

		[ContextMenu("Add Sync Float")]
		public void AddSyncFloat()
		{
			_values.Add(new SyncFloat());			
		}

		[ContextMenu("Add Sync V3")]
		public void AddSyncV3()
		{
			_values.Add(new SyncVector3());
		}
		
	}
}