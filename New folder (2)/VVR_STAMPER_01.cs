public class VVR_STAMPER_01 : SceneBehaviorManager
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
		startLoop.Stop();
	}
}
