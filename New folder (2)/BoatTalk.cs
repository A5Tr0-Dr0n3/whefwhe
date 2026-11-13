using UnityEngine;

public class BoatTalk : MonoBehaviour
{
	public AudioSourceLoudness asl;

	public Transform mouth;

	public float maxScaleVal = 1.25f;

	private float zLerp;

	private void Awake()
	{
		if (asl == null)
		{
			asl = GetComponentInParent<AudioSourceLoudness>();
		}
	}

	private void Update()
	{
		if (mouth != null)
		{
			zLerp = Mathf.Lerp(zLerp, asl.lerpLoudness * 150f - 30f, Time.deltaTime * 5f);
			mouth.transform.localEulerAngles = new Vector3(0f, 0f, zLerp);
		}
	}
}
