using System;
using System.Collections;
using Edelweiss.Coroutine;
using TButt;
using UnityEngine;

public class JobSunset : SceneBehaviorJob
{
	[Header("Scene References")]
	public ObjectSun sunObj;

	public VVRDialogueSource boat;

	public GvrAudioSource music;

	public GvrAudioSource recordScratch;

	public WaterBoiling waterBoiler;

	public GvrAudioSource angryWind;

	public GameObject radioSubtitles;

	public BuoyantElectricObject radio;

	[Header("Tweakables")]
	public SunsetSequenceScripableObject tweakables;

	[Header("Timing Variables")]
	[SerializeField]
	private float introTime = 3f;

	[SerializeField]
	private float boatReactionTime = 1f;

	[SerializeField]
	private float sunStartPos = -(float)Math.PI / 4f;

	[SerializeField]
	private float minSunPos = -0.055f;

	[SerializeField]
	private float sunSpeed = 0.005f;

	private int thingsGrabbed;

	private ObjectBase[] allObjects;

	[SerializeField]
	private SunsetCombustable boatFire;

	[Header("Reviews")]
	public JobReviewScriptableObject badEnding;

	public JobReviewScriptableObject fireEnding;

	public JobReviewScriptableObject goodEnding;

	public JobReviewScriptableObject perfectEnding;

	private ShipState mShipState;

	private bool controllerIsMoving;

	private bool hasGrabbedSomething;

	private bool hasTeleportedOnce;

	private int lastRand = 4;

	private bool waterBoiling;

	private bool exitedThroughGrabbingThings;

	private bool rantingAboutSun;

	private SafeCoroutine mainThread;

