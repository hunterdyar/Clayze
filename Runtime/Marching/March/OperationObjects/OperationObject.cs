using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marching.Operations
{
	public class OperationObject : MonoBehaviour
	{
		[HideInInspector]
		public Volume _volume;

		public OperationType OperationType => _operationType;
		[SerializeField] private OperationType _operationType = OperationType.Add;
		
		protected Vector3 _min;
		protected Vector3 _max;
		protected OperationType lastOpType;
		public void SetVolume(Volume volume)
		{
			_volume = volume;
		}

		
		protected virtual void Update()
		{
			var worldBounds = OperationWorldBounds();
			_min = worldBounds.Item1;
			_max = worldBounds.Item2;
			
			if (_volume != null)
			{
				if (DidUpdate())
				{
					_volume.OperationChanged(worldBounds); //should only call this once per frame
				}
			}
		}

		protected virtual void LateUpdate()
		{
			lastOpType = _operationType;
		}

		//Todo: I think it makes sense to flip this around? right now _volume.operationChanged(), we tell it that we update; but it could ask us? and tell us when it samples to reset.
		public virtual bool DidUpdate()
		{
			return lastOpType != _operationType;
		}

		public virtual (Vector3, Vector3) OperationWorldBounds()
		{
			return (Vector3.zero, Vector3.zero);
		}

		public float Mix(float original, float newVal)
		{
			switch (_operationType)
			{
				case OperationType.Add:
					return GeometryUtility.SmoothMinCubic(original, newVal, 0.2f);
				case OperationType.Remove:
					return Mathf.Max(original, -newVal);
				case OperationType.Pass:
					return original;
			}

			return original;
		}
		public virtual void Sample(Vector3 worldPoint, ref float f)
		{
			f = 0;
		}
	}
}