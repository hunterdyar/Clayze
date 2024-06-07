using UnityEngine;

namespace Marching
{
	public class GeometryUtility
	{
		public static bool CubesIntersect(Vector3Int aMin, Vector3Int aMax, Vector3Int bMin, Vector3Int bMax)
		{
			return (aMax.x >= bMin.x && aMin.x <= bMax.x)
			       && (aMax.y >= bMin.y && aMin.y <= bMax.y)
			       && (aMax.z >= bMin.z && aMin.z <= bMax.z);
		}

		public static bool PointInBounds(Vector3 point, Vector3 min, Vector3 max)
		{
			return (point.x >= min.x && point.x <= max.x)
			       && (point.y >= min.y && point.y <= max.y)
			       && (point.z >= min.z && point.z <= max.z);
		}

		//https://iquilezles.org/articles/smin/
		public static float SmoothMinCubic(float a, float b, float k)
		{
				float h = Mathf.Max(k - Mathf.Abs(a - b), 0f) / k;
				return Mathf.Min(a, b) - h * h * h * k * (1f / 6f);
		}

		float SmoothMinPolynomial(float a, float b, float k)
		{
			float h = Mathf.Max(k - Mathf.Abs(a - b), 0f) / k;
			return Mathf.Min(a, b) - h * h * k * (1f / 4f);
		}

		public static float DistanceFromCamera(Vector3 position)
		{
			return (position - Camera.main.transform.position).sqrMagnitude;
		}
	}
}