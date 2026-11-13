using System;
using UnityEngine;

[Serializable]
public class ElevatorDialogue
{
	[Header("Elevator Dialogue:")]
	public NPCDialogueClip greeting_FirstTimeEntry;

	public NPCDialogueClip greeting_YoullFindArchivesEncouragement;

	public NPCDialogueClip greeting_HasBeenToArchives;

	public NPCDialogueClip greeting_ReactToEmergencyPhoneJail;

	public NPCDialogueClip hrButtonNoAccess;

	public NPCDialogueClip prButtonSequence;

	public NPCDialogueClip cafeteriaButtonSequence;

	public NPCDialogueClip cloudButtonSequence;

	public NPCDialogueClip boidButtonSequence;

	public NPCDialogueClip chazGameRoomSequence;

	public NPCDialogueClip favoriteRoomLeadup;

	public NPCDialogueClip favoriteRoomReaction;

	public NPCDialogueClip[] archiveIntro_AlreadyTriedHRButton;

	public NPCDialogueClip archiveIntro_HasntTriedHRButton;

	public NPCDialogueClip archiveIntro_AfterHRButtonPress;

	public NPCDialogueClip reactToEmergencyPhone;

	[Header("Chaz Dialogue:")]
	public NPCDialogueClip chazEmergencyPhone;
}
