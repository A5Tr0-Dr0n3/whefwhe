using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using GVR.Gfx;
using UnityEngine;

public class JobTina : SceneBehaviorJob
{
	[Header("Main Sequence Positions")]
	[SerializeField]
	private DollMainSequencePath tinaSequencePath;

	[SerializeField]
	private DollMainSequencePath playerSequencePath;

	[Header("Reviews")]
	[SerializeField]
	private JobReviewScriptableObject vacuumReview;

	[SerializeField]
	private JobReviewScriptableObject goodReview;

	[SerializeField]
	private JobReviewScriptableObject badReview;

	[Header("Environment Stuff")]
	[SerializeField]
	private EnvironmentProperties interruptProps;

	[SerializeField]
	private EnvironmentProperties lightProps;

	[SerializeField]
	private EnvironmentProperties startProps;

	[SerializeField]
	private EagerScene mEagerScene;

	[SerializeField]
	private GvrAudioSource music;

	[SerializeField]
	private GvrAudioSource bigLightSound;

	[SerializeField]
	private GvrAudioSource smallLightSound;

	[Header("Scene References")]
	[SerializeField]
	private AnimationCurve rotationCurve;

	[SerializeField]
	private Transform[] tableEnds;

	[SerializeField]
	private WashingMachine washingMachine;

	[SerializeField]
	public GvrAudioSource popSound;

	[SerializeField]
	public VVRDialogueSource tinaDialogueSource;

	[SerializeField]
	public Transform dollhouseRoot;

	public Transform holdRoot;

	public Transform table;

	[SerializeField]
	private Dollhouse theDollHouse;

	[SerializeField]
	private DollhouseDoll notDabsTheRabbit;

	[SerializeField]
	private DollhouseSpinner spinner;

	[SerializeField]
	private AudioSourceLoudness tinaTalker;

	[SerializeField]
	private FurnitureNavigation jerichoNavRef;

	[SerializeField]
	private GameObject whiteJericho;

	[SerializeField]
	private GameObject redJericho;

	[SerializeField]
	private GameObject jerichoSock;

	public DollhouseObjectSyncer navSyncer;

	[SerializeField]
	public VVRConsumable tinaEatTrigger;

	[SerializeField]
	internal DollhouseDoll jericho;

	[SerializeField]
	private FurnitureNavigation donaldNavRef;

	[SerializeField]
	private DollhouseDoll[] playerDolls;

	[SerializeField]
	private Transform donaldChargePosition;

	[SerializeField]
	private KukaController tina;

	[SerializeField]
	private DollDoor[] dollDoors;

	[SerializeField]
	private GameObject tinaRemote;

	[SerializeField]
	private GlitchedTV tv;

	[SerializeField]
	private VVRConsumable[] tinaConsumables;

	[SerializeField]
	private VVRConsumable[] objectsToBecomeConsumable;

	[SerializeField]
	private GameObject[] objectsToDestroy;

	[SerializeField]
	private VVRConsumable[] objectsToDestroyOnConsume;

	[SerializeField]
	private AudioLoop windLoop;

	[SerializeField]
	private Transform forceJerichoLaundryPos;

	[SerializeField]
	private FurnitureNavigation[] diningRoomNavRefs;

	[SerializeField]
	private MeshRenderer[] renderersToDisable;

	[SerializeField]
	private Transform startDonaldHome;

	[Header("Tweakables")]
	[SerializeField]
	public TinaJobObject tweakables;

	public TinaState currentState;

	private SafeCoroutine mainThread;

	private SafeCoroutine mouseOverThread;

	private ObjectBase notThatCharacter;

	internal DollhouseDoll tinaDoll;

	internal DollhouseDoll playerDoll;

	private bool characterSelected;

	private int grabbedTinasDollIndex;

	private int genericFurnitureResponseIndex;

	private bool playerHasTeleported;

	private int hitTinaIndex;

	private bool hitTinaRecently;

	private List<DollhouseRoom> roomsPlayerVisited = new List<DollhouseRoom>();

	private List<NpcDollName> npcDollsReacted = new List<NpcDollName>();

	private bool frontFacing = true;

	private bool ending;

	internal bool talkingSeqeunce;

	private bool hasBoxInterrupted;

	private DollhouseSide currentSide;

	private bool diningRoomSequenceStarted;

	private float characterSelectTimer;

	private float animationTime;

	private int animationState;

	private float audioTime;

	public static JobTina instance;

	private bool FirstActivation = true;

	private List<DialoguePlaybackInstance> holdQueue;

	private bool lightsShouldBeOff = true;

	internal SafeCoroutine reactThread;

	private bool tinasDollGrabbedRecently;

	private int vacuumIndex;

	private List<NpcDollName> dollGrabbedReacted = new List<NpcDollName>();

	private bool hasInterruptedDuringStart;

	private int returnFromInterruptIndex;

	private bool isDarkEnvironment = true;

	private bool firstCharacterSelected;

	private SafeCoroutine selectCharacterThread;

	private DollhouseDoll cindy;

	internal bool ended;

