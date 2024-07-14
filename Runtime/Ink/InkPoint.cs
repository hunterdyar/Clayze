using System;
using UnityEngine;

namespace Clayze.Ink
{
	[System.Serializable]
	public class InkPoint
	{
		public float x;
		public float y;
		public byte Width;
		public float DistanceFromPrevious;
		public InkPoint(float x, float y, byte width)
		{
			this.x = x;
			this.y = y;
			this.Width = width;
			DistanceFromPrevious = 0;
		}

		public InkPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.Width = 255;
			DistanceFromPrevious = 0;
		}

		public static float Distance(InkPoint a, InkPoint b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2);
		}

		public static float Distance(InkPoint a, Vector2 b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2);
		}

		public static float Distance(Vector2 a, InkPoint b)
		{
			float num1 = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2);
		}

		public void SetDistance(float f)
		{
			DistanceFromPrevious = f;
		}
	}
		
}