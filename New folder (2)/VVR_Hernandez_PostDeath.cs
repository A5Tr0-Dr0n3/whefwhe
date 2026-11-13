using UnityEngine;

public class VVR_Hernandez_PostDeath : SceneBehaviorManager
{
	[SerializeField]
	private HernandezC4Helper helperC4Controller;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		helperC4Controller.SetHernandezDeadState();
	}
}
