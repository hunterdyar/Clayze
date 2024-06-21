using System;
using Clayze.Marching.Operations;
using UnityEngine;

namespace Clayze.Marching
{
	public class BoxStamp : MonoBehaviour
	{
		[SerializeField] private OperationCollection opCollection;
		[SerializeField] private Vector3 size;
		
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				if(transform.rotation == Quaternion.identity){
					var op = new AlignedBoxOp(transform.position,size,OperationType.Add);
					opCollection.Add(op);
				}
				else
				{
					var op = new BoxOp(transform.position, size, transform.rotation,OperationType.Add);
					opCollection.Add(op);
				}
			}

			if (Input.GetKeyDown(KeyCode.V))
			{
				if (transform.rotation == Quaternion.identity)
				{
					var op = new AlignedBoxOp(transform.position, size, OperationType.Remove);
					opCollection.Add(op);
				}
				else
				{
					var op = new BoxOp(transform.position, size, transform.rotation, OperationType.Remove);
					opCollection.Add(op);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(transform.position, size);
		}
	}
}