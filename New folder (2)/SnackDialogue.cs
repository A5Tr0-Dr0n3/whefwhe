using System;
using UnityEngine;

[Serializable]
public class SnackDialogue
{
	[Header("Intro Dialogue:")]
	public NPCDialogueClip OhItsYou;

	public NPCDialogueClip LetsGetStarted;

	public NPCDialogueClip PutThemOnMe;

	public NPCDialogueClip HopeForImprovement;

	[Header("Critiques:")]
	public NPCDialogueClip[] Critiques;

	[Header("Feedback:")]
	public FeedbackDialogueGroup Underdone;

	public FeedbackDialogueGroup Perfect;

	public FeedbackDialogueGroup Burnt;

	public FeedbackDialogueGroup BurntAngry;

	[Header("Stop/Vacuum Dialogue:")]
	public NPCDialogueClip Stop;

	public NPCDialogueClip[] VacuumMonologue;

	[Header("Vacuum Reaction:")]
	public NPCDialogueClip WhatsThis;

	public NPCDialogueClip YesItIs;

	public NPCDialogueClip NoWordOfThis;

	public NPCDialogueClip ImReady;

	[Header("Yells:")]
	public NPCDialogueClip[] Yells;
}
