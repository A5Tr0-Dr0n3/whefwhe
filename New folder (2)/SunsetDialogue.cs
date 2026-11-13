using System;
using UnityEngine;

[Serializable]
public class SunsetDialogue
{
	[Header("Intro")]
	public NPCDialogueClip itsANiceDay;

	public NPCDialogueClip putYourControllerDown;

	public NPCDialogueClip perfect;

	[Header("Monologue One")]
	public DialogueScriptableObject beforeTheSunSets;

	public NPCDialogueClip endOfFirstMonologuea;

	public NPCDialogueClip endOfFirstMonologueb;

	public NPCDialogueClip endOfFirstMonologuec;

	[Header("Monologue Two")]
	public NPCDialogueClip really;

	public NPCDialogueClip whatTheHellMan;

	public NPCDialogueClip notAHealthyLifestyle;

	public DialogueScriptableObject monologueBody;

	public NPCDialogueClip giveMeThat;

	public NPCDialogueClip countingSunsets;

	public NPCDialogueClip doneHere;

	public NPCDialogueClip alignYourSoul;

	[Header("Rapid Sunsets")]
	public NPCDialogueClip oneTwoThree;

	public NPCDialogueClip youLikeThat;

	public NPCDialogueClip fourFiveSix;

	public NPCDialogueClip wellDontYou;

	public NPCDialogueClip sevenEight;

	public NPCDialogueClip boringYou;

	[Header("Grabbing Interruptions Monologue 1")]
	public NPCDialogueClip[] grabReactions;

	[Header("Grabbing Interruptions Monologue 2")]
	public NPCDialogueClip[] grabReactions2;

	[Header("Misc")]
	public NPCDialogueClip[] miscDialogue;
}
