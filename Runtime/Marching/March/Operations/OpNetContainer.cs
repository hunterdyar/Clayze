using System;
using UnityEngine;

namespace Marching.Operations
{
	public struct OpNetContainer : IEquatable<OpNetContainer>
	{
		private OperationName opName;
		private OperationType opType;
		private Vector3 PositionA;
		private float FloatA;
		
		public OpNetContainer(IOperation operation)
		{
			opName = OperationName.Pass;
			
			opType = operation.OperationType;
			opName = operation.OpName;

			switch (operation.OpName)
			{
				case OperationName.Sphere:
					var sphere = (SphereOp)operation;
					PositionA = sphere.Center;
					FloatA = sphere.Radius;
				break;
				default:
					FloatA = default;
					PositionA = default;
					break;
			}
		}

		public IOperation GetOperation()
		{
			switch (opName)
			{
				case OperationName.Sphere:
				default:
					return new SphereOp(PositionA, FloatA, opType);
			}
		}

		public bool Equals(OpNetContainer other)
		{
			return opName == other.opName && opType == other.opType && PositionA.Equals(other.PositionA) && FloatA.Equals(other.FloatA);
		}

		public override bool Equals(object obj)
		{
			return obj is OpNetContainer other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)opName, (int)opType, PositionA, FloatA);
		}

		
		// public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		// {
		// 	serializer.SerializeValue(ref opName);
		// 	serializer.SerializeValue(ref opType);
		// 	
		// 	//we could refactor this by implementing networkSerializer on the operation, here, i think? nested? but i think I just like an if.else chain here.
		// 	//its messy, but netcode is netcode and operations are operations. I really want to isolate these parts of the codebase.
		//
		// 	//if opname == clearOp is not needed, no values;
		// 	if (opName == OperationName.Sphere)
		// 	{
		// 		serializer.SerializeValue(ref PositionA);
		// 		serializer.SerializeValue(ref FloatA);
		// 	}
		// }
	}
}