using UnityEngine;

namespace Marching.Operations
{
	public static class OpUtility
	{
		public static float Mix(OperationType opType, float original, float newVal)
		{
			switch (opType)
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
	}
}