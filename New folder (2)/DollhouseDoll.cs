using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class DollhouseDoll : DollhouseObject
{
	[SerializeField]
	private NpcDollName mDollName = NpcDollName.None;

	[Header("Main Sequence")]
	public DollMainSequencePath sequencePath;

	[Header("Misc")]
	public bool playerDoll;

	internal PlayerDollState mainSequenceState = PlayerDollState.toFront;

	public bool isTinaDoll;

	private bool hasGrabReacted;

	public DollhouseRoom currentRoom;

	internal bool doingMainSequence;

	private PlayerDollState lastMainsequenceState;

	[Header("Tina only stuff")]
	public GameObject meshRoot;

	public GameObject antennaRoot;

	public TransformShaker antennaShakerRoot;

	internal SafeCoroutine mainSequenceTalkThread;

	private bool tinaTalking;

	internal bool tinaInterrupted;

	[SerializeField]
	private LayerMask groundLayerMask;

	public override void OnDrop()
	{
		base.OnDrop();
		if (isTinaDoll)
		{
			if (sceneRef.navSyncer.tinaMovementThread != null && sceneRef.navSyncer.tinaMovementThread.IsPaused)
			{
				sceneRef.navSyncer.tinaMovementThread.Resume();
				sceneRef.navSyncer.SafetyThread.Resume();
			}
		}
		else if (currentState != DollhouseObjectState.onGround)
		{
			if (currentState == DollhouseObjectState.inactive && !sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetState(DollhouseObjectState.onGround);
			}
			else if (doingMainSequence && currentState != DollhouseObjectState.onGround)
			{
				SetStateOnDelay(DollhouseObjectState.gettingUp, 1.5f);
			}
		}
	}

	protected override void FixedUpdate()
	{
		if (!isTinaDoll && !playerDoll && !isGrabbed)
		{
			base.FixedUpdate();
		}
		if (playerDoll && !isTinaDoll)
		{
			if (!isGrabbed)
			{
				base.FixedUpdate();
			}
		}
		else if (!isTinaDoll)
		{
		}
	}

	protected override void processState()
	{
		if (isGrabbed)
		{
			SafeDebug.LogWarning("This should never happen outside of debug states. Check DollhouseDoll fixed update");
			return;
		}
		if (currentState == DollhouseObjectState.onGround || currentState == DollhouseObjectState.jumpToTable || isGrabbed)
		{
			if (transform.parent != sceneRef.holdRoot)
			{
				transform.SetParent(sceneRef.holdRoot, worldPositionStays: true);
			}
		}
		else if (transform.parent != startParent)
		{
			transform.SetParent(startParent, worldPositionStays: true);
		}
		switch (currentState)
		{
		case DollhouseObjectState.home:
			if (!playerDoll)
			{
				if (transitionT <= 1f)
				{
					transform.position = Vector3.Lerp(holdPos, homeTransform.position, transitionT);
					transform.rotation = Quaternion.Lerp(holdRot, homeTransform.rotation, transitionT);
					transitionT += Time.deltaTime * 3f;
				}
				if (Vector3.Distance(transform.position, homeTransform.position) > 0.5f && !isGrabbed)
				{
					SetState(DollhouseObjectState.gettingUp);
				}
			}
			break;
		case DollhouseObjectState.onTable:
			if (!sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetState(DollhouseObjectState.onGround);
			}
			else if (!playerDoll)
			{
				if ((double)Vector3.Distance(transform.position, homeTransform.position) < 0.1)
				{
					SetState(DollhouseObjectState.home);
					rb.isKinematic = false;
				}
			}
			else if (sceneRef.currentState >= TinaState.PlayTime)
			{
				if (ShouldNavSync())
				{
					SetState(DollhouseObjectState.scriptedSequence);
				}
			}
			else if ((double)Vector3.Distance(transform.position, homeTransform.position) < 0.1)
			{
				SetState(DollhouseObjectState.home);
				rb.isKinematic = false;
			}
			break;
		case DollhouseObjectState.onGround:
			if (sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
			}
			else if (sceneRef != null && sceneRef.table != null && rb.velocity.sqrMagnitude < 0.05f)
			{
				target.position = sceneRef.table.position;
				processHop();
			}
			break;
		case DollhouseObjectState.jumpToTable:
			if (sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
				break;
			}
			mRocket.SetRocketDisabled(s: true);
			if (rb.velocity.sqrMagnitude < 0.005f)
			{
				hopTimer += Time.deltaTime;
			}
			if (hopTimer > 1f)
			{
				if (firstTableJump)
				{
					hopTimer = 0.8f;
				}
				else
				{
					hopTimer = 0f;
				}
				tryLeap();
			}
			break;
		case DollhouseObjectState.gettingUp:
			if (isTinaDoll)
			{
				break;
			}
			if (!sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetState(DollhouseObjectState.onGround);
				break;
			}
			mRocket.SetRocketDisabled(s: false);
			mRocket.SetTarget(target);
			getUpTimer += Time.deltaTime;
			if ((double)rb.velocity.sqrMagnitude < 0.005 && getUpTimer > 1f)
			{
				mRocket.SetMaxForce(startRocketForce);
				SetState(DollhouseObjectState.onTable);
			}
			break;
		case DollhouseObjectState.inactive:
			if (!sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetState(DollhouseObjectState.onGround);
			}
			else if (!settingStateOnDelay && !isGrabbed && sceneRef.playerDoll != null)
			{
				SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
			}
			else if (doingMainSequence && !isGrabbed && !settingStateOnDelay)
			{
				SetState(DollhouseObjectState.scriptedSequence);
			}
			break;
		case DollhouseObjectState.scriptedSequence:
			if (!sceneRef.IsOnTable(transform.position, gameObject))
			{
				SetState(DollhouseObjectState.onGround);
			}
			else if (playerDoll)
			{
				if (mainSequenceState != lastMainsequenceState)
				{
					sceneRef.navSyncer.MovePlayerDollToSequencePosition(mainSequenceState);
					lastMainsequenceState = mainSequenceState;
				}
			}
			else if ((double)Vector3.Distance(transform.position, homeTransform.position) < 0.1)
			{
				SetState(DollhouseObjectState.home);
				rb.isKinematic = false;
			}
			break;
		case DollhouseObjectState.inDollhouse:
		case DollhouseObjectState.toHome:
		case DollhouseObjectState.travelSequence:
			break;
		}
	}

	public void HopInPlace()
	{
		distanceMod = 3.4f;
		dirToTarget = Vector3.up;
		rb.AddForce(Vector3.up * ((float)Random.Range(100, 150) * Mathf.Clamp(rb.mass, 0.5f, 1f)) * launchPower + dirToTarget * 15f * launchPower * distanceMod);
		GlobalCoroutineContainer.AddThread(StraightenOutCoroutine(1f), this);
	}

	private IEnumerator StraightenOutCoroutine(float t)
	{
		Quaternion hold = transform.rotation;
		float timer = 0f;
		while (timer <= 1f)
		{
			timer += Time.deltaTime / t;
			transform.rotation = Quaternion.Slerp(hold, Quaternion.identity, t);
			yield return null;
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (isTinaDoll)
		{
			rb.isKinematic = true;
		}
		if (sceneRef.playerDoll == null && playerDoll)
		{
			sceneRef.SetCharacterSelected(s: true, this);
		}
		if (isTinaDoll)
		{
			sceneRef.ReactPlayerGrabbedTinasDoll();
		}
		if (mDollName != NpcDollName.None)
		{
			SetState(DollhouseObjectState.inactive);
		}
		if (playerDoll && sceneRef.navSyncer.playerDollThread != null && sceneRef.navSyncer.playerDollThread.IsRunning)
		{
			sceneRef.navSyncer.playerDollThread.Stop();
			sceneRef.navSyncer.playerDollThread = null;
		}
		if (sceneRef.currentState <= TinaState.SelectCharacter)
		{
			return;
		}
		bool secondReaction = hasGrabReacted;
		if (mDollName == NpcDollName.Jericho)
		{
			if (sceneRef.reactThread == null)
			{
				hasGrabReacted = true;
			}
			sceneRef.ReactDollGrabbed(mDollName, secondReaction);
		}
		if (mDollName == NpcDollName.Bunny)
		{
			if (sceneRef.reactThread == null)
			{
				hasGrabReacted = true;
			}
			sceneRef.ReactDollGrabbed(mDollName, secondReaction);
		}
		if (mDollName == NpcDollName.Dad)
		{
			if (sceneRef.reactThread == null)
			{
				hasGrabReacted = true;
			}
			sceneRef.ReactDollGrabbed(mDollName, secondReaction);
		}
		if (mDollName == NpcDollName.Mom)
		{
			if (sceneRef.reactThread == null)
			{
				hasGrabReacted = true;
			}
			sceneRef.ReactDollGrabbed(mDollName, secondReaction);
		}
	}

	protected override void init()
	{
		base.init();
		if (mRocket != null)
		{
			mRocket.SetRocketDisabled(s: true);
		}
	}

	public NpcDollName GetName()
	{
		return mDollName;
	}

	public SafeCoroutine TalkInterrupt(float duration)
	{
		return GlobalCoroutineContainer.AddThread(TalkInterruptCoroutine(duration), this);
	}

	private IEnumerator TalkInterruptCoroutine(float duration)
	{
		if (mainSequenceTalkThread != null && mainSequenceTalkThread.IsRunning)
		{
			mainSequenceTalkThread.Pause();
		}
		sceneRef.tinaDialogueSource.transform.SetParent(transform);
		sceneRef.tinaDialogueSource.transform.localPosition = Vector3.zero;
		tinaTalking = true;
		float timer = 0f;
		Quaternion holdRot = transform.rotation;
		Quaternion holdRot2 = transform.rotation;
		talkT = 0f;
		talkDir = 1;
		bobT = 0f;
		while (timer <= 1f)
		{
			while (tinaInterrupted)
			{
				yield return null;
			}
			timer += Time.deltaTime / duration;
			stepTalkT();
			Quaternion talkRot = Quaternion.AngleAxis(Mathf.Clamp((float)(200 * talkDir) * sceneRef.GetTinaTalker().lerpLoudness, -20f, 20f), transform.forward);
			transform.rotation = Quaternion.Slerp(holdRot, talkRot * holdRot, talkT);
			holdRot2 = transform.rotation;
			yield return null;
		}
		timer = 0f;
		Quaternion postTalkRot = transform.rotation;
		while (timer <= 1f)
		{
			timer += Time.deltaTime * 8f;
			transform.rotation = Quaternion.Slerp(postTalkRot, holdRot, timer);
			yield return null;
		}
		tinaTalking = false;
		if (mainSequenceTalkThread != null && mainSequenceTalkThread.IsPaused)
		{
			mainSequenceTalkThread.Resume();
		}
	}

	public SafeCoroutine Talk(float duration)
	{
		return mainSequenceTalkThread = GlobalCoroutineContainer.AddThread(TalkCoroutine(duration), this);
	}

	private IEnumerator TalkCoroutine(float duration)
	{
		sceneRef.tinaDialogueSource.transform.SetParent(transform);
		sceneRef.tinaDialogueSource.transform.localPosition = Vector3.zero;
		tinaTalking = true;
		float timer = 0f;
		Quaternion holdRot = transform.rotation;
		Quaternion holdRot2 = transform.rotation;
		talkT = 0f;
		talkDir = 1;
		bobT = 0f;
		while (timer <= 1f)
		{
			while (tinaInterrupted)
			{
				yield return null;
			}
			timer += Time.deltaTime / duration;
			stepTalkT();
			Quaternion talkRot = Quaternion.AngleAxis(Mathf.Clamp((float)(200 * talkDir) * sceneRef.GetTinaTalker().lerpLoudness, -20f, 20f), transform.forward);
			transform.rotation = Quaternion.Slerp(holdRot, talkRot * holdRot, talkT);
			holdRot2 = transform.rotation;
			yield return null;
		}
		timer = 0f;
		Quaternion postTalkRot = transform.rotation;
		while (timer <= 1f)
		{
			timer += Time.deltaTime * 8f;
			transform.rotation = Quaternion.Slerp(postTalkRot, holdRot, timer);
			yield return null;
		}
		tinaTalking = false;
	}

	public SafeCoroutine TurnToFacePlayer(float time)
	{
		if (ShouldNavSync())
		{
			return GlobalCoroutineContainer.AddThread(TurnToFacePlayerCoroutine(time), this);
		}
		return null;
	}

	private IEnumerator TurnToFacePlayerCoroutine(float time)
	{
		float timer = 0f;
		Quaternion startRot = transform.rotation;
		Vector3 flattenedPlayerPos = EagerMaster.instance.mainCamera.transform.position.Flatten();
		Vector3 dirVec = flattenedPlayerPos - transform.position.Flatten();
		Quaternion endRot = Quaternion.LookRotation(dirVec, transform.up);
		while (timer < 1f)
		{
			timer += Time.deltaTime / time;
			transform.rotation = Quaternion.Slerp(startRot, endRot, timer);
			yield return null;
		}
	}

	public SafeCoroutine SinkToFloor(float duration)
	{
		if (ShouldNavSync())
		{
			return GlobalCoroutineContainer.AddThread(SinkToFloorCoroutine(duration), this);
		}
		return null;
	}

	private IEnumerator SinkToFloorCoroutine(float duration)
	{
		Vector3 floorPos = transform.position;
		Vector3 startPos = transform.position;
		if (Physics.Raycast(transform.position, -transform.up, out var hit, (int)groundLayerMask))
		{
			floorPos.y = hit.point.y;
			floorPos.y += 0.09f;
		}
		float timer = 0f;
		while (timer <= 1f)
		{
			timer += Time.deltaTime / duration;
			transform.position = Vector3.Lerp(startPos, floorPos, timer);
			yield return null;
		}
		yield return null;
	}

	public bool ShouldNavSync()
	{
		if ((currentState == DollhouseObjectState.onGround && currentState == DollhouseObjectState.jumpToTable && currentState == DollhouseObjectState.gettingUp && currentState == DollhouseObjectState.inactive) || isGrabbed)
		{
			return false;
		}
		return true;
	}

	public bool IsTalking()
	{
		if (tinaTalking && !tinaInterrupted)
		{
			return true;
		}
		return false;
	}
}