	private bool radioPlaying;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		mainThread = GlobalCoroutineContainer.AddThread(SunsetCoroutine(), this);
		mShipState = ShipState.starting;
		if (mShipState == ShipState.starting)
		{
			sunObj.GetSunsetBehavior().SetSunT(sunStartPos);
			sunObj.GetSunsetBehavior().SetSunsetSpeed(0f);
			sunObj.ungrabbable = true;
		}
		allObjects = UnityEngine.Object.FindObjectsOfType<ObjectBase>();
		for (int i = 0; i < allObjects.Length; i++)
		{
			if (allObjects[i] != sunObj)
			{
				allObjects[i].onBecomeGrabbed.AddListener(incrementThingsGrabbed);
			}
		}
		Teleporter.OnFinishTeleportEvent += playerTeleported;
		if (boatFire == null)
		{
			boatFire = boat.GetComponentInChildren<SunsetCombustable>();
		}
		if (boatFire != null)
		{
			boatFire.OnStartFire += skipToFireEnding;
		}
	}

	protected void OnDisable()
	{
		Teleporter.OnFinishTeleportEvent -= playerTeleported;
	}

	private bool ControllerIsMoving()
	{
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (TBInput.GetAngularVelocity(ToolManager.ActiveToolGroups[i].Controller).magnitude > 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator SunsetCoroutine()
	{
		if (mShipState == ShipState.loading)
		{
			yield return null;
		}
		if (mShipState == ShipState.starting)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(introTime, this);
		}
		mShipState = ShipState.preMonologue1;
		if (mShipState == ShipState.preMonologue1)
		{
			sunObj.GetSunsetBehavior().SetSunsetSpeed(sunSpeed);
			boat.PlayClip(tweakables.dialogueOptions.itsANiceDay);
			while (boat.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(boatReactionTime, this);
			if (ControllerIsMoving())
			{
				boat.PlayClip(tweakables.dialogueOptions.putYourControllerDown);
				while (boat.isPlaying)
				{
					yield return null;
				}
				if (!ControllerIsMoving())
				{
					boat.PlayClip(tweakables.dialogueOptions.perfect);
					while (boat.isPlaying)
					{
						yield return null;
					}
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(boatReactionTime, this);
				mShipState = ShipState.monologue1;
			}
			else
			{
				boat.PlayClip(tweakables.dialogueOptions.perfect);
				while (boat.isPlaying)
				{
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(boatReactionTime, this);
				mShipState = ShipState.monologue1;
			}
		}
		if (mShipState == ShipState.monologue1)
		{
			sunObj.ungrabbable = false;
			boat.PlayClip(tweakables.dialogueOptions.beforeTheSunSets.dialogueClip);
			while (boat.isPlaying)
			{
				if (thingsGrabbed >= 5)
				{
					recordScratch.Play();
					music.Stop();
					radioSubtitles.SetActive(value: false);
					radioPlaying = false;
					boat.ForcePlayClipOverwrite(tweakables.dialogueOptions.miscDialogue[2]);
					boat.recordScratchOnInterrupt = false;
					while (boat.isPlaying)
					{
						yield return null;
					}
					sunObj.GetSunsetBehavior().SetIsInOrbit(value: false);
					if (!sunObj.isGrabbed)
					{
						sunObj.GetSunsetBehavior().MoveSun(new Vector3(0f, 20f, 45f), 8f);
					}
					boat.ForcePlayClipOverwrite(tweakables.dialogueOptions.miscDialogue[8]);
					while (boat.isPlaying)
					{
						yield return null;
					}
					if (!sunObj.isGrabbed)
					{
						sunObj.GetSunsetBehavior().MoveSun(new Vector3(0f, 20f, 30f), 8f);
					}
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
					exitedThroughGrabbingThings = true;
					thingsGrabbed = 0;
					mShipState = ShipState.monologue2;
					break;
				}
				if (sunObj.isGrabbed)
				{
					if (sunObj.GetTimesGrabbed() > 1)
					{
						mShipState = ShipState.monologue2;
						break;
					}
					rantingAboutSun = true;
					bool interrupted = false;
					boat.InterruptWithClip(tweakables.dialogueOptions.miscDialogue[0]);
					while (!boat.isPlayingInterruption() && boat.GetClipQueue().Count > 0)
					{
						yield return null;
					}
					while (boat.isPlayingInterruption())
					{
						interrupted = true;
						if (waterBoiler.boiling && !waterBoiling)
						{
							boat.InterruptWithClip(tweakables.dialogueOptions.miscDialogue[6]);
							waterBoiling = true;
							while (boat.isPlayingInterruption())
							{
								yield return null;
							}
						}
						yield return null;
					}
					if (sunObj.isGrabbed && interrupted)
					{
						sunObj.ForceDropByPlayer();
					}
					if (interrupted)
					{
						sunObj.GetSunsetBehavior().SetIsInOrbit(value: true);
					}
					if (mShipState != ShipState.completed && interrupted)
					{
						boat.InterruptWithClip(tweakables.dialogueOptions.miscDialogue[7], enqueueIfCantInterrupt: false, stackInterruptions: false, forceNoRecordScratch: true);
					}
					while (boat.isPlayingInterruption())
					{
						yield return null;
					}
				}
				rantingAboutSun = false;
				while (boat.isPlayingInterruption())
				{
					hasGrabbedSomething = false;
					yield return null;
				}
				if (!hasGrabbedSomething && !sunObj.isGrabbed && ToolManager.PlayerGrabbingSomething())
				{
					ToolManager.DropEverything();
				}
				if (sunObj.GetSunsetBehavior().GetSunT() < minSunPos)
				{
					sunObj.GetSunsetBehavior().SetSunsetSpeed(0f);
				}
				yield return null;
			}
			if (mShipState == ShipState.monologue1)
			{
				if (sunObj.isGrabbed)
				{
					sunObj.ForceDropByPlayer();
				}
				sunObj.GetSunsetBehavior().SetIsInOrbit(value: true);
				sunObj.GetSunsetBehavior().SetSunT(minSunPos);
				sunObj.GetSunsetBehavior().SetSunsetSpeed(sunSpeed * 3f);
				boat.PlayClip(tweakables.dialogueOptions.endOfFirstMonologuea);
				while (boat.isPlaying)
				{
					yield return null;
				}
				sunObj.GetSunsetBehavior().SetSunT((float)Math.PI / 4f);
				sunObj.GetSunsetBehavior().SetSunsetSpeed(sunSpeed * 30f);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
				boat.PlayClip(tweakables.dialogueOptions.endOfFirstMonologuec);
				while (boat.isPlaying)
				{
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
				boat.PlayClip(tweakables.dialogueOptions.endOfFirstMonologueb);
				while (boat.isPlaying)
				{
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(boatReactionTime, this);
				mShipState = ShipState.completed;
				if (thingsGrabbed <= 0)
				{
					JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Sunset, perfectEnding.mReview);
				}
				else
				{
					JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Sunset, goodEnding.mReview);
				}
				CompleteJob();
			}
		}
		if (mShipState == ShipState.monologue2)
		{
			boat.recordScratchOnInterrupt = false;
			music.Stop();
			radioSubtitles.SetActive(value: false);
			radioPlaying = false;
			if (angryWind != null)
			{
				angryWind.Play();
			}
			if (!exitedThroughGrabbingThings)
			{
				boat.ForcePlayClipOverwrite(tweakables.dialogueOptions.really);
				while (boat.isPlaying)
				{
					yield return null;
				}
			}
			boat.PlayClip(tweakables.dialogueOptions.monologueBody.dialogueClip);
			while (boat.isPlaying)
			{
				if (waterBoiler.boiling && !waterBoiling)
				{
					boat.InterruptWithClip(tweakables.dialogueOptions.miscDialogue[6]);
					waterBoiling = true;
					while (boat.isPlayingInterruption())
					{
						yield return null;
					}
				}
				yield return null;
			}
			if (sunObj.isGrabbed)
			{
				boat.PlayClip(tweakables.dialogueOptions.giveMeThat);
				while (boat.isPlaying)
				{
					yield return null;
				}
				sunObj.ForceDropByPlayer();
			}
			sunObj.GetSunsetBehavior().SetIsInOrbit(value: true);
			while (boat.isPlaying)
			{
				yield return null;
			}
			mShipState = ShipState.crazyMad;
			yield return null;
		}
		if (mShipState == ShipState.crazyMad)
		{
			sunObj.ungrabbable = true;
			sunObj.GetSunsetBehavior().isAngry = true;
			boat.PlayClip(tweakables.dialogueOptions.oneTwoThree);
			while (boat.isPlaying)
			{
				angryWind.volume = Mathf.Lerp(angryWind.volume, 1f, Time.deltaTime);
				yield return null;
			}
			boat.PlayClip(tweakables.dialogueOptions.youLikeThat);
			while (boat.isPlaying)
			{
				yield return null;
			}
			sunObj.GetSunsetBehavior().SetSunsetSpeed(2.5f);
			boat.PlayClip(tweakables.dialogueOptions.fourFiveSix);
			while (boat.isPlaying)
			{
				yield return null;
			}
			boat.PlayClip(tweakables.dialogueOptions.wellDontYou);
			while (boat.isPlaying)
			{
				yield return null;
			}
			boat.PlayClip(tweakables.dialogueOptions.sevenEight);
			while (boat.isPlaying)
			{
				yield return null;
			}
			boat.PlayClip(tweakables.dialogueOptions.boringYou);
			while (boat.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			boat.PlayClip(tweakables.dialogueOptions.doneHere);
			while (boat.isPlaying)
			{
				yield return null;
			}
			mShipState = ShipState.completed;
		}
		if (mShipState == ShipState.completed)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Sunset, badEnding.mReview);
			CompleteJob();
		}
	}

	private void skipToFireEnding()
	{
		boat.PurgeAllDialogue();
		mainThread.Stop();
		GlobalCoroutineContainer.AddThread(FireEnd(), this);
	}

	private IEnumerator FireEnd()
	{
		boat.PlayClip(tweakables.dialogueOptions.miscDialogue[5]);
		while (boat.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(boatReactionTime, this);
		JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Sunset, fireEnding.mReview);
		CompleteJob();
	}

	public void incrementThingsGrabbed()
	{
		if (boat.isPlayingInterruption())
		{
			return;
		}
		hasGrabbedSomething = true;
		if (mShipState != ShipState.monologue1 || rantingAboutSun)
		{
			return;
		}
		if (thingsGrabbed != 5)
		{
			int num;
			for (num = UnityEngine.Random.Range(1, tweakables.dialogueOptions.grabReactions.Length - 1); num == lastRand; num = UnityEngine.Random.Range(1, tweakables.dialogueOptions.grabReactions.Length - 1))
			{
			}
			DialoguePlaybackInstance dialoguePlaybackInstance = boat.InterruptWithClip(tweakables.dialogueOptions.grabReactions[num]);
			lastRand = num;
			if (dialoguePlaybackInstance != null)
			{
				if (!dialoguePlaybackInstance.interruptingOnDelay)
				{
					GlobalCoroutineContainer.AddThread(dropOnDelay(tweakables.dialogueOptions.grabReactions[num].clip.length), this);
				}
				else
				{
					GlobalCoroutineContainer.AddThread(dropOnDelay(tweakables.dialogueOptions.grabReactions[num].clip.length + dialoguePlaybackInstance.timeToInterrupt), this);
				}
			}
			else
			{
				GlobalCoroutineContainer.AddThread(dropOnDelay(tweakables.dialogueOptions.grabReactions[num].clip.length + 2f), this);
			}
		}
		thingsGrabbed++;
	}

	private void playerTeleported(Vector3 startpos, Vector3 endpos)
	{
		if (mShipState == ShipState.monologue1 && !hasTeleportedOnce && boat.InterruptWithClip(tweakables.dialogueOptions.miscDialogue[3]) != null)
		{
			hasTeleportedOnce = true;
		}
	}

	private IEnumerator dropOnDelay(float s)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		ToolManager.DropEverything();
	}

	private void OnEnable()
	{
		if (radioPlaying && !radio.broken)
		{
			music.Play();
		}
	}
}
