using UnityEngine;

[CreateAssetMenu(fileName = "New Tumbleweed Two Sequence Content", menuName = "TenderClaws/Create Tumbleweed Two Sequence Content", order = 1)]
public class TumbleweedTwoScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float tumbleSectionOne = 15f;

	public float tumbleSectionTwo = 15f;

	public float tumbleSectionThree = 15f;

	[Header("Dialogue:")]
	public TumbleweedTwoDialogue dialogue;
}
