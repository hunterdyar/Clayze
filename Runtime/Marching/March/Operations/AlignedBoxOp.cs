using UnityEngine;

namespace Marching.Operations
{
	public class AlignedBoxOp : IOperation
	{
		public OperationName OpName => OperationName.AABox;
		public OperationType OperationType => _operationType;
		[SerializeField] private OperationType _operationType;
		public uint UniqueID { get; set; }

		public Vector3 Center => _center;
		[SerializeField] private Vector3 _center;
		
		public Vector3 Size => _size;
		[SerializeField] private Vector3 _size;

		public AlignedBoxOp(Vector3 center, Vector3 size, OperationType opType = OperationType.Add, uint uniqueID = 0)
		{
			_center = center;
			_size = size;
			this._operationType = opType;
			this.UniqueID = uniqueID;
		}
		
		public (Vector3, Vector3) OperationWorldBounds()
		{
			return (_center-_size,_center+_size);
		}

		public void Sample(Vector3 worldPoint, ref float f)
		{

				float x = Mathf.Max
				(worldPoint.x - _center.x - _size.x / 2,
					_center.x - worldPoint.x - _size.x / 2
				);

				float y = Mathf.Max
				(worldPoint.y - _center.y - _size.y / 2,
					_center.y - worldPoint.y - _size.y / 2
				);

				float z = Mathf.Max
				(worldPoint.z - _center.z - _size.z / 2,
					_center.z - worldPoint.z - _size.z / 2
				);

				float d = x;
				d = Mathf.Max(d, y);
				d = Mathf.Max(d, z);
				f = OpUtility.Mix(_operationType, f, d);

		}
		
		

		// var q = (new Vector3(Mathf.Abs(worldPoint.x), Mathf.Abs(worldPoint.y), Mathf.Abs(worldPoint.z))) - _size;
		// q = new Vector3(Mathf.Abs(q.x), Mathf.Abs(q.y), Mathf.Abs(q.z));
		// float s = q.magnitude + Mathf.Min(Mathf.Max(q.x, Mathf.Max(q.y, q.z)), 0.0f);
		
		
		//Sample
		//			f = OpUtility.Mix(_operationType, f, vmax((worldPoint - _center) - (_size)));

		float vmax(Vector3 v)
		{
			return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
		}
	}
}