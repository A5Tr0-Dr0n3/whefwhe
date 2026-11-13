using System;
using UnityEngine;

[Serializable]
public class HubDialogue
{
	[Header("Intro Sequence")]
	public NPCDialogueClip iIntro;

	public NPCDialogueClip iFishLiven;

	public NPCDialogueClip iPlantTiedTogether;

	public NPCDialogueClip iReviewIntro;

	public NPCDialogueClip iReviewReaction;

	public NPCDialogueClip iApologizing;

	public NPCDialogueClip iHRIntro;

	public NPCDialogueClip iHRCage;

	public NPCDialogueClip iGetAcquainted;

	public NPCDialogueClip iPartingWords;

	public NPCDialogueClip iBoot;

	[Header("Opening the Game to Hub")]
	public NPCDialogueClip[] loginWelcomes;

	public NPCDialogueClip mondayWelcome;

	public NPCDialogueClip tuesdayWelcome;

	public NPCDialogueClip wednesdayWelcome;

	public NPCDialogueClip thursdayWelcome;

	public NPCDialogueClip weekendWelcome;

	[Header("Commentary")]
	public NPCDialogueClip[] HRHeadset;

	public NPCDialogueClip[] chazAbuse;

	public NPCDialogueClip[] saveButton;

	public NPCDialogueClip whistle;

	public NPCDialogueClip balloons;

	public NPCDialogueClip fishbowl;

	public NPCDialogueClip plant;

	public NPCDialogueClip stapler;

	public NPCDialogueClip plushie;

	[Header("Small Talk")]
	public NPCDialogueClip backpat;

	public NPCDialogueClip brainJuice;

	public NPCDialogueClip clientQuery;

	public NPCDialogueClip color;

	public NPCDialogueClip eyes;

	public NPCDialogueClip goodhere;

	public NPCDialogueClip nap;

	public NPCDialogueClip overthink;

	public NPCDialogueClip pingPong;

	public NPCDialogueClip readyBee;

	public NPCDialogueClip realLabor;

	public NPCDialogueClip seas;

	public NPCDialogueClip standInCorner;

	public NPCDialogueClip wall;

	public NPCDialogueClip yourCall;

	[Header("Job Introductions")]
	public NPCDialogueClip macyIntro;

	public NPCDialogueClip gardenOneIntro;

	public NPCDialogueClip sunsetIntro;

	public NPCDialogueClip tumbleOneIntro;

	public NPCDialogueClip butterTwoIntroEnd;

	public NPCDialogueClip tinaIntro;

	public NPCDialogueClip gardenTwoIntro;

	public NPCDialogueClip tumbleTwoIntro;

	public NPCDialogueClip[] endlessDesireIntro;

	[Header("Job Interruptions")]
	public NPCDialogueClip macyInter;

	public NPCDialogueClip gardenOneInter;

	public NPCDialogueClip sunsetInter;

	public NPCDialogueClip tumbleOneInter;

	public NPCDialogueClip butterTwoInter;

	public NPCDialogueClip gardenTwoInter;

	public NPCDialogueClip tumbleTwoInter;

	public NPCDialogueClip tinaInter;

	public NPCDialogueClip[] genericInter;

	public NPCDialogueClip[] suspiciousInter;

	[Header("Job Completion Reactions")]
	public NPCDialogueClip gardenOne;

	public NPCDialogueClip sunset;

	public NPCDialogueClip unionIntro;

	public NPCDialogueClip unionOutro;

	public NPCDialogueClip sigh;

	public NPCDialogueClip macy;

	public NPCDialogueClip tinaGood;

	public NPCDialogueClip tinaBad;

	[Header("After Capture One")]
	public NPCDialogueClip afterBackstageCapture;

	[Header("After Capture Two")]
	public NPCDialogueClip iWarnedYou;

	public NPCDialogueClip seeThis;

	public NPCDialogueClip aestheticLuxury;

	public NPCDialogueClip youLikeThis;

	public NPCDialogueClip shame;

	public NPCDialogueClip cleanUpThatMess;

	[Header("After Capture Three")]
	public NPCDialogueClip signOfPeace;

	public NPCDialogueClip lifeWithoutBeauty;

	public NPCDialogueClip youreAGem;

	public NPCDialogueClip youreGoingToLoveIt;

	public NPCDialogueClip danceWithMe;

	public NPCDialogueClip fireworks;

	public NPCDialogueClip tooLoud;

	public NPCDialogueClip coffeeCup;

	public NPCDialogueClip lunchBox;

	public NPCDialogueClip croquetSet;

	public NPCDialogueClip surprise;

	public NPCDialogueClip beachballAndMore;

	public NPCDialogueClip sameSide;

	public NPCDialogueClip proudOfYou;

	[Header("After Hernandez Capture")]
	public NPCDialogueClip irrelevant;

	[Header("After tina kicks you out")]
	public NPCDialogueClip timeToGetBigger;

	public NPCDialogueClip nowYoureBigger;

	[Header("Flipper Audio")]
	public NPCDialogueClip snackZone;

	public NPCDialogueClip snackZoneSoda;

	public NPCDialogueClip snackZoneFridge;

	public NPCDialogueClip snackZoneToast;

	public NPCDialogueClip relaxationZone;

	public NPCDialogueClip relaxationZoneMeditation;

	public NPCDialogueClip relaxationZoneFollowup;

	public NPCDialogueClip vrZone;

	public NPCDialogueClip vrZoneStereoscope;

	public NPCDialogueClip vrZoneLinkTrainer;

	public NPCDialogueClip vrZoneSensorama;

	public NPCDialogueClip flipperSetup;

	[Header("Long Relaxation Monologue")]
	public NPCDialogueClip[] relaxationMonologues;

	[Header("Flip Tutorial")]
	public NPCDialogueClip flipTutorial;

	public NPCDialogueClip playerFlipped;
}
