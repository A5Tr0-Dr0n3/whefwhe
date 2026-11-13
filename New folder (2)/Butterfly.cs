using System.Collections;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
	public GameObject leftWing;

	public GameObject rightWing;

	public Transform rotForwardLeft;

	public Transform rotBackLeft;

	public Transform rotForwardRight;

	public Transform rotBackRight;

	private Vector3 destination;

	private Vector3 originalPos;

	public float moveSpeed = 1f;

	public float flapSpeed = 15f;

	public float rotSpeed;

	public float moveRadius = 1f;

	public float newSpotMin = 0.25f;

	public float newSpotMax = 0.5f;

	private float lp;

	private float flapCount = 0.02f;

	private float flapTimer;

	private void Start()
	{
		originalPos = transform.position;
		StartCoroutine(NewDestination());
	}

	private void Update()
	{
		flapTimer += Time.deltaTime;
		if (flapTimer > flapCount)
		{
			flapTimer = 0f;
			lp = (Mathf.Sin(Time.time * flapSpeed) + 1f) * 0.5f;
			leftWing.transform.rotation = Quaternion.Lerp(rotForwardLeft.rotation, rotBackLeft.rotation, lp);
			rightWing.transform.rotation = Quaternion.Lerp(rotForwardRight.rotation, rotBackRight.rotation, lp);
			transform.position = Utility.TCVRLerp(transform.position, destination, moveSpeed);
			transform.rotation = Utility.TCVRLerp(transform.rotation, Random.rotation, rotSpeed);
		}
	}

	private IEnumerator NewDestination()
	{
		while (true)
		{
			destination = Random.insideUnitSphere * moveRadius + originalPos;
			yield return new WaitForSeconds(Random.Range(newSpotMin, newSpotMax));
		}
	}
}
