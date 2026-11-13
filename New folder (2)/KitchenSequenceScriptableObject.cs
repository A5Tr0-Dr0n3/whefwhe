using UnityEngine;

[CreateAssetMenu(fileName = "New Job Kitchen Sequence Content", menuName = "TenderClaws/Create Kitchen Sequence Content", order = 1)]
public class KitchenSequenceScriptableObject : ScriptableObject
{
	public KitchenDialogue dialogue;

	public float attentionGrabberPadding = 2.5f;

	public float additionalAttentionGrabberPadding = 15f;

	[Range(0f, 1f)]
	public float maxResetLineDialogueCompletion = 0.3f;

	[Range(0f, 1f)]
	public float maxInterruptionDialogueCompletion = 0.8f;

	public int maxSoliloquyInterruptions = 1;

	public float chatiseDelayAfterButterDrop = 1.5f;

	public float chatiseDelayAfterUserIsGrabbingShit = 0.75f;

	public float soliloquySentencesBeforeChatise = 999f;

	public float toasterPopInTime = 2f;

	public float toasterPopInMinPitch = 1f;

	public float toasterPopInMaxPitch = 1.6f;

	public float minimumToastVacuumPercentage = 0.1f;

	public float vacuumCutoffTime = 50f;

	[Range(0f, 1f)]
	public float cutoffKitchenToastVacuumPercentage = 0.1f;

	[Range(0f, 1f)]
	public float cutoffButterToastVacuumPercentage = 0.1f;
}
