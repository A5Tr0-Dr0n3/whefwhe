using System;
using UnityEngine;

[Serializable]
public class MacyDialogue
{
	[Header("Intro")]
	public NPCDialogueClip welcomeHuman;

	public NPCDialogueClip[] goAwayList;

	public NPCDialogueClip goAwayRepeat;

	[Header("Re-entry")]
	public NPCDialogueClip[] monologueA;

	[Header("Parade Begin")]
	public NPCDialogueClip iRemember;

	public NPCDialogueClip[] monologueB;

	[Header("Balloon Pull")]
	public NPCDialogueClip monoBContinuation;

	public NPCDialogueClip monoBInterruption;

	public NPCDialogueClip[] balloonPullIntro;

	public NPCDialogueClip[] miscDialogue;

	[Header("Balloon Pull Reactions")]
	public NPCDialogueClip[] owlBalloon;

	public NPCDialogueClip[] frogBalloon;

	public NPCDialogueClip[] rabbitBalloon;

	public NPCDialogueClip[] carrotBalloon;

	public NPCDialogueClip[] chazBalloon;

	public NPCDialogueClip[] starBalloon;

	public NPCDialogueClip[] fishBalloon;

	public NPCDialogueClip[] soccerBallBalloon;

	public NPCDialogueClip[] surfboardBalloon;

	public NPCDialogueClip[] toastBalloon;

	public NPCDialogueClip[] cactusBalloon;

	public NPCDialogueClip[] possumBalloon;

	public NPCDialogueClip[] gloveBalloon;

	public NPCDialogueClip[] genericReaction;

	[Header("Endings")]
	public NPCDialogueClip someBalloonsLeft;

	public NPCDialogueClip noBalloonsLeft;
}
