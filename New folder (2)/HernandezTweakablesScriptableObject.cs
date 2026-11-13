using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hernandez Sequence Content", menuName = "TenderClaws/Create Hernandez Sequence Content", order = 1)]
public class HernandezTweakablesScriptableObject : ScriptableObject
{
	[Serializable]
	public class HernandezDialogue
	{
		[Header("Intro:")]
		public NPCDialogueClip preEntryRambling;

		public NPCDialogueClip chewOutC4;

		public NPCDialogueClip greetPlayer;

		[Header("Memo:")]
		public NPCDialogueClip memoGrabThisStylus;

		public HurryUpDialogueUnit memoStylusHurryUp;

		public NPCDialogueClip memoBeginning;

		public HurryUpDialogueUnit memoStartWritingHurryUp;

		public NPCDialogueClip memoFull;

		public FeedbackDialogueGroup memoHurryUps;

		[Header("Lose leaf")]
		public NPCDialogueClip leafFallMemoEnd;

		public NPCDialogueClip leafFallMemoChastize;

		public NPCDialogueClip leafFallReactToLeafFalling;

		public HurryUpDialogueUnit leafFallReattachHurryUps;

		public NPCDialogueClip leafFallExplanation;

		public NPCDialogueClip leafFallReattachReaction;

		[Header("Gym:")]
		public NPCDialogueClip gymTransition;

		public NPCDialogueClip gymJumpingJacks;

		public FeedbackDialogueGroup jumpingJackDips;

		public HurryUpDialogueUnit gymStartBuildingHurryUp;

		public NPCDialogueClip gymBuildingMonologue;

		public NPCDialogueClip gymUserHasntBuiltWeights;

		public NPCDialogueClip gymHernandezLifting;

		public HurryUpDialogueUnit gymGrabWeightHurryup;

		public NPCDialogueClip gymNarratePlayerLifting;

		public HurryUpDialogueUnit gymKeepLiftingHurryup;

		public NPCDialogueClip gymForcePlayerDrop;

		[Header("Restoration")]
		public NPCDialogueClip restorationIntro;

		public HurryUpDialogueUnit restorationStartBuildingHurryUp;

		public BuildPercentageDialogue[] restorationMonologue;

		public FeedbackDialogueGroup restorationKeepBuildingFeedback;

		[Header("Death")]
		public NPCDialogueClip deathWhosTheBoss;

		public NPCDialogueClip deathPluckMeIntro;

		public NPCDialogueClip[] deathPleasePluckMe;

		public HurryUpDialogueUnit deathPluckMeHurryUp;

		public NPCDialogueClip deathPostPluckEnthusiastic;

		public NPCDialogueClip deathPluckExclamation;

		public NPCDialogueClip[] deathPluckMonologueScared;

		public NPCDialogueClip deathFinalBegging;

		public NPCDialogueClip deathScream;

		[Header("Player Returns")]
		public FeedbackDialogueGroup playerReturnedFeedback;
	}

	[Serializable]
	public class HernandezHelperDialogue
	{
		public NPCDialogueClip mobileHelperWarning;

		public NPCDialogueClip hernandezIsDeadGreeting;

		public List<NPCDialogueClip> pluckHimChantClips;

		public NPCDialogueClip hernandezDeathCelebration;
	}

	[Serializable]
	public class BuildPercentageDialogue
	{
		public float buildPercentage;

		public NPCDialogueClip buildReaction;

		public NPCDialogueClip leafFallReaction;
	}

	[Serializable]
	public class GiantChazDialogue
	{
		public AudioClip giantChazFootsteps;

		public NPCDialogueClip chazChastize;
	}

	public HernandezDialogue hernandezDialogue;

	public HernandezHelperDialogue helperDialogue;

	public GiantChazDialogue chazDialogue;
}
