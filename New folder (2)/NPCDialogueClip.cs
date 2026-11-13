using System;
using UnityEngine;

[Serializable]
public class NPCDialogueClip
{
	public AudioClip clip;

	public Interruptability mInterruptability = Interruptability.limitedInterruptions;

	public Interruptability paddingInterruptability = Interruptability.unlimitedInterruptions;

	public float prePadding;

	public float postPadding;

	public bool hasCustomInterrupts;

	public uint numberOfSegments;

	public DialogueSegment[] customInterruptTimes;

	public float duration => clip.length + prePadding + postPadding;

	public SubtitleContainer GetSubtitles()
	{
		if (clip != null)
		{
			if (SubtitleLoader.instance.GetDictionary().ContainsKey(clip.name))
			{
				return SubtitleLoader.instance.GetSubtitleContainer(clip.name);
			}
			return default;
		}
		return default;
	}
}
