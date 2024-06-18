using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clayze.Extensions
{
	public static class Extensions
	{
		public static IEnumerator<Vector3Int> GetEnumerator(this BoundsInt bounds)
		{
			for (int ix = (int)bounds.min.x; ix < bounds.max.x; ix++)
			{
				for (int iy = (int)bounds.min.y; iy < bounds.max.y; iy++)
				{
					for (int iz = (int)bounds.min.x; iz < bounds.max.x; iz++)
					{
						yield return new Vector3Int(ix, iy, iz);
					}
				}
			}
		}

		public static Vector3 GetTranslation(this Matrix4x4 matrix)
		{
			Vector3 translate;
			translate.x = matrix.m03;
			translate.y = matrix.m13;
			translate.z = matrix.m23;
			return translate;
		}

		public static Quaternion GetRotation(this Matrix4x4 matrix)
		{
			Vector3 forward;
			forward.x = matrix.m02;
			forward.y = matrix.m12;
			forward.z = matrix.m22;

			Vector3 up;
			up.x = matrix.m01;
			up.y = matrix.m11;
			up.z = matrix.m21;

			return Quaternion.LookRotation(forward, up);
		}

		public static Vector3 GetScale(this Matrix4x4 matrix)
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}

		public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.GetTranslation();
			transform.localRotation = matrix.GetRotation();
			transform.localScale = matrix.GetScale();
		}
	}
}