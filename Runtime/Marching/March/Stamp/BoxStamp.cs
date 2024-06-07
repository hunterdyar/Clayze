using System;
using UnityEngine;

namespace Marching.Operations
{
	public class BoxStamp : MonoBehaviour
	{
		[SerializeField] private OperationCollection opCollection;

		[SerializeField] private Vector3 size;
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				var op = new AlignedBoxOp(transform.position,size,OperationType.Add);
				opCollection.Add(op);
			}

			if (Input.GetKeyDown(KeyCode.V))
			{
				var op = new AlignedBoxOp(transform.position, size, OperationType.Remove);
				opCollection.Add(op);
			}
		}

		private void OnDrawGizmos()
		{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(transform.position, size);
		}
	}
}