using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FeedbackDialogueGroup
{
	public enum FeedbackExhaustedBehavior
	{
		restartList,
		stayOnLastLine
	}

	public List<AudioClip> feedbackDialogue;

	public FeedbackExhaustedBehavior mFeedbackExhaustedBehavior;

	[NonSerialized]
	private int dialogueIndex;

	public AudioClip FireFeedbackClip()
	{
		if (dialogueIndex >= feedbackDialogue.Count)
		{
			if (mFeedbackExhaustedBehavior == FeedbackExhaustedBehavior.restartList)
			{
				dialogueIndex = 0;
			}
			else if (mFeedbackExhaustedBehavior == FeedbackExhaustedBehavior.stayOnLastLine)
			{
				dialogueIndex = feedbackDialogue.Count - 1;
			}
		}
		AudioClip result = feedbackDialogue[dialogueIndex];
		dialogueIndex++;
		return result;
	}

	public AudioClip FireFeedbackClip(int index)
	{
		return feedbackDialogue[index];
	}
}
