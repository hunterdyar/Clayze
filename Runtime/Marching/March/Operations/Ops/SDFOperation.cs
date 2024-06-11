using System.Linq;
using UnityEngine;

namespace Marching.Operations
{
	[System.Serializable]
	public class SDFOperation : IOperation
	{
		public OperationName OpName => OperationName.SDF;
		public OperationType OperationType { get; }
		public uint UniqueID { get; set; }
		public short Width => _width;
		private short _width;//x
		public short Height => _height;
		private short _height;//
		public short Depth => _depth;
		private short _depth;//z
		public Matrix4x4 Transform => _transform;
		private Matrix4x4 _transform;

		//todo: use width,height,depth,float[] instead of texture3d. it'll be less overhead than making unity assets at runtime.
		public float[] Points => _points;
		private float[] _points;

		public SDFOperation(Texture3D sdf, Vector3 position, Quaternion rotation, Vector3 scale, OperationType opType)
		{
			//_texture3D = sdf;
			_width = (short)sdf.width;
			_height = (short)sdf.height;
			_depth = (short)sdf.depth;
			_points = new float[_width * _height * _depth];
			_points = sdf.GetPixels().Select(c=>c.r).ToArray();
			
			scale = scale / (Mathf.Max(sdf.width, sdf.height, sdf.depth));
			_transform = Matrix4x4.TRS(position, rotation, scale);
			_transform = _transform.inverse;
			OperationType = opType;
		}

		public SDFOperation(Matrix4x4 transform, short w, short h, short d, float[] points, OperationType opType)
		{
			OperationType = opType;
			_transform = transform;
			_width = w;
			_height = h;
			_depth = d;
			_points = points;

		}

		public (Vector3, Vector3) OperationWorldBounds()
		{
			//uhhh?
			Vector3 min = _transform.inverse.MultiplyPoint(Vector3.zero);
			Vector3 max = _transform.inverse.MultiplyPoint(new Vector3(_width, _height, _depth));
			return (min, max);
		}

		public void Sample(Vector3 worldPoint, ref float f)
		{
			var p = _transform.MultiplyPoint(worldPoint);
			int x = Mathf.FloorToInt(p.x);
			int y = Mathf.FloorToInt(p.y);
			int z = Mathf.FloorToInt(p.z);
			float s = 1;
			if (x >= 0 && x < _width && y >= 0 && y < _height && z >= 0 && z < _depth)
			{
				//s = _texture3D.GetPixel(x, y, z).r;
				s = _points[z + y * _depth + x * _depth * _height];
			}

			f = OpUtility.Mix(OperationType, f, s);
		}
	}
}