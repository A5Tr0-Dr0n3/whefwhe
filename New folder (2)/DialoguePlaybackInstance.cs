using System;
using UnityEngine;

[Serializable]
public class DialoguePlaybackInstance
{
	public NPCDialogueClip mClipData;

	public GvrAudioSource mSource;

	private DialogueSegment currentDialogueSegment;

	private const float maxResetLineDialogueCompletion = 0.3f;

	private const float maxInterruptionDialogueCompletion = 0.8f;

	public float progressTime;

	public DialoguePlaybackProgress playbackProgress;

	public int interruptionCount;

	public bool isInterruption;

	public bool interruptingOnDelay;

	public float timeToInterrupt;

	public DialoguePlaybackInstance(NPCDialogueClip clipIn, GvrAudioSource sourceIn, bool isInterruptionIn)
	{
		mClipData = clipIn;
		progressTime = 0f;
		playbackProgress = DialoguePlaybackProgress.prePadding;
		mSource = sourceIn;
		isInterruption = isInterruptionIn;
	}

	public bool TryInterruptClip(VVRDialogueSource owner, NPCDialogueClip clip, bool ignoreCustomInterrupts = false)
	{
		switch ((playbackProgress != DialoguePlaybackProgress.playingSample) ? mClipData.paddingInterruptability : mClipData.mInterruptability)
		{
		case Interruptability.uninterruptable:
			return false;
		case Interruptability.limitedInterruptions:
			if (interruptionCount < 1 && GetNormalizedPlaybackT() < 0.8f)
			{
				return interrupt(owner, clip, ignoreCustomInterrupts);
			}
			return false;
		case Interruptability.unlimitedInterruptions:
			if (GetNormalizedPlaybackT() < 0.8f || mClipData.hasCustomInterrupts)
			{
				return interrupt(owner, clip, ignoreCustomInterrupts);
			}
			return false;
		default:
			return false;
		}
	}

	private bool interrupt(VVRDialogueSource owner, NPCDialogueClip clip, bool ignoreCustomInterrupts = false)
	{
		interruptionCount++;
		if (!mClipData.hasCustomInterrupts || mClipData.customInterruptTimes.Length < 1 || ignoreCustomInterrupts)
		{
			if (playbackProgress == DialoguePlaybackProgress.playingSample && GetNormalizedPlaybackT() < 0.3f && interruptionCount < 1)
			{
				playbackProgress = DialoguePlaybackProgress.prePadding;
				progressTime = 0f;
				return true;
			}
			playbackProgress = DialoguePlaybackProgress.paused;
			return true;
		}
		if (mClipData.customInterruptTimes.Length > 0)
		{
			DialogueSegment dialogueSegment = findNearestCustomInterrupt();
			if (dialogueSegment.endTime == 0f)
			{
				dialogueSegment.endTime = mClipData.clip.length;
			}
			if (dialogueSegment.mInterruptability == Interruptability.uninterruptable)
			{
				return false;
			}
			if (progressTime < dialogueSegment.getResetBound())
			{
				progressTime = dialogueSegment.startTime;
				playbackProgress = DialoguePlaybackProgress.paused;
				return true;
			}
			if (progressTime > dialogueSegment.getResetBound() && progressTime < dialogueSegment.getContinueBound())
			{
				playbackProgress = DialoguePlaybackProgress.paused;
				return true;
			}
			if (progressTime > dialogueSegment.getContinueBound() && progressTime < dialogueSegment.endTime)
			{
				interruptingOnDelay = true;
				timeToInterrupt = dialogueSegment.endTime - progressTime;
				owner.InterruptOnDelay(dialogueSegment.endTime - progressTime, clip);
				return false;
			}
		}
		return false;
	}

	public void Pause()
	{
		playbackProgress = DialoguePlaybackProgress.paused;
		mSource.Pause();
	}

	public void Resume()
	{
		playbackProgress = DialoguePlaybackProgress.playingSample;
		mSource.UnPause();
	}