	protected override void OnFirstTimeSceneActivated()
	{
		if (instance == null)
		{
			instance = this;
		}
		if (renderersToDisable != null && renderersToDisable.Length > 0)
		{
			for (int i = 0; i < renderersToDisable.Length; i++)
			{
				renderersToDisable[i].enabled = false;
			}
		}
		windLoop.Play();
		base.OnFirstTimeSceneActivated();
		init();
		mainThread = GlobalCoroutineContainer.AddThread(TinaCoroutine(), this);
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (FirstActivation)
		{
			FirstActivation = false;
			return;
		}
		if (currentState > TinaState.Intro)
		{
			music.Play();
		}
		for (int i = 0; i < tinaDialogueSource.GetClipQueue().Count; i++)
		{
			tinaDialogueSource.GetClipQueue()[i] = holdQueue[i];
		}
		tinaDialogueSource.mAudioSource.time = audioTime;
		tinaDialogueSource.mAudioSource.Play();
		TinaAnimationState mAnimationState = tina.mAnimationState;
		switch (mAnimationState)
		{
		case TinaAnimationState.Intro:
			tina.mAnimator.SetBool("DoIntro", value: true);
			tina.mAnimator.Play(mAnimationState.ToString(), 0, animationTime);
			tina.mAnimator.Play(mAnimationState.ToString(), 1, animationTime);
			break;
		case TinaAnimationState.VacuumInterruptionIntro:
			tina.mAnimator.SetBool("DoIntro", value: true);
			tina.mAnimator.Play(TinaAnimationState.Intro.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.Intro.ToString(), 1, tina.introResumeTime);
			tina.StartVacuumAnimation();
			break;
		case TinaAnimationState.PickCharacterLoop:
			tina.mAnimator.Play(mAnimationState.ToString(), 0, animationTime);
			tina.mAnimator.Play(mAnimationState.ToString(), 1, animationTime);
			break;
		case TinaAnimationState.PickCharacterInterr:
			tina.mAnimator.Play(TinaAnimationState.PickCharacterLoop.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.PickCharacterLoop.ToString(), 1, tina.introResumeTime);
			tina.StartInterruptionAnimation();
			break;
		case TinaAnimationState.PickADoll:
			tina.mAnimator.Play(mAnimationState.ToString(), 0, animationTime);
			tina.mAnimator.Play(mAnimationState.ToString(), 1, animationTime);
			break;
		case TinaAnimationState.VacuumInterruptionPickCharacter:
			tina.mAnimator.Play(TinaAnimationState.PickCharacterLoop.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.PickCharacterLoop.ToString(), 1, tina.introResumeTime);
			tina.StartVacuumAnimation();
			break;
		case TinaAnimationState.PickUpRemote:
			tina.mAnimator.Play(mAnimationState.ToString(), 0, animationTime);
			tina.mAnimator.Play(mAnimationState.ToString(), 1, animationTime);
			break;
		case TinaAnimationState.VacuumInterruptionRemote:
			tina.mAnimator.Play(TinaAnimationState.PickUpRemote.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.PickUpRemote.ToString(), 1, tina.introResumeTime);
			tina.StartVacuumAnimation();
			break;
		case TinaAnimationState.VacuumInterruption:
			tina.mAnimator.Play(TinaAnimationState.MainLoop.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.MainLoop.ToString(), 1, tina.introResumeTime);
			tina.StartVacuumAnimation();
			break;
		case TinaAnimationState.MainLoop:
			tina.mAnimator.Play(mAnimationState.ToString(), 0, animationTime);
			tina.mAnimator.Play(mAnimationState.ToString(), 1, animationTime);
			break;
		case TinaAnimationState.Interruption:
			tina.mAnimator.Play(TinaAnimationState.MainLoop.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.MainLoop.ToString(), 1, tina.introResumeTime);
			tina.StartInterruptionAnimation();
			break;
		case TinaAnimationState.VacuumInterruptionEnd:
			tina.mAnimator.Play(TinaAnimationState.EndSequence.ToString(), 0, tina.introResumeTime);
			tina.mAnimator.Play(TinaAnimationState.EndSequence.ToString(), 1, tina.introResumeTime);
			tina.StartVacuumAnimation();
			break;
		case TinaAnimationState.EndSequence:
			tina.mAnimator.Play(TinaAnimationState.EndSequence.ToString(), 0, animationTime);
			tina.mAnimator.Play(TinaAnimationState.EndSequence.ToString(), 1, animationTime);
			break;
		}
	}

	public override void OnSceneDeactivated()
	{
		base.OnSceneDeactivated();
		animationTime = tina.normalizedTime;
		holdQueue = tinaDialogueSource.GetClipQueue();
		if (tinaDialogueSource.GetClipQueue().Count > 0)
		{
			audioTime = tinaDialogueSource.GetClipQueue()[0].progressTime;
		}
		tina.headsetResumeTime = tina.introResumeTime;
	}

	protected void OnEnable()
	{
	}

	protected void OnDisable()
	{
	}

