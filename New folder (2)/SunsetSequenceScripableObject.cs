using UnityEngine;

[CreateAssetMenu(fileName = "New Sunset Sequence Content", menuName = "TenderClaws/Create Sunset Sequence Content", order = 1)]
public class SunsetSequenceScripableObject : ScriptableObject
{
	[Header("Audio")]
	public SunsetDialogue dialogueOptions;
}
