using UnityEngine;

[RequireComponent(typeof(AudioSourceLoudness))]
public class C4Talker : MonoBehaviour
{
	[SerializeField]
	private float quietestYScale = 0.5f;

	[SerializeField]
	private float loudestYScale = 1.5f;

	[SerializeField]
	private float quietestXZScale = 0.9f;

	[SerializeField]
	private float loudestXZScale = 1.2f;

	[SerializeField]
	private float loudnessLerpScalar = 5f;

	[SerializeField]
	private GvrAudioSource talkerAudioSource;

	private AudioSourceLoudness mLoudnessModule;

	private Vector3 referenceScale;

	private void Start()
	{
		mLoudnessModule = GetComponent<AudioSourceLoudness>();
		referenceScale = transform.localScale;
	}

	private void Update()
	{
		if (talkerAudioSource.isPlaying)
		{
			float y = Mathf.Lerp(quietestYScale, loudestYScale, mLoudnessModule.lerpLoudness * loudnessLerpScalar);
			float num = Mathf.Lerp(quietestXZScale, loudestXZScale, mLoudnessModule.lerpLoudness * loudnessLerpScalar);
			transform.localScale = new Vector3(num, y, num);
		}
		else
		{
			transform.localScale = referenceScale;
		}
	}
}
