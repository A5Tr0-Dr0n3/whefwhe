using System;
using UnityEngine;

[Serializable]
public class KitchenDialogue
{
	[Header("Intro Dialogue:")]
	public NPCDialogueClip[] aHeyOverHereAttentionGrabbers;

	public NPCDialogueClip[] aAdditionalAttentionGrabbers;

	public NPCDialogueClip aYoureNewHere;

	public NPCDialogueClip aPickMeUp;

	public NPCDialogueClip aPutMeOnTable;

	public NPCDialogueClip aTableAreYouIncapable;

	public NPCDialogueClip aSlipSomethingMoreComfortable;

	public NPCDialogueClip aChangeRadio;

	[Header("Toast Tutorial Dialogue:")]
	public NPCDialogueClip[] bIndulgeMeTutorialSoliloquy;

	public NPCDialogueClip bPayAttentionInterruption;

	public NPCDialogueClip bClickTheToaster;

	public NPCDialogueClip bPickUpTheToast;

	public NPCDialogueClip[] bToastReady;

	public FeedbackDialogueGroup bFeedbackUnderdone;

	public FeedbackDialogueGroup bFeedbackPerfect;

	public FeedbackDialogueGroup bFeedbackBurnt;

	public NPCDialogueClip bButWhyHaveOneToaster;

	[Header("Main Toast Sequence:")]
	public NPCDialogueClip cHowRich;

	public NPCDialogueClip[] cMainToastSoliloquy;

	public FeedbackDialogueGroup cFeedbackUnderdone;

	public FeedbackDialogueGroup cFeedbackPerfect;

	public FeedbackDialogueGroup cFeedbackBurnt;

	public FeedbackDialogueGroup cFeedbackTooSlow;

	[Header("Vacuum Sequence:")]
	public NPCDialogueClip dJustStop;

	public NPCDialogueClip dHighHopes;

	public NPCDialogueClip dVacuumUpThisMess;

	public NPCDialogueClip[] dIneptMonologue;

	public NPCDialogueClip dCleanMeUpAndYouCanGo;

	public NPCDialogueClip dActuallyThatDoesntFeelSoBad;

	public NPCDialogueClip dRightInTheGlobules;

	public NPCDialogueClip dHurryVacuumMeUp;

	public NPCDialogueClip dOneLodgedDeep;

	public NPCDialogueClip dScream;

	[Header("Object Stick Reactions:")]
	public NPCDialogueClip oCarrot;

	public NPCDialogueClip oFrog;

	public NPCDialogueClip oHouesplant;

	public NPCDialogueClip oMug;

	public NPCDialogueClip oToaster;

	public NPCDialogueClip[] oOther;
}
