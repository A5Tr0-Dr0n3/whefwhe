using UnityEngine;

public class BallTopTransform : MonoBehaviour
{
	public Transform ballTransform;

	public float verticalOffset;

	private Transform myTransform;

	private void Awake()
	{
	}

	private void Start()
	{
		myTransform = transform;
		myTransform.position = ballTransform.position + Vector3.up * verticalOffset;
	}

	private void Update()
	{
		myTransform.position = ballTransform.position + Vector3.up * verticalOffset;
	}
}
