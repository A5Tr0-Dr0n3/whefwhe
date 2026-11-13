using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;
using UnityEngine.AI;

public class DollhouseObjectSyncer : MonoBehaviour
{
	[Header("Object Pairs")]
	[SerializeField]
	private List<DollhouseObjectPair> furnitureObjects = new List<DollhouseObjectPair>();

	[SerializeField]
	private List<DollhouseDollPair> npcDollObjects = new List<DollhouseDollPair>();

	[SerializeField]
	private DollhouseDollPair tinaDollPair;

	[SerializeField]
	private DollhouseDollPair[] playerDollPairs;

	[SerializeField]
	private DollhouseDollPair dawkensPair;

	[Header("All Nav Objects")]
	[SerializeField]
	private NavMeshObstacle[] allCutters;

	[Header("Misc Scene References")]
	[SerializeField]
	private Transform holdRoot;

	[SerializeField]
	private Transform[] dawkensTargets;

	[SerializeField]
	private JobTina sceneRef;

	public Transform holdTransform;

	internal DollhouseDollPair activePlayerDollPair;

	public bool diningRoomTime;

	private bool playerDollChosen;

	private bool tinaDollChosen;

	internal SafeCoroutine playerDollThread;

	internal SafeCoroutine tinaMovementThread;

	private List<FurnitureNavigation> AgentsWithoutPath = new List<FurnitureNavigation>();

	internal bool cuttersEnabled = true;

	internal bool ignoreJericho;

	internal SafeCoroutine SafetyThread;

	private bool done;

	private void Update()
	{
		if (done)
		{
			enabled = false;
			return;
		}
		syncFurniture();
		syncNPCDolls();
		if (sceneRef.currentState > TinaState.SelectCharacter && tinaDollChosen)
		{
			syncTinaDoll();
		}
		if (playerDollChosen)
		{
			syncPlayerDolls();
		}
		SyncDawkens();
		handleCutters();
	}

	public void SetActivePlayerDollPairAndStartSequence(DollhouseDoll playerDoll)
	{
		DollhouseDollPair dollhouseDollPair = null;
		for (int i = 0; i < playerDollPairs.Length; i++)
		{
			if (playerDollPairs[i].realObject == playerDoll)
			{
				dollhouseDollPair = playerDollPairs[i];
			}
		}
		if (dollhouseDollPair == null)
		{
			SafeDebug.LogError("Trying to set active player doll pair to a pair that does not exist. this will likely break a lot of things");
		}
		else
		{
			activePlayerDollPair = dollhouseDollPair;
		}
		playerDollChosen = true;
		playerDoll.mainSequenceState = PlayerDollState.toFront;
		activePlayerDollPair.navMeshObject.SetMainSequenceTarget(playerDoll.sequencePath.outsideHousePos);
		activePlayerDollPair.navMeshObject.SetState(NavFurnitureState.MainSequence, activePlayerDollPair.realObject.transform.localPosition, activePlayerDollPair.realObject.transform.localRotation);
	}

	public void SetTinaDoll(DollhouseDoll tinaDoll)
	{
		DollhouseDollPair dollhouseDollPair = null;
		for (int i = 0; i < playerDollPairs.Length; i++)
		{
			if (playerDollPairs[i].realObject == tinaDoll)
			{
				dollhouseDollPair = playerDollPairs[i];
			}
		}
		if (dollhouseDollPair == null)
		{
			SafeDebug.LogError("Trying to set tina doll pair to a pair that does not exist. this will likely break a lot of things");
		}
		else
		{
			tinaDollPair = dollhouseDollPair;
		}
		tinaDollPair.navMeshObject.agent.speed += 0.5f;
		tinaDollChosen = true;
	}

