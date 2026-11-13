using UnityEngine;

[CreateAssetMenu(fileName = "New Merge Sequence Content", menuName = "TenderClaws/Create Merge Sequence Content", order = 1)]
public class MergeScriptableObject : ScriptableObject
{
	[Header("Dialogue:")]
	public MergeDialogue dialogue;
}
