using System.Collections;

public class UnderwaterScript : SceneBehaviorManager
{
	public VVRDialogueSource chazDialogueSource;

	public NPCDialogueClip bernice;

	public NPCDialogueClip surface;

	protected override void OnFirstTimeSceneActivated()
	{
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BOID_DREAM);
		if (chazDialogueSource != null)
		{
			GlobalCoroutineContainer.AddThread(quickMono(), this);
		}
	}

	private IEnumerator quickMono()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazDialogueSource.ForcePlayClipOverwrite(bernice);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(8f, this);
		chazDialogueSource.ForcePlayClipOverwrite(surface);
		while (true)
		{
			yield return null;
		}
	}
}
