using UnityEngine;

public class FireFlicker : MonoBehaviour
{
	public float baseIntensity;

	public float baseRange;

	public float noise = 0.3f;

	private VVR_Point point;

	private void Start()
	{
		point = GetComponent<VVR_Point>();
	}

	private void Update()
	{
		point.intensity = baseIntensity * (1f - noise) + baseIntensity * noise * Mathf.PerlinNoise(Time.time * 8f, 0f);
		point.range = baseRange * (1f - noise) + baseRange * noise * Mathf.PerlinNoise(Time.time * 5f, 0f);
	}
}
