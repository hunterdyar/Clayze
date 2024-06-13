using System;
using SyncedProperty;
using UnityEngine;

namespace Packages.Clayze.Editor.Utility
{
	public class NetTransform : MonoBehaviour
	{
		public SyncVector3 _positionValue;
		public SyncQuaternion _rotationValue;

		private void OnEnable()
		{
			_positionValue.OnChange += OnPositionChange;
			_rotationValue.OnChange += OnRotationChange;
		}

		private void OnRotationChange(Quaternion rot)
		{
			transform.rotation = rot;
		}

		private void OnPositionChange(Vector3 vec)
		{
			transform.position = vec;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				transform.position += Vector3.left;
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				transform.position += Vector3.up;
			}

			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				transform.position += Vector3.down;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				transform.position += Vector3.right;
			}
			
			OutgoingSyncTick();
		}

		private void OutgoingSyncTick()
		{
			if (_positionValue.IsOwner)
			{
				if (_positionValue.Value != transform.position)
				{
					_positionValue.SetValue(transform.position);
				}
			}

			if (_rotationValue.IsOwner)
			{
				if (_rotationValue.Value != transform.rotation)
				{
					_rotationValue.SetValue(transform.rotation);
				}
			}
		}
	}
}