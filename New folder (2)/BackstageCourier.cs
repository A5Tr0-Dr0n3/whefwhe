using UnityEngine;

public class BackstageCourier : MonoBehaviour
{
	public float maxY = 200f;

	public float minY = -200f;

	public float speed = 100f;

	public float minTime = 5f;

	public float maxTime = 20f;

	public GvrAudioSource audioSource;

	private float timeCount;

	private bool movingUp;

	private float newY;

	private void Start()
	{
		if (Random.Range(0f, 1f) >= 0.5f)
		{
			movingUp = false;
			transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
		}
		else
		{
			transform.position = new Vector3(transform.position.x, minY, transform.position.z);
			movingUp = true;
		}
		timeCount = Random.Range(minTime, maxTime);
		GvrAudioSource component = GetComponent<GvrAudioSource>();
		if (audioSource == null && component != null)
		{
			audioSource = component;
		}
	}

	private void Update()
	{
		timeCount -= Time.deltaTime;
		if (timeCount < 0f)
		{
			if (audioSource != null && !audioSource.isPlaying)
			{
				audioSource.UnPause();
			}
			newY = speed * Time.deltaTime;
			if (!movingUp)
			{
				newY *= -1f;
			}
			transform.position = new Vector3(transform.position.x, transform.position.y + newY, transform.position.z);
			if (movingUp && transform.position.y > maxY)
			{
				movingUp = false;
				timeCount = Random.Range(minTime, maxTime);
			}
			else if (!movingUp && transform.position.y < minY)
			{
				movingUp = true;
				timeCount = Random.Range(minTime, maxTime);
			}
		}
		else if (audioSource != null && audioSource.isPlaying)
		{
			audioSource.Pause();
		}
	}
}
