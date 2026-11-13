using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class VVRDialogueSource : MonoBehaviour
{
	private enum ClipInsertBehavior
	{
		insertAtEnd,
		insertAfterCurrent,
		interruptCurrent
	}

	[SerializeField]
	public bool recordScratchOnInterrupt;

	[SerializeField]
	public NPCDialogueClip recordScratchClip;

	[SerializeField]
	public GvrAudioSource musicToPause;

	[SerializeField]
	public SubtitleRenderer mSubtitleRenderer;

	private List<DialoguePlaybackInstance> clipQueue;

	private int framesSinceNotPlaying;

	private bool interruptingOnDelay;

	private bool playFaster;

	private float playbackSpeed = 1f;

	public GvrAudioSource mAudioSource { get; private set; }

	public bool isPlaying => framesSinceNotPlaying < 2;

	private void Awake()
	{
		mAudioSource = GetComponent<GvrAudioSource>();
		clipQueue = new List<DialoguePlaybackInstance>();
	}

	public DialoguePlaybackInstance PlayClip(AudioClip rawAudioClip)
	{
		NPCDialogueClip nPCDialogueClip = new NPCDialogueClip();
		nPCDialogueClip.clip = rawAudioClip;
		return PlayClip(nPCDialogueClip);
	}

	public DialoguePlaybackInstance PlayClip(NPCDialogueClip dialogueClip)
	{
		framesSinceNotPlaying = 0;
		return enqueueClip(dialogueClip, ClipInsertBehavior.insertAtEnd);
	}

	public DialoguePlaybackInstance ForcePlayClipInterrupt(NPCDialogueClip dialogueClip)
	{
		framesSinceNotPlaying = 0;
		return enqueueClip(dialogueClip, ClipInsertBehavior.interruptCurrent);
	}

	public DialoguePlaybackInstance ForcePlayClipOverwrite(NPCDialogueClip dialogueClip)
	{
		framesSinceNotPlaying = 0;
		clipQueue.Clear();
		return enqueueClip(dialogueClip, ClipInsertBehavior.insertAtEnd);
	}

	public void PurgePlaybackClipFromQueue(DialoguePlaybackInstance clipToPurge)
	{
		if (clipQueue.Contains(clipToPurge))
		{
			clipToPurge.Pause();
			clipQueue.Remove(clipToPurge);
		}
	}

	public void PurgeAllDialogue()
	{
		while (clipQueue.Count > 0)
		{
			clipQueue.RemoveAt(0);
		}
	}

	public bool isPlayingInterruption()
	{
		if (clipQueue.Count > 0)
		{
			for (int i = 0; i < clipQueue.Count; i++)
			{
				if (clipQueue[i].isInterruption)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public DialoguePlaybackInstance InterruptWithClip(AudioClip rawAudioClip, bool enqueueIfCantInterrupt = false, bool stackInterruptions = false)
	{
		NPCDialogueClip nPCDialogueClip = new NPCDialogueClip();
		nPCDialogueClip.clip = rawAudioClip;
		return InterruptWithClip(nPCDialogueClip, enqueueIfCantInterrupt);
	}

	public DialoguePlaybackInstance InterruptWithClip(NPCDialogueClip dialogueClip, bool enqueueIfCantInterrupt = false, bool stackInterruptions = false, bool forceNoRecordScratch = false, bool ignoreCustomInterrupts = false)
	{
		if (clipQueue.Count > 0)
		{
			if (clipQueue[0].TryInterruptClip(this, dialogueClip, ignoreCustomInterrupts))
			{
				if (recordScratchOnInterrupt && !forceNoRecordScratch)
				{
					musicToPause.Pause();
					enqueueClip(dialogueClip, ClipInsertBehavior.interruptCurrent, isInterruption: true);
					GlobalCoroutineContainer.AddThread(UnpauseMusic(dialogueClip.clip.length + recordScratchClip.clip.length + 1.5f), this);
					return enqueueClip(recordScratchClip, ClipInsertBehavior.interruptCurrent, isInterruption: true);
				}
				return enqueueClip(dialogueClip, ClipInsertBehavior.interruptCurrent, isInterruption: true);
			}
			if (enqueueIfCantInterrupt)
			{
				if (stackInterruptions || !isPlayingInterruption())
				{
					return enqueueClip(dialogueClip, ClipInsertBehavior.insertAfterCurrent, isInterruption: true);
				}
				return null;
			}
			return null;
		}
		return enqueueClip(dialogueClip, ClipInsertBehavior.insertAtEnd);
	}

	public void InterruptOnDelay(float seconds, NPCDialogueClip dialogueClip)
	{
		if (!interruptingOnDelay)
		{
			GlobalCoroutineContainer.AddThread(InterruptCoroutine(seconds, dialogueClip), this);
			interruptingOnDelay = true;
		}
	}

	public IEnumerator InterruptCoroutine(float seconds, NPCDialogueClip dialogueClip)
	{
		interruptingOnDelay = true;
		yield return new WaitForSeconds(seconds);
		interruptingOnDelay = false;
		InterruptWithClip(dialogueClip);
	}

	public IEnumerator UnpauseMusic(float s)
	{
		yield return new WaitForSeconds(s);
		if (recordScratchOnInterrupt)
		{
			musicToPause.UnPause();
		}
	}

	public void Pause()
	{
		mAudioSource.Pause();
		mAudioSource.pitch = 0f;
	}

	public void UnPause()
	{
		mAudioSource.UnPause();
		mAudioSource.pitch = 1f;
	}

	public void PitchDownPause(float s)
	{
		GlobalCoroutineContainer.AddThread(PitchDownPauseCoroutine(s), this);
	}

	private IEnumerator PitchDownPauseCoroutine(float s)
	{
		SafeDebug.Log("pause pitch down over " + s + " seconds");
		float t = 0f;
		mAudioSource.pitch = 1f;
		while (t < s)
		{
			t += Time.deltaTime;
			mAudioSource.pitch -= Time.deltaTime / s;
			yield return null;
		}
		mAudioSource.pitch = 0f;
		mAudioSource.Pause();
	}

	public void PitchUpUnpause(float s)
	{
		GlobalCoroutineContainer.AddThread(PitchUpUnpauseCoroutine(s), this);
	}

	private IEnumerator PitchUpUnpauseCoroutine(float s)
	{
		SafeDebug.Log("unpause pitch up over " + s + " seconds");
		float t = 0f;
		mAudioSource.pitch = 0f;
		while (t < s)
		{
			t += Time.deltaTime;
			mAudioSource.pitch += Time.deltaTime / s;
			yield return null;
		}
		mAudioSource.pitch = 1f;
		mAudioSource.UnPause();
	}

	private DialoguePlaybackInstance enqueueClip(NPCDialogueClip clip, ClipInsertBehavior insertBehavior, bool isInterruption = false)
	{
		if (clipQueue == null)
		{
			SafeDebug.LogError("You're trying to play audio before its set up, breaking");
			return null;
		}
		DialoguePlaybackInstance dialoguePlaybackInstance = new DialoguePlaybackInstance(clip, mAudioSource, isInterruption);
		switch (insertBehavior)
		{
		case ClipInsertBehavior.insertAtEnd:
			clipQueue.Add(dialoguePlaybackInstance);
			break;
		case ClipInsertBehavior.insertAfterCurrent:
			if (clipQueue.Count > 0)
			{
				clipQueue.Insert(1, dialoguePlaybackInstance);
			}
			else
			{
				clipQueue.Add(dialoguePlaybackInstance);
			}
			break;
		case ClipInsertBehavior.interruptCurrent:
			if (clipQueue.Count > 0)
			{
				clipQueue.Insert(0, dialoguePlaybackInstance);
			}
			else
			{
				clipQueue.Add(dialoguePlaybackInstance);
			}
			break;
		}
		return dialoguePlaybackInstance;
	}

	private bool shouldPlay()
	{
		return Time.timeScale > 0f;
	}

	private void Update()
	{
		if (shouldPlay())
		{
			if (clipQueue.Count > 0 && mAudioSource.pitch >= 0.001f)
			{
				if (mSubtitleRenderer != null && !mSubtitleRenderer.IsPlaying())
				{
					mSubtitleRenderer.SetIsPlaying(s: true);
				}
				framesSinceNotPlaying = 0;
				clipQueue[0].PlayUpdate(Time.deltaTime * playbackSpeed);
				if (clipQueue[0].playbackProgress == DialoguePlaybackProgress.finished)
				{
					clipQueue.RemoveAt(0);
					if (clipQueue.Count > 0 && clipQueue[0].mClipData.prePadding == 0f)
					{
						mAudioSource.clip = clipQueue[0].mClipData.clip;
						mAudioSource.Play();
					}
				}
			}
			else
			{
				if (mSubtitleRenderer != null && mSubtitleRenderer.IsPlaying())
				{
					mSubtitleRenderer.SetIsPlaying(s: false);
				}
				framesSinceNotPlaying++;
			}
		}
		else
		{
			if (clipQueue.Count <= 0)
			{
				return;
			}
			framesSinceNotPlaying++;
			if (clipQueue[0].playbackProgress == DialoguePlaybackProgress.playingSample)
			{
				clipQueue[0].Pause();
				if (mSubtitleRenderer != null && mSubtitleRenderer.IsPlaying())
				{
					mSubtitleRenderer.SetIsPlaying(s: false);
				}
			}
		}
	}

	private void OnDisable()
	{
		if (clipQueue != null && clipQueue.Count > 0 && clipQueue[0].playbackProgress == DialoguePlaybackProgress.playingSample)
		{
			clipQueue[0].Pause();
			if (mSubtitleRenderer != null && mSubtitleRenderer.IsPlaying())
			{
				mSubtitleRenderer.SetIsPlaying(s: false);
			}
		}
	}

	public DialogueSegment GetCurrentDialogueSegment()
	{
		if (clipQueue.Count <= 0)
		{
			return null;
		}
		if (clipQueue[0] != null)
		{
			return clipQueue[0].GetCurrentDialogueSegment();
		}
		return null;
	}

	public List<DialoguePlaybackInstance> GetClipQueue()
	{
		if (clipQueue != null)
		{
			return clipQueue;
		}
		return null;
	}
}
