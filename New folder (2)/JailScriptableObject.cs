using UnityEngine;

[CreateAssetMenu(fileName = "New Jail Sequence Content", menuName = "TenderClaws/Create Jail Sequence Content", order = 1)]
public class JailScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public JailDialogue dialogue;
}
