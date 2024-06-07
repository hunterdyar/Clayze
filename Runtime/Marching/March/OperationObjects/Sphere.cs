using UnityEngine;

namespace Marching.Operations
{
	public class Sphere : OperationObject
	{
		public float radius;

		//monitor changes, set didChangeLastFrame in update
		private float _lastRadius;
		private Vector3 _lastPosition;

		public override bool DidUpdate()
		{
			return _lastRadius != radius || _lastPosition != transform.position || base.DidUpdate();
		}

		protected override void LateUpdate()
		{
			_lastPosition = transform.position;
			_lastRadius = radius;
			base.LateUpdate();
		}
		public override void Sample(Vector3 worldPoint, ref float f)
		{
			float s = -Mathf.Clamp(radius*radius - (transform.position- worldPoint).sqrMagnitude,-1,1);
			//s = Mathf.Round(s);
			f = Mix(f, s);
		}

		public override (Vector3, Vector3) OperationWorldBounds()
		{
			var center = transform.position;
			var min = center - Vector3.one * radius;
			var max = center + Vector3.one * radius;
			return (new Vector3Int(Mathf.RoundToInt(min.x), Mathf.RoundToInt(min.y), Mathf.RoundToInt(min.z)),
				new Vector3Int(Mathf.CeilToInt(max.x), Mathf.CeilToInt(max.y), Mathf.CeilToInt(max.z)));
		}
	}
}