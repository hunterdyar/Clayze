using UnityEngine;

namespace Clayze.Marching.Operations
{
	public abstract class RotatedOperation<T> : IOperation where T : IOperation
	{
		private T operation;
		public virtual OperationName OpName { get; }
		public virtual OperationType OperationType { get; }
		public virtual uint UniqueID { get; set; }

		
		public virtual (Vector3, Vector3) OperationWorldBounds()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Sample(Vector3 worldPoint, ref float f)
		{
			
		}
	}
}