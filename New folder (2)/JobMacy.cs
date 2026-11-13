using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using GVR.Gfx;
using UnityEngine;

public class JobMacy : SceneBehaviorJob
{
	[Header("References")]
	[SerializeField]
	private MacyJobObject tweakables;

	[SerializeField]
	private MultiPointDialogue bigMacyDialogueSource;

	[SerializeField]
	private MultiPointDialogue smallMacyDialogueSource;

	[SerializeField]
	private Transform bigPlayerPosition;

	[SerializeField]
	private GameObject OneTwentyRoot;

	[SerializeField]
	private EnvironmentProperties OneTwentyEnvironent;

	[SerializeField]
	private GameObject OneOneRoot;

	[SerializeField]
	private EnvironmentProperties OneOneEnvironent;

	[SerializeField]
	private List<MacyFloat> balloons;

	[SerializeField]
	private GameObject balloonObj;

	[SerializeField]
	private AudioLoop _music;

	[SerializeField]
	private JobReviewScriptableObject _goodReview;

	[SerializeField]
	private JobReviewScriptableObject _badReview;

	[SerializeField]
	private TargetFollow _farClipSphere;

	[Header("Values")]
	[SerializeField]
	private float _oneTwentyFarClipDistance = 16f;

	public MacyState currentState;

	private SafeCoroutine mainThread;

	private int balloonPullCount;

	public static JobMacy instance;

	private EagerScene macyEagerScene;

