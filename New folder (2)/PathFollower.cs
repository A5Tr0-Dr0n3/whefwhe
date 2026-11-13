using UnityEngine;

public class PathFollower : MonoBehaviour
{
	public TransformFollow path;

	public float flockSpeed = 1f;

	private Transform pathTransform;

	private Transform myTransform;

	private void Start()
	{
		pathTransform = path.transform;
		myTransform = transform;
	}

	private void Update()
	{
		myTransform.position = Vector3.MoveTowards(myTransform.position, pathTransform.position, flockSpeed * Time.deltaTime);
		myTransform.LookAt(pathTransform);
	}
}
