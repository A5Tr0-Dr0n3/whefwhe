using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ElevatorFloorManager : MonoBehaviour
{
	public ElevatorTweakablesScriptableObject tweakables;

	[SerializeField]
	private Transform insideElevatorReference;

	[SerializeField]
	private TextMesh currentFloorIndicator;

	[SerializeField]
	private float maxElevatorVelocity = 2f;

	[SerializeField]
	private float elevatorAccelerationTime = 1f;

	[SerializeField]
	private AnimationCurve elevatorAccelerationCurve;

	[SerializeField]
	private AnimationCurve elevatorDecelerationCurve;

	[SerializeField]
	private float slowdownDistance = 4f;

	[SerializeField]
	private float slowdownTime = 1f;

	private List<ElevatorFloor> constructedFloors;

	private float velocity;

	private SafeCoroutine goToFloorThread;

	private Transform floorParent;

	private ElevatorFloor targetElevatorFloor;

	private ElevatorFloorSpecialPlace specialPlace;

	private void Awake()
	{
		tweakables = UnityEngine.Object.Instantiate(tweakables);
	}

	private void Start()
	{
		constructedFloors = new List<ElevatorFloor>();
		buildFloors();
	}

	private void buildFloors()
	{
		floorParent = new GameObject().transform;
		floorParent.name = "Floor Parent";
		floorParent.position = transform.position;
		floorParent.SetParent(transform);
		for (int i = 0; i < tweakables.floors.floorList.Length; i++)
		{
			ElevatorFloor elevatorFloor = tweakables.floors.floorList[i];
			constructedFloors.Add(elevatorFloor);
			if (string.IsNullOrEmpty(elevatorFloor.floorName))
			{
				elevatorFloor.floorName = "Floor " + (i + 1);
			}
			if (elevatorFloor.floorPrefab == null)
			{
				elevatorFloor.floorPrefab = tweakables.floors.defaultElevatorFloorPrefab;
			}
			elevatorFloor.spawned = UnityEngine.Object.Instantiate(elevatorFloor.floorPrefab).GetComponent<ElevatorFloorGameObjectAssembly>();
			elevatorFloor.spawned.name = elevatorFloor.floorName + " Spawned Instance";
			if (i == 0)
			{
				elevatorFloor.spawned.transform.position = transform.position;
			}
			else
			{
				float y = (elevatorFloor.spawned.GetFloorBottomPoint() - constructedFloors[i - 1].spawned.GetFloorTopPoint()).y;
				elevatorFloor.spawned.transform.position -= Vector3.up * y;
			}
			if (i != 0)
			{
				if (elevatorFloor.customFloorAudio != null)
				{
					elevatorFloor.spawned.mAudioSource.clip = elevatorFloor.customFloorAudio;
				}
				else
				{
					elevatorFloor.spawned.mAudioSource.clip = tweakables.floors.defaultElevatorAmbiances[UnityEngine.Random.Range(0, tweakables.floors.defaultElevatorAmbiances.Length)];
					elevatorFloor.spawned.mAudioSource.time = UnityEngine.Random.Range(0f, elevatorFloor.spawned.mAudioSource.clip.length);
				}
			}
			elevatorFloor.spawned.transform.parent = floorParent;
			ElevatorFloorSpecialPlace component = elevatorFloor.spawned.GetComponent<ElevatorFloorSpecialPlace>();
			if (component != null)
			{
				specialPlace = component;
			}
		}
	}

	private void Update()
	{
		currentFloorIndicator.text = getCurrentFloorByPosition().floorName.ToUpper();
	}

	private void FixedUpdate()
	{
		if (floorParent != null)
		{
			floorParent.position += Vector3.up * velocity * Time.fixedDeltaTime;
		}
	}

	public void GoToFloor(string floorName, bool succeed, float travelDuration)
	{
		int targetFloorIndex = lookUpFloorIndexByName(floorName);
		GoToFloor(targetFloorIndex, succeed, travelDuration);
	}

	public void GoToFloor(int targetFloorIndex, bool succeed, float travelDuration)
	{
		if (goToFloorThread != null && goToFloorThread.IsRunning)
		{
			goToFloorThread.Stop();
			goToFloorThread = null;
		}
		goToFloorThread = GlobalCoroutineContainer.AddThread(GoToFloorSequence(targetFloorIndex, succeed, travelDuration), this);
	}

	private IEnumerator GoToFloorSequence(int targetFloorIndex, bool mustArrive, float travelDuration)
	{
		targetElevatorFloor = constructedFloors[targetFloorIndex];
		float originalDistance = insideElevatorReference.position.y - targetElevatorFloor.spawned.transform.position.y;
		float directionSign = Mathf.Sign(originalDistance);
		float travelTimer = 0f;
		while ((!hasMetDestination(directionSign) && mustArrive) || (!hasMetDestination(directionSign) && travelTimer <= travelDuration))
		{
			travelTimer += Time.deltaTime;
			float dist = Mathf.Abs(insideElevatorReference.position.y - targetElevatorFloor.spawned.transform.position.y);
			if ((!mustArrive && travelDuration - travelTimer <= slowdownTime) || dist < slowdownDistance)
			{
				float num = 1f - Mathf.Clamp01(dist / slowdownDistance);
				float time = num;
				if (!mustArrive)
				{
					float b = 1f - Mathf.Clamp01((travelDuration - travelTimer) / slowdownTime);
					time = Mathf.Max(num, b);
				}
				velocity = directionSign * maxElevatorVelocity * elevatorDecelerationCurve.Evaluate(time);
			}
			else
			{
				float time2 = Mathf.Clamp01(travelTimer / elevatorAccelerationTime);
				velocity = directionSign * maxElevatorVelocity * elevatorAccelerationCurve.Evaluate(time2);
			}
			yield return null;
		}
		velocity = 0f;
	}

	private bool hasMetDestination(float directionSign)
	{
		return (directionSign > 0f && insideElevatorReference.position.y <= targetElevatorFloor.spawned.transform.position.y) || (directionSign <= 0f && insideElevatorReference.position.y >= targetElevatorFloor.spawned.transform.position.y);
	}

	public void OpenSpecialPlaceBarrier(bool open)
	{
		specialPlace.EnablePropDoor(!open);
	}

	public bool IsTraveling()
	{
		return goToFloorThread != null && goToFloorThread.IsRunning;
	}

	private int lookUpFloorIndexByName(string targetName)
	{
		for (int i = 0; i < constructedFloors.Count; i++)
		{
			if (constructedFloors[i].floorName.Equals(targetName, StringComparison.InvariantCultureIgnoreCase))
			{
				return i;
			}
		}
		SafeDebug.LogError("You've requested an elevator floor that doesn't exist!");
		return 0;
	}

	private ElevatorFloor getCurrentFloorByPosition()
	{
		for (int i = 0; i < constructedFloors.Count; i++)
		{
			if (insideElevatorReference.position.y <= constructedFloors[i].spawned.GetFloorBottomPoint().y)
			{
				if (i == 0)
				{
					return constructedFloors[0];
				}
				return constructedFloors[i - 1];
			}
		}
		return constructedFloors[constructedFloors.Count - 1];
	}
}
