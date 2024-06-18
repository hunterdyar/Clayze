using System;
using UnityEngine;

namespace Packages.Clayze.Editor.Utility
{
	[Obsolete("ArrowKeysMovment was for internal testing. Do not use in real project.")]
	public class ArrowKeysMovement : MonoBehaviour
	{
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
		}
	}
}