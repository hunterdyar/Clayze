using System;
using UnityEngine;

namespace Marching.Operations
{
	public struct ClearOp : IOperation, IEquatable<ClearOp>
	{
		public uint UniqueID { get; set; }
		public OperationName OpName => OperationName.Clear;
		public OperationType OperationType { get; }
		public (Vector3, Vector3) OperationWorldBounds()
		{
			//This should work fine because Intersect Bounds is just <= and >= right?
			return (Vector3.negativeInfinity, Vector3.positiveInfinity);
		}

		public void Sample(Vector3 worldPoint, ref float f)
		{
			switch (OperationType)
			{
				case OperationType.Add:
					f = 1;
					break;
				case OperationType.Remove:
					f = -1;
					break;
			}
		}

		public bool Equals(ClearOp other)
		{
			return OperationType == other.OperationType;
		}
	}
}