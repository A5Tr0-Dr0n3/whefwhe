public class VVR_STAMPER_AudioOnly : SceneBehaviorManager
{
	public AudioLoop startLoop;

	protected override void OnSceneActivated()
	{
		if (startLoop != null)
		{
			startLoop.Play();
		}
	}

	private void OnDisable()
	{
		if (startLoop != null)
		{
			startLoop.Stop();
		}
	}
}
