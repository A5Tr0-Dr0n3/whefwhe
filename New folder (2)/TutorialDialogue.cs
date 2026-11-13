using System;
using UnityEngine;

[Serializable]
public class TutorialDialogue
{
	[Header("Intro Sequence")]
	public NPCDialogueClip iSequenceIntro;

	public NPCDialogueClip iHereAtActivitude;

	public NPCDialogueClip iEmployeeNumber;

	public NPCDialogueClip iLetsCallYouBee;

	public NPCDialogueClip iLetsStartTraining;

	[Header("Grab Tutorial Sequence")]
	public NPCDialogueClip gSequenceIntro;

	public NPCDialogueClip gChazFlattered;

	public NPCDialogueClip gChazLetGo;

	public NPCDialogueClip gShoeOrFish;

	public NPCDialogueClip gPlant;

	public NPCDialogueClip gPutOnCoffeeTable;

	public NPCDialogueClip gPickUpWateringCan;

	public NPCDialogueClip gPickUpFishFood;

	public NPCDialogueClip gRespectTrainingMaterials;

	public NPCDialogueClip gCareful1;

	public NPCDialogueClip gCareful2;

	public NPCDialogueClip gFishShoeChastise;

	public NPCDialogueClip gPlantChastise;

	public NPCDialogueClip gFishShoeNourish;

	public NPCDialogueClip gPlantNourish;

	public NPCDialogueClip gTakingTooLongToWater;

	public NPCDialogueClip gTakingTooLongToFeed;

	public NPCDialogueClip gLeashLength;

	public NPCDialogueClip gFishFoodUpsideDown;

	public NPCDialogueClip gNowYouveGotIt;

	public NPCDialogueClip gThereYouGo;

	public NPCDialogueClip gFastCompletion;

	public NPCDialogueClip gSlowCompletion;

	[Header("Movement Tutorial Sequence")]
	public NPCDialogueClip mPracticeTeleporting;

	public NPCDialogueClip mFavoritePartMonologue;

	public NPCDialogueClip mTheKeyMonologue;

	public NPCDialogueClip mDoYouFeelIt;

	public NPCDialogueClip mThereItIs;

	public NPCDialogueClip mKeepHolding;

	public NPCDialogueClip mButtonPressed;

	public NPCDialogueClip mFiguredItOut;

	public NPCDialogueClip mSoProud;

	public NPCDialogueClip mFinalInstructions;

	public NPCDialogueClip mFearOfLava;

	public NPCDialogueClip mStandingInLava;

	public NPCDialogueClip mRoughInTheMiddle;

	[Header("ALT Movement Tutorial Sequence")]
	public NPCDialogueClip mSequenceIntro;

	public NPCDialogueClip mOpeningLavaPit;

	public NPCDialogueClip mLavaCountdown;

	public NPCDialogueClip mLavaTimerExpired;

	public NPCDialogueClip mTeleportedIntoLava;

	public NPCDialogueClip mTeleportedSucessCheer;

	[Header("Headset Tutorial Sequence")]
	public NPCDialogueClip hSequenceIntro;

	public NPCDialogueClip hFurtherInstruction;

	public NPCDialogueClip hPleaseSelectAnAccessPoint;

	public NPCDialogueClip hFailToSelectAccessPoint;

	public NPCDialogueClip hItsInMyMouth;

	public NPCDialogueClip hWelcomeBack1;

	public NPCDialogueClip hWelcomeBack2;

	public NPCDialogueClip[] hAnother;

	public NPCDialogueClip hReactToBackstageSlipUp;

	public NPCDialogueClip hSkipAheadToGraduation;

	[Header("Graduation")]
	public NPCDialogueClip gCongratulationsFirstClient;

	public NPCDialogueClip gFirstTask1;

	public NPCDialogueClip gFirstTask2;

	public NPCDialogueClip gFirstTask3;

	public NPCDialogueClip gHurryUp;

	[Header("User Exits Butter")]
	public NPCDialogueClip eUserExitedButter1;

	public NPCDialogueClip eUserExitedButter2;
}
