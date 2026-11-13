using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueScriptableObject : ScriptableObject
{
	public NPCDialogueClip dialogueClip;

	public static DialogueScriptableObject CreateDialogueScriptableObject(List<float> endTimes, float endTime, AudioClip newClip)
	{
		DialogueScriptableObject dialogueScriptableObject = new DialogueScriptableObject();
		dialogueScriptableObject.dialogueClip = new NPCDialogueClip();
		List<DialogueSegment> list = new List<DialogueSegment>();
		for (int i = 0; i < endTimes.Count; i++)
		{
			DialogueSegment dialogueSegment = new DialogueSegment();
			if (i == 0)
			{
				dialogueSegment.startTime = 0f;
				dialogueSegment.endTime = endTimes[i];
			}
			else if (i < endTimes.Count - 1)
			{
				dialogueSegment.startTime = endTimes[i - 1];
				dialogueSegment.endTime = endTimes[i];
			}
			else if (i == endTimes.Count - 1)
			{
				dialogueSegment.startTime = endTimes[i];
				dialogueSegment.endTime = endTime;
			}
			list.Add(dialogueSegment);
		}
		dialogueScriptableObject.dialogueClip.customInterruptTimes = list.ToArray();
		dialogueScriptableObject.dialogueClip.clip = newClip;
		dialogueScriptableObject.dialogueClip.hasCustomInterrupts = true;
		dialogueScriptableObject.dialogueClip.mInterruptability = Interruptability.uninterruptable;
		return null;
	}
}