	private IEnumerator TinaCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.introDelay, this);
		while (currentState == TinaState.Intro)
		{
			tinaDialogueSource.PlayClip(tweakables.dialogue.fullIntro.dialogueClip);
			yield return tina.PlayIntroClip();
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			lightsShouldBeOff = false;
			startProps.gameObject.SetActive(value: false);
			music.Play();
			yield return SetLightEnvironment(bigSound: true);
			currentState = TinaState.SelectCharacter;
			yield return null;
		}
		while (currentState == TinaState.SelectCharacter)
		{
			for (int i = 0; i < dollDoors.Length; i++)
			{
				if (i == dollDoors.Length - 1)
				{
					yield return dollDoors[i].RaiseDoll(1.7f);
				}
				else
				{
					dollDoors[i].RaiseDoll(1.7f);
				}
			}
			bool hasRemindedPlayer = false;
			while (!characterSelected)
			{
				characterSelectTimer += Time.deltaTime;
				if (characterSelectTimer > tweakables.remindPlayerToPickDollTime && !hasRemindedPlayer)
				{
					hasRemindedPlayer = true;
					tinaDialogueSource.PlayClip(tweakables.dialogue.pickADoll);
					tina.StartPickADoll();
					while (tinaDialogueSource.isPlaying)
					{
						yield return null;
					}
				}
				if (characterSelectTimer > tweakables.remindPlayerToPickDollTime * 2f)
				{
					DollhouseDoll hold = null;
					tinaDialogueSource.PlayClip(tweakables.dialogue.youBeJaime);
					if (firstCharacterSelected)
					{
						for (int j = 0; j < playerDolls.Length; j++)
						{
							if (playerDolls[j].isActiveAndEnabled && hold == null)
							{
								hold = playerDolls[j];
								firstCharacterSelected = true;
								yield return SetCharacterSelected(s: true, hold);
							}
						}
					}
					else
					{
						dollDoors[0].LowerDoll(0.7f);
						hold = playerDolls[1];
						firstCharacterSelected = true;
						yield return SetCharacterSelected(s: true, hold);
					}
				}
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			while (tinaDoll == null)
			{
				yield return null;
			}
			currentState = TinaState.PlayTime;
			yield return null;
		}
		while (currentState == TinaState.PlayTime)
		{
			tinaDoll.ungrabbable = false;
			tinaDialogueSource.PlayClip(tweakables.dialogue.iWillPlay.dialogueClip);
			playerDoll.doingMainSequence = true;
			playerDoll.mainSequenceState = PlayerDollState.toFront;
			if (!playerDoll.isGrabbed && playerDoll.delayedSwitchThread == null && playerDoll.currentState != DollhouseObjectState.gettingUp)
			{
				playerDoll.SetState(DollhouseObjectState.scriptedSequence);
			}
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toFront, 4.5f);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = true;
			for (int k = 0; k < dollDoors.Length; k++)
			{
				dollDoors[k].LowerDoll(0.7f, lowerDoll: false);
			}
			tinaDialogueSource.PlayClip(tweakables.dialogue.wereHome.dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.wereHome.dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			playerDoll.mainSequenceState = PlayerDollState.toBedroom;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toBedroom, 5f);
			talkingSeqeunce = true;
			tinaDialogueSource.PlayClip(tweakables.dialogue.masterBedroom.dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.masterBedroom.dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			currentSide = DollhouseSide.back;
			tryFlip();
			playerDoll.mainSequenceState = PlayerDollState.toLivingroom;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toLivingroom, 7f);
			talkingSeqeunce = true;
			tinaDialogueSource.PlayClip(tweakables.dialogue.livingRoom[0].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.livingRoom[0].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			if (reactThread != null && !reactThread.IsRunning)
			{
				SafeDebug.Log("Activing non progression failsafe check ur code");
				reactThread = null;
			}
			while (reactThread != null)
			{
				yield return null;
			}
			yield return TvInterruption();
			tinaDialogueSource.PlayClip(tweakables.dialogue.livingRoom[2].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.livingRoom[2].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			playerDoll.mainSequenceState = PlayerDollState.toKidsRoom;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toKidsRoom, 7f);
			talkingSeqeunce = true;
			if (notDabsTheRabbit.currentRoom == DollhouseRoom.KidRoom)
			{
				tinaDialogueSource.PlayClip(tweakables.dialogue.childrensRoom[0].dialogueClip);
				yield return tinaDoll.Talk(tweakables.dialogue.childrensRoom[0].dialogueClip.duration);
			}
			tinaDialogueSource.PlayClip(tweakables.dialogue.childrensRoom[1].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.childrensRoom[1].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			playerDoll.mainSequenceState = PlayerDollState.toAttic;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toAttic, 7f);
			talkingSeqeunce = true;
			tinaDialogueSource.PlayClip(tweakables.dialogue.attic[0].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.attic[0].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			tinaDialogueSource.PlayClip(tweakables.dialogue.attic[1].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.attic[1].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			if (IsOnTable(jericho.transform.position, gameObject))
			{
				jericho.SetState(DollhouseObjectState.scriptedSequence);
			}
			sendNPCDollsToDiningRoom();
			tryFlip();
			currentSide = DollhouseSide.front;
			playerDoll.mainSequenceState = PlayerDollState.toDiningroom;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toDiningroom, 15f);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			if (washingMachine.GetComponent<DollhouseObject>().currentState == DollhouseObjectState.onGround)
			{
				washingMachine.GetComponent<DollhouseObject>().SetState(DollhouseObjectState.home);
			}
			float jerichoTimer = 0f;
			bool forceContinue = false;
			while ((double)jerichoNavRef.DistanceFromDiningRoom() > 0.6 && !forceContinue)
			{
				jerichoTimer += Time.deltaTime;
				if (jerichoTimer > 7f)
				{
					forceContinue = true;
					popSound.Play();
					if (jericho.isGrabbed)
					{
						jericho.ForceDropByPlayer();
					}
					jerichoNavRef.SetState(NavFurnitureState.DiningSequence, jerichoNavRef.diningroomTarget.localPosition, jerichoNavRef.diningroomTarget.localRotation);
					jericho.SetState(DollhouseObjectState.scriptedSequence);
					jerichoNavRef.transform.position = jerichoNavRef.diningroomTarget.position;
					jericho.transform.localPosition = jerichoNavRef.transform.localPosition;
				}
				yield return null;
			}
			jerichoTimer = 0f;
			talkingSeqeunce = true;
			tinaDialogueSource.PlayClip(tweakables.dialogue.diningRoom[0].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.diningRoom[0].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			if (reactThread != null && !reactThread.IsRunning)
			{
				SafeDebug.Log("Activing non progression failsafe check ur code");
				reactThread = null;
			}
			while (reactThread != null)
			{
				yield return null;
			}
			yield return SeatingHeirarchiesInterruption();
			startDonaldCharge();
			tinaDialogueSource.PlayClip(tweakables.dialogue.diningRoom[2].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.diningRoom[2].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			talkingSeqeunce = false;
			stopDonaldCharge();
			playerDoll.mainSequenceState = PlayerDollState.toLaundryroom;
			navSyncer.diningRoomTime = false;
			jerichoNavRef.SetMainSequenceTarget(jerichoNavRef.laundryRoomPos);
			jerichoNavRef.SetHomeTarget(jerichoNavRef.laundryRoomPos);
			tryFlip();
			currentSide = DollhouseSide.back;
			yield return navSyncer.MoveTinaDollToSequencePosition(PlayerDollState.toLaundryroom, 5f);
			bool jerichoForceSet = false;
			while ((double)jerichoNavRef.DistanceFromAgentTarget() > 0.7 && !jerichoForceSet)
			{
				jerichoTimer += Time.deltaTime;
				if (jerichoTimer > 5f)
				{
					popSound.Play();
					if (jericho.isGrabbed)
					{
						jericho.ForceDropByPlayer();
					}
					jericho.SetState(DollhouseObjectState.notSetUp);
					jericho.rb.isKinematic = true;
					navSyncer.ignoreJericho = true;
					jericho.transform.SetParent(forceJerichoLaundryPos);
					jericho.transform.localPosition = Vector3.zero;
					jericho.enabled = false;
					jerichoForceSet = true;
				}
				yield return null;
			}
			if (jerichoForceSet)
			{
				jericho.transform.position = forceJerichoLaundryPos.position;
				jericho.transform.SetParent(forceJerichoLaundryPos);
				jericho.transform.localPosition = Vector3.zero;
			}
			washingMachine.GetComponent<DollhouseObject>().ungrabbable = true;
			jericho.ungrabbable = true;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			tinaDialogueSource.PlayClip(tweakables.dialogue.laundryRoom[0].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.laundryRoom[0].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			whiteJericho.gameObject.SetActive(value: false);
			tinaDialogueSource.PlayClip(tweakables.dialogue.laundryRoom[1].dialogueClip);
			yield return washingMachine.Vibrate(tweakables.dialogue.laundryRoom[1].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			jerichoSock.SetActive(value: true);
			jericho.ungrabbable = false;
			washingMachine.GetComponent<DollhouseObject>().ungrabbable = false;
			redJericho.gameObject.SetActive(value: true);
			tinaDialogueSource.PlayClip(tweakables.dialogue.laundryRoom[2].dialogueClip);
			yield return tinaDoll.Talk(tweakables.dialogue.laundryRoom[2].dialogueClip.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			yield return EndLevel(TinaEndState.Best);
			yield return null;
		}
	}

	public void ReactPlayerGrabbedTinasDoll()
	{
		if (currentState <= TinaState.SelectCharacter || ending || reactThread != null)
		{
			GlobalCoroutineContainer.AddThread(DropTinaOnDelayCoroutine(0.35f), this);
		}
		else if (!tinasDollGrabbedRecently)
		{
			tinasDollGrabbedRecently = true;
			reactThread = GlobalCoroutineContainer.AddThread(ReactPlayerGrabbedTinasDollCoroutine(), this);
			GlobalCoroutineContainer.AddThread(ResetGrabTinaDollReactionCoroutine(1f), this);
		}
	}

	private IEnumerator DropTinaOnDelayCoroutine(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		tinaDoll.ForceDropByPlayer();
	}

	private IEnumerator ReactPlayerGrabbedTinasDollCoroutine()
	{
		tinaDoll.ungrabbable = true;
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (grabbedTinasDollIndex < tweakables.dialogue.grabTinaDollReactions.Length - 1)
		{
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.grabTinaDollReactions[grabbedTinasDollIndex]);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
			tinaDoll.ForceDropByPlayer();
			grabbedTinasDollIndex++;
			yield return exitInterruption(hold, wasTinaTalking);
		}
		else
		{
			EndLevel(TinaEndState.Dog);
			reactThread = null;
		}
	}

	private IEnumerator ResetGrabTinaDollReactionCoroutine(float time)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(time, this);
		tinasDollGrabbedRecently = false;
	}

	private void PlayerTeleportReaction(Vector3 startpos, Vector3 endpos)
	{
	}

	private IEnumerator PlayerTeleportReactionCoroutine(Vector3 startpos, Vector3 endpos)
	{
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (currentState < TinaState.PlayTime)
		{
			if (tinaDialogueSource.GetClipQueue().Count > 0)
			{
				float playbackTime = tinaDialogueSource.GetClipQueue()[0].progressTime;
				hold = tinaDialogueSource.ForcePlayClipInterrupt(tweakables.dialogue.teleportFarAway);
				tina.StopAllAnimations();
				while (tinaDialogueSource.GetClipQueue()[0].mClipData == hold.mClipData)
				{
					yield return null;
				}
				if (tinaDialogueSource.GetClipQueue().Count > 0)
				{
					tinaDialogueSource.mAudioSource.clip = tinaDialogueSource.GetClipQueue()[0].mClipData.clip;
					tinaDialogueSource.mAudioSource.time = playbackTime;
					tinaDialogueSource.mAudioSource.Play();
				}
				tina.ResumeAllAnimations();
				reactThread = null;
				playerHasTeleported = true;
			}
			else
			{
				tinaDialogueSource.ForcePlayClipInterrupt(tweakables.dialogue.teleportFarAway);
			}
			reactThread = null;
			yield break;
		}
		if (!playerHasTeleported)
		{
			if (Vector3.Distance(endpos, table.position) > 7.5f)
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.teleportFarAway);
				playerHasTeleported = true;
			}
			yield return exitInterruption(hold, wasTinaTalking);
		}
		yield return null;
		reactThread = null;
	}

	public SafeCoroutine ReactToPlayerDollInrRoom(DollhouseRoom room)
	{
		if (currentState <= TinaState.SelectCharacter || ending || reactThread != null || roomsPlayerVisited.Contains(room) || diningRoomSequenceStarted || !playerDoll.isGrabbed)
		{
			return null;
		}
		return reactThread = GlobalCoroutineContainer.AddThread(ReactToPlayerDollInRoomCoroutine(room, 1.5f), this);
	}

	private IEnumerator ReactToPlayerDollInRoomCoroutine(DollhouseRoom room, float time)
	{
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(time, this);
		if (playerDoll.currentRoom == room && tinaDoll.currentRoom != room)
		{
			DialoguePlaybackInstance hold = null;
			switch (room)
			{
			case DollhouseRoom.Kitchen:
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.playerInKitchen[0]);
				roomsPlayerVisited.Add(room);
				yield return exitInterruption(hold, wasTinaTalking);
				break;
			case DollhouseRoom.Bathroom:
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.playerInBathroom);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.playerInBathroom.duration);
				roomsPlayerVisited.Add(room);
				navSyncer.ResumeTinaDollPair();
				reactThread = null;
				break;
			case DollhouseRoom.Foyer:
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.playerInFoyer);
				roomsPlayerVisited.Add(room);
				yield return exitInterruption(hold, wasTinaTalking);
				break;
			case DollhouseRoom.DiningRoom:
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.playerInDiningroom);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.playerInDiningroom.duration);
				roomsPlayerVisited.Add(room);
				navSyncer.ResumeTinaDollPair();
				reactThread = null;
				break;
			case DollhouseRoom.Attic:
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.playerInAttic);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.playerInAttic.duration);
				roomsPlayerVisited.Add(room);
				navSyncer.ResumeTinaDollPair();
				reactThread = null;
				break;
			case DollhouseRoom.Office:
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.playerInoffice);
				roomsPlayerVisited.Add(room);
				yield return exitInterruption(hold, wasTinaTalking);
				break;
			default:
				reactThread = null;
				break;
			}
		}
		else
		{
			yield return null;
			reactThread = null;
		}
	}

	public void ReactToVacuum()
	{
		EndLevel(TinaEndState.Vacuum);
	}

	private IEnumerator ReactToVacuumCoroutine()
	{
		bool wasTinaTalking = true;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (tinaEatTrigger == null)
		{
			EndLevel(TinaEndState.Vacuum);
			reactThread = null;
			yield break;
		}
		if (vacuumIndex >= tweakables.dialogue.vacuumTina.Length - 1)
		{
			EndLevel(TinaEndState.Vacuum);
			reactThread = null;
			yield break;
		}
		yield return SetDarkEnvironment();
		characterSelectTimer = 0f;
		hold = InterruptTinaWithClip(tweakables.dialogue.vacuumTina[vacuumIndex], vacuum: true);
		yield return exitInterruption(hold, wasTinaTalking, vacuum: true);
		characterSelectTimer = 0f;
		if (vacuumIndex == 0)
		{
			tinaEatTrigger.unconsumable = false;
		}
		vacuumIndex++;
		reactThread = null;
	}

	public SafeCoroutine ReactDollGrabbed(NpcDollName name, bool secondReaction)
	{
		if (currentState < TinaState.SelectCharacter || ending || reactThread != null || dollGrabbedReacted.Contains(name))
		{
			return null;
		}
		if (diningRoomSequenceStarted)
		{
			return null;
		}
		return reactThread = GlobalCoroutineContainer.AddThread(ReactDollGrabbedCoroutine(name, secondReaction), this);
	}

	private IEnumerator ReactDollGrabbedCoroutine(NpcDollName name, bool secondReaction)
	{
		DialoguePlaybackInstance hold = null;
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		switch (name)
		{
		case NpcDollName.Mom:
			if (!secondReaction)
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.grabbedMom[0]);
			}
			else if (!dollGrabbedReacted.Contains(name))
			{
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.hiMom);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.hiMom.duration);
				navSyncer.ResumeTinaDollPair();
				yield return null;
				dollGrabbedReacted.Add(name);
				reactThread = null;
				yield break;
			}
			break;
		case NpcDollName.Jericho:
			if (!secondReaction)
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.thatsJericho[0]);
			}
			else if (!dollGrabbedReacted.Contains(name))
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.jerichoThisIsMyfriend);
				dollGrabbedReacted.Add(name);
			}
			break;
		case NpcDollName.Dad:
			if (!secondReaction)
			{
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.dad);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.dad.duration);
				navSyncer.ResumeTinaDollPair();
				yield return null;
				reactThread = null;
				yield break;
			}
			if (!dollGrabbedReacted.Contains(name))
			{
				navSyncer.PauseTinaDollPair();
				hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.goodEveningDad);
				yield return tinaDoll.TalkInterrupt(tweakables.dialogue.goodEveningDad.duration);
				navSyncer.ResumeTinaDollPair();
				yield return null;
				dollGrabbedReacted.Add(name);
				reactThread = null;
				yield break;
			}
			break;
		case NpcDollName.Bunny:
			if (!secondReaction)
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.thatsDonald);
			}
			else if (!dollGrabbedReacted.Contains(name))
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.donaldGetBack);
				dollGrabbedReacted.Add(name);
			}
			break;
		}
		yield return exitInterruption(hold, wasTinaTalking);
		reactThread = null;
	}

	public SafeCoroutine PlayFurnitureInterruption(FurnitureType type)
	{
		if ((currentState == TinaState.Intro || currentState == TinaState.SelectCharacter) && hasInterruptedDuringStart)
		{
			return null;
		}
		if (ending || reactThread != null)
		{
			return null;
		}
		if (type == FurnitureType.generic && genericFurnitureResponseIndex >= 2)
		{
			return null;
		}
		return reactThread = GlobalCoroutineContainer.AddThread(PlayFurnitureInterruptionCoroutine(type), this);
	}

	private IEnumerator PlayFurnitureInterruptionCoroutine(FurnitureType type)
	{
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (tinaDoll != null)
		{
			tinaDoll.tinaInterrupted = true;
		}
		if (currentState == TinaState.SelectCharacter || currentState == TinaState.Intro)
		{
			hasInterruptedDuringStart = true;
			if (tinaDialogueSource.GetClipQueue().Count > 0)
			{
				float playbackTime = tinaDialogueSource.GetClipQueue()[0].progressTime;
				hold = tinaDialogueSource.ForcePlayClipInterrupt(tweakables.dialogue.genericFurnitureResponses[Random.Range(0, tweakables.dialogue.genericFurnitureResponses.Length - 1)]);
				tina.StopAllAnimations();
				while (tinaDialogueSource.GetClipQueue()[0].mClipData == hold.mClipData)
				{
					yield return null;
				}
				if (tinaDialogueSource.GetClipQueue().Count > 0)
				{
					tinaDialogueSource.mAudioSource.clip = tinaDialogueSource.GetClipQueue()[0].mClipData.clip;
					tinaDialogueSource.mAudioSource.time = playbackTime;
					tinaDialogueSource.mAudioSource.Play();
				}
				tina.ResumeAllAnimations();
				reactThread = null;
			}
			else
			{
				hold = tinaDialogueSource.ForcePlayClipInterrupt(tweakables.dialogue.genericFurnitureResponses[Random.Range(0, tweakables.dialogue.genericFurnitureResponses.Length - 1)]);
				while (tinaDialogueSource.isPlaying)
				{
					yield return null;
				}
				reactThread = null;
			}
			yield break;
		}
		switch (type)
		{
		case FurnitureType.toilet:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.toliet);
			break;
		case FurnitureType.tub:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.tub);
			break;
		case FurnitureType.bed:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.bed);
			break;
		case FurnitureType.plant:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.plant);
			break;
		case FurnitureType.foyerTable:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.foyerTable);
			break;
		case FurnitureType.toaster:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.toaster);
			break;
		case FurnitureType.fridge:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.fridge);
			break;
		case FurnitureType.lamp:
			yield return SetDarkEnvironment();
			hold = InterruptTinaWithClip(tweakables.dialogue.lamp);
			break;
		case FurnitureType.generic:
			if (genericFurnitureResponseIndex < 2)
			{
				yield return SetDarkEnvironment();
				hold = InterruptTinaWithClip(tweakables.dialogue.genericFurnitureResponses[genericFurnitureResponseIndex]);
				genericFurnitureResponseIndex++;
			}
			break;
		}
		yield return exitInterruption(hold, wasTinaTalking);
		reactThread = null;
	}

	public SafeCoroutine TvInterruption()
	{
		if (reactThread != null)
		{
			SafeDebug.LogError("React should absolutely not exist right now");
			return null;
		}
		return reactThread = GlobalCoroutineContainer.AddThread(TvInterruptionCoroutine(), this);
	}

	private IEnumerator TvInterruptionCoroutine()
	{
		tv.gameObject.SetActive(value: true);
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (tinaDoll != null)
		{
			tinaDoll.tinaInterrupted = true;
		}
		yield return SetDarkEnvironment();
		tv.StartGlitchAnimation(tweakables.dialogue.livingRoom[1].dialogueClip.duration * 0.85f);
		hold = InterruptTinaWithClip(tweakables.dialogue.livingRoom[1].dialogueClip);
		yield return exitInterruption(hold, wasTinaTalking);
		yield return null;
		tv.gameObject.SetActive(value: false);
	}

	public SafeCoroutine SeatingHeirarchiesInterruption()
	{
		if (reactThread != null)
		{
			SafeDebug.LogError("React should absolutely not exist right now");
			return null;
		}
		return reactThread = GlobalCoroutineContainer.AddThread(SeatingHeirarchiesInterruptionCoroutine(), this);
	}

	private IEnumerator SeatingHeirarchiesInterruptionCoroutine()
	{
		bool wasTinaTalking = false;
		if (tinaDialogueSource.isPlaying)
		{
			wasTinaTalking = true;
		}
		DialoguePlaybackInstance hold = null;
		if (tinaDoll != null)
		{
			tinaDoll.tinaInterrupted = true;
		}
		yield return SetDarkEnvironment();
		hold = InterruptTinaWithClip(tweakables.dialogue.diningRoom[1].dialogueClip);
		yield return exitInterruption(hold, wasTinaTalking);
		yield return null;
		reactThread = null;
	}

	private SafeCoroutine exitInterruption(DialoguePlaybackInstance hold, bool tinaTalking, bool vacuum = false)
	{
		return GlobalCoroutineContainer.AddThread(exitInterruptionCoroutine(hold, tinaTalking, vacuum), this);
	}

	private IEnumerator exitInterruptionCoroutine(DialoguePlaybackInstance hold, bool tinaTalking, bool vacuum = false)
	{
		while (hold != null && tinaDialogueSource.GetClipQueue().Count > 0 && tinaDialogueSource.GetClipQueue()[0].mClipData == hold.mClipData)
		{
			characterSelectTimer = 0f;
			yield return null;
		}
		tinaDialogueSource.Pause();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		if (!lightsShouldBeOff)
		{
			yield return SetLightEnvironment();
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		if (hold != null && tinaTalking && !ending)
		{
			hold = tinaDialogueSource.InterruptWithClip(tweakables.dialogue.returnFromInterruption[returnFromInterruptIndex], enqueueIfCantInterrupt: false, stackInterruptions: true, forceNoRecordScratch: false, ignoreCustomInterrupts: true);
			returnFromInterruptIndex++;
			if (returnFromInterruptIndex > tweakables.dialogue.returnFromInterruption.Length - 1)
			{
				returnFromInterruptIndex = 0;
			}
		}
		if (vacuum)
		{
			tina.StopVacuumAnimation();
		}
		else
		{
			tina.StopInterruptionAnimation();
		}
		tinaDialogueSource.UnPause();
		while (hold != null && tinaDialogueSource.GetClipQueue().Count > 0 && tinaDialogueSource.GetClipQueue()[0].mClipData == hold.mClipData)
		{
			yield return null;
		}
		if (tinaDoll != null)
		{
			tinaDoll.ungrabbable = false;
			tinaDoll.tinaInterrupted = false;
			tinaDoll.antennaShakerRoot.gameObject.SetActive(value: true);
		}
		navSyncer.ResumeTinaDollPair();
		tinaDialogueSource.UnPause();
		reactThread = null;
	}

	private DialoguePlaybackInstance InterruptTinaWithClip(NPCDialogueClip clip, bool vacuum = false)
	{
		characterSelectTimer = 0f;
		if (tinaDoll != null)
		{
			tinaDoll.tinaInterrupted = true;
		}
		navSyncer.PauseTinaDollPair();
		if (vacuum)
		{
			tina.StartVacuumAnimation();
		}
		else
		{
			tina.StartInterruptionAnimation();
		}
		tinaDialogueSource.transform.SetParent(tina.mMouth);
		tinaDialogueSource.transform.localPosition = Vector3.zero;
		if (tinaDoll != null)
		{
			tinaDoll.antennaShakerRoot.gameObject.SetActive(value: false);
		}
		return tinaDialogueSource.ForcePlayClipInterrupt(clip);
	}

	public SafeCoroutine SetDarkEnvironment()
	{
		return GlobalCoroutineContainer.AddThread(SetDarkEnvironmentCoroutine(), this);
	}

	private IEnumerator SetDarkEnvironmentCoroutine()
	{
		if (!isDarkEnvironment)
		{
			smallLightSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
			lightProps.gameObject.SetActive(value: false);
			interruptProps.gameObject.SetActive(value: true);
			mEagerScene.mEnvironmentProperties = interruptProps;
			mEagerScene.mEnvironmentProperties.Activate();
			mEagerScene.mEnvironmentProperties.pushAllProperties();
			music.Pause();
			isDarkEnvironment = true;
		}
	}

	public SafeCoroutine SetLightEnvironment(bool bigSound = false)
	{
		return GlobalCoroutineContainer.AddThread(SetLightEnvironmentCoroutine(bigSound), this);
	}

	private IEnumerator SetLightEnvironmentCoroutine(bool bigSound = false)
	{
		if (isDarkEnvironment)
		{
			if (bigSound)
			{
				bigLightSound.Play();
			}
			else
			{
				smallLightSound.Play();
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
			lightProps.gameObject.SetActive(value: true);
			interruptProps.gameObject.SetActive(value: false);
			mEagerScene.mEnvironmentProperties = lightProps;
			mEagerScene.mEnvironmentProperties.Activate();
			mEagerScene.mEnvironmentProperties.pushAllProperties();
			music.UnPause();
			isDarkEnvironment = false;
		}
	}

	private void init()
	{
	}

	public SafeCoroutine SetCharacterSelected(bool s, DollhouseDoll d)
	{
		if (selectCharacterThread == null)
		{
			return selectCharacterThread = GlobalCoroutineContainer.AddThread(SetCharacterSelectedCoroutine(s, d), this);
		}
		return null;
	}

	private IEnumerator SetCharacterSelectedCoroutine(bool s, DollhouseDoll d)
	{
		characterSelectTimer = 0f;
		if (!firstCharacterSelected)
		{
			d.rb.isKinematic = true;
			firstCharacterSelected = true;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.1f, this);
			d.ForceDropByPlayer();
			d.ungrabbable = true;
			cindy = d;
			tinaDialogueSource.PlayClip(tweakables.dialogue.notCindy);
			DollDoor hold = null;
			for (int i = 0; i < dollDoors.Length; i++)
			{
				if (dollDoors[i].GetDoll() == d)
				{
					hold = dollDoors[i];
				}
			}
			if (hold != null)
			{
				hold.LowerDoll(0.4f);
			}
			selectCharacterThread = null;
			yield break;
		}
		playerDoll = d;
		playerDoll.enabled = true;
		playerDoll.sequencePath = playerSequencePath;
		characterSelected = true;
		playerDoll.SetState(DollhouseObjectState.inactive);
		DollhouseDoll hold2 = null;
		for (int j = 0; j < playerDolls.Length; j++)
		{
			if (playerDolls[j].gameObject.activeInHierarchy && playerDolls[j] != playerDoll && playerDolls[j] != cindy)
			{
				hold2 = playerDolls[j];
			}
		}
		tina.StartGrabRemote();
		if (hold2.isGrabbed)
		{
			hold2.ForceDropByPlayer();
		}
		hold2.antennaRoot.SetActive(value: true);
		hold2.ungrabbable = true;
		popSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3.5f, this);
		tinaDoll = hold2;
		tinaDoll.rb.isKinematic = false;
		tinaDoll.isTinaDoll = true;
		tinaDoll.sequencePath = tinaSequencePath;
		tinaDoll.ungrabbable = false;
		navSyncer.SetTinaDoll(tinaDoll);
		navSyncer.SetActivePlayerDollPairAndStartSequence(playerDoll);
		selectCharacterThread = null;
	}

	private DollhouseSide getClosestPlayerSide()
	{
		float num = Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, tableEnds[0].position);
		float num2 = Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, tableEnds[1].position);
		if (num < num2)
		{
			return DollhouseSide.front;
		}
		return DollhouseSide.back;
	}

	public bool IsOnTable(Vector3 pos, GameObject debug)
	{
		if (ended)
		{
			return false;
		}
		Vector3 a = pos.Flatten();
		Vector3 b = table.position.Flatten();
		if ((double)Vector3.Distance(a, b) < 0.769)
		{
			return true;
		}
		return false;
	}

	public TinaController GetTina()
	{
		return null;
	}

	public AudioSourceLoudness GetTinaTalker()
	{
		return tinaTalker;
	}

	private void tryFlip(bool force = true)
	{
		Transform transform = EagerMaster.instance.mainCamera.transform;
		if (transform.position.z > dollhouseRoot.position.z)
		{
			if (frontFacing)
			{
				spinner.FlipTable();
				frontFacing = !frontFacing;
			}
			else if (force)
			{
				spinner.FlipTable();
				frontFacing = !frontFacing;
			}
		}
		else if (!frontFacing)
		{
			spinner.FlipTable();
			frontFacing = !frontFacing;
		}
		else if (force)
		{
			spinner.FlipTable();
			frontFacing = !frontFacing;
		}
	}

	private void sendNPCDollsToDiningRoom()
	{
		for (int i = 0; i < diningRoomNavRefs.Length; i++)
		{
			diningRoomNavRefs[i].GetCutter().carving = false;
			diningRoomNavRefs[i].GetCutter().enabled = false;
			diningRoomNavRefs[i].neverUseCarver = true;
		}
		diningRoomSequenceStarted = true;
		navSyncer.diningRoomTime = true;
	}

	private void startDonaldCharge()
	{
		navSyncer.AddAgentToNoPathList(donaldNavRef);
		donaldNavRef.diningroomTarget = donaldChargePosition;
	}

	private void stopDonaldCharge()
	{
		navSyncer.RemoveAgentFromNoPathList(donaldNavRef);
		donaldNavRef.diningroomTarget = startDonaldHome;
		donaldNavRef.SetHomeTarget(startDonaldHome);
		navSyncer.diningRoomTime = false;
	}

	public SafeCoroutine EndLevel(TinaEndState e)
	{
		if (ending)
		{
			return null;
		}
		return GlobalCoroutineContainer.AddThread(EndSequenceCoroutine(e), this);
	}

	private IEnumerator EndSequenceCoroutine(TinaEndState e)
	{
		tinaDialogueSource.transform.SetParent(tina.mMouth);
		tinaDialogueSource.PurgeAllDialogue();
		if (mainThread.IsRunning)
		{
			mainThread.Pause();
			mainThread.Stop();
			mainThread = null;
		}
		if (navSyncer.tinaMovementThread != null && navSyncer.tinaMovementThread.IsRunning)
		{
			navSyncer.tinaMovementThread.Stop();
			if (navSyncer.SafetyThread != null && navSyncer.SafetyThread.IsRunning)
			{
				navSyncer.SafetyThread.Stop();
			}
		}
		windLoop.Stop();
		music.Stop();
		tinaDialogueSource.PurgeAllDialogue();
		ending = true;
		switch (e)
		{
		case TinaEndState.Dog:
		{
			tinaDialogueSource.ForcePlayClipInterrupt(tweakables.dialogue.grabTinaDollReactions[grabbedTinasDollIndex]);
			if (navSyncer.tinaMovementThread != null && navSyncer.tinaMovementThread.IsRunning)
			{
				navSyncer.tinaMovementThread.Stop();
			}
			DollhouseDoll oldTinaDoll = tinaDoll;
			tinaDoll.enabled = false;
			tryFlip(force: false);
			yield return dollDoors[0].RaiseDawkens(0.7f, out tinaDoll);
			tinaDoll.isTinaDoll = true;
			popSound.Play();
			oldTinaDoll.antennaRoot.SetActive(value: false);
			oldTinaDoll.rb.isKinematic = false;
			oldTinaDoll.ungrabbable = false;
			tinaDoll.antennaRoot.SetActive(value: true);
			tinaDoll.enabled = false;
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			tinaDialogueSource.PlayClip(tweakables.dialogue.woff);
			yield return navSyncer.DawkensSequence(tweakables.dialogue.woff.duration);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			tinaDialogueSource.PlayClip(tweakables.dialogue.alrightNext);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tina, badReview.mReview);
			CompleteJob();
			break;
		}
		case TinaEndState.Best:
			if (VVR_HUB.instance != null)
			{
				VVR_HUB.instance.tinaGoodEnding = true;
			}
			popSound.Play();
			tinaRemote.SetActive(value: false);
			tina.StartEndSequence();
			tinaDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.sequenceEnd.dialogueClip);
			yield return null;
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tina, goodReview.mReview);
			CompleteJob();
			break;
		case TinaEndState.Vacuum:
		{
			theDollHouse.BecomeConsumable();
			tinaDialogueSource.PurgeAllDialogue();
			tinaDialogueSource.PitchDownPause(0.1f);
			tina.StopAllAnimations();
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tina, vacuumReview.mReview);
			tina.enabled = false;
			navSyncer.ShutItDown();
			for (int i = 0; i < tinaConsumables.Length; i++)
			{
				tinaConsumables[i].unconsumable = false;
				tinaConsumables[i].destroyOnConsume = true;
				yield return null;
				tinaConsumables[i].OneShotTransfer(ToolManager.BestBlackHoleConsumer(tinaConsumables[i].transform.position), -15f);
			}
			for (int j = 0; j < objectsToBecomeConsumable.Length; j++)
			{
				objectsToBecomeConsumable[j].unconsumable = false;
			}
			for (int k = 0; k < objectsToDestroyOnConsume.Length; k++)
			{
				objectsToDestroyOnConsume[k].unconsumable = false;
				objectsToDestroyOnConsume[k].destroyOnConsume = true;
				if (objectsToDestroyOnConsume[k].currentObjectState == VVRConsumable.objectState.fullyConsumedStatic || objectsToDestroyOnConsume[k].currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer)
				{
					Object.Destroy(objectsToDestroyOnConsume[k].gameObject);
				}
			}
			for (int l = 0; l < objectsToDestroy.Length; l++)
			{
				Object.Destroy(objectsToDestroy[l]);
			}
			ended = true;
			GoToBackstage();
			break;
		}
		case TinaEndState.Hit:
			tinaDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hitTina[4]);
			while (tinaDialogueSource.isPlaying)
			{
				yield return null;
			}
			GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tina, badReview.mReview);
			CompleteJob();
			break;
		}
		yield return null;
	}
}
