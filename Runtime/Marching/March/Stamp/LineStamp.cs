using System;
using UnityEngine;
using Clayze.Marching.Operations;

namespace Clayze.Marching
{
	public class LineStamp : MonoBehaviour
	{
		[SerializeField] private Transform a;

		[SerializeField]
		private Transform b;

		[SerializeField] private OperationCollection opCollection;

		[SerializeField] private float radius;
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				var linop = new LineOp(a.position,b.position,radius,OperationType.Add);
				opCollection.Add(linop);
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				var linop = new LineOp(a.position, b.position, radius, OperationType.Remove);
				opCollection.Add(linop);
			}
		}

		private void OnDrawGizmos()
		{
			if (a != null && b != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(a.position, b.position);
			}
		}
	}
}