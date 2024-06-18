using System;
using Clayze.Extensions;
using SyncedProperty;
using UnityEngine;

namespace Packages.Clayze.Editor.Utility
{
	public class NetTransform : MonoBehaviour
	{
		public SyncTransformMatrix _transformMatrix;

		private void OnEnable()
		{
			_transformMatrix.OnChange += OnMatrixChange;
		}

		private void OnMatrixChange(Matrix4x4 m)
		{
			//thanks to MatrixUtility
			transform.SetFromMatrix(m);
		}

		private void Update()
		{
			
			OutgoingSyncTick();
		}

		private void OutgoingSyncTick()
		{
			if (_transformMatrix.IsOwner)
			{
				if (_transformMatrix.Value != transform.localToWorldMatrix)
				{
					_transformMatrix.SetValue(transform.localToWorldMatrix);
				}
			}
		}
	}
}