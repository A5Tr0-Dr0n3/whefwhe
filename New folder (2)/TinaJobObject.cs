using UnityEngine;

[CreateAssetMenu(fileName = "New Tina Sequence Content", menuName = "TenderClaws/Create Tina Sequence Content", order = 1)]
public class TinaJobObject : ScriptableObject
{
	[Header("Intro")]
	public float introDelay = 1.5f;

	public float remindPlayerToPickDollTime = 5f;

	[Header("Audio")]
	public TinaDialogue dialogue;
}
