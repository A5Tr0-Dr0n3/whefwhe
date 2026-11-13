using UnityEngine;

[CreateAssetMenu(fileName = "New Garden Sequence Content", menuName = "TenderClaws/Create Garden Sequence Content", order = 1)]
public class GardenScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public GardenDialogue dialogue;
}
