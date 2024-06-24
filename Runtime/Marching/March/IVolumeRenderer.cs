using UnityEngine;

namespace Marching.March
{
	public interface IVolumeRenderer
	{
		public ComputeShader MarchingCompute { get;}
		public float SurfaceLevel { get;  }
		public float Smoothness { get; }
	}
}