using UnityEngine;

public class DesktopFireFlicker : MonoBehaviour
{
	internal float startIntensity;

	internal float startRange;

	public float noise = 0.3f;

	public DesktopLight mLight;

	private void Start()
	{
		startIntensity = mLight._intensity;
		startRange = mLight._range;
	}

	private void Update()
	{
		mLight.mLight.intensity = startIntensity * (1f - noise) + startIntensity * noise * Mathf.PerlinNoise(Time.time * 8f, 0f);
		mLight.mLight.range = startRange * (1f - noise) + startRange * noise * Mathf.PerlinNoise(Time.time * 5f, 0f);
	}
}
