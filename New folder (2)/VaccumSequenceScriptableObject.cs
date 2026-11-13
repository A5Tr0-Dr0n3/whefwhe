using UnityEngine;

[CreateAssetMenu(fileName = "New Vaccum Sequence Content", menuName = "TenderClaws/ACreate Vaccum Sequence", order = 1)]
public class VaccumSequenceScriptableObject : ScriptableObject
{
	[Header("Timing")]
	public float startDelay = 3f;

	[Header("Audio")]
	public VaccumDialogue dialogueOptions;
}
