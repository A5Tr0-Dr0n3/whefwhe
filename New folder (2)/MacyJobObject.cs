using UnityEngine;

[CreateAssetMenu(fileName = "New Macy Sequence Content", menuName = "TenderClaws/Create Macy Sequence Content", order = 1)]
public class MacyJobObject : ScriptableObject
{
	[Header("Timing")]
	public float introDelay = 3f;

	public float pleaseLeaveDelays = 8f;

	public float reentryDelay = 1.5f;

	public float monologueClipCutoff = 1.5f;

	public float balloonPullDuration = 45f;

	public float silenceMax = 10f;

	public float endTime = 1.5f;

	[Header("Audio")]
	public MacyDialogue dialogue;

	[HideInInspector]
	public int owlBalloonIndex;

	[HideInInspector]
	public int frogBalloonIndex;

	[HideInInspector]
	public int rabbitBalloonIndex;

	[HideInInspector]
	public int carrotBalloonIndex;

	[HideInInspector]
	public int chazBalloonIndex;

	[HideInInspector]
	public int starBalloonIndex;

	[HideInInspector]
	public int fishBalloonIndex;

	[HideInInspector]
	public int soccerBallBalloonIndex;

	[HideInInspector]
	public int surfboardBalloonIndex;

	[HideInInspector]
	public int toastBalloonIndex;

	[HideInInspector]
	public int cactusBalloonIndex;

	[HideInInspector]
	public int possumBalloonIndex;

	[HideInInspector]
	public int gloveBalloonIndex;

	[HideInInspector]
	public int genericBalloonIndex;
}
