using UnityEngine;

public class ChazCaptureAnimationEvents : MonoBehaviour
{
	[SerializeField]
	private VVR_Hernandez hernandezLevelController;

	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	private HernandezTweakablesScriptableObject hernandezTweakables;

	[SerializeField]
	private AudioEvent footstepSoundSource;

	public void LazerHeadset()
	{
		hernandezLevelController.ChazLazerHeadset();
	}

	public void GiantChazChastize()
	{
		chazDialogueSource.PlayClip(hernandezTweakables.chazDialogue.chazChastize);
	}

	public void GiantFootsteps()
	{
		footstepSoundSource.Play();
	}
}
