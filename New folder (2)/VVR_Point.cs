using UnityEngine;

[ExecuteInEditMode]
public class VVR_Point : MonoBehaviour
{
	[Range(0f, 50f)]
	public float intensity = 1f;

	public float range = 10f;

	public bool mainLight;

	public bool dontLerpValues;

	public bool noAttenuation;

	private float startIntensity;

	private float startRange;

	private bool startValuesInitialized;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, range);
		Gizmos.DrawSphere(transform.position, 0.2f);
	}

	public void LerpValues(float t)
	{
		if (!dontLerpValues)
		{
			if (!startValuesInitialized)
			{
				startIntensity = intensity;
				startRange = range;
				startValuesInitialized = true;
			}
			intensity = t * startIntensity;
			range = t * startRange;
		}
	}
}
