using System;
using UnityEngine;

namespace Clayze.Marching.Analysis
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

		[Range(0.1f,10)]//black to white from 0 to this
		[SerializeField] private float debugscale;
		//Does the raycast on the SDF data... poorly.
		public bool RaycastOnSDFs(Vector3 start, Vector3 direction, float maxDistance, out Vector3 hit)
		{
			lastStart = start;
			lastDir = direction;
			hit = Vector3.zero;
			float stepSize = 0.03f;
			direction.Normalize();
			//sample is in localspace.
			Vector3 sampleW = start;
			float distanceChecked = 0;
			float f;
			
			//todo: calculate the intersection point with the world bounds, then do the cast only inside of the volume.
			
			while(distanceChecked<maxDistance)
			{
				if (_volume.IsInBounds(sampleW))
				{
					f = _volume.SampleWorldPoint(sampleW);
					Gizmos.color = Color.Lerp(Color.black, Color.white, f / debugscale);

				}
				else
				{
					//todo: we need to cast against a box of the bounds. local to the volume object is axis-aligned...
					f = 10;//approach infinity.
					Gizmos.color = Color.cyan;
				}

				Gizmos.DrawSphere(sampleW, stepSize / 2f);

				
				if (f < SurfaceMin)
				{
					hit = sampleW;
					return true;
				}
				else if (f > SurfaceMax)
				{
					hit = sampleW;
					return false;
				}
				else
				{
					//move f units in direction.
					sampleW = sampleW + (direction * stepSize);
					distanceChecked += stepSize;
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
				else
				{
					//?
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
			bool hit = this.RaycastOnSDFs(transform.position, transform.forward, 10f,out var f);
			if (hit)
			{
				// var g = Mathf.Abs(f);
				Gizmos.DrawSphere(f,0.2f);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(f, 0.2f);
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position,f);
		}
	}
}