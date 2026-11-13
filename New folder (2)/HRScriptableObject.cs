using UnityEngine;

[CreateAssetMenu(fileName = "New HR Sequence Content", menuName = "TenderClaws/Create HR Sequence Content", order = 1)]
public class HRScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public HRDialogue dialogue;
}
