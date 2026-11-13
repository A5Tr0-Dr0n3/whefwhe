using System.Collections;

public class GardenVignetteScript : SceneBehaviorManager
{
	public ChazController chazControl;

	public VVRDialogueSource chazDialogueSource;

	public NPCDialogueClip chazQuickMono;

	public NPCDialogueClip timeToGo;

	protected override void OnFirstTimeSceneActivated()
	{
		if (chazControl != null)
		{
		}
		if (chazDialogueSource != null && chazQuickMono != null)
		{
			GlobalCoroutineContainer.AddThread(quickMono(), this);
		}
	}

	private IEnumerator quickMono()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazDialogueSource.ForcePlayClipOverwrite(chazQuickMono);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(10f, this);
		chazDialogueSource.ForcePlayClipOverwrite(timeToGo);
	}
}
