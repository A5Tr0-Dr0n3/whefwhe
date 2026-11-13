using UnityEngine;

public class RadioTalker : MonoBehaviour
{
	public AudioSourceLoudness sourceMeter;

	public Transform littleGlow;

	public Transform bigGlow;

	public VVR_Point vvrLight;

	public VVRConsumer shaker;

	public float lightBase = 0.5f;

	public float lightScalar = 2f;

	private Vector3 initLGS;

	private Vector3 initBGS;

	private float initRR;

	private float initRP;

	private float initIntensity;

	private void Start()
	{
		if (littleGlow != null)
		{
			initLGS = littleGlow.localScale;
		}
		if (bigGlow != null)
		{
			initBGS = bigGlow.localScale;
		}
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
		if (littleGlow != null)
		{
			littleGlow.localScale = initLGS * sourceMeter.lerpLoudness * 2f;
		}
		if (bigGlow != null)
		{
			bigGlow.localScale = initBGS * sourceMeter.lerpLoudness + new Vector3(1f, 1f, 1f);
		}
		if (shaker != null)
		{
			shaker.restRange = initRR * sourceMeter.lerpLoudness * 1.5f + 1.8f;
			shaker.restPower = initRP * sourceMeter.lerpLoudness * 3f + 0.15f;
		}
		if (vvrLight != null)
		{
			vvrLight.intensity = initIntensity * sourceMeter.lerpLoudness * lightScalar + lightBase;
		}
	}
}
