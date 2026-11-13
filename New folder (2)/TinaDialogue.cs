using System;
using UnityEngine;

[Serializable]
public class TinaDialogue
{
	[Header("Intro")]
	public DialogueScriptableObject fullIntro;

	public NPCDialogueClip pickADoll;

	public NPCDialogueClip youBeJaime;

	public NPCDialogueClip notCindy;

	[Header("Tina Arc")]
	public DialogueScriptableObject iWillPlay;

	public DialogueScriptableObject wereHome;

	public DialogueScriptableObject masterBedroom;

	public DialogueScriptableObject[] livingRoom;

	public DialogueScriptableObject[] childrensRoom;

	public DialogueScriptableObject[] attic;

	public DialogueScriptableObject[] diningRoom;

	public DialogueScriptableObject[] laundryRoom;

	public DialogueScriptableObject sequenceEnd;

	[Header("Dynamic Character Audio")]
	public NPCDialogueClip[] grabTinaDollReactions;

	public NPCDialogueClip woff;

	public NPCDialogueClip alrightNext;

	public NPCDialogueClip[] thatsJericho;

	public NPCDialogueClip thatsDonald;

	public NPCDialogueClip dad;

	public NPCDialogueClip[] grabbedMom;

	public NPCDialogueClip hiMom;

	public NPCDialogueClip donaldGetBack;

	public NPCDialogueClip jerichoThisIsMyfriend;

	public NPCDialogueClip goodEveningDad;

	[Header("Dynamic Furniture Responses")]
	public NPCDialogueClip toliet;

	public NPCDialogueClip tub;

	public NPCDialogueClip bed;

	public NPCDialogueClip plant;

	public NPCDialogueClip foyerTable;

	public NPCDialogueClip toaster;

	public NPCDialogueClip fridge;

	public NPCDialogueClip lamp;

	public NPCDialogueClip[] genericFurnitureResponses;

	[Header("Player Doll Reactions")]
	public NPCDialogueClip playerInBathroom;

	public NPCDialogueClip playerInAttic;

	public NPCDialogueClip playerInDiningroom;

	public NPCDialogueClip[] playerInKitchen;

	public NPCDialogueClip playerInoffice;

	public NPCDialogueClip playerInFoyer;

	[Header("Misc")]
	public NPCDialogueClip[] hitTina;

	public NPCDialogueClip teleportFarAway;

	public NPCDialogueClip grabbedTinasRemote;

	public NPCDialogueClip toyBoxInterruption;

	public NPCDialogueClip spinHouse;

	public NPCDialogueClip[] vacuumTina;

	public NPCDialogueClip[] returnFromInterruption;
}
