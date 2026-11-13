using UnityEngine;

public class FoamTalker : MonoBehaviour
{
	public VVR_Point vvrLight;

	public VVRConsumer shaker;

	public float lightBase = 0.5f;

	public float lightScalar = 2f;

	public float rangeScaler = 1.5f;

	public float powerScalar = 3f;

	private float initRR;

	private float initRP;

	private float initIntensity;

	private float perlinScalar;

	private float perlinSubScalar;

	public float noiseSpeed = 1f;

	private void Start()
	{
		if (shaker != null)
		{
			initRR = shaker.restRange;
		}
		if (shaker != null)
		{
			initRP = shaker.restPower;
		}
		if (vvrLight != null)
		{
			initIntensity = vvrLight.intensity;
		}
	}

	private void Update()
	{
		perlinScalar = Mathf.PerlinNoise(Time.time * noiseSpeed, 2f);
		perlinScalar *= 0.95f + 0.05f * Mathf.PerlinNoise(Time.time * noiseSpeed * 5.2f, 23.2f);
		if (shaker != null)
		{
			shaker.restRange = initRR * perlinScalar * rangeScaler + 1.8f;
			shaker.restPower = initRP * perlinScalar * powerScalar + 0.15f;
		}
		if (vvrLight != null)
		{
			vvrLight.intensity = initIntensity * perlinScalar * lightScalar + lightBase;
		}
	}
}
