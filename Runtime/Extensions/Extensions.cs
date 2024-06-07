using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
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
	}
}