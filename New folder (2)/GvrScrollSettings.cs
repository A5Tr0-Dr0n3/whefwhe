using UnityEngine;

public class GvrScrollSettings : MonoBehaviour, IGvrScrollSettings
{
	[Tooltip("Determines if movement inertia is enabled.")]
	public bool inertiaOverride = true;

	[Tooltip("The rate at which movement slows down.")]
	public float decelerationRateOverride = 0.05f;

	public bool InertiaOverride => inertiaOverride;

	public float DecelerationRateOverride => decelerationRateOverride;
}
