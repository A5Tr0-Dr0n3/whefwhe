using TButt;
using UnityEngine;

public class HeadsetSignSwapper : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer ControllerMesh;

	[SerializeField]
	private GameObject Button;

	[SerializeField]
	private Texture GoTexture;

	[SerializeField]
	private Texture GearTexture;

	[SerializeField]
	private Texture RiftTexture;

	private void Start()
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.None:
			break;
		case VRPlatform.OculusPC:
			Object.Destroy(Button);
			ControllerMesh.material.mainTexture = RiftTexture;
			break;
		case VRPlatform.SteamVR:
			break;
		case VRPlatform.OculusMobile:
			if (OVRPlugin.productName == "Oculus Go")
			{
				ControllerMesh.material.mainTexture = GoTexture;
			}
			else
			{
				ControllerMesh.material.mainTexture = GearTexture;
			}
			break;
		case VRPlatform.PlayStationVR:
			break;
		case VRPlatform.Daydream:
			break;
		case VRPlatform.Cardboard:
			break;
		case VRPlatform.WindowsMR:
			break;
		}
	}
}
