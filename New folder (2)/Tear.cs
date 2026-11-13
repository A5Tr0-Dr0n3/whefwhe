using UnityEngine;

public class Tear : MonoBehaviour
{
	private bool found;

	private Vector3 startPosition;

	private float t;

	private void OnCollisionEnter(Collision collision)
	{
		found = true;
		startPosition = transform.position;
		Object.Destroy(GetComponent<Rigidbody>());
		Object.Destroy(GetComponent<Collider>());
	}

	private void Update()
	{
		if (found)
		{
			AnimateTear();
		}
	}

	private void AnimateTear()
	{
		t += 1f * Time.deltaTime;
		startPosition.y += 4f * Time.deltaTime;
		transform.position = Vector3.Lerp(startPosition, EagerMaster.instance.player.transform.position, t);
		if (t > 1f)
		{
			Object.Destroy(gameObject);
		}
	}
}
