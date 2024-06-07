using UnityEngine;

namespace Marching.Operations
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

		public void SetID(uint uniqueID)
		{
			UniqueID = uniqueID;
		}
	}
}