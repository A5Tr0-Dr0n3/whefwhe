using System;
using UnityEngine;

[Serializable]
public class ElizaDialogue
{
	[Header("Intro Dialogue:")]
	public NPCDialogueClip[] aElizaGreetingsSection01;

	public NPCDialogueClip[] aElizaGreetingsSection02;

	public NPCDialogueClip[] aElizaGreetingsSection03;

	public NPCDialogueClip[] aElizaGreetingsSection04;

	public NPCDialogueClip[] aElizaGreetingsSection05;

	public FeedbackDialogueGroup aElizaGlitchAds;

	[Header("Environment-Specific Dialogue")]
	public NPCDialogueClip[] aOrderedGreetingsDialogue;

	public NPCDialogueClip[] aSunsetDialogue;

	public NPCDialogueClip[] aSunsetMermanDialogue;

	public NPCDialogueClip[] aCanyonDialogue;

	public NPCDialogueClip[] aCanyonAnnouncerDialogue;

	public NPCDialogueClip[] aVolcanoDialogue;

	public NPCDialogueClip[] aVolcanoAnnouncerDialogue;

	public NPCDialogueClip[] aCaveDialogue;

	public NPCDialogueClip[] aSpaceDialogue;

	public NPCDialogueClip[] aSpaceSaturnDialogue;

	[Header("On Sequence Dialogue:")]
	public NPCDialogueClip[] bOnMonologue;

	[Header("Shell Sequence Dialogue:")]
	public int cShellSections = 3;

	public NPCDialogueClip cPresentFirstShellLine;

	public ElizaFinalMonologueData cShellMonologueSection01;

	public ElizaFinalMonologueData cShellMonologueSection02;

	public ElizaFinalMonologueData cShellMonologueSection03;

	public NPCDialogueClip cPresentFinalShellLine;

	public FeedbackDialogueGroup cPreShellEncouragements;

	public FeedbackDialogueGroup cShellEncouragements;

	public ElizaShellData[] cShellTedTalks;

	[Header("End of Sequence Dialogue:")]
	public NPCDialogueClip[] dEndMonologue;

	public NPCDialogueClip dKickoutLine;
}
