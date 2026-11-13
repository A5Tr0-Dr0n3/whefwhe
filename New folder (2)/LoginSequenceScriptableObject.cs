using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Login Sequence Content", menuName = "TenderClaws/Create Tutorial Login Sequence Content", order = 1)]
public class LoginSequenceScriptableObject : ScriptableObject
{
	[Header("Tweakables:")]
	public float pourSequenceHurryUpDelay = 10f;

	public float pourSequenceTargetCompletionTime = 15f;

	public float lavaContractTime = 15f;

	public float headsetSpawnRate = 60f;

	public float headsetHurryUpDelay = 10f;

	[Header("Dialogue:")]
	public TutorialDialogue dialogue;
}