	private List<MacyFloat> freeBalloons;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		if (VVR_HUB.instance != null)
		{
			currentState = MacyState.Intro;
		}
		mainThread = GlobalCoroutineContainer.AddThread(MacyCoroutine(), this);
		if (instance == null)
		{
			instance = this;
		}
	}

	protected override void OnSceneActivated()
	{
		if (currentState == MacyState.ReEntry)
		{
			EagerHeadsetManager.instance.SetForceFatalError(s: false);
			PlayerScaler.instance.ScaleHead(Vector3.one);
		}
	}

	private IEnumerator MacyCoroutine()
	{
		ActivateWorld(oneTwenty: false);
		balloonObj.SetActive(value: false);
		if (currentState == MacyState.Intro)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.introDelay, this);
			bigMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.welcomeHuman);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(18.5f, this);
			currentState = MacyState.KickedOut;
		}
		int goAwayIter = 0;
		float t = tweakables.pleaseLeaveDelays - 1.5f;
		if (macyEagerScene == null)
		{
			macyEagerScene = GetComponent<EagerScene>();
		}
		macyEagerScene.farClipDistance = _oneTwentyFarClipDistance;
		while (currentState == MacyState.KickedOut)
		{
			t += Time.deltaTime;
			if (t > tweakables.pleaseLeaveDelays)
			{
				if (goAwayIter >= tweakables.dialogue.goAwayList.Length)
				{
					bigMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.goAwayRepeat);
				}
				else
				{
					bigMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.goAwayList[goAwayIter]);
				}
				goAwayIter++;
				t = 0f;
			}
			while (bigMacyDialogueSource.isPlaying && currentState != MacyState.ReEntry)
			{
				yield return null;
			}
			yield return null;
		}
		ActivateWorld(oneTwenty: true);
		_music.Play();
		if (currentState == MacyState.ReEntry)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.reentryDelay, this);
			for (int i = 0; i < tweakables.dialogue.monologueA.Length; i++)
			{
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.monologueA[i]);
				while (smallMacyDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			currentState = MacyState.ParadeBegin;
		}
		bool firstBalloonPulled = false;
		bool playLineUntilEnd = false;
		balloonObj.SetActive(value: true);
		if (currentState == MacyState.ParadeBegin)
		{
			smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.iRemember);
			while (smallMacyDialogueSource.isPlaying)
			{
				yield return null;
			}
			for (int j = 0; j < tweakables.dialogue.monologueB.Length; j++)
			{
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.monologueB[j]);
				while (smallMacyDialogueSource.isPlaying)
				{
					if (freeBalloons != null && freeBalloons.Count > 0)
					{
						firstBalloonPulled = true;
					}
					if (firstBalloonPulled && !playLineUntilEnd)
					{
						if (!(tweakables.dialogue.monologueB[j].clip.length - smallMacyDialogueSource.time < tweakables.monologueClipCutoff))
						{
							break;
						}
						playLineUntilEnd = true;
					}
					yield return null;
				}
				if (firstBalloonPulled)
				{
					break;
				}
			}
			if (firstBalloonPulled)
			{
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.monoBInterruption);
			}
			else
			{
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.monoBContinuation);
				while (smallMacyDialogueSource.isPlaying)
				{
					yield return null;
				}
				if (freeBalloons == null)
				{
					int index = 0;
					float num = float.PositiveInfinity;
					for (int k = 0; k < balloons.Count; k++)
					{
						float num2 = Vector3.SqrMagnitude(EagerMaster.instance.mainCamera.transform.position - balloons[k].balloonRb.transform.position);
						if (num > num2)
						{
							index = k;
							num = num2;
						}
					}
					balloons[index].BreakAllLines();
				}
			}
			while (smallMacyDialogueSource.isPlaying)
			{
				yield return null;
			}
			if (freeBalloons != null)
			{
				freeBalloons.Clear();
			}
			for (int l = 0; l < tweakables.dialogue.balloonPullIntro.Length; l++)
			{
				bool isInterrupting = false;
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.balloonPullIntro[l]);
				while (smallMacyDialogueSource.isPlaying)
				{
					if (!isInterrupting && tweakables.dialogue.balloonPullIntro[l].mInterruptability != Interruptability.uninterruptable && freeBalloons != null && freeBalloons.Count > 0)
					{
						NPCDialogueClip clipForBalloon = GetClipForBalloon(freeBalloons[0]);
						smallMacyDialogueSource.ForcePlayClipOverwrite(clipForBalloon);
						freeBalloons.RemoveAt(0);
						isInterrupting = true;
					}
					yield return null;
				}
			}
			while (smallMacyDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return null;
			currentState = MacyState.BalloonPull;
		}
		if (currentState == MacyState.BalloonPull)
		{
			t = 0f;
			float silentTime = 0f;
			int[] miscDialogueShuffle = RandomNonRepeat(tweakables.dialogue.miscDialogue.Length);
			string shuffleString = string.Empty;
			for (int m = 0; m < miscDialogueShuffle.Length; m++)
			{
				shuffleString = shuffleString + miscDialogueShuffle[m] + ", ";
			}
			SafeDebug.Log(shuffleString);
			int shuffleIter = 0;
			if (freeBalloons != null)
			{
				freeBalloons.Clear();
			}
			while (t < tweakables.balloonPullDuration && balloons.Count > 0)
			{
				if (freeBalloons != null && freeBalloons.Count > 0)
				{
					NPCDialogueClip balloonClip = GetClipForBalloon(freeBalloons[0]);
					if (balloonClip == null)
					{
						SafeDebug.Log(string.Concat(freeBalloons[0], " returned null"));
					}
					smallMacyDialogueSource.ForcePlayClipOverwrite(balloonClip);
					freeBalloons.RemoveAt(0);
					while (smallMacyDialogueSource.isPlaying)
					{
						yield return null;
					}
					silentTime = 0f;
				}
				else
				{
					silentTime += Time.deltaTime;
					if (silentTime > tweakables.silenceMax)
					{
						smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.miscDialogue[miscDialogueShuffle[shuffleIter]]);
						silentTime = 0f - tweakables.dialogue.miscDialogue[miscDialogueShuffle[shuffleIter]].clip.length;
						shuffleIter++;
						if (shuffleIter >= miscDialogueShuffle.Length)
						{
							shuffleIter = 0;
						}
					}
				}
				t += Time.deltaTime;
				yield return null;
			}
			yield return null;
			currentState = MacyState.Ending;
		}
		if (currentState == MacyState.Ending)
		{
			while (smallMacyDialogueSource.isPlaying)
			{
				yield return null;
			}
			if (balloons.Count <= 0)
			{
				JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Macy, _goodReview.mReview);
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.noBalloonsLeft);
				while (smallMacyDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			else
			{
				JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Macy, _badReview.mReview);
				smallMacyDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.someBalloonsLeft);
				while (smallMacyDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			yield return null;
			currentState = MacyState.Completed;
		}
		if (currentState == MacyState.Completed)
		{
			VVR_HUB.instance.GetComponent<HubExt>().EnableFlipper();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.endTime, this);
			CompleteJob();
		}
		yield return null;
	}

	private void ActivateWorld(bool oneTwenty)
	{
		OneOneRoot.SetActive(!oneTwenty);
		OneTwentyRoot.SetActive(oneTwenty);
		OneOneEnvironent.gameObject.SetActive(!oneTwenty);
		OneTwentyEnvironent.gameObject.SetActive(oneTwenty);
		if (macyEagerScene == null)
		{
			macyEagerScene = GetComponent<EagerScene>();
		}
		if (oneTwenty)
		{
			OneTwentyEnvironent.Activate();
			macyEagerScene.mEnvironmentProperties = OneTwentyEnvironent;
			macyEagerScene.farClipDistance = _oneTwentyFarClipDistance;
			EagerHeadsetManager.instance.farClipPlane.transform.localPosition = Vector3.forward * Mathf.Abs(_oneTwentyFarClipDistance - 1f);
			EagerScenes.instance.MovePlayerSafe(bigPlayerPosition.position);
			_farClipSphere.target = EagerMaster.instance.mainCamera.gameObject;
		}
		else
		{
			OneOneEnvironent.Activate();
			macyEagerScene.mEnvironmentProperties = OneOneEnvironent;
		}
	}

	public void BalloonFree(MacyFloat balloon)
	{
		if (freeBalloons == null)
		{
			freeBalloons = new List<MacyFloat> { balloon };
		}
		else
		{
			freeBalloons.Add(balloon);
		}
	}

	private NPCDialogueClip GetClipForBalloon(MacyFloat balloon)
	{
		NPCDialogueClip nPCDialogueClip = null;
		switch (balloon.type)
		{
		case MacyFloat.BalloonType.Cactus:
			nPCDialogueClip = tweakables.dialogue.cactusBalloon[tweakables.cactusBalloonIndex];
			tweakables.cactusBalloonIndex++;
			if (tweakables.cactusBalloonIndex >= tweakables.dialogue.cactusBalloon.Length)
			{
				tweakables.cactusBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Carrot:
			nPCDialogueClip = tweakables.dialogue.carrotBalloon[tweakables.carrotBalloonIndex];
			tweakables.carrotBalloonIndex++;
			if (tweakables.carrotBalloonIndex >= tweakables.dialogue.carrotBalloon.Length)
			{
				tweakables.carrotBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Chaz:
			nPCDialogueClip = tweakables.dialogue.chazBalloon[tweakables.chazBalloonIndex];
			tweakables.cactusBalloonIndex++;
			if (tweakables.cactusBalloonIndex >= tweakables.dialogue.cactusBalloon.Length)
			{
				tweakables.cactusBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Fish:
			nPCDialogueClip = tweakables.dialogue.fishBalloon[tweakables.fishBalloonIndex];
			tweakables.fishBalloonIndex++;
			if (tweakables.fishBalloonIndex >= tweakables.dialogue.fishBalloon.Length)
			{
				tweakables.fishBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Frog:
			nPCDialogueClip = tweakables.dialogue.frogBalloon[tweakables.frogBalloonIndex];
			tweakables.frogBalloonIndex++;
			if (tweakables.frogBalloonIndex >= tweakables.dialogue.frogBalloon.Length)
			{
				tweakables.frogBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Owl:
			nPCDialogueClip = tweakables.dialogue.owlBalloon[tweakables.owlBalloonIndex];
			tweakables.owlBalloonIndex++;
			if (tweakables.owlBalloonIndex >= tweakables.dialogue.owlBalloon.Length)
			{
				tweakables.owlBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Possum:
			nPCDialogueClip = tweakables.dialogue.possumBalloon[tweakables.possumBalloonIndex];
			tweakables.possumBalloonIndex++;
			if (tweakables.possumBalloonIndex >= tweakables.dialogue.possumBalloon.Length)
			{
				tweakables.possumBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Rabbit:
			nPCDialogueClip = tweakables.dialogue.rabbitBalloon[tweakables.rabbitBalloonIndex];
			tweakables.rabbitBalloonIndex++;
			if (tweakables.rabbitBalloonIndex >= tweakables.dialogue.rabbitBalloon.Length)
			{
				tweakables.rabbitBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.SoccerBall:
			nPCDialogueClip = tweakables.dialogue.soccerBallBalloon[tweakables.soccerBallBalloonIndex];
			tweakables.soccerBallBalloonIndex++;
			if (tweakables.soccerBallBalloonIndex >= tweakables.dialogue.soccerBallBalloon.Length)
			{
				tweakables.soccerBallBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Star:
			nPCDialogueClip = tweakables.dialogue.starBalloon[tweakables.starBalloonIndex];
			tweakables.starBalloonIndex++;
			if (tweakables.starBalloonIndex >= tweakables.dialogue.starBalloon.Length)
			{
				tweakables.starBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Surfdog:
			nPCDialogueClip = tweakables.dialogue.surfboardBalloon[tweakables.surfboardBalloonIndex];
			tweakables.surfboardBalloonIndex++;
			if (tweakables.surfboardBalloonIndex >= tweakables.dialogue.surfboardBalloon.Length)
			{
				tweakables.surfboardBalloonIndex = 0;
			}
			return nPCDialogueClip;
		case MacyFloat.BalloonType.Toast:
			nPCDialogueClip = tweakables.dialogue.toastBalloon[tweakables.toastBalloonIndex];
			tweakables.toastBalloonIndex++;
			if (tweakables.toastBalloonIndex >= tweakables.dialogue.toastBalloon.Length)
			{
				tweakables.toastBalloonIndex = 0;
			}
			return nPCDialogueClip;
		default:
			nPCDialogueClip = tweakables.dialogue.genericReaction[tweakables.genericBalloonIndex];
			tweakables.genericBalloonIndex++;
			if (tweakables.genericBalloonIndex >= tweakables.dialogue.genericReaction.Length)
			{
				tweakables.genericBalloonIndex = 0;
			}
			return nPCDialogueClip;
		}
	}

	private int[] RandomNonRepeat(int length)
	{
		int num = length - 1;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = Random.Range(0, i + 1);
			array[i] = array[num2];
			array[num2] = i;
		}
		return array;
	}
}
