using UnityEngine;

public class WhaleBoundingBox : MonoBehaviour
{
	public VVR_Whale whaleSceneLogic;

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == whaleSceneLogic.headset.gameObject && !ToolManager.PlayerGrabbingGameObject(other.gameObject))
		{
			whaleSceneLogic.ResetHeadset();
		}
	}
}
