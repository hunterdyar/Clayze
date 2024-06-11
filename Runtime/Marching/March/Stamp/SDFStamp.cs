using Clayze.Marching.Operations;
using UnityEngine;

namespace Clayze.Marching
{
	public class SDFStamp : MonoBehaviour
	{
		[SerializeField] private OperationCollection opCollection;
		[SerializeField] private Texture3D sdf;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				var op = new SDFOperation(sdf, transform.position,transform.rotation,transform.localScale, OperationType.Add);
				opCollection.Add(op);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(transform.position,transform.localScale);
		}
	}
}