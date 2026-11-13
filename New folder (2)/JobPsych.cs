using System.Collections;
using UnityEngine;

public class JobPsych : SceneBehaviorJob
{
	public static JobPsych instance;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of JobPsych in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		GlobalCoroutineContainer.AddThread(PsychCoroutine(), this);
	}

	private IEnumerator PsychCoroutine()
	{
		yield return new WaitForSeconds(5f);
		GoToBackstage();
	}
}
