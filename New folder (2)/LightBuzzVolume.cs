using TButt;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class LightBuzzVolume : MonoBehaviour
{
	public enum Mode
	{
		Intensity,
		Range
	}

	public Mode mode;

	public VVR_Point light;

	public DesktopLight desktopLight;

	public float scalar = 1f;

	private GvrAudioSource buzz;

	private void Start()
	{
		buzz = GetComponent<GvrAudioSource>();
	}

	private void Update()
	{
		if (!(buzz != null))
		{
			return;
		}
		if (mode == Mode.Intensity)
		{
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				buzz.volume = desktopLight.mLight.intensity * scalar;
			}
		}
		else if (mode == Mode.Range && TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			buzz.volume = desktopLight.mLight.range * scalar;
		}
	}
}
