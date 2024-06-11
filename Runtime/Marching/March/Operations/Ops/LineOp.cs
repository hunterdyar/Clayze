using UnityEngine;

namespace Marching.Operations
{
	[System.Serializable]
	public class LineOp : IOperation
	{
		public OperationName OpName => OperationName.Line;
		public OperationType OperationType => _operationType;
		[SerializeField] private OperationType _operationType;
		public uint UniqueID { get; set; }

		public Vector3 PointA => _pointA;
		[SerializeField] private Vector3 _pointA;
		public Vector3 PointB => _pointB;
		[SerializeField] private Vector3 _pointB;
		public float Radius => _radius;
		[SerializeField] private float _radius;

		public LineOp(Vector3 pointA, Vector3 pointB, float radius, OperationType opType = OperationType.Add, uint uniqueID = 0)
		{
			_pointA = pointA;
			_pointB = pointB;
			_radius = radius;
			_operationType = opType;
			UniqueID = uniqueID;
			if (radius == 0)
			{
				Debug.LogWarning("Line Created with Radius of 0");
			}

			if (Vector3.Distance(_pointA, _pointB) < Mathf.Epsilon)
			{
				Debug.LogWarning("Line Created with no length!");
			}
		}
		public (Vector3, Vector3) OperationWorldBounds()
		{
			var extents = new Vector3(_radius, _radius, _radius); 
			var min = Vector3.Min(_pointA - extents, _pointB - extents);
			var max = Vector3.Max(_pointA + extents, _pointB + extents);
			return (min, max);
		}

		//Lines are actually Capsules (they have spherical caps)
		public void Sample(Vector3 worldPoint, ref float f)
		{
			var pa = worldPoint - _pointA;
			var ba = _pointB - _pointA;
			float h = Mathf.Clamp01(Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba));
			float s = (pa - ba * h).magnitude - _radius;
			f = OpUtility.Mix(_operationType, f, s);
		}
	}
}