using UnityEngine;

public class SpotlightTurnOn : MonoBehaviour
{
	private VVR_Point point;

	private float maxRange;

	private void Start()
	{
		point = GetComponent<VVR_Point>();
		maxRange = point.range;
		point.range = 0f;
	}

	private void Update()
	{
		point.range = Mathf.Clamp(point.range += Time.deltaTime, 0f, maxRange);
		if (maxRange == point.intensity)
		{
			enabled = false;
		}
	}
}
