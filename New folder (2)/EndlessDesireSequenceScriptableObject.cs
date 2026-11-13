using UnityEngine;

[CreateAssetMenu(fileName = "New Job Endless Desire Sequence Content", menuName = "TenderClaws/Create Endless Desire Sequence Content", order = 1)]
public class EndlessDesireSequenceScriptableObject : ScriptableObject
{
	public EndlessDesireDialogue dialogue;

	public float beat = 1.5f;

	public float waitBeforeFirstLine = 3f;
}
