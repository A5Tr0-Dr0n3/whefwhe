using UnityEngine;

public class TransformFollowMulti : MonoBehaviour
{
	[Header("Tweakables")]
	[Range(0f, 10f)]
	[SerializeField]
	private float _speed = 4f;

	[SerializeField]
	private bool _rotate = true;

	[Header("Followers")]
	[SerializeField]
	private Transform[] _followers;

	[SerializeField]
	private int[] _currFollowerTarget;

	[Header("Path")]
	public Vector3[] nodes;

	private float _sqrDistFromNode;

	private float _magicNumber = 0.9f;

	private void OnDrawGizmosSelected()
	{
		if (nodes == null || nodes.Length <= 1)
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
				else
				{
					Gizmos.DrawLine(nodes[i], nodes[0]);
				}
			}
		}
	}

	private void Update()
	{
		if (_followers.Length != _currFollowerTarget.Length)
		{
			Debug.LogWarning("TransformFollowMulti follower array lengths do not match!");
			return;
		}
		for (int i = 0; i < _followers.Length; i++)
		{
			_followers[i].position = Vector3.MoveTowards(_followers[i].position, nodes[_currFollowerTarget[i]], _speed * Time.deltaTime);
			if (_followers[i].position == nodes[_currFollowerTarget[i]])
			{
				_currFollowerTarget[i] = ((_currFollowerTarget[i] + 1 < nodes.Length) ? (_currFollowerTarget[i] + 1) : 0);
			}
			if (_rotate)
			{
				_followers[i].LookAt(Vector3.Lerp(_followers[i].position + _followers[i].forward, nodes[_currFollowerTarget[i]], _magicNumber * _speed * Time.deltaTime));
			}
		}
	}
}
