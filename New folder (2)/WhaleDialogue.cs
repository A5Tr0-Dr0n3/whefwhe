using System;
using UnityEngine;

[Serializable]
public class WhaleDialogue
{
	[Header("Intro Dialogue:")]
	public NPCDialogueClip aBerniceHumming;

	public NPCDialogueClip[] aBerniceMonologue;

	public NPCDialogueClip[] aBerniceMonologueInterruptions;

	public NPCDialogueClip aFinishPluggingCables;

	public NPCDialogueClip aFinalPlugLine;

	public NPCDialogueClip[] aWaitForCables;

	[Header("Barnacle Sequence:")]
	public NPCDialogueClip[] bBarnaclePleas;

	public FeedbackDialogueGroup bFeedbackBarnacleOff;

	public NPCDialogueClip[] bBarnacleInterruptions;

	public NPCDialogueClip[] bBarnacleEncouragement;

	[Header("Complete Barnacle Sequence:")]
	public NPCDialogueClip[] cBerniceThanks;

	[Header("Post Headset Dialogue:")]
	public NPCDialogueClip[] dBernicePassDialogue;

	public NPCDialogueClip[] dBerniceHints;
}
