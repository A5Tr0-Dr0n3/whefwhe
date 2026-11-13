using UnityEngine;

public class ChazTalk : MonoBehaviour
{
	public AudioSourceLoudness asl;

	private Vector3 startScale;

	private Vector3 maxScale;

	public Transform mouth;

	public float maxScaleVal = 1.25f;

	public bool invertTalk;

	public float lerpSpeed = 4f;

	private void Awake()
	{
		startScale = mouth.localScale;
		maxScale = startScale * maxScaleVal;
		if (asl == null)
		{
			asl = GetComponentInParent<AudioSourceLoudness>();
		}
		if (invertTalk)
		{
			maxScaleVal = 1f / maxScaleVal;
		}
		maxScale = startScale * maxScaleVal;
	}

	private void Update()
	{
		if (mouth != null)
		{
			mouth.localScale = Vector3.Lerp(startScale, maxScale, asl.lerpLoudness * lerpSpeed);
		}
	}
}