	public void PlayUpdate(float deltaTime)
	{
		progressTime += deltaTime;
		if (mClipData.hasCustomInterrupts)
		{
			findCurrentDialogueClip(progressTime);
		}
		if (playbackProgress == DialoguePlaybackProgress.prePadding)
		{
			silenceSource();
			if (progressTime > mClipData.prePadding)
			{
				loadSourceClip(mClipData.clip);
				playbackProgress = DialoguePlaybackProgress.playingSample;
				progressTime = 0f;
			}
		}
		else if (playbackProgress == DialoguePlaybackProgress.paused)
		{
			loadSourceClip(mClipData.clip, progressTime);
			playbackProgress = DialoguePlaybackProgress.playingSample;
		}
		else if (playbackProgress == DialoguePlaybackProgress.playingSample)
		{
			progressTime = mSource.time;
			if (!mSource.isPlaying)
			{
				playbackProgress = DialoguePlaybackProgress.postPadding;
				progressTime = 0f;
			}
		}
		else if (playbackProgress == DialoguePlaybackProgress.postPadding)
		{
			silenceSource();
			if (progressTime > mClipData.postPadding)
			{
				playbackProgress = DialoguePlaybackProgress.finished;
			}
		}
	}

	private void silenceSource()
	{
		if (mSource.isPlaying)
		{
			mSource.Stop();
			mSource.clip = null;
		}
	}

	private void loadSourceClip(AudioClip clipIn)
	{
		mSource.time = 0f;
		progressTime = 0f;
		mSource.clip = clipIn;
		mSource.timeSamples = 0;
		mSource.Play();
		mSource.timeSamples = 0;
	}

	private void loadSourceClip(AudioClip clipIn, float timeIn)
	{
		mSource.clip = clipIn;
		mSource.time = timeIn;
		mSource.Play();
	}

	private float GetNormalizedPlaybackT()
	{
		if (playbackProgress == DialoguePlaybackProgress.playingSample)
		{
			return progressTime / mClipData.clip.length;
		}
		return 0f;
	}

	private DialogueSegment findNearestCustomInterrupt()
	{
		DialogueSegment dialogueSegment = mClipData.customInterruptTimes[0];
		if (dialogueSegment.startTime > progressTime)
		{
			playbackProgress = DialoguePlaybackProgress.prePadding;
			return mClipData.customInterruptTimes[0];
		}
		playbackProgress = DialoguePlaybackProgress.paused;
		for (int i = 0; i < mClipData.customInterruptTimes.Length; i++)
		{
			DialogueSegment dialogueSegment2 = mClipData.customInterruptTimes[i];
			if (progressTime <= dialogueSegment2.startTime && progressTime >= dialogueSegment.startTime)
			{
				return dialogueSegment;
			}
			if (currentDialogueSegment.startTime >= dialogueSegment2.startTime && progressTime <= currentDialogueSegment.endTime)
			{
				return currentDialogueSegment;
			}
			dialogueSegment = mClipData.customInterruptTimes[i];
		}
		SafeDebug.LogWarning("Couldn't find a breakpoint");
		return mClipData.customInterruptTimes[0];
	}

	private void findCurrentDialogueClip(float time)
	{
		if (mClipData.customInterruptTimes.Length > 0)
		{
			for (int i = 0; i < mClipData.customInterruptTimes.Length; i++)
			{
				if (progressTime >= mClipData.customInterruptTimes[i].startTime && progressTime < mClipData.customInterruptTimes[i].endTime)
				{
					currentDialogueSegment = mClipData.customInterruptTimes[i];
				}
			}
			if (currentDialogueSegment == null)
			{
				currentDialogueSegment = mClipData.customInterruptTimes[0];
			}
		}
		else
		{
			currentDialogueSegment = null;
		}
	}

	public DialogueSegment GetCurrentDialogueSegment()
	{
		return currentDialogueSegment;
	}
}
