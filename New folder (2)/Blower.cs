using UnityEngine;

public class Blower : MonoBehaviour
{
	public Rigidbody rotator;

	public float zoneSize = 3f;

	public float gustForce = 0.25f;

	public float beginningTorque;

	public float frontSpawn;

	public float backSpawn;

	public GvrAudioSource[] audio;

	private Rigidbody[] headsetRb;

	private float gust;

	private void Start()
	{
		headsetRb = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < headsetRb.Length; i++)
		{
			headsetRb[i].AddTorque(new Vector3(Random.Range(0f, beginningTorque), Random.Range(0f, beginningTorque), Random.Range(0f, beginningTorque)));
		}
	}

	private void FixedUpdate()
	{
		gust = rotator.angularVelocity.z;
		for (int i = 0; i < headsetRb.Length; i++)
		{
			if (headsetRb[i].transform.position.z > frontSpawn)
			{
				headsetRb[i].transform.position = new Vector3(Random.Range(transform.position.x - zoneSize, transform.position.x + zoneSize), Random.Range(transform.position.y - zoneSize, transform.position.y + zoneSize), backSpawn + 0.1f);
			}
			if (headsetRb[i].transform.position.z < backSpawn)
			{
				headsetRb[i].transform.position = new Vector3(Random.Range(transform.position.x - zoneSize, transform.position.x + zoneSize), Random.Range(transform.position.y - zoneSize, transform.position.y + zoneSize), frontSpawn - 0.1f);
			}
			headsetRb[i].AddForce(Vector3.forward * gust * gustForce);
		}
		if (audio != null && audio.Length > 0)
		{
			for (int j = 0; j < audio.Length; j++)
			{
				audio[j].volume = Mathf.Clamp(Mathf.Lerp(audio[j].volume, Mathf.Abs(gust), 0.01f), 0.1f, 0.8f);
			}
		}
	}
}
