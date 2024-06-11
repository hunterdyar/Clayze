using UnityEngine;

namespace Marching.Operations
{
	[System.Serializable]
	public class BoxOp : IOperation
    {
	    private Matrix4x4 _samplePointTransformation;
	    public OperationName OpName => OperationName.Box;
        public OperationType OperationType => _operationType;
        [SerializeField] private OperationType _operationType;
        public uint UniqueID { get; set; }

        public Vector3 Center => _center;
        [SerializeField] private Vector3 _center;
        
        public Vector3 Size => _size;
        [SerializeField] private Vector3 _size;

        //todo: the setter for this needs to update the matrix.
        public Quaternion Rotation => _rotation;
        [SerializeField] private Quaternion _rotation;
        public BoxOp(Vector3 center, Vector3 size, Quaternion rotation, OperationType opType = OperationType.Add, uint uniqueID = 0)
        {
        	_center = center;
        	_size = size;
            _rotation = rotation;
            _samplePointTransformation = Matrix4x4.Rotate(rotation).inverse;
            this._operationType = opType;
        	this.UniqueID = uniqueID;
            
        }
        
        public (Vector3, Vector3) OperationWorldBounds()
        {
        	return (_center-_size,_center+_size);
        }

        public void Sample(Vector3 worldPoint, ref float f)
        {
				worldPoint = _samplePointTransformation.MultiplyPoint3x4(worldPoint-_center)+_center;
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
    }
}