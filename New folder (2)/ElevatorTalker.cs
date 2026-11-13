using UnityEngine;

public class ElevatorTalker : MonoBehaviour
{
	[SerializeField]
	private float minPointLightIntensity = 0.7f;

	[SerializeField]
	private float maxPointLightIntensity = 3.5f;

	[SerializeField]
	private float loudnessLerpMultiplier = 2f;

	private AudioSourceLoudness elevatorLoudness;

	private VVR_Point mPointSource;

	private void Start()
	{
		elevatorLoudness = GetComponent<AudioSourceLoudness>();
		mPointSource = GetComponent<VVR_Point>();
	}

	private void Update()
	{
		mPointSource.intensity = Mathf.Lerp(minPointLightIntensity, maxPointLightIntensity, elevatorLoudness.lerpLoudness * loudnessLerpMultiplier);
	}
}
