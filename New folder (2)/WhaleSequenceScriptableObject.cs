using UnityEngine;

[CreateAssetMenu(fileName = "New Archive Whale Sequence Content", menuName = "TenderClaws/Create Archive Whale Sequence Content", order = 1)]
public class WhaleSequenceScriptableObject : ScriptableObject
{
	public WhaleDialogue dialogue;

	[Header("Whale Completion Effect:")]
	public float blockyMinDistortion = 0.89f;

	public float blockyOneCableDistortion = 1.5f;

	public float blockyTwoCabeDistortion = 3.5f;

	public float blockyComplete = 100f;

	public float blockyMassive = 5000f;

	public float transitionTime = 1f;

	public float completeTransitionTime = 5f;

	public float barnacleCompletionPercentage = 0.5f;

	public float barnacleCompletionPercentageDuringMonologue = 0.9f;

	[Header("Whale Dialogue Timing:")]
	public float berniceMonologuePrePadding = 2f;

	public float berniceMonologueInterruptPadding = 3f;

	public float barnaclePleaPadding = 3f;

	public float barnacleEncouragementPadding = 8f;

	public float postHeadSetDialoguePadding = 8f;

	public float cablePleaPadding = 10f;

	public float cablePleaRepeatTime = 45f;
}
