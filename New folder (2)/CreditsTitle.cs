using UnityEngine;

public class CreditsTitle : MonoBehaviour
{
	private Vector3 defaultLocalPos;

	public Transform CharacterTransform;

	private void Start()
	{
		defaultLocalPos = transform.position - CharacterTransform.position;
	}

	private void Update()
	{
		Vector3 b = CharacterTransform.transform.position + defaultLocalPos;
		transform.position = Vector3.Lerp(transform.position, b, 0.05f);
		Quaternion b2 = Quaternion.LookRotation(transform.position - EagerMaster.instance.mainCamera.transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, b2, 0.05f);
	}
}
