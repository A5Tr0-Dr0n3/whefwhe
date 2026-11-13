using System;
using UnityEngine;

[Serializable]
public class GardenNightDialogue
{
	[Header("Sequence Start")]
	public NPCDialogueClip wCryLoop;

	public NPCDialogueClip wWhatAreYouDoingHere;

	public NPCDialogueClip wItDoesntEvenMatter;

	public NPCDialogueClip wTakeTheBroom;

	public NPCDialogueClip wDeadToMe;

	[Header("Sweeping Sequence")]
	public NPCDialogueClip sStartSweeping;

	public NPCDialogueClip sSideToSide;

	public NPCDialogueClip sInsideAndOut;

	public NPCDialogueClip sSweepFaster;

	public NPCDialogueClip sSweepUpYourShame;

	[Header("Done Sweeping")]
	public NPCDialogueClip dYouThinkYoureDone;

	public NPCDialogueClip dCrying;

	public NPCDialogueClip dDirtDirtDirt;

	public NPCDialogueClip dLookAway;

	[Header("Vacuuming Scene")]
	public NPCDialogueClip vNoooo;

	public NPCDialogueClip vWhy;
}
