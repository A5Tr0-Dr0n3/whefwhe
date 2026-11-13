using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TumbleweedOneDialogue
{
	[Header("Intro Sequence")]
	public NPCDialogueClip Yeehaw;

	public NPCDialogueClip Intro;

	public NPCDialogueClip StartOffGentle;

	public NPCDialogueClip WrapYerMitts;

	public NPCDialogueClip TooFast;

	[Header("Main Sequence")]
	public NPCDialogueClip NowWereCookin;

	public NPCDialogueClip GrizzlyTerritory;

	public NPCDialogueClip GrizzlyAttack;

	public NPCDialogueClip ThatsTheTicket;

	[Header("Final Sequence")]
	public NPCDialogueClip BadReview;

	public NPCDialogueClip GoodReview;

	[Header("Tumbling Reactions")]
	public List<NPCDialogueClip> whistles;

	public List<NPCDialogueClip> warningsFront;

	public List<NPCDialogueClip> warningsSide;

	public List<NPCDialogueClip> complaints;

	public List<NPCDialogueClip> screamClips;

	public List<NPCDialogueClip> respawnLines;
}
