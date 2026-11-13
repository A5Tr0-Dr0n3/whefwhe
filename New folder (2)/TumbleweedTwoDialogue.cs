using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TumbleweedTwoDialogue
{
	[Header("Tumbleweed")]
	public NPCDialogueClip intro;

	public NPCDialogueClip grabLeafBlower;

	public NPCDialogueClip tooFast;

	public NPCDialogueClip introductions;

	public NPCDialogueClip letsGetTumblin;

	public NPCDialogueClip vacuumReaction;

	public List<NPCDialogueClip> whistles;

	public List<NPCDialogueClip> mrWeedRespawn;

	[Header("Lil' Pete")]
	public NPCDialogueClip howdy;

	public NPCDialogueClip cantThisThing;

	public NPCDialogueClip speedErUp;

	public NPCDialogueClip faster;

	public NPCDialogueClip groan;

	public List<NPCDialogueClip> lilPeetRespawn;
}
