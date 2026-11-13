using UnityEngine;

[CreateAssetMenu(fileName = "New Garden Night Sequence Content", menuName = "TenderClaws/Create Garden Night Sequence Content", order = 1)]
public class GardenNightScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public GardenNightDialogue dialogue;
}
