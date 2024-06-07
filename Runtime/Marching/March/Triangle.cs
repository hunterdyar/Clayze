using UnityEngine;

namespace Marching
{
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	struct Triangle
	{
#pragma warning disable 649 // disable unassigned variable warning
		public Vector3 A;
		public Vector3 B;
		public Vector3 C;

		public Vector3 this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:
						return A;
					case 1:
						return B;
					default:
						return C;
				}
			}
		}
	}
}