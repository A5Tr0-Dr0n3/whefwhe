using UnityEngine;

public class TargetFollowRotate : MonoBehaviour
{
	[Range(0f, 10f)]
	public float speed = 5f;

	public GameObject target;

	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, speed * Time.deltaTime * 10f);
	}
}
