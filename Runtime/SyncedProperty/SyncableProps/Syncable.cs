using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SyncedProperty
{
	public abstract class SyncableBase : ScriptableObject
	{
		[HideInInspector] public bool IsDirty;
		public uint ID;
		public bool IsOwner;
		public abstract byte[] ToBytes();
		public abstract void SetFromBytes(byte[] data);

		public void SetFromBytes(ArraySegment<byte> data)
		{
			SetFromBytes(data.Array);
			IsDirty = false;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			uint highestID = ID;
			if (ID == 0)
			{
				var g = AssetDatabase.FindAssets("t:SyncableBase", null);
				foreach (string guid in g)
				{
					var x = AssetDatabase.LoadAssetAtPath<SyncableBase>(AssetDatabase.GUIDToAssetPath(guid));
					if (x != null)
					{
						if (x == this)
						{
							continue;
						}
						if (x.ID > highestID)
						{
							highestID = x.ID+1;
						}
					}
				}
			}

			ID = highestID;
		}
#endif

	}
	public abstract class Syncable<T> : SyncableBase
	{
		//This will a single-value version of the operation class.
		//We can use this for "Normal" net-code behaviour, like sync-ing transforms for previewing each other in a multi-user scene.
		
		//A separate websocket connection will be created for a list of values, and it should use a unique property endpoint, as values will bee saved by index.
		public Action<T> OnChange;
		public T Value => _value;
		[SerializeField]
		protected T _value;

		public virtual void SetValue(T newValue, bool fromLocal = true)
		{
			IsDirty = fromLocal;
			_value = newValue;
			OnChange?.Invoke(newValue);
		}

		public override string ToString()
		{
			return _value.ToString();
		}
	}
	
}