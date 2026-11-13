using UnityEngine;

public class TransformFollow : MonoBehaviour
{
	[Range(0f, 10f)]
	public float speed = 4f;

	public bool looping = true;

	public bool rotate;

	private bool finished;

	public Vector3[] nodes;

	private int currentNode;

	private float _distFromNode;

	private float _magicNumber = 9f;

	private void OnDrawGizmosSelected()
	{
		if (nodes == null || nodes.Length <= 0)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			ref Vector3 reference = ref nodes[0];
			reference = transform.position;
		}
		for (int i = 0; i < nodes.Length; i++)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(nodes[i], 0.1f);
			if (nodes.Length >= 2)
			{
				if (i + 1 < nodes.Length)
				{
					Gizmos.DrawLine(nodes[i], nodes[i + 1]);
				}
				else if (looping)
				{
					Gizmos.DrawLine(nodes[i], nodes[0]);
				}
			}
		}
	}

	private void Update()
	{
		_distFromNode = Vector3.Distance(transform.position, nodes[currentNode]);
		if (_distFromNode < speed * Time.deltaTime)
		{
			if (!looping && currentNode + 1 >= nodes.Length)
			{
				finished = true;
			}
			currentNode = ((currentNode + 1 < nodes.Length) ? (currentNode + 1) : 0);
			_distFromNode = Vector3.Distance(transform.position, nodes[currentNode]);
		}
		if (!looping || !finished)
		{
			transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode], speed * Time.deltaTime);
			if (rotate)
			{
				transform.LookAt(Vector3.Lerp(transform.position + transform.forward * _distFromNode, nodes[currentNode], _magicNumber * speed * Time.deltaTime));
			}
		}
	}

	public Vector3 destination()
	{
		return nodes[currentNode];
	}
}
