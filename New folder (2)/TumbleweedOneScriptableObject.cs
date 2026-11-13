using UnityEngine;

[CreateAssetMenu(fileName = "New Tumbleweed One Sequence Content", menuName = "TenderClaws/Create Tumbleweed One Sequence Content", order = 1)]
public class TumbleweedOneScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float firstWait = 5f;

	public float tumbleSectionOne = 15f;

	public float grizzlyWait = 3f;

	public float tumbleSectionTwo = 15f;

	public float tumbleSectionThree = 7f;

	public float tumbleSectionFour = 10f;

	[Header("Dialogue:")]
	public TumbleweedOneDialogue dialogue;
}
