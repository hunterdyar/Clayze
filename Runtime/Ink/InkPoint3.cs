using System;
using UnityEngine;

namespace Clayze.Ink
{
	[System.Serializable]
	public class InkPoint3
	{
		public float x;
		public float y;
		public float z;
		public byte Width;
		public float DistanceFromPrevious;
		public InkPoint3(float x, float y, float z, byte width)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.Width = width;
			DistanceFromPrevious = 0;
		}

		public InkPoint3(Vector3 point)
		{
			this.x = point.x;
			this.y = point.y;
			this.z = point.z;
			this.Width = 255;
			DistanceFromPrevious = 0;
		}

		public InkPoint3(Vector3 point, byte width)
		{
			this.x = point.x;
			this.y = point.y;
			this.z = point.z;
			this.Width = width;
			DistanceFromPrevious = 0;
		}

		public static float Distance(InkPoint3 a, InkPoint3 b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return(float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 +
			                        (double)num3 * (double)num3);
		}

		public static float Distance(InkPoint3 a, Vector3 b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 +
			                        (double)num3 * (double)num3);
		}

		public static float Distance(Vector3 a, InkPoint3 b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 +
			                        (double)num3 * (double)num3);
		}

		public void SetDistance(float f)
		{
			DistanceFromPrevious = f;
		}

		public Vector3 GetVector3()
		{
			return new Vector3(x, y, z);
		}
	}
		
}