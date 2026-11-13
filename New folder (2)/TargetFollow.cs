using UnityEngine;

public class TargetFollow : MonoBehaviour
{
	public bool immediate;

	[Range(0f, 10f)]
	public float speed = 5f;

	public GameObject target;

	private void Update()
	{
		if (immediate)
		{
			transform.position = target.transform.position;
			return;
		}
		transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
		transform.LookAt(target.transform);
	}
}
