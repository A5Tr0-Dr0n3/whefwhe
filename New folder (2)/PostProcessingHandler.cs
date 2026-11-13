using TButt;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingHandler : MonoBehaviour
{
	[SerializeField]
	private PostProcessVolume mVolume;

	[SerializeField]
	private PostProcessProfile steamVRProfle;

	[SerializeField]
	private PostProcessProfile oculusVRProfile;

	[SerializeField]
	private PostProcessProfile psvrProfile;

	private void Awake()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			mVolume.profile = oculusVRProfile;
			mVolume.enabled = true;
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.PlayStationVR)
		{
			mVolume.profile = psvrProfile;
			mVolume.enabled = true;
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.SteamVR)
		{
			mVolume.profile = steamVRProfle;
			mVolume.enabled = true;
		}
		else
		{
			SafeDebug.Log("You are on a platform that does not support post processing, destroying post processing handler");
			Object.Destroy(gameObject);
		}
	}
}
