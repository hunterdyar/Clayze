using UnityEngine;

namespace Clayze.Marching.Operations
{
	public interface IOperation 
	{
		public OperationName OpName { get; }
		public OperationType OperationType { get; }
		public uint UniqueID { get; set; }
		public abstract (Vector3, Vector3) OperationWorldBounds();

		/// <summary>
		/// Note, for operation type to work, be sure to call Mix(old,new) or manually implement it in Sample
		/// </summary>
		public abstract void Sample(Vector3 worldPoint, ref float f);

		public virtual void Sample(Volume volume, int x, int y, int z, ref float f)
		{
			Sample(volume.VolumeToWorld(x, y, z), ref f);
		}
		public void SetID(uint uniqueID)
		{
			UniqueID = uniqueID;
		}
	}
}