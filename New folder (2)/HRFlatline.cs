using UnityEngine;

public class HRFlatline : MonoBehaviour
{
	public GvrAudioSource flatline;

	public GvrAudioSource danger;

	private HRAICore lastCoreGrabbed;

	private void Update()
	{
		if (ToolManager.PlayerGrabbedActiveCore() != null)
		{
			lastCoreGrabbed = ToolManager.PlayerGrabbedActiveCore();
			if (!danger.isPlaying)
			{
				danger.transform.position = lastCoreGrabbed.transform.position;
				danger.Play();
			}
			return;
		}
		if (lastCoreGrabbed != null && lastCoreGrabbed.isDisconnected && !lastCoreGrabbed.isChaz)
		{
			flatline.transform.position = lastCoreGrabbed.transform.position;
			flatline.Play();
			lastCoreGrabbed = null;
		}
		danger.Stop();
	}
}
