using UnityEngine;

[CreateAssetMenu(fileName = "New Job Snack Sequence Content", menuName = "TenderClaws/Create Snack Sequence Content", order = 1)]
public class SnackSequenceScriptableObject : ScriptableObject
{
	public SnackDialogue dialogue;

	public float beat = 1.5f;

	public float waitBeforeFirstLine = 3f;
}
