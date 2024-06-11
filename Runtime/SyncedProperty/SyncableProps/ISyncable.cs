using UnityEngine;

namespace SyncedProperty
{
	public interface ISyncable
	{
		//This will a single-value version of the operation class.
		//We can use this for "Normal" net-code behaviour, like sync-ing transforms for previewing each other in a multi-user scene.
		
		//A separate websocket connection will be created for a list of values, and it should use a unique property endpoint, as values will bee saved by index.
		public bool IsOwner { get;}
		public string Title { get; }
		public byte[] ToBytes();
		public void SetFromBytes(byte[] data);
	}
}