	public void PauseTinaDollPair()
	{
		if (tinaMovementThread != null && tinaMovementThread.IsRunning)
		{
			tinaMovementThread.Pause();
			if (SafetyThread != null && SafetyThread.IsRunning)
			{
				SafetyThread.Pause();
			}
			tinaDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, tinaDollPair.navMeshObject.transform.localPosition, tinaDollPair.navMeshObject.transform.localRotation);
			tinaDollPair.realObject.SetState(DollhouseObjectState.inactive);
		}
	}

	public void ResumeTinaDollPair()
	{
		if (tinaMovementThread != null && tinaMovementThread.IsPaused)
		{
			tinaMovementThread.Resume();
			if (SafetyThread.IsPaused)
			{
				SafetyThread.Resume();
			}
			tinaDollPair.navMeshObject.SetState(NavFurnitureState.MainSequence, tinaDollPair.navMeshObject.transform.localPosition, tinaDollPair.navMeshObject.transform.localRotation);
			tinaDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
		}
	}

	public SafeCoroutine MoveTinaDollToSequencePosition(PlayerDollState state, float time)
	{
		tinaDollPair.realObject.mainSequenceState = state;
		if ((tinaDollPair.realObject.currentState != DollhouseObjectState.onGround && tinaDollPair.realObject.currentState != DollhouseObjectState.jumpToTable && tinaDollPair.realObject.currentState != DollhouseObjectState.gettingUp && tinaDollPair.realObject.currentState != DollhouseObjectState.inactive) || (!tinaDollPair.realObject.isGrabbed && tinaMovementThread == null))
		{
			return tinaMovementThread = GlobalCoroutineContainer.AddThread(MoveTinaDollToSequencePositionCoroutine(state, time), this);
		}
		return null;
	}

	private IEnumerator MoveTinaDollToSequencePositionCoroutine(PlayerDollState state, float time)
	{
		SafetyThread = GlobalCoroutineContainer.AddThread(ForcePlayerAndTinaToNextState(time), this);
		tinaDollPair.navMeshObject.SetState(NavFurnitureState.MainSequence, tinaDollPair.realObject.transform.localPosition, tinaDollPair.realObject.transform.localRotation);
		tinaDollPair.realObject.doingMainSequence = true;
		switch (state)
		{
		case PlayerDollState.toFront:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.outsideHousePos);
			break;
		case PlayerDollState.toBedroom:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.bedRoomPos);
			break;
		case PlayerDollState.toLivingroom:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.livingRoomPos);
			break;
		case PlayerDollState.toKidsRoom:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.kidsRoomPos);
			break;
		case PlayerDollState.toAttic:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.atticPos);
			break;
		case PlayerDollState.toDiningroom:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.diningRoomPos);
			break;
		case PlayerDollState.toLaundryroom:
			tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.laundryRoomPos);
			break;
		}
		while (tinaDollPair.navMeshObject.DistanceFromTarget() > 1f)
		{
			if (tinaDollPair.realObject.currentState == DollhouseObjectState.inactive)
			{
				tinaDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
				tinaDollPair.navMeshObject.SetState(NavFurnitureState.MainSequence, tinaDollPair.realObject.transform.localPosition, tinaDollPair.realObject.transform.localRotation);
				switch (state)
				{
				case PlayerDollState.toFront:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.outsideHousePos);
					break;
				case PlayerDollState.toBedroom:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.bedRoomPos);
					break;
				case PlayerDollState.toLivingroom:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.livingRoomPos);
					break;
				case PlayerDollState.toKidsRoom:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.kidsRoomPos);
					break;
				case PlayerDollState.toAttic:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.atticPos);
					break;
				case PlayerDollState.toDiningroom:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.diningRoomPos);
					break;
				case PlayerDollState.toLaundryroom:
					tinaDollPair.navMeshObject.SetMainSequenceTarget(tinaDollPair.realObject.sequencePath.laundryRoomPos);
					break;
				}
			}
			yield return null;
		}
		tinaDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, tinaDollPair.realObject.transform.localPosition, tinaDollPair.realObject.transform.localRotation);
		if (SafetyThread != null && SafetyThread.IsRunning)
		{
			SafetyThread.Stop();
		}
		yield return tinaDollPair.realObject.TurnToFacePlayer(0.2f);
		yield return tinaDollPair.realObject.SinkToFloor(0.1f);
		tinaDollPair.realObject.rb.isKinematic = true;
		tinaMovementThread = null;
		yield return null;
	}

	public SafeCoroutine MovePlayerDollToSequencePosition(PlayerDollState state)
	{
		if (!playerDollChosen)
		{
			return null;
		}
		switch (state)
		{
		case PlayerDollState.toFront:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.outsideHousePos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.outsideHousePos);
			break;
		case PlayerDollState.toBedroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.bedRoomPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.bedRoomPos);
			break;
		case PlayerDollState.toLivingroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.livingRoomPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.livingRoomPos);
			break;
		case PlayerDollState.toKidsRoom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.kidsRoomPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.kidsRoomPos);
			break;
		case PlayerDollState.toAttic:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.atticPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.atticPos);
			break;
		case PlayerDollState.toDiningroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.diningRoomPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.diningRoomPos);
			break;
		case PlayerDollState.toLaundryroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.laundryRoomPos);
			activePlayerDollPair.navMeshObject.SetHomeTarget(activePlayerDollPair.realObject.sequencePath.laundryRoomPos);
			break;
		}
		activePlayerDollPair.realObject.mainSequenceState = state;
		if (playerDollThread != null)
		{
			return null;
		}
		if (activePlayerDollPair.realObject.currentState != DollhouseObjectState.onGround && activePlayerDollPair.realObject.currentState != DollhouseObjectState.jumpToTable && activePlayerDollPair.realObject.currentState != DollhouseObjectState.gettingUp && activePlayerDollPair.realObject.currentState != DollhouseObjectState.inactive && !activePlayerDollPair.realObject.isGrabbed)
		{
			return playerDollThread = GlobalCoroutineContainer.AddThread(MovePlayerDollToSequencePositionCoroutine(state), this);
		}
		return null;
	}

	private IEnumerator MovePlayerDollToSequencePositionCoroutine(PlayerDollState state)
	{
		activePlayerDollPair.navMeshObject.SetState(NavFurnitureState.MainSequence, sceneRef.playerDoll.transform.localPosition, sceneRef.playerDoll.transform.localRotation);
		activePlayerDollPair.realObject.doingMainSequence = true;
		switch (state)
		{
		case PlayerDollState.toFront:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.outsideHousePos);
			break;
		case PlayerDollState.toBedroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.bedRoomPos);
			break;
		case PlayerDollState.toLivingroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.livingRoomPos);
			break;
		case PlayerDollState.toKidsRoom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.kidsRoomPos);
			break;
		case PlayerDollState.toAttic:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.atticPos);
			break;
		case PlayerDollState.toDiningroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.diningRoomPos);
			break;
		case PlayerDollState.toLaundryroom:
			activePlayerDollPair.navMeshObject.SetMainSequenceTarget(activePlayerDollPair.realObject.sequencePath.laundryRoomPos);
			break;
		}
		bool moved = false;
		while (activePlayerDollPair.navMeshObject.DistanceFromTarget() > 1f)
		{
			moved = true;
			yield return null;
		}
		activePlayerDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, activePlayerDollPair.realObject.transform.localPosition, activePlayerDollPair.realObject.transform.localRotation);
		if (moved)
		{
			yield return activePlayerDollPair.realObject.TurnToFacePlayer(0.2f);
			yield return activePlayerDollPair.realObject.SinkToFloor(0.1f);
		}
		activePlayerDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, sceneRef.playerDoll.transform.localPosition, sceneRef.playerDoll.transform.localRotation);
		activePlayerDollPair.realObject.rb.isKinematic = true;
		playerDollThread = null;
		yield return null;
	}

	public SafeCoroutine DawkensSequence(float duration)
	{
		return GlobalCoroutineContainer.AddThread(DawkensSequenceCoroutine(duration), this);
	}

	private IEnumerator DawkensSequenceCoroutine(float time)
	{
		dawkensPair.navMeshObject.SetState(NavFurnitureState.Active, dawkensPair.realObject.transform.localPosition, dawkensPair.realObject.transform.localRotation);
		float timer = 0f;
		bool endTarget = true;
		dawkensPair.navMeshObject.SetHomeTarget(dawkensTargets[1]);
		dawkensPair.navMeshObject.SetMainSequenceTarget(dawkensTargets[1]);
		while (timer < time)
		{
			timer += Time.deltaTime;
			if (dawkensPair.navMeshObject.DistanceFromTarget() < 1f)
			{
				if (endTarget)
				{
					dawkensPair.navMeshObject.SetMainSequenceTarget(dawkensTargets[0]);
					dawkensPair.navMeshObject.SetHomeTarget(dawkensTargets[0]);
					endTarget = false;
				}
				else
				{
					dawkensPair.navMeshObject.SetMainSequenceTarget(dawkensTargets[1]);
					dawkensPair.navMeshObject.SetHomeTarget(dawkensTargets[1]);
					endTarget = true;
				}
			}
			yield return null;
		}
	}

	private IEnumerator ForcePlayerAndTinaToNextState(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		if (tinaMovementThread != null && tinaMovementThread.IsRunning)
		{
			tinaMovementThread.Stop();
			tinaMovementThread = null;
		}
		if (playerDollThread != null && playerDollThread.IsRunning)
		{
			playerDollThread.Stop();
			playerDollThread = null;
		}
		sceneRef.popSound.Play();
		tinaDollPair.navMeshObject.transform.position = tinaDollPair.navMeshObject.GetMainSequenceTarget().transform.position;
		tinaDollPair.realObject.transform.localPosition = tinaDollPair.navMeshObject.transform.localPosition;
		activePlayerDollPair.navMeshObject.transform.position = activePlayerDollPair.navMeshObject.GetMainSequenceTarget().transform.position;
		activePlayerDollPair.realObject.transform.localPosition = activePlayerDollPair.navMeshObject.transform.localPosition;
		activePlayerDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, activePlayerDollPair.realObject.transform.localPosition, activePlayerDollPair.realObject.transform.localRotation);
		tinaDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, activePlayerDollPair.realObject.transform.localPosition, activePlayerDollPair.realObject.transform.localRotation);
		yield return activePlayerDollPair.realObject.TurnToFacePlayer(0.1f);
		yield return activePlayerDollPair.realObject.SinkToFloor(0.1f);
		yield return tinaDollPair.realObject.TurnToFacePlayer(0.1f);
		yield return tinaDollPair.realObject.SinkToFloor(0.1f);
	}

	private void setNavMeshState(DollhouseObjectPair pair)
	{
		if (pair.realObject.currentState == DollhouseObjectState.notSetUp)
		{
			pair.navMeshObject.SetState(NavFurnitureState.FollowingPuppet, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (pair.realObject.currentState == DollhouseObjectState.onGround || pair.realObject.currentState == DollhouseObjectState.jumpToTable || pair.realObject.isGrabbed)
		{
			pair.navMeshObject.SetParent(holdRoot);
			pair.navMeshObject.SetState(NavFurnitureState.FollowingPuppet, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (pair.realObject.currentState == DollhouseObjectState.inactive || pair.realObject.currentState == DollhouseObjectState.gettingUp)
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.FollowingPuppet, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (pair.realObject.currentState == DollhouseObjectState.home || !pair.realObject.gameObject.activeInHierarchy)
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.Inactive, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.Active, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
	}

	private void setNavMeshState(DollhouseDollPair pair)
	{
		if (pair.realObject.currentState == DollhouseObjectState.onGround || pair.realObject.currentState == DollhouseObjectState.jumpToTable || pair.realObject.isGrabbed)
		{
			pair.navMeshObject.SetParent(holdRoot);
			pair.navMeshObject.SetState(NavFurnitureState.FollowingPuppet, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (pair.realObject.currentState == DollhouseObjectState.inactive || pair.realObject.currentState == DollhouseObjectState.gettingUp)
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.FollowingPuppet, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (diningRoomTime && !pair.realObject.isTinaDoll)
		{
			pair.navMeshObject.SetHomeTarget(pair.navMeshObject.diningroomTarget);
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.DiningSequence, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if ((pair.realObject.currentState == DollhouseObjectState.home || !pair.realObject.isActiveAndEnabled) && !pair.realObject.playerDoll)
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			pair.navMeshObject.SetState(NavFurnitureState.Inactive, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
		else if (!pair.realObject.playerDoll && !pair.realObject.isTinaDoll)
		{
			pair.navMeshObject.SetParent(null, resetParent: true);
			if (!diningRoomTime)
			{
				pair.navMeshObject.SetState(NavFurnitureState.Active, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
			}
		}
		else if (pair.realObject.playerDoll && pair.realObject.currentState == DollhouseObjectState.onTable)
		{
			pair.navMeshObject.SetState(NavFurnitureState.Active, pair.realObject.transform.localPosition, pair.realObject.transform.localRotation);
		}
	}

	private void syncFurniture()
	{
		for (int i = 0; i < furnitureObjects.Count; i++)
		{
			setNavMeshState(furnitureObjects[i]);
			switch (furnitureObjects[i].navMeshObject.currentState)
			{
			case NavFurnitureState.Active:
			{
				furnitureObjects[i].realObject.rb.isKinematic = true;
				Vector3 localPosition = furnitureObjects[i].navMeshObject.transform.localPosition;
				Quaternion localRotation = furnitureObjects[i].navMeshObject.transform.localRotation;
				furnitureObjects[i].realObject.SetPosFromNavAgent(localPosition, localRotation);
				break;
			}
			case NavFurnitureState.FollowingPuppet:
				furnitureObjects[i].navMeshObject.transform.localPosition = furnitureObjects[i].realObject.transform.localPosition;
				furnitureObjects[i].navMeshObject.transform.localRotation = furnitureObjects[i].realObject.transform.localRotation;
				break;
			case NavFurnitureState.Inactive:
				furnitureObjects[i].navMeshObject.transform.localPosition = furnitureObjects[i].realObject.transform.localPosition;
				furnitureObjects[i].navMeshObject.transform.localRotation = furnitureObjects[i].realObject.transform.localRotation;
				break;
			}
		}
	}

	private void syncNPCDolls()
	{
		for (int i = 0; i < npcDollObjects.Count && (!ignoreJericho || !(npcDollObjects[i].realObject == sceneRef.jericho)); i++)
		{
			setNavMeshState(npcDollObjects[i]);
			switch (npcDollObjects[i].navMeshObject.currentState)
			{
			case NavFurnitureState.Active:
			{
				npcDollObjects[i].realObject.rb.isKinematic = true;
				Vector3 localPosition3 = npcDollObjects[i].navMeshObject.transform.localPosition;
				Quaternion localRotation3 = npcDollObjects[i].navMeshObject.transform.localRotation;
				if ((double)npcDollObjects[i].navMeshObject.DistanceFromAgentTarget() > 0.58)
				{
					npcDollObjects[i].realObject.SetPosFromNavAgent(localPosition3, localRotation3);
				}
				else
				{
					npcDollObjects[i].realObject.SetPosFromNavAgent(localPosition3, localRotation3, bob: false);
				}
				break;
			}
			case NavFurnitureState.MainSequence:
			{
				npcDollObjects[i].realObject.rb.isKinematic = true;
				Vector3 localPosition2 = npcDollObjects[i].navMeshObject.transform.localPosition;
				Quaternion localRotation2 = npcDollObjects[i].navMeshObject.transform.localRotation;
				npcDollObjects[i].realObject.SetPosFromNavAgent(localPosition2, localRotation2);
				break;
			}
			case NavFurnitureState.DiningSequence:
			{
				npcDollObjects[i].realObject.SetState(DollhouseObjectState.scriptedSequence);
				npcDollObjects[i].realObject.rb.isKinematic = true;
				Vector3 localPosition = npcDollObjects[i].navMeshObject.transform.localPosition;
				Quaternion localRotation = npcDollObjects[i].navMeshObject.transform.localRotation;
				if ((double)npcDollObjects[i].navMeshObject.DistanceFromDiningRoom() > 0.58)
				{
					npcDollObjects[i].realObject.SetPosFromNavAgent(localPosition, localRotation);
				}
				else
				{
					npcDollObjects[i].realObject.SetPosFromNavAgent(localPosition, localRotation, bob: false);
				}
				break;
			}
			case NavFurnitureState.FollowingPuppet:
				npcDollObjects[i].navMeshObject.transform.localPosition = npcDollObjects[i].realObject.transform.localPosition;
				npcDollObjects[i].navMeshObject.transform.localRotation = npcDollObjects[i].realObject.transform.localRotation;
				break;
			}
		}
	}

	private void syncPlayerDolls()
	{
		setNavMeshState(activePlayerDollPair);
		if (activePlayerDollPair.realObject.isGrabbed)
		{
			return;
		}
		if (activePlayerDollPair.navMeshObject.transform.parent != activePlayerDollPair.navMeshObject.startParent)
		{
			activePlayerDollPair.navMeshObject.SetParent(activePlayerDollPair.navMeshObject.startParent);
		}
		switch (activePlayerDollPair.navMeshObject.currentState)
		{
		case NavFurnitureState.Active:
		{
			activePlayerDollPair.realObject.rb.isKinematic = true;
			Vector3 localPosition2 = activePlayerDollPair.navMeshObject.transform.localPosition;
			Quaternion localRotation2 = activePlayerDollPair.navMeshObject.transform.localRotation;
			if ((double)activePlayerDollPair.navMeshObject.DistanceFromTarget() > 0.58)
			{
				activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition2, localRotation2);
			}
			else
			{
				activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition2, localRotation2, bob: false);
			}
			break;
		}
		case NavFurnitureState.MainSequence:
			if (activePlayerDollPair.realObject.ShouldNavSync())
			{
				activePlayerDollPair.realObject.rb.isKinematic = true;
				activePlayerDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
				Vector3 localPosition3 = activePlayerDollPair.navMeshObject.transform.localPosition;
				Quaternion localRotation3 = activePlayerDollPair.navMeshObject.transform.localRotation;
				if ((double)activePlayerDollPair.navMeshObject.DistanceFromTarget() > 0.58)
				{
					activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition3, localRotation3);
				}
				else
				{
					activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition3, localRotation3, bob: false);
				}
			}
			break;
		case NavFurnitureState.DiningSequence:
			if (activePlayerDollPair.realObject.ShouldNavSync())
			{
				activePlayerDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
				activePlayerDollPair.realObject.rb.isKinematic = true;
				Vector3 localPosition = activePlayerDollPair.navMeshObject.transform.localPosition;
				Quaternion localRotation = activePlayerDollPair.navMeshObject.transform.localRotation;
				if ((double)activePlayerDollPair.navMeshObject.DistanceFromDiningRoom() > 0.58)
				{
					activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition, localRotation);
				}
				else
				{
					activePlayerDollPair.realObject.SetPosFromNavAgent(localPosition, localRotation, bob: false);
				}
			}
			break;
		case NavFurnitureState.FollowingPuppet:
			activePlayerDollPair.navMeshObject.transform.localPosition = activePlayerDollPair.realObject.transform.localPosition;
			activePlayerDollPair.navMeshObject.transform.localRotation = activePlayerDollPair.realObject.transform.localRotation;
			break;
		case NavFurnitureState.Inactive:
			break;
		}
	}

	private void SyncDawkens()
	{
		if (!dawkensPair.realObject.gameObject.activeInHierarchy)
		{
			dawkensPair.navMeshObject.SetState(NavFurnitureState.Inactive, dawkensPair.realObject.transform.localPosition, dawkensPair.realObject.transform.localRotation);
		}
		if (dawkensPair.realObject.isGrabbed)
		{
			return;
		}
		if (dawkensPair.navMeshObject.transform.parent != dawkensPair.navMeshObject.startParent)
		{
			dawkensPair.navMeshObject.SetParent(dawkensPair.navMeshObject.startParent);
		}
		switch (dawkensPair.navMeshObject.currentState)
		{
		case NavFurnitureState.Active:
		{
			dawkensPair.realObject.rb.isKinematic = true;
			Vector3 localPosition = dawkensPair.navMeshObject.transform.localPosition;
			Quaternion localRotation = dawkensPair.navMeshObject.transform.localRotation;
			if ((double)dawkensPair.navMeshObject.DistanceFromTarget() > 0.58)
			{
				dawkensPair.realObject.SetPosFromNavAgent(localPosition, localRotation);
			}
			else
			{
				dawkensPair.realObject.SetPosFromNavAgent(localPosition, localRotation, bob: false);
			}
			break;
		}
		case NavFurnitureState.Inactive:
			dawkensPair.navMeshObject.transform.localPosition = dawkensPair.realObject.transform.localPosition;
			dawkensPair.navMeshObject.transform.localRotation = dawkensPair.realObject.transform.localRotation;
			break;
		}
	}

	private void syncTinaDoll()
	{
		if (sceneRef.talkingSeqeunce)
		{
			tinaDollPair.navMeshObject.SetState(NavFurnitureState.Inactive, tinaDollPair.realObject.transform.localPosition, tinaDollPair.realObject.transform.localRotation);
		}
		else
		{
			setNavMeshState(tinaDollPair);
		}
		switch (tinaDollPair.navMeshObject.currentState)
		{
		case NavFurnitureState.Active:
		{
			tinaDollPair.realObject.rb.isKinematic = true;
			Vector3 localPosition3 = tinaDollPair.navMeshObject.transform.localPosition;
			Quaternion localRotation3 = tinaDollPair.navMeshObject.transform.localRotation;
			tinaDollPair.realObject.SetPosFromNavAgent(localPosition3, localRotation3);
			break;
		}
		case NavFurnitureState.MainSequence:
		{
			tinaDollPair.realObject.rb.isKinematic = true;
			tinaDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
			Vector3 localPosition2 = tinaDollPair.navMeshObject.transform.localPosition;
			Quaternion localRotation2 = tinaDollPair.navMeshObject.transform.localRotation;
			tinaDollPair.realObject.SetPosFromNavAgent(localPosition2, localRotation2);
			break;
		}
		case NavFurnitureState.DiningSequence:
		{
			tinaDollPair.realObject.SetState(DollhouseObjectState.scriptedSequence);
			tinaDollPair.realObject.rb.isKinematic = true;
			Vector3 localPosition = tinaDollPair.navMeshObject.transform.localPosition;
			Quaternion localRotation = tinaDollPair.navMeshObject.transform.localRotation;
			if ((double)tinaDollPair.navMeshObject.DistanceFromDiningRoom() > 0.58)
			{
				tinaDollPair.realObject.SetPosFromNavAgent(localPosition, localRotation);
			}
			else
			{
				tinaDollPair.realObject.SetPosFromNavAgent(localPosition, localRotation, bob: false);
			}
			break;
		}
		case NavFurnitureState.FollowingPuppet:
			tinaDollPair.navMeshObject.transform.localPosition = tinaDollPair.realObject.transform.localPosition;
			tinaDollPair.navMeshObject.transform.localRotation = tinaDollPair.realObject.transform.localRotation;
			break;
		case NavFurnitureState.Inactive:
			break;
		}
	}

	private void syncInactiveDoll()
	{
		for (int i = 0; i < playerDollPairs.Length; i++)
		{
			setNavMeshState(playerDollPairs[i]);
			switch (playerDollPairs[i].navMeshObject.currentState)
			{
			case NavFurnitureState.Active:
			{
				playerDollPairs[i].realObject.rb.isKinematic = true;
				Vector3 localPosition3 = playerDollPairs[i].navMeshObject.transform.localPosition;
				Quaternion localRotation3 = playerDollPairs[i].navMeshObject.transform.localRotation;
				if ((double)playerDollPairs[i].navMeshObject.DistanceFromTarget() > 0.6)
				{
					playerDollPairs[i].realObject.SetPosFromNavAgent(localPosition3, localRotation3);
				}
				else
				{
					playerDollPairs[i].realObject.SetPosFromNavAgent(localPosition3, localRotation3, bob: false);
				}
				break;
			}
			case NavFurnitureState.MainSequence:
			{
				playerDollPairs[i].realObject.rb.isKinematic = true;
				Vector3 localPosition2 = playerDollPairs[i].navMeshObject.transform.localPosition;
				Quaternion localRotation2 = playerDollPairs[i].navMeshObject.transform.localRotation;
				playerDollPairs[i].realObject.SetPosFromNavAgent(localPosition2, localRotation2);
				break;
			}
			case NavFurnitureState.DiningSequence:
			{
				playerDollPairs[i].realObject.SetState(DollhouseObjectState.scriptedSequence);
				playerDollPairs[i].realObject.rb.isKinematic = true;
				Vector3 localPosition = playerDollPairs[i].navMeshObject.transform.localPosition;
				Quaternion localRotation = playerDollPairs[i].navMeshObject.transform.localRotation;
				if ((double)playerDollPairs[i].navMeshObject.DistanceFromDiningRoom() > 0.58)
				{
					playerDollPairs[i].realObject.SetPosFromNavAgent(localPosition, localRotation);
				}
				else
				{
					playerDollPairs[i].realObject.SetPosFromNavAgent(localPosition, localRotation, bob: false);
				}
				break;
			}
			case NavFurnitureState.FollowingPuppet:
				playerDollPairs[i].navMeshObject.transform.localPosition = playerDollPairs[i].realObject.transform.localPosition;
				playerDollPairs[i].navMeshObject.transform.localRotation = playerDollPairs[i].realObject.transform.localRotation;
				break;
			}
		}
	}

	public void ShutItDown()
	{
		done = true;
		if (tinaMovementThread != null && tinaMovementThread.IsRunning)
		{
			tinaMovementThread.Stop();
		}
		if (playerDollThread != null && playerDollThread.IsRunning)
		{
			playerDollThread.Stop();
		}
		for (int i = 0; i < furnitureObjects.Count; i++)
		{
			furnitureObjects[i].navMeshObject.agent.enabled = false;
			if (furnitureObjects[i].navMeshObject.GetCutter() != null)
			{
				furnitureObjects[i].navMeshObject.GetCutter().enabled = false;
			}
			furnitureObjects[i].navMeshObject.enabled = false;
			if (furnitureObjects[i].realObject.rb != null)
			{
				furnitureObjects[i].realObject.rb.isKinematic = false;
			}
			furnitureObjects[i].realObject.enabled = false;
		}
		for (int j = 0; j < npcDollObjects.Count; j++)
		{
			npcDollObjects[j].navMeshObject.agent.enabled = false;
			npcDollObjects[j].navMeshObject.enabled = false;
			npcDollObjects[j].realObject.rb.isKinematic = false;
			npcDollObjects[j].realObject.enabled = false;
		}
		if (tinaDollChosen)
		{
			tinaDollPair.realObject.rb.isKinematic = false;
			tinaDollPair.realObject.enabled = false;
			tinaDollPair.navMeshObject.agent.enabled = false;
			tinaDollPair.navMeshObject.enabled = false;
		}
	}

	public void AddAgentToNoPathList(FurnitureNavigation obj)
	{
		if (!AgentsWithoutPath.Contains(obj))
		{
			AgentsWithoutPath.Add(obj);
			SetCuttersEnabled(s: false);
		}
	}

	public void RemoveAgentFromNoPathList(FurnitureNavigation obj)
	{
		if (AgentsWithoutPath.Contains(obj))
		{
			AgentsWithoutPath.Remove(obj);
		}
	}

	private void handleCutters()
	{
		if (AgentsWithoutPath.Count > 0)
		{
			if (cuttersEnabled)
			{
				SetCuttersEnabled(s: false);
			}
		}
		else if (!cuttersEnabled)
		{
			SetCuttersEnabled(s: true);
		}
	}

	private void SetCuttersEnabled(bool s)
	{
		if (s)
		{
			SafeDebug.Log("EnablingCutters");
		}
		else
		{
			SafeDebug.Log("DisablingCutters");
		}
		for (int i = 0; i < allCutters.Length; i++)
		{
			allCutters[i].carving = s;
			allCutters[i].enabled = s;
		}
		for (int j = 0; j < furnitureObjects.Count; j++)
		{
			if (furnitureObjects[j].navMeshObject.neverUseCarver)
			{
				furnitureObjects[j].navMeshObject.GetCutter().carving = false;
				furnitureObjects[j].navMeshObject.GetCutter().enabled = false;
			}
			if (furnitureObjects[j].navMeshObject.GetAgent().enabled)
			{
				furnitureObjects[j].navMeshObject.GetCutter().enabled = false;
			}
		}
		cuttersEnabled = s;
	}
}
