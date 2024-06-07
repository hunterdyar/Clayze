using System;
using UnityEngine;

namespace Marching.Operations
{
	public class SDF : OperationObject
	{
		public float scale;
		public Texture3D sdf;//cpu memory
		private Vector3 _prevPosition;
		private float _prevScale;
		[Tooltip("mip level of the texture3D to sample. 0 is highest, default. increase for better performance.")]
		public int mipMapLevel = 0;

		
		public override bool DidUpdate()
		{
			return _prevPosition != transform.position || _prevScale != scale || base.DidUpdate();
		}

		private void NormalizeSDF()
		{
			//read every point, normalize it to how we want our data (?) 
		}
		protected override void LateUpdate()
		{
			_prevPosition = transform.position;
			_prevScale = scale;
		}

		public override (Vector3, Vector3) OperationWorldBounds()
		{
			Vector3 w = transform.TransformPoint(Vector3.zero);
			return (w,w + new Vector3(scale, scale, scale));
		}

		public override void Sample(Vector3 world, ref float f)
		{
			if (!GeometryUtility.PointInBounds(world, _min, _max))
			{
				//don't touch f
				return;
			}
			//Vector3 world = _volume.VolumeToWorld(volPos);
			Vector3 local = transform.InverseTransformPoint(world);
			local = local / scale;
			//todo: create a dictionary lookup and only update if changed or moved? how do we only read from the sdf when we have to?
			
			//todo: apply the local transform of the object to rotate, etc, the sdf
			
			//if out of bounds, return 0.
			
			//volPos -> local -> sdf space.
			var uvw = new Vector3(local.x, local.y, local.z);
			//uvw = uvw*sdf.
			//todo: normalize to aspect ratio of texture.
			
			float s= Mathf.Clamp(sdf.GetPixelBilinear(uvw.x,uvw.y,uvw.z,mipMapLevel).r,-1,1);
			f = Mix(f, s);
		}
	}
}