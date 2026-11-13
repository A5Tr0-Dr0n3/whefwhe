using UnityEngine;

[CreateAssetMenu(fileName = "New Backstage Sequence Content", menuName = "TenderClaws/Create Backstage Sequence Content", order = 1)]
public class BackstageScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	[Header("Dialogue:")]
	public BackstageDialogue dialogue;
}
