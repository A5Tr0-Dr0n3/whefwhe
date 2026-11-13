using System.Collections;
using Edelweiss.Coroutine;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class VVR_ElevatorNew : SceneBehaviorManager
{
	[SerializeField]
	private GameObject[] floors;

	[SerializeField]
	private GameObject elevator;

	[SerializeField]
	private ObjectTrigger playerTrigger;

	[SerializeField]
	private ElevatorDoorController elevatorDoors;

	[SerializeField]
	private GameObject elevatorShaftUp;

	[SerializeField]
	private ElizaEnvironmentProperties shaftEnvironment;

	[SerializeField]
	private ElevatorEnvironments envController;

	[SerializeField]
	private Material buttonOffMat;

	[SerializeField]
	private Material buttonOnMat;

	[SerializeField]
	private Renderer buttonUpRender;

	[SerializeField]
	private Renderer buttonDownRenderer;

	[SerializeField]
	private ElevatorButtonNew buttonUp;

	[SerializeField]
	private ElevatorButtonNew buttonDown;

	[SerializeField]
	private GameObject[] elevatorButtons;

	[SerializeField]
	private GameObject elevatorButtonActive;

	[SerializeField]
	private GvrAudioSource moveSound1;

	[SerializeField]
	private GvrAudioSource moveSound2;

	[SerializeField]
	private ElevatorKey goldKey;

	[SerializeField]
	private GameObject lockedSafe;

	[SerializeField]
	private Transform keyStartTransform;

	[HideInInspector]
	public bool isMoving;

	private SafeCoroutine elevatorCoroutine;

	private bool goingUp;

	public int currentFloor = 1;

	private int nextFloor;

	private int elevatorFloor;

	private ElevatorHeadset currentHeadset;

	private ElevatorHeadset lastHeadset;

	private ObjectBoid goldBoid;

	private Vector3 floorPos = new Vector3(0f, 90f, 0f);

	private float elevatorMoveTime = 4.5f;

	private float count;

	private float lerp;

	private float direction;

	private Vector3 playerPos;

	private Vector3 nextPos;

	private Vector3 phonePos;

	private EnvironmentProperties nextEnv;

	private bool animatedEnv;

	private void Start()
	{
		for (int i = 0; i < floors.Length; i++)
		{
			if (i == currentFloor)
			{
				floors[i].SetActive(value: true);
			}
			else
			{
				floors[i].SetActive(value: false);
			}
		}
		if (EagerScenes.instance.headsets.Count > 0)
		{
			lastHeadset = EagerScenes.instance.headsets[EagerScenes.instance.headsets.Count - 1].GetComponent<ElevatorHeadset>();
		}
	}

	private void OnDisable()
	{
		Teleporter.SetTeleportEnabled(enabledIn: true);
		ToolManager.AllManipulators.UserCanGrab = true;
		ToolManager.AllCursorReticles.HideCursor = false;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		if (PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Elevator)
		{
			goldKey.transform.SetParent(goldKey.elevatorDock, worldPositionStays: true);
			goldKey.transform.position = goldKey.elevatorDock.position;
			goldKey.transform.rotation = goldKey.elevatorDock.rotation;
			if (goldKey.GetComponent<ObjectBase>() != null)
			{
				goldKey.GetComponent<ObjectBase>().ungrabbable = true;
			}
			if (goldKey.GetComponent<ObjectBoid>() != null)
			{
				goldKey.GetComponent<ObjectBoid>().enabled = false;
			}
			if (goldKey.GetComponent<Rigidbody>() != null)
			{
				goldKey.GetComponent<Rigidbody>().useGravity = false;
				goldKey.GetComponent<Rigidbody>().isKinematic = true;
			}
			goldKey.angelicMusic.volume = 0.8f;
			goldKey.angelicMusic.Play();
			goldKey.dockedGlowCard.SetActive(value: true);
		}
		else if (PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Unlocked)
		{
			goldKey.gameObject.SetActive(value: false);
			lockedSafe.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
		}
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (EagerScenes.instance.headsets.Count > 0)
		{
			currentHeadset = EagerScenes.instance.headsets[EagerScenes.instance.headsets.Count - 1].GetComponent<ElevatorHeadset>();
		}
		if (currentHeadset != null)
		{
			EagerMaster.instance.player.transform.position = currentHeadset.GetComponent<Headset>().spawnPos;
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
			}
		}
		if (currentHeadset == null)
		{
			SafeDebug.LogError("ALL ELEVATOR HEADSETS NEED A ElevatorHeadset SCRIPT");
			ResetElevatorScene(currentFloor);
		}
		if (currentHeadset != lastHeadset)
		{
			ResetElevatorScene(currentHeadset.headsetFloor);
			SafeDebug.Log("TRYING TO RESET ELEVATOR");
		}
		else if (isMoving)
		{
			Teleporter.SetTeleportEnabled(enabledIn: false);
			ToolManager.AllManipulators.UserCanGrab = false;
			ToolManager.AllCursorReticles.HideCursor = true;
		}
		lastHeadset = currentHeadset;
		if (PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Flying)
		{
			goldBoid = goldKey.GetComponent<ObjectBoid>();
			if (goldBoid != null && !goldBoid.Power)
			{
				goldBoid.Power = true;
			}
			goldKey.transform.position = keyStartTransform.position;
			goldKey.transform.rotation = keyStartTransform.rotation;
		}
	}

	public void FloorButtonPressed(int floor)
	{
		int num = currentFloor + floor;
		if (num < 0 || num > floors.Length - 1)
		{
			return;
		}
		nextFloor = num;
		if (!isMoving && playerTrigger.playerInside && elevatorDoors.DoorsAreClosed())
		{
			if (nextFloor > currentFloor)
			{
				goingUp = true;
			}
			else
			{
				goingUp = false;
			}
			elevatorCoroutine = GlobalCoroutineContainer.AddThread(ElevatorChangeFloor(), this);
		}
		else
		{
			moveSound2.Play();
		}
	}

	public void ResetElevatorScene(int resetFloor)
	{
		SafeDebug.Log("RESETING ELEVATOR");
		if (elevatorCoroutine != null && (elevatorCoroutine.IsRunning || elevatorCoroutine.IsPaused))
		{
			elevatorCoroutine.Stop();
		}
		elevator.transform.position = Vector3.zero;
		currentFloor = resetFloor;
		EagerMaster.instance.player.transform.position = new Vector3(EagerMaster.instance.player.transform.position.x, 0f, EagerMaster.instance.player.transform.position.z);
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
		}
		for (int i = 0; i < floors.Length; i++)
		{
			if (i == currentFloor)
			{
				floors[i].SetActive(value: true);
			}
			else
			{
				floors[i].SetActive(value: false);
			}
		}
		envController.MatchEnvironment(0f, currentFloor);
		elevatorDoors.AnimateDoors(closingDoors: false);
		isMoving = false;
		Teleporter.SetTeleportEnabled(enabledIn: true);
		ToolManager.AllManipulators.UserCanGrab = true;
		ToolManager.AllCursorReticles.HideCursor = false;
		if (currentFloor == floors.Length - 1)
		{
			elevatorShaftUp.SetActive(value: false);
		}
		else
		{
			elevatorShaftUp.SetActive(value: true);
		}
		CheckButtonStates();
	}

	public void CheckButtonStates()
	{
		if (currentFloor == floors.Length - 1 || !elevatorDoors.DoorsAreClosed())
		{
			buttonUpRender.sharedMaterial = buttonOffMat;
			buttonUp.ungrabbable = true;
		}
		else
		{
			buttonUpRender.sharedMaterial = buttonOnMat;
			buttonUp.ungrabbable = false;
		}
		if (currentFloor == 0 || !elevatorDoors.DoorsAreClosed())
		{
			buttonDownRenderer.sharedMaterial = buttonOffMat;
			buttonDown.ungrabbable = true;
		}
		else
		{
			buttonDownRenderer.sharedMaterial = buttonOnMat;
			buttonDown.ungrabbable = false;
		}
		for (int i = 0; i < elevatorButtons.Length; i++)
		{
			if (i == currentFloor)
			{
				elevatorButtonActive.transform.position = elevatorButtons[i].transform.position;
				elevatorButtons[i].SetActive(value: false);
				elevatorButtonActive.SetActive(value: true);
			}
			else
			{
				elevatorButtons[i].SetActive(value: true);
			}
		}
	}

	private IEnumerator ElevatorChangeFloor()
	{
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_OFFICES);
		isMoving = true;
		Teleporter.SetTeleportEnabled(enabledIn: false);
		ToolManager.DropEverything();
		ToolManager.AllManipulators.UserCanGrab = false;
		direction = ((!goingUp) ? (-1f) : 1f);
		if (currentHeadset != null)
		{
			currentHeadset.headsetFloor = nextFloor;
		}
		moveSound2.Play();
		if (direction == 1f)
		{
			buttonUpRender.sharedMaterial = buttonOnMat;
			buttonDownRenderer.sharedMaterial = buttonOffMat;
		}
		else
		{
			buttonDownRenderer.sharedMaterial = buttonOnMat;
			buttonUpRender.sharedMaterial = buttonOffMat;
		}
		moveSound1.Play();
		count = 0f;
		lerp = 0f;
		animatedEnv = false;
		ToolManager.AllCursorReticles.HideCursor = true;
		while (lerp < 1f)
		{
			count += Time.deltaTime;
			lerp = Mathf.Clamp01(count / elevatorMoveTime);
			elevator.transform.position = Vector3.Lerp(Vector3.zero, floorPos * direction, lerp);
			playerPos = EagerMaster.instance.player.transform.position;
			playerPos.y = elevator.transform.position.y;
			EagerMaster.instance.player.transform.position = playerPos;
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
			}
			if (direction == 1f && elevator.transform.position.y > 9f && !animatedEnv)
			{
				animatedEnv = true;
				envController.MatchEnvironment(0.5f, shaftEnvironment);
			}
			else if (direction == -1f && elevator.transform.position.y < -2.5f && !animatedEnv)
			{
				animatedEnv = true;
				envController.MatchEnvironment(0.5f, shaftEnvironment);
			}
			yield return null;
		}
		floors[currentFloor].SetActive(value: false);
		floors[nextFloor].SetActive(value: true);
		if (nextFloor == floors.Length - 1)
		{
			elevatorShaftUp.SetActive(value: false);
		}
		else
		{
			elevatorShaftUp.SetActive(value: true);
		}
		count = 0f;
		lerp = 0f;
		animatedEnv = false;
		while (lerp < 1f)
		{
			count += Time.deltaTime;
			lerp = Mathf.Clamp01(count / elevatorMoveTime);
			nextPos = Vector3.Lerp(-floorPos * direction, Vector3.zero, lerp);
			elevator.transform.position = nextPos;
			playerPos = EagerMaster.instance.player.transform.position;
			playerPos.y = nextPos.y;
			EagerMaster.instance.player.transform.position = playerPos;
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
			}
			if (direction == 1f && lerp > 0.9f && !animatedEnv)
			{
				animatedEnv = true;
				envController.MatchEnvironment(elevatorMoveTime * 0.1f, nextFloor);
			}
			else if (direction == -1f && EagerMaster.instance.player.transform.position.y < 12.5f && !animatedEnv && lerp > 0.5f)
			{
				animatedEnv = true;
				envController.MatchEnvironment(0.75f, nextFloor);
			}
			yield return null;
		}
		moveSound1.Stop();
		moveSound2.Play();
		ToolManager.AllCursorReticles.HideCursor = false;
		playerPos = EagerMaster.instance.player.transform.position;
		playerPos.y = 0f;
		EagerMaster.instance.player.transform.position = playerPos;
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
		}
		Teleporter.SetTeleportEnabled(enabledIn: true);
		ToolManager.AllManipulators.UserCanGrab = true;
		currentFloor = nextFloor;
		CheckButtonStates();
		isMoving = false;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.1f, this);
		if (currentFloor == 0 && elevatorDoors.DoorsAreClosed() && PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Elevator)
		{
			elevatorDoors.DoorButtonPressed();
		}
	}
}
