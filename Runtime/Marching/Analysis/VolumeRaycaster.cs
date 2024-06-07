using System;
using UnityEngine;

namespace Marching.Analysis
{
	public class VolumeRaycaster : MonoBehaviour
	{
		[SerializeField] private VolumeRenderer _volumeRenderer;
		[SerializeField] private Volume _volume;
		[SerializeField] private float SurfaceMin = 0.1f;
		[SerializeField] private float SurfaceMax = 10;
		private Vector3 lastStart;
		private Vector3 lastDir;
		private Vector3 lastHit;
		private bool lastDidHit;
		
		//Does the raycast on the SDF data... poorly.
		public bool RaycastOnSDFs(Vector3 start, Vector3 direction, out Vector3 hit)
		{
			lastStart = start;
			lastDir = direction;
			hit = Vector3.zero;
			float stepSize = 0.01f;
			var localDir = _volume.transform.InverseTransformDirection(direction)*stepSize;
			direction.Normalize();
			//sample is in localspace.
			Vector3Int sample = _volume.WorldToLocal(start);
			float f = 1;
				for (int i = 0; i < _volume.Size*_volume.Size; i++)
				{
					if (_volume.IsInBounds(start))
					{
						var p = sample;
						f = _volume.SamplePoint(p.x, p.y, p.z);
					}
					else
					{
						//todo: we need to cast against a box of the bounds. local to the volume object is axis-aligned...
						f = 10;//approach infinity.
					}

					if (f < SurfaceMin)
					{
						hit = _volume.VolumeToWorld(sample);
						return true;
					}
					else if (f > SurfaceMax)
					{
						hit = _volume.VolumeToWorld(sample);
						return false;
					}
					else
					{
						//move f units in direction.
						var newSample = sample + (localDir * stepSize);
						sample = new Vector3Int(Mathf.RoundToInt(newSample.x),Mathf.RoundToInt(newSample.y),Mathf.RoundToInt(newSample.z));
					}
				}

			return false;
		}

		public bool RaycastOnRenderer(Vector3 start, Vector3 direction, out Vector3 hit)
		{
			lastStart = start;
			lastDir = direction;
			hit = Vector3.zero;
			float stepSize = 0.01f;
			direction.Normalize();
			//sample is in localspace.
			Vector3 sample = start;
			float f = 1;
			for (int i = 0; i < _volume.Size * _volume.Size; i++)
			{
				if(_volumeRenderer.TryGetPoint(sample, out f))
				{
					hit = sample;
				}
				
				if (f < _volumeRenderer.SurfaceLevel)
				{
					hit = sample;
					return true;
				}
				 if (f > SurfaceMax)
				 {
					 hit = sample;
					return false;
				}
				 //move f units in direction.
				sample = sample + (direction * stepSize);
			}

			return false;
		}

		private void OnDrawGizmos()
		{
		

			bool hit = _volumeRenderer.TryGetPoint(transform.position, out var f);
			if (hit)
			{
				// var g = Mathf.Abs(f);
				f = Mathf.InverseLerp(-4, 4, f);
				Gizmos.color = Color.Lerp(Color.green, Color.magenta, f);
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawSphere(transform.position, 0.1f);

		}
	}
}