using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VVR_Hernandez : SceneBehaviorManager
{
	public enum HernandezLevelState
	{
		preEntry,
		hernandezIntro,
		takingMemo,
		loseFirstLeaf,
		gym,
		restoration,
		hernandezDeath,
		done
	}

	public delegate void C4ArrivesCallback();

	[SerializeField]
	private HernandezTweakablesScriptableObject tweakables;

	private HernandezLevelState levelState;

	private int restorationMonologueIndex;

	[SerializeField]
	private VVRDialogueSource hDialogue;

	[SerializeField]
	private HernandezMovementController hernandezBody;

	[SerializeField]
	private Transform hernandezCenter;

	[SerializeField]
	private ConsumableGroup gymConsumableGroup;

	[SerializeField]
	private ConsumableGroup secondRestorationConsumableGroup;

	[SerializeField]
	private Transform officeCenter;

	[SerializeField]
	private FreeDrawStylus mStylus;

	[SerializeField]
	private ConsumableGroup barbellConsumableGroup;

	[SerializeField]
	private HernandezLeafController leafManager;

	[SerializeField]
	private HernandezC4Helper helperC4Controller;

	[SerializeField]
	private OfficeCurtainController curtainController;

	[SerializeField]
	private HernandezHelperBody[] helpers;

	[SerializeField]
	private HernandezLeaf firstDropLeaf;

	[SerializeField]
	private HernandezLiftingHand[] liftingHands;

	[SerializeField]
	private HernandezBarbellController[] barbells;

	[SerializeField]
	private ChazController giantAngryChaz;

	[SerializeField]
	private GameObject chazParent;

	[SerializeField]
	private ObjectHeadsetLOD elizaHeadsetObj;

	[SerializeField]
	private Headset elizaHeadset;

	[SerializeField]
	private HeadsetWrangler elizaHeadsetWrangler;

	[SerializeField]
	private Animator chazLeanAnimator;

	[SerializeField]
	private Animator glowArrowAnimator;

	[SerializeField]
	private HernandezMusicController musicController;

	[SerializeField]
	private float callC4Lead = 4.5f;

	[SerializeField]
	private float stylusDisuseHurryUpTime = 5f;

	[SerializeField]
	private float hernandezOfficeNoticeRadius = 8f;

	[SerializeField]
	private float buildNaggingInterval = 10f;

	[SerializeField]
	private float getScaredLeafPercentage = 0.5f;

	[SerializeField]
	private float unlockInnerLeafPercentage = 0.5f;

	[SerializeField]
	private float helperSurroundRadius = 2.5f;

	[SerializeField]
	private int firstLeafReattachRepetitions = 3;

	[SerializeField]
	private float[] leafFallDelays = new float[3] { 4f, 5f, 7f };

	private const float hernandezMemoPausePoint = 24f;

	private const float hernandezMemoPauseEndPoint = 25.5f;

	private bool leafReattachedTrigger;

	private bool leafPluckedTrigger;

	private bool hArrivedAtGymTrigger;

	private bool c4ArrivedTrigger;

	private DialoguePlaybackInstance returnReaction;

	private void OnDrawGizmos()
	{
		if (officeCenter != null)
		{
			Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
			Gizmos.DrawWireSphere(officeCenter.position, hernandezOfficeNoticeRadius);
		}
	}

	private void Awake()
	{
		HernandezLeafAnchor.OnLeafReattached += reactToHernandezLeafAttached;
		HernandezLeaf.OnLeafPlucked += reactToHernandezLeafPlucked;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		levelState = PersistentDataManager.GetHernandezState();
		setUpSceneObjects();
		GlobalCoroutineContainer.AddThread(HernandezCoroutine(), this);
		tweakables = Object.Instantiate(tweakables);
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (levelState >= HernandezLevelState.takingMemo && levelState < HernandezLevelState.done && (returnReaction == null || returnReaction.playbackProgress == DialoguePlaybackProgress.finished))
		{
			returnReaction = hDialogue.InterruptWithClip(tweakables.hernandezDialogue.playerReturnedFeedback.FireFeedbackClip());
		}
		else if (levelState >= HernandezLevelState.done)
		{
			helperC4Controller.SetHernandezDeadState();
			if (hernandezBody != null)
			{
				Object.Destroy(hernandezBody.gameObject);
			}
		}
	}

	private void setUpSceneObjects()
	{
		mStylus.gameObject.SetActive(value: false);
		chazParent.SetActive(value: false);
	}

	private IEnumerator HernandezCoroutine()
	{
		if (levelState >= HernandezLevelState.hernandezIntro && levelState < HernandezLevelState.done)
		{
			helperC4Controller.ForcePostHernandezPath();
		}
		gymConsumableGroup.gameObject.SetActive(value: false);
		if (levelState == HernandezLevelState.preEntry)
		{
			yield return null;
			hDialogue.PlayClip(tweakables.hernandezDialogue.preEntryRambling);
			yield return new WaitForSeconds(Mathf.Clamp(tweakables.hernandezDialogue.preEntryRambling.clip.length - callC4Lead, 0f, 999f));
			helperC4Controller.HeadToHernandez(reactToC4Arriving);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			hernandezBody.SetLookAtTarget(helperC4Controller.transform);
			while (!c4ArrivedTrigger)
			{
				yield return null;
			}
			hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.chewOutC4);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			helperC4Controller.DismissFromHernandez();
			while (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position.Flatten(), officeCenter.position.Flatten()) > hernandezOfficeNoticeRadius)
			{
				yield return null;
			}
			setAndSaveState(HernandezLevelState.hernandezIntro);
		}
		if (levelState < HernandezLevelState.done)
		{
			helperC4Controller.DismissFromHernandez();
		}
		curtainController.Open();
		hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
		if (levelState == HernandezLevelState.hernandezIntro)
		{
			hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.greetPlayer);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			setAndSaveState(HernandezLevelState.takingMemo);
		}
		if (levelState == HernandezLevelState.takingMemo)
		{
			hDialogue.PlayClip(tweakables.hernandezDialogue.memoGrabThisStylus);
			mStylus.gameObject.SetActive(value: true);
			glowArrowAnimator.SetTrigger("PopIn");
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			while (!ToolManager.PlayerGrabbingObjectBase(mStylus))
			{
				tweakables.hernandezDialogue.memoStylusHurryUp.Tick(Time.deltaTime, hDialogue);
				yield return null;
			}
			glowArrowAnimator.SetTrigger("PopOut");
			hDialogue.PlayClip(tweakables.hernandezDialogue.memoBeginning);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			while (!mStylus.HasBeenUsedOnce())
			{
				tweakables.hernandezDialogue.memoStartWritingHurryUp.Tick(Time.deltaTime, hDialogue);
				yield return null;
			}
			DialoguePlaybackInstance memoPlayback = hDialogue.PlayClip(tweakables.hernandezDialogue.memoFull);
			hernandezBody.PaceInOffice();
			hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
			DialogueSegment lastMemoSeg = hDialogue.GetCurrentDialogueSegment();
			bool hernandezPause = false;
			while (hDialogue.isPlaying)
			{
				if (lastMemoSeg != null && hDialogue.GetCurrentDialogueSegment() != null && lastMemoSeg.startTime != hDialogue.GetCurrentDialogueSegment().startTime && mStylus.GetTimeInactive() >= stylusDisuseHurryUpTime)
				{
					DialoguePlaybackInstance interruption = hDialogue.InterruptWithClip(tweakables.hernandezDialogue.memoHurryUps.FireFeedbackClip());
					hernandezBody.StopWalking();
					while (interruption != null && interruption.playbackProgress != DialoguePlaybackProgress.finished)
					{
						yield return null;
					}
					hernandezBody.PaceInOffice();
				}
				if (!hernandezPause && memoPlayback.progressTime > 24f)
				{
					hernandezPause = true;
					hernandezBody.StopWalking();
				}
				if (hernandezPause && memoPlayback.progressTime > 25.5f)
				{
					hernandezPause = false;
					hernandezBody.PaceInOffice();
				}
				lastMemoSeg = hDialogue.GetCurrentDialogueSegment();
				yield return null;
			}
			setAndSaveState(HernandezLevelState.loseFirstLeaf);
		}
		if (levelState == HernandezLevelState.loseFirstLeaf)
		{
			hernandezBody.StopWalking();
			hDialogue.PlayClip(tweakables.hernandezDialogue.leafFallMemoEnd);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			mStylus.EraseMemo();
			hDialogue.PlayClip(tweakables.hernandezDialogue.leafFallMemoChastize);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			hDialogue.PlayClip(tweakables.hernandezDialogue.leafFallReactToLeafFalling);
			firstDropLeaf.DropFromBody();
			musicController.StopMusic();
			Transform dummyTarget = new GameObject("Dummy Target").transform;
			dummyTarget.parent = base.transform;
			dummyTarget.position = firstDropLeaf.transform.position;
			yield return new WaitForSeconds(1f);
			hernandezBody.SetLookAtTarget(dummyTarget);
			leafManager.SetAcceptReattaching(newValue: true);
			leafReattachedTrigger = false;
			while (!ToolManager.PlayerGrabbingObjectBase(firstDropLeaf))
			{
				if (!hDialogue.isPlaying)
				{
					tweakables.hernandezDialogue.leafFallReattachHurryUps.Tick(Time.deltaTime, hDialogue);
				}
				yield return null;
			}
			Object.Destroy(dummyTarget.gameObject);
			hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
			musicController.RestartMusic();
			DialoguePlaybackInstance leafFallExplanation = hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.leafFallExplanation);
			int reattachCount = 0;
			bool firedReattachDialogue = false;
			bool hitLookAroundClip = false;
			while (hDialogue.isPlaying)
			{
				if (leafReattachedTrigger)
				{
					reattachCount++;
					if (reattachCount == 1)
					{
						hernandezBody.PaceInOffice();
					}
					if (!firedReattachDialogue && reattachCount >= firstLeafReattachRepetitions)
					{
						firedReattachDialogue = true;
						hDialogue.InterruptWithClip(tweakables.hernandezDialogue.leafFallReattachReaction);
					}
					else
					{
						float delay = leafFallDelays[Mathf.Clamp(reattachCount - 1, 0, leafFallDelays.Length)];
						firstDropLeaf.DropOnDelay(delay, reactToDelayDropLeaf);
					}
					leafReattachedTrigger = false;
				}
				if (!hitLookAroundClip && leafFallExplanation.playbackProgress == DialoguePlaybackProgress.playingSample && leafFallExplanation.progressTime >= 30.8f)
				{
					hernandezBody.StopWalking();
					Transform transform = new GameObject("Dummy Hernandez LookAt Target ('look at what I've made')").transform;
					transform.parent = base.transform;
					transform.transform.position = hernandezBody.transform.position + (hernandezBody.transform.position.Flatten() - EagerMaster.instance.mainCamera.transform.position.Flatten()).normalized * 5f;
					hernandezBody.SetLookAtTarget(transform);
					hitLookAroundClip = true;
				}
				yield return null;
			}
			yield return new WaitForSeconds(1.5f);
			musicController.RestartMusic();
			leafManager.SetAcceptReattaching(newValue: false);
			setAndSaveState(HernandezLevelState.gym);
		}
		hernandezBody.WalkToGym(reactToHernandezArrivedAtGym);
		hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
		gymConsumableGroup.gameObject.SetActive(value: true);
		if (levelState == HernandezLevelState.gym)
		{
			hDialogue.PlayClip(tweakables.hernandezDialogue.gymTransition);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			while (!hArrivedAtGymTrigger)
			{
				yield return null;
			}
			hernandezBody.DoJumpingJacks(fireJumpingJackDip);
			hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
			hDialogue.PlayClip(tweakables.hernandezDialogue.gymJumpingJacks);
			while (gymConsumableGroup.GetConsumableCompletionPercentage(countPartials: true) <= 0.01f)
			{
				if (!hDialogue.isPlaying)
				{
					tweakables.hernandezDialogue.gymStartBuildingHurryUp.Tick(Time.deltaTime, hDialogue);
				}
				yield return null;
			}
			hDialogue.PlayClip(tweakables.hernandezDialogue.gymBuildingMonologue);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			bool whinedAboutWeights = false;
			while (barbellConsumableGroup.GetConsumableCompletionPercentage(countPartials: false) < 0.5f)
			{
				if (!whinedAboutWeights)
				{
					hDialogue.PlayClip(tweakables.hernandezDialogue.gymUserHasntBuiltWeights);
					whinedAboutWeights = true;
				}
				yield return null;
			}
			hernandezBody.WalkToLiftPoint();
			List<Collider> barbellColliders = new List<Collider>();
			for (int i = 0; i < barbells.Length; i++)
			{
				barbellColliders.AddRange(barbells[i].colliders);
			}
			hDialogue.PlayClip(tweakables.hernandezDialogue.gymHernandezLifting);
			leafManager.IgnoreCollisionWithObjects(barbellColliders.ToArray(), ignoreCollision: true);
			int handIndex = 0;
			for (int j = 0; j < barbells.Length; j++)
			{
				VVRConsumable component = barbells[j].GetComponent<VVRConsumable>();
				if (component == null)
				{
					SafeDebug.LogError("Barbell weight should have a vvrconsumable! You are entering a world of pain my friend");
				}
				if (((barbells[j].isActiveAndEnabled && component.currentObjectState == VVRConsumable.objectState.wholeStatic) || component.currentObjectState == VVRConsumable.objectState.wholeShaking) && handIndex < 2)
				{
					barbells[j].SetLiftBehaviour(topParticle: true, bottomParticle: false, strokeSound: false);
					liftingHands[handIndex].GrabWeight(barbells[j].rb);
					liftingHands[handIndex].BeginLifting();
					handIndex++;
				}
			}
			musicController.StopMusic();
			musicController.PlayLiftMusic();
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			for (int k = 0; k < liftingHands.Length; k++)
			{
				liftingHands[k].QuitLifting();
			}
			for (int l = 0; l < barbells.Length; l++)
			{
				barbells[l].SetLiftBehaviour(topParticle: true, bottomParticle: true, strokeSound: true);
			}
			leafManager.IgnoreCollisionWithObjects(barbellColliders.ToArray(), ignoreCollision: false);
			musicController.StopLiftMusic();
			while (!ToolManager.PlayerGrabbingObjectType<HernandezBarbellController>())
			{
				tweakables.hernandezDialogue.gymGrabWeightHurryup.Tick(Time.deltaTime, hDialogue);
				yield return null;
			}
			HernandezBarbellController interactiveBarbell = null;
			for (int m = 0; m < ToolManager.ActiveToolGroups.Count; m++)
			{
				if (ToolManager.ActiveToolGroups[m].Manipulator.isGrabbing() && ToolManager.ActiveToolGroups[m].Manipulator.HeldObjectBase is HernandezBarbellController)
				{
					interactiveBarbell = (HernandezBarbellController)ToolManager.ActiveToolGroups[m].Manipulator.HeldObjectBase;
					break;
				}
			}
			musicController.PlayLiftMusic();
			interactiveBarbell.undroppable = true;
			DialoguePlaybackInstance narratePlayerLifting = hDialogue.PlayClip(tweakables.hernandezDialogue.gymNarratePlayerLifting);
			float oldNarratePlayerLiftingTime = 0f;
			DialoguePlaybackInstance oldPlaybackInstance = narratePlayerLifting;
			while (hDialogue.isPlaying)
			{
				if (interactiveBarbell.IsNotBeingLifted() && !tweakables.hernandezDialogue.gymKeepLiftingHurryup.hasFiredOnce)
				{
					tweakables.hernandezDialogue.gymKeepLiftingHurryup.Tick(Time.deltaTime, hDialogue);
				}
				if (narratePlayerLifting.progressTime > 7f)
				{
					if (oldNarratePlayerLiftingTime < 7f || (oldPlaybackInstance != narratePlayerLifting && hDialogue.GetClipQueue()[0] == narratePlayerLifting))
					{
						hernandezBody.TurnForAside(-70f);
					}
					else if (oldPlaybackInstance == narratePlayerLifting && hDialogue.GetClipQueue()[0] != narratePlayerLifting)
					{
						hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
					}
				}
				oldNarratePlayerLiftingTime = narratePlayerLifting.progressTime;
				if (hDialogue.GetClipQueue().Count > 0)
				{
					oldPlaybackInstance = hDialogue.GetClipQueue()[0];
				}
				yield return null;
			}
			musicController.RestartMusic();
			musicController.StopLiftMusic();
			hernandezBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
			hDialogue.PlayClip(tweakables.hernandezDialogue.gymForcePlayerDrop);
			interactiveBarbell.undroppable = false;
			interactiveBarbell.ForceDropByPlayer();
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			setAndSaveState(HernandezLevelState.restoration);
		}
		if (levelState == HernandezLevelState.restoration)
		{
			hernandezBody.FollowPlayer();
			hDialogue.PlayClip(tweakables.hernandezDialogue.restorationIntro);
			while (hDialogue.isPlaying)
			{
				yield return null;
			}
			float notBuildingTimer = 0f;
			float oldBuildPercentage = secondRestorationConsumableGroup.GetConsumableCompletionPercentage(countPartials: false);
			restorationMonologueIndex = PersistentDataManager.GetHernandezRestorationMonologueIndex();
			while (restorationMonologueIndex == 0 && secondRestorationConsumableGroup.GetConsumableCompletionPercentage(countPartials: false) <= oldBuildPercentage)
			{
				if (!tweakables.hernandezDialogue.restorationStartBuildingHurryUp.hasFiredOnce)
				{
					tweakables.hernandezDialogue.restorationStartBuildingHurryUp.Tick(Time.deltaTime, hDialogue);
				}
				oldBuildPercentage = secondRestorationConsumableGroup.GetConsumableCompletionPercentage(countPartials: false);
				yield return null;
			}
			while (restorationMonologueIndex < tweakables.hernandezDialogue.restorationMonologue.Length)
			{
				float currentPercentage = secondRestorationConsumableGroup.GetConsumableCompletionPercentage(countPartials: false);
				HernandezTweakablesScriptableObject.BuildPercentageDialogue currentRestoSection = tweakables.hernandezDialogue.restorationMonologue[restorationMonologueIndex];
				if (currentPercentage > currentRestoSection.buildPercentage)
				{
					hDialogue.PlayClip(currentRestoSection.buildReaction);
					notBuildingTimer = 0f;
					while (hDialogue.isPlaying)
					{
						yield return null;
					}
					if (currentRestoSection.leafFallReaction != null)
					{
						leafManager.DropRandomLeaf();
						hDialogue.PlayClip(currentRestoSection.leafFallReaction);
					}
					setAndSaveBuildMonologueIndex(restorationMonologueIndex + 1);
				}
				if (!hDialogue.isPlaying)
				{
					notBuildingTimer += Time.deltaTime;
					if (notBuildingTimer > buildNaggingInterval)
					{
						notBuildingTimer = 0f;
						hDialogue.PlayClip(tweakables.hernandezDialogue.restorationKeepBuildingFeedback.FireFeedbackClip());
					}
				}
				oldBuildPercentage = currentPercentage;
				yield return null;
			}
			setAndSaveState(HernandezLevelState.hernandezDeath);
		}
		yield return null;
		if (levelState < HernandezLevelState.done)
		{
			summonHelpersToHernandez();
		}
		if (levelState != HernandezLevelState.hernandezDeath)
		{
			yield break;
		}
		hernandezBody.StopWalking();
		leafManager.MakeOuterLeavesPluckable();
		hDialogue.PlayClip(tweakables.hernandezDialogue.deathWhosTheBoss);
		while (hDialogue.isPlaying)
		{
			yield return null;
		}
		leafManager.DropRandomLeaf();
		hDialogue.PlayClip(tweakables.hernandezDialogue.deathPluckMeIntro);
		leafPluckedTrigger = false;
		for (int n = 0; n < tweakables.hernandezDialogue.deathPleasePluckMe.Length; n++)
		{
			hDialogue.PlayClip(tweakables.hernandezDialogue.deathPleasePluckMe[n]);
		}
		while (!leafPluckedTrigger)
		{
			if (!hDialogue.isPlaying)
			{
				tweakables.hernandezDialogue.deathPluckMeHurryUp.Tick(Time.deltaTime, hDialogue);
				if (tweakables.hernandezDialogue.deathPluckMeHurryUp.hasFiredOnce)
				{
					tweakables.hernandezDialogue.deathPluckMeHurryUp.Reset();
				}
			}
			yield return null;
		}
		DialoguePlaybackInstance enthusiasticDeathMonologue = hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.deathPostPluckEnthusiastic);
		leafPluckedTrigger = false;
		bool reactedToLeafPlucked = false;
		while (leafManager.GetRemainingLeafPercentage() > getScaredLeafPercentage && enthusiasticDeathMonologue.playbackProgress != DialoguePlaybackProgress.finished)
		{
			if (leafPluckedTrigger && !reactedToLeafPlucked)
			{
				hDialogue.InterruptWithClip(tweakables.hernandezDialogue.deathPluckExclamation);
				reactedToLeafPlucked = true;
			}
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		musicController.StopMusic();
		leafPluckedTrigger = false;
		bool activatedLeaves = false;
		hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.deathPluckMonologueScared[0]);
		int deathMonologueIndex = 0;
		while (deathMonologueIndex < tweakables.hernandezDialogue.deathPluckMonologueScared.Length && !leafManager.isDead)
		{
			if (leafPluckedTrigger || !hDialogue.isPlaying)
			{
				deathMonologueIndex++;
				if (deathMonologueIndex < tweakables.hernandezDialogue.deathPluckMonologueScared.Length)
				{
					hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.deathPluckMonologueScared[deathMonologueIndex]);
				}
			}
			if (leafPluckedTrigger)
			{
				leafPluckedTrigger = false;
			}
			if (!activatedLeaves && leafManager.GetRemainingLeafPercentage() < unlockInnerLeafPercentage)
			{
				leafManager.MakeInnerLeavesPluckable();
				activatedLeaves = true;
			}
			yield return null;
		}
		if (!leafManager.isDead)
		{
			leafManager.MakeInnerLeavesPluckable();
			hDialogue.ForcePlayClipOverwrite(tweakables.hernandezDialogue.deathFinalBegging);
			while (hDialogue.isPlaying && !leafManager.isDead)
			{
				yield return null;
			}
			leafPluckedTrigger = false;
			while (!leafPluckedTrigger && !leafManager.isDead)
			{
				yield return null;
			}
			if (!leafManager.isDead)
			{
				leafManager.DropAllLeavesAndDie();
			}
		}
		hDialogue.PurgeAllDialogue();
		Object.Destroy(hDialogue.gameObject);
		elizaHeadsetWrangler.enabled = true;
		elizaHeadsetWrangler.CalibrateDesiredPoint();
		musicController.JumpToPostHernandezDeathMusic(3f);
		for (int num = 0; num < helpers.Length; num++)
		{
			helpers[num].RevertToDefaultPath();
			helpers[num].SetLookAtTarget(null);
		}
		VVR_Archive.ArmChazCaptureSequence();
		while (Vector3.Distance(elizaHeadset.transform.position, EagerMaster.instance.mainCamera.transform.position) > 1f || !elizaHeadset.insideRotationTrigger)
		{
			yield return null;
		}
		chazParent.SetActive(value: true);
		chazLeanAnimator.SetTrigger("ChazLeanOver");
		giantAngryChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3.65f, this);
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		EagerHeadsetManager.instance.RemoveHeadset();
	}

	private void reactToHernandezLeafAttached()
	{
		leafReattachedTrigger = true;
	}

	private void reactToHernandezLeafPlucked(HernandezLeaf leaf)
	{
		leafPluckedTrigger = true;
		if (leaf == leafManager.mouthLeaf)
		{
			muffleHernandezTalker();
		}
	}

	private void reactToDelayDropLeaf()
	{
		if (levelState != HernandezLevelState.loseFirstLeaf)
		{
		}
	}

	private void muffleHernandezTalker()
	{
		hDialogue.mAudioSource.pitch *= 0.95f;
		hDialogue.mAudioSource.volume *= 0.45f;
	}

	private void reactToHernandezArrivedAtGym()
	{
		hArrivedAtGymTrigger = true;
	}

	private void fireJumpingJackDip()
	{
		if (!hDialogue.isPlaying)
		{
			hDialogue.PlayClip(tweakables.hernandezDialogue.jumpingJackDips.FireFeedbackClip());
		}
	}

	public void ChazLazerHeadset()
	{
		giantAngryChaz.LazerObject(elizaHeadsetObj.gameObject, destroy: true);
	}

	private void reactToC4Arriving()
	{
		c4ArrivedTrigger = true;
	}

	private void setAndSaveState(HernandezLevelState newState)
	{
		levelState = newState;
		PersistentDataManager.OverwriteHernandezState(newState);
	}

	private void setAndSaveBuildMonologueIndex(int newIndex)
	{
		restorationMonologueIndex = newIndex;
		PersistentDataManager.SetHernandezRestorationMonologueIndex(newIndex);
	}

	private void summonHelpersToHernandez()
	{
		for (int i = 0; i < helpers.Length; i++)
		{
			Vector3 normalized = (helpers[i].transform.position - hernandezBody.transform.position).normalized;
			float num = 1f;
			Vector3 position = hernandezBody.transform.position + normalized * helperSurroundRadius + Vector3.up * 1.25f;
			while (Physics.OverlapSphere(position, 1f).Length > 0 && num > 0f)
			{
				num -= 0.25f;
				position = hernandezBody.transform.position + normalized * helperSurroundRadius * num + Vector3.up * 2f;
			}
			Transform transform = new GameObject().transform;
			transform.parent = base.transform;
			transform.name = "Helper Summon point " + i;
			transform.position = position;
			helpers[i].OverridePath(new Transform[1] { transform }, newLooping: false);
			helpers[i].SetLookAtTarget(hernandezCenter);
			helpers[i].UseTolerantTargetMode(tolerant: true);
		}
	}
}
