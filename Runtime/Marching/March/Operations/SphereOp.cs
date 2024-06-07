using System;
using UnityEngine;

namespace Marching.Operations
{
	[System.Serializable]
	public struct SphereOp : IOperation, IEquatable<SphereOp>
	{
		public uint UniqueID { get; set; }
		public OperationName OpName => OperationName.Sphere;
		public OperationType OperationType => _operationType;
		[SerializeField] private OperationType _operationType;
		public float Radius => _radius;
		[SerializeField] private float _radius;
		public Vector3 Center => _center;
		[SerializeField] private Vector3 _center;
		public SphereOp(Vector3 center, float radius, OperationType t = OperationType.Add)
		{
			_center = center;
			_radius = radius;
			_operationType = t;
			UniqueID = 0;
		}

		public (Vector3, Vector3) OperationWorldBounds()
		{
			var min = _center - Vector3.one * _radius;
			var max = _center + Vector3.one * _radius;
			return (new Vector3Int(Mathf.RoundToInt(min.x), Mathf.RoundToInt(min.y), Mathf.RoundToInt(min.z)),
				new Vector3Int(Mathf.CeilToInt(max.x), Mathf.CeilToInt(max.y), Mathf.CeilToInt(max.z)));

		}

		public void Sample(Vector3 worldPoint, ref float f)
		{
			float s = -Mathf.Clamp( _radius*_radius - (_center - worldPoint).sqrMagnitude, -1, 1);
			//s = Mathf.Round(s);
			f = OpUtility.Mix(_operationType,f, s);
		}

		public bool Equals(SphereOp other)
		{
			return _operationType == other._operationType && _radius.Equals(other._radius) && _center.Equals(other._center);
		}

		public override bool Equals(object obj)
		{
			return obj is SphereOp other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)_operationType, _radius, _center);
		}

		public static bool operator ==(SphereOp left, SphereOp right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SphereOp left, SphereOp right)
		{
			return !left.Equals(right);
		}
	}
}