using System;
using UnityEngine;

namespace Clayze.Marching.Operations
{
	/// <summary>
	/// It's like an SDF Operation, but in volume space, it should match the size of the volume, and implicitly affects the entire volume. 
	/// The reason we have a separate operation, is because we know for sure that this one can be collected of all the prior operations.
	/// </summary>
	[System.Serializable]
	public class VolumeCacheOp : IOperation
	{
		public float[] Points => _points;

		[SerializeField]
		private float[] _points;
		public OperationName OpName => OperationName.Cache;
		public OperationType OperationType => OperationType.SetLocal;
		public uint UniqueID { get; set; }
		[SerializeField]
		private int size;
		[SerializeField]
		private Volume _volume;
		// private int height;
		// private int depth;

		public VolumeCacheOp(Volume volume)
		{
			_volume = volume;
			size = volume.Size;
			_points = new float[size * size * size];//width*height*depth
			Bake();
		}

		public void Bake()
		{
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					for (int z = 0; z < size; z++)
					{
						_points[Volume.IndexFromCoord(x, y, z, size)] = _volume.SamplePoint(x, y, z);
					}
				}
			}
		}
		
		public (Vector3, Vector3) OperationWorldBounds()
		{
			return (Vector3.negativeInfinity, Vector3.positiveInfinity);
		}

		public void Sample(Vector3 worldPoint, ref float f)
		{
			throw new NotImplementedException("Cache operation must be of type 'SetLocal'");
		}

		public void Sample(Volume v, int x, int y, int z, ref float f)
		{
			 f = OpUtility.Mix(OperationType.Add,f,_points[Volume.IndexFromCoord(x,y,z,size)]);
		}
	}
}