using UnityEngine;

namespace Marching.Operations
{
	/// <summary>
	/// It's like an SDF Operation, but in volume space, it should match the size of the volume, and implicitly affects the entire volume. 
	/// The reason we have a separate operation, is because we know for sure that this one can be collected of all the prior operations.
	/// </summary>
	public class PointCacheOp : IOperation
	{
		public float[] Points => _points;
		private float[] _points;
		public OperationName OpName => OperationName.SDF;
		public OperationType OperationType { get; }
		public uint UniqueID { get; set; }
		public (Vector3, Vector3) OperationWorldBounds()
		{
			return (Vector3.negativeInfinity, Vector3.positiveInfinity);
		}

		public void Sample(Vector3 worldPoint, ref float f)
		{
			throw new System.NotImplementedException();
		}
	}
}