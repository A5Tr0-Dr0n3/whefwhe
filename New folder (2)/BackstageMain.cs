using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackstageMain : Backstage
{
	private BackstageStates backstageStates;

	[SerializeField]
	private GameObject infoBooth;

	[SerializeField]
	private GameObject elevatorHelp;

	[SerializeField]
	private GameObject[] backstageStateObjects;

	public GameObject[] randomGlobes;

	public GameObject hugeGlobeClosed;

	public GameObject jobGlobe;

	public AggroTimer mainSwarmAggro;

	[SerializeField]
	private GameObject roostCourier;

	[SerializeField]
	private BackstageJobDisplay[] jobDisplays;

	[SerializeField]
	private BackstageCourier[] couriers;

	[SerializeField]
	private VVR_Point spotlightScript;

	[SerializeField]
	private GameObject spotlight;

	[SerializeField]
	private Rigidbody spotlightRB;

	[Header("Union Calls")]
	public UnionStapler stapler;

	public UnionStapler eleStapler;

	public AudioClip message;

	public AudioClip eleMessage;

	private void Awake()
	{
		if (Backstage.instance != null)
		{
			throw new UnityException("Only one instance of Backstage in a scene is allowed.");
		}
		Backstage.instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		backstageStates = PersistentDataManager.GetBackstageState();
		StartBackstage();
	}

	private void StartBackstage()
	{
		for (int i = 0; i < backstageStateObjects.Length; i++)
		{
			backstageStateObjects[i].SetActive(value: false);
		}
		backstageStateObjects[(int)backstageStates].SetActive(value: true);
		if (PersistentDataManager.GetInfoDeskState())
		{
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE);
			infoBooth.SetActive(value: true);
			elevatorHelp.SetActive(value: false);
			if (!PersistentDataManager.GetInfoMessageHeard())
			{
				stapler.RingStapler(message, UnionStapler.StaplerLocation.InfoDesk);
			}
		}
		else
		{
			infoBooth.SetActive(value: false);
			ElevatorSaveData elevatorData = PersistentDataManager.GetElevatorData();
			if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.LOOP && PersistentDataManager.GetKitchenLoopCounter() >= 2 && !elevatorData.hasUsedPhone && elevatorData.keyState == ElevatorSaveData.KeyStates.Flying && !PersistentDataManager.GetHasAdmin())
			{
				elevatorHelp.SetActive(value: true);
				eleStapler.RingStapler(eleMessage, UnionStapler.StaplerLocation.Other, 3f);
			}
		}
		if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.LOOP)
		{
			mainSwarmAggro.enabled = false;
		}
		switch (backstageStates)
		{
		case BackstageStates.Intro:
			BackstageIntro();
			break;
		case BackstageStates.JobIntrusion:
			BackstageJobIntrusion();
			break;
		}
	}

	private void BackstageIntro()
	{
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		BackstageIntrusionFromJobSetup();
		GlobalCoroutineContainer.AddThread(BackstageIntroCoroutine(), this);
	}

	private IEnumerator BackstageIntroCoroutine()
	{
		mainSwarmAggro.failsafeTimer = 60f;
		mainSwarmAggro.chaseTimer = 40f;
		mainSwarmAggro.searchTimer = 20f;
		yield return null;
		Vector3 playerStartPosition = EagerMaster.instance.mainCamera.transform.position;
		while (Vector3.SqrMagnitude(playerStartPosition - EagerMaster.instance.mainCamera.transform.position) < 25f)
		{
			yield return null;
		}
		mainSwarmAggro.failsafeTimer += 20f;
		mainSwarmAggro.chaseTimer += 20f;
		mainSwarmAggro.searchTimer += 20f;
	}

	private void BackstageJobIntrusion()
	{
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.hasStartedJob = true;
			VVR_HUB.instance.wasInBackstage = true;
		}
		else
		{
			SafeDebug.LogError("No VVR_HUB instance, BAD");
		}
		BackstageIntrusionFromJobSetup();
		GlobalCoroutineContainer.AddThread(BackstageJobIntrusionCoroutine(), this);
	}

	private IEnumerator BackstageJobIntrusionCoroutine()
	{
		yield return null;
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
	}

	private void BackstageIntrusionFromJobSetup()
	{
		List<int> list = new List<int>();
		BackstageJobDisplay.ResetDisplayList();
		for (int i = 0; i < randomGlobes.Length; i++)
		{
			randomGlobes[i].SetActive(value: false);
		}
		if (isLoadingFromJob)
		{
			if (jobLoadIndex != jobGlobes.Length - 1)
			{
				jobDisplays[jobGlobes.Length - 1].SetupDisplay(BackstageJobDisplay.DisplayState.InUse);
				jobGlobe.transform.position = jobGlobes[jobLoadIndex].transform.position - Vector3.up * 0.05f;
				hugeGlobeClosed.SetActive(value: true);
			}
			else
			{
				jobDisplays[jobGlobes.Length - 1].SetupDisplay(BackstageJobDisplay.DisplayState.Error);
				hugeGlobeClosed.SetActive(value: false);
				jobGlobe.SetActive(value: false);
			}
		}
		else
		{
			jobDisplays[jobGlobes.Length - 1].SetupDisplay(BackstageJobDisplay.DisplayState.InUse);
			jobGlobe.SetActive(value: false);
			hugeGlobeClosed.SetActive(value: true);
			jobLoadIndex = 100;
		}
		for (int j = 0; j < jobGlobes.Length - 1; j++)
		{
			if (j == jobLoadIndex)
			{
				jobDisplays[j].SetupDisplay(BackstageJobDisplay.DisplayState.Error);
				continue;
			}
			int num = Random.Range(0, randomGlobes.Length);
			while (list.Contains(num))
			{
				num = Random.Range(0, randomGlobes.Length);
			}
			list.Add(num);
			randomGlobes[num].SetActive(value: true);
			if (randomGlobes[num].name.Contains("InUse"))
			{
				jobDisplays[j].SetupDisplay(BackstageJobDisplay.DisplayState.InUse);
			}
			else
			{
				jobDisplays[j].SetupDisplay(BackstageJobDisplay.DisplayState.Offline);
				List<VVRConsumable> list2 = new List<VVRConsumable>();
				randomGlobes[num].GetComponentsInChildren(list2);
				mainSwarmController.AddToConsumableList(toConstruct: true, list2, "OfflineGlobe");
			}
			randomGlobes[num].transform.position = jobGlobes[j].transform.position - Vector3.up * 0.05f;
		}
		if (isLoadingFromJob)
		{
			couriers[jobLoadIndex].gameObject.SetActive(value: false);
			roostCourier.transform.position = new Vector3(couriers[jobLoadIndex].transform.position.x, 0f, couriers[jobLoadIndex].transform.position.z);
			roostCourier.transform.rotation = couriers[jobLoadIndex].transform.rotation;
		}
		else
		{
			int num2 = Random.Range(0, couriers.Length);
			couriers[num2].gameObject.SetActive(value: false);
			roostCourier.transform.position = new Vector3(couriers[num2].transform.position.x, 0f, couriers[num2].transform.position.z);
			roostCourier.transform.rotation = couriers[num2].transform.rotation;
		}
		if (isLoadingFromJob)
		{
			spotlightRB.isKinematic = true;
			if (jobLoadIndex == 0)
			{
				spotlight.transform.rotation = Quaternion.Euler(Vector3.up * -135f);
			}
			else if (jobLoadIndex == 1)
			{
				spotlight.transform.rotation = Quaternion.Euler(Vector3.up * 135f);
			}
			else if (jobLoadIndex == 2)
			{
				spotlight.transform.rotation = Quaternion.Euler(Vector3.up * -45f);
			}
			else if (jobLoadIndex == 3)
			{
				spotlight.transform.rotation = Quaternion.Euler(Vector3.up * 45f);
			}
			else if (jobLoadIndex == 4)
			{
				spotlight.transform.rotation = Quaternion.Euler(Vector3.zero);
			}
			spotlightRB.isKinematic = false;
		}
		else
		{
			spotlightRB.isKinematic = true;
			spotlight.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
			spotlightRB.isKinematic = false;
		}
		isLoadingFromJob = false;
	}
}
