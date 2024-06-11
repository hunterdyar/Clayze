using UnityEngine;

namespace Clayze.Marching.Operations
{
	/// <summary>
	/// It's like an SDF Operation, but in volume space, it should match the size of the volume, and implicitly affects the entire volume. 
	/// The reason we have a separate operation, is because we know for sure that this one can be collected of all the prior operations.
	/// </summary>
	public class VolumeCacheOp : IOperation
	{
		public float[] Points => _points;
		private float[] _points;
		public OperationName OpName => OperationName.Cache;
		public OperationType OperationType => OperationType.Recompose;
		public uint UniqueID { get; set; }
		private int size;

		private float[] points;
		// private int height;
		// private int depth;

		public VolumeCacheOp(Volume volume)
		{
			size = volume.Size;
			points = new float[size * size * size];//width*height*depth
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					for (int z = 0; z < size; z++)
					{
						points[Volume.IndexFromCoord(x, y, z, size)] = volume.SamplePoint(x, y, z);
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
			throw new System.NotImplementedException();
		}
	}
}