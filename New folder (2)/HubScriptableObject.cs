using UnityEngine;

[CreateAssetMenu(fileName = "New Hub Sequence Content", menuName = "TenderClaws/Create Hub Sequence Content", order = 1)]
public class HubScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public HubDialogue dialogue;
}
