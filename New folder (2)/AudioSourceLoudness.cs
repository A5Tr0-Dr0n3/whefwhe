using UnityEngine;

public class AudioSourceLoudness : MonoBehaviour
{
	public GvrAudioSource audioSource;

	public float updateStep = 0.06f;

	public int sampleDataLength = 128;

	public int sampleOffset = 7000;

	private float currentUpdateTime;

	private float clipLoudness;

	private float[] clipSampleData;

	[SerializeField]
	private AnimationCurve loudnessFilter = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

	private Vector3 startPosition;

	private int sampleTime;

	private int i;

	public float lerpLoudness { get; private set; }

	private void Awake()
	{
		if (!audioSource)
		{
			audioSource = GetComponentInChildren<GvrAudioSource>();
		}
		if (!audioSource)
		{
			SafeDebug.LogError(string.Concat(GetType(), ".Awake: there was no audioSource set."));
		}
		clipSampleData = new float[sampleDataLength];
	}

	public float GetFilteredLoudness()
	{
		return loudnessFilter.Evaluate(lerpLoudness);
	}

	private void Update()
	{
		if (audioSource.isPlaying)
		{
			currentUpdateTime += Time.deltaTime;
			if (currentUpdateTime >= updateStep)
			{
				sampleTime = Mathf.Min(audioSource.timeSamples + sampleOffset, audioSource.clip.samples - sampleDataLength);
				currentUpdateTime = 0f;
				audioSource.clip.GetData(clipSampleData, sampleTime);
				clipLoudness = 0f;
				i = 0;
				for (i = 0; i < clipSampleData.Length; i++)
				{
					clipLoudness += Mathf.Abs(clipSampleData[i]);
				}
				clipLoudness /= sampleDataLength;
			}
			lerpLoudness = Mathf.Lerp(lerpLoudness, clipLoudness, 13f * Time.deltaTime);
		}
		else
		{
			lerpLoudness = Mathf.Lerp(lerpLoudness, 0f, 13f * Time.deltaTime);
		}
	}
}